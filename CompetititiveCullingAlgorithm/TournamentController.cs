using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            void Choose(PhotoChoice choice);
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

        private Tournament<PhotoPath> Tournament;

        public IPageUIClient CurrentPage { get; private set; }

        public delegate void NewWinnerEventHandler(int place, PhotoPath item);
        public event NewWinnerEventHandler NewWinnerEvent;

        public delegate void NewPageEventHandler(IPageUIClient page);
        public event NewPageEventHandler NewPageEvent;

        public delegate void TournamentFinishedEventHandler(List<PhotoPath> bestPhotos);
        public event TournamentFinishedEventHandler TournamentFinishedEvent;

        public delegate void PreloadPhotosAdvicedEventHandler(List<PhotoPath> nextPhotos);
        public event PreloadPhotosAdvicedEventHandler PreloadPhotosAdvicedEvent;

        private class PhotoGUIComparator: IAsyncComparator<PhotoPath>
        {
            public PhotoGUIComparator(TournamentController controller)
            {
                Controller = controller;
            }

            private TournamentController Controller { get; }

            public Task<int> CompareAsync(string item, string other)
            {
                Controller.PreloadPhotosAdvicedEvent?.Invoke(
                    Controller.Tournament.PredictItemsWorthPreloading().ToList());
                Page page = new Page(item, other);
                Controller.CurrentPage = page;
                Controller.NewPageEvent(page);
                return page.BetterPhotoPromise.Task.ContinueWith(best =>
                    best.Result == PhotoChoice.PhotoAIsBetter ? 1 : -1,
                    TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private static List<string> FindPhotosInPath(string rootPath)
        {
            return Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories)
                .Where(path => Regex.Match(path, @".*\.(jpg|jpeg|png|tif|tiff|bmp|gif)$",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success)
                .ToList();
        }

        public void StartNewTournament(string sourcePhotoFolder, int totalPlaces)
        {
            List<PhotoPath> photos = FindPhotosInPath(sourcePhotoFolder);
            Tournament = new Tournament<PhotoPath>(new PhotoGUIComparator(this), photos, totalPlaces);
            Tournament.NewWinnerEvent += Tournament_NewWinnerEvent; ;
            Tournament.CalculateTopN().ContinueWith(bestPhotosTask => {
                var bestPhotos = bestPhotosTask.Result;
                CurrentPage = null;
                TournamentFinishedEvent(bestPhotos);
            }, TaskContinuationOptions.ExecuteSynchronously);
            if (photos.Count > 0)
                Debug.Assert(CurrentPage != null);
        }

        private void Tournament_NewWinnerEvent(int place, PhotoPath item)
        {
            NewWinnerEvent(place, item);
        }
    }
}
