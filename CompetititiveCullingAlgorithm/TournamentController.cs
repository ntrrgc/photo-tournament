using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private Tournament<ComparablePhoto> Tournament;

        public IPageUIClient CurrentPage { get; private set; }

        public delegate void NewWinnerEventHandler(int place, PhotoPath item);
        public event NewWinnerEventHandler NewWinnerEvent;

        public delegate void NewPageEventHandler(IPageUIClient page);
        public event NewPageEventHandler NewPageEvent;

        public delegate void TournamentFinishedEventHandler(List<PhotoPath> bestPhotos);
        public event TournamentFinishedEventHandler TournamentFinishedEvent;

        private class ComparablePhoto : IAsyncComparable<ComparablePhoto>
        {
            public ComparablePhoto(TournamentController controller, PhotoPath photoPath)
            {
                Controller = controller;
                PhotoPath = photoPath;
            }

            private TournamentController Controller { get; }
            public PhotoPath PhotoPath { get; }

            public Task<int> CompareToAsync(ComparablePhoto other)
            {
                Page page = new Page(this.PhotoPath, other.PhotoPath);
                Controller.CurrentPage = page;
                return page.BetterPhotoPromise.Task.ContinueWith(async best =>
                    await best == PhotoChoice.PhotoAIsBetter ? 1 : -1).Unwrap();
            }
        }

        public TournamentController(List<PhotoPath> items, int totalPlaces)
        {
            Tournament = new Tournament<ComparablePhoto>(items.Select(x => new ComparablePhoto(this, x)).ToList(), totalPlaces);
            Tournament.CalculateTopN().ContinueWith(async bestPhotos =>
                TournamentFinishedEvent((await bestPhotos).Select(x => x.PhotoPath).ToList()));
        }
    }
}
