using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace CompetititiveCullingAlgorithm
{
    using PhotoPath = String;

    public class TournamentController
    {
        public enum PhotoChoice
        {
            PhotoAIsBetter,
            PhotoBIsBetter
        }

        public interface IPageUIClient
        {
            PhotoPath PhotoA { get; }
            PhotoPath PhotoB { get; }
        }

        private class Page : IPageUIClient
        {
            public Page(PhotoPath photoA, PhotoPath photoB) {
                PhotoA = photoA;
                PhotoB = photoB;
            }
            public PhotoPath PhotoA { get; }
            public PhotoPath PhotoB { get; }
            public TaskCompletionSource<PhotoChoice> BetterPhotoPromise { get; } = new TaskCompletionSource<PhotoChoice>();

            public void Choose(PhotoChoice choice)
            {
                BetterPhotoPromise.TrySetResult(choice);
            }
        }

        public Tournament<PhotoPath> Tournament { get; private set; }
        private CancellationTokenSource tournamentTaskToken;
        private Task tournamentTask;
        public readonly UndoStack UndoStack = new UndoStack();

        private Page currentPage;
        public IPageUIClient CurrentPage { get => currentPage; }

        public string TournamentFilePath { get; private set; } = null;
        public bool MadeAnyChanges { get => Tournament != null && fileVersionId != memoryVersionId; }

        // These fields are used to know when the file in memory and in disk differ
        private int lastVersionId = 0;
        private int fileVersionId;
        private int memoryVersionId;

        public delegate void NewWinnerEventHandler(int place, PhotoPath item);
        public event NewWinnerEventHandler NewWinnerEvent;

        public delegate void NewPageEventHandler(IPageUIClient page);
        public event NewPageEventHandler NewPageEvent;

        public delegate void TournamentFinishedEventHandler(List<PhotoPath> bestPhotos);
        public event TournamentFinishedEventHandler TournamentFinishedEvent;

        public delegate void PreloadPhotosAdvicedEventHandler(List<PhotoPath> nextPhotos);
        public event PreloadPhotosAdvicedEventHandler PreloadPhotosAdvicedEvent;

        private class ChoosePhotoUndoable : IUndoable
        {
            private readonly TournamentController controller;
            private readonly PhotoChoice choice;
            private readonly Tournament<PhotoPath>.SavedState savedState;
            private readonly int versionIdBefore;
            private readonly int versionIdAfter;

            public ChoosePhotoUndoable(TournamentController controller, PhotoChoice choice) {
                this.controller = controller;
                this.choice = choice;
                this.savedState = controller.Tournament.SaveState();
                this.versionIdBefore = controller.memoryVersionId;
                this.versionIdAfter = ++controller.lastVersionId;
            }

            public string Name => "Choose photo";

            public void Do()
            {
                controller.memoryVersionId = versionIdAfter;
                controller.currentPage.Choose(choice);
            }

            public void Undo()
            {
                controller.memoryVersionId = versionIdBefore;
                controller.ReplaceTournament(savedState.Instantiate());
            }
        }

        private class PhotoGUIComparator: IAsyncComparator<PhotoPath>
        {
            public PhotoGUIComparator(TournamentController controller)
            {
                Controller = controller;
            }

            private TournamentController Controller { get; }

            public async Task<int> CompareAsync(string item, string other, CancellationToken cancellationToken)
            {
                Controller.PreloadPhotosAdvicedEvent?.Invoke(
                    Controller.Tournament.PredictItemsWorthPreloading().ToList());
                Page page = new Page(item, other);
                Controller.currentPage = page;
                Controller.NewPageEvent(page);
                PhotoChoice best = await page.BetterPhotoPromise.Task;
                return best == PhotoChoice.PhotoAIsBetter ? 1 : -1;
            }
        }

        private static List<string> FindPhotosInPath(string rootPath)
        {
            return Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories)
                .Where(path => Regex.Match(path, @".*\.(jpg|jpeg|png|tif|tiff|bmp|gif)$",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success)
                .ToList();
        }

        private void StartTournament(Tournament<PhotoPath> tournament)
        {
            Tournament = tournament;
            Tournament.NewWinnerEvent += Tournament_NewWinnerEvent;

            tournamentTaskToken = new CancellationTokenSource();
            var cancellationToken = tournamentTaskToken.Token;
            async Task calculateFnAsync()
            {
                var bestPhotos = await Tournament.CalculateTopN(new PhotoGUIComparator(this), cancellationToken);
                currentPage = null;
                TournamentFinishedEvent(bestPhotos);
            }
            tournamentTask = calculateFnAsync();
        }

        public class NoPhotosException: Exception
        {
        }

        public void StartNewTournament(string sourcePhotoFolder, int totalPlaces)
        {
            List<PhotoPath> photos = FindPhotosInPath(sourcePhotoFolder);
            if (photos.Count == 0)
                throw new NoPhotosException();
            StartTournament(new Tournament<PhotoPath>(photos, totalPlaces));
            Debug.Assert(CurrentPage != null);
            TournamentFilePath = null;
            fileVersionId = 0; 
            memoryVersionId = lastVersionId = 1; // Enable Save right now
        }

        public void Save(string path)
        {
            Tournament.SaveState().SaveToFile(path);
            TournamentFilePath = path;
            fileVersionId = memoryVersionId;
        }

        public void Load(string path)
        {
            ReplaceTournament(Tournament<PhotoPath>.SavedState.LoadFromFile(path).Instantiate());
            UndoStack.Clear();
            TournamentFilePath = path;
            memoryVersionId = fileVersionId = lastVersionId = 0;
        }

        private static string QuickSavePath { get { return Application.UserAppDataPath + @"\quick-save.xml"; } }

        public bool QuickSaveExists { get; private set; } = File.Exists(QuickSavePath);

        public void SaveQuick()
        {
            Save(QuickSavePath);
            QuickSaveExists = true;
        }

        public void LoadQuick()
        {
            Load(QuickSavePath);
        }

        public void DoChoosePhotoUndoable(PhotoChoice choice)
        {
            UndoStack.Do(new ChoosePhotoUndoable(this, choice));
        }

        private void ReplaceTournament(Tournament<PhotoPath> newTournament)
        {
            if (Tournament != null)
            {
                this.tournamentTaskToken.Cancel();
                this.Tournament.NewWinnerEvent -= Tournament_NewWinnerEvent;
            }
            this.currentPage = null;

            StartTournament(newTournament);
        }

        private void Tournament_NewWinnerEvent(int place, PhotoPath item)
        {
            currentPage = null;
            NewWinnerEvent(place, item);
        }

        private void ExportWinnerPhotos(string destPath, Func<string, int, string> nameFormula)
        {
            int position = 0;
            foreach (var srcPath in Tournament.RankingWinners)
            {
                position++;
                File.Copy(srcPath, destPath + "\\" +
                    nameFormula(Path.GetFileNameWithoutExtension(srcPath), position) +
                    Path.GetExtension(srcPath), true);
            }
        }

        public void ExportWinnerPhotosUnsorted(string destPath)
        {
            ExportWinnerPhotos(destPath, (name, position) => $"{name} - Rank{position:000}");
        }

        public void ExportWinnerPhotosSorted(string destPath)
        {
            ExportWinnerPhotos(destPath, (name, position) => $"Rank{position:000} - {name}");
        }
    }
}
