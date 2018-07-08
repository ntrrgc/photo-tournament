using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompetititiveCullingAlgorithm
{
    using PhotoPath = String;

    public partial class MainWindow : Form
    {
        private TournamentController controller = new TournamentController();
        private ImageCache imageCache = new ImageCache();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            controller.NewWinnerEvent += Tournament_NewWinnerEvent;
            controller.TournamentFinishedEvent += Tournament_TournamentFinishedEvent;
            controller.NewPageEvent += Tournament_NewPageEvent;
            controller.PreloadPhotosAdvicedEvent += Controller_PreloadPhotosAdvicedEvent;
            controller.StartNewTournament(@"V:\Photos\2018\2018-03-26 Fotos en Salamanca", 3);

            UpdateGUI();
        }

        private void Controller_PreloadPhotosAdvicedEvent(List<string> nextPhotos)
        {
            imageCache.ReplaceCache(nextPhotos.ToList());
        }

        class ImageLoadRequest
        {
            public ImageCache.RefCountedImage RcImage;
            public CancellationTokenSource CancellationTokenSource;

            public async void WaitAndSetImage(MainWindow window, PictureBox pictureBox)
            {
                Console.WriteLine($">{RcImage.Path}");
                var image = await RcImage.Task;
                if (CancellationTokenSource.Token.IsCancellationRequested)
                    return;
                Console.WriteLine($"!{RcImage.Path}");
                pictureBox.Image = image;
                window.UpdateGUI();
            }
        }

        private void UpdatePictureBoxWithPhoto(PictureBox pictureBox, string photoPath)
        {
            var oldRequest = (ImageLoadRequest) pictureBox.Tag;
            if (oldRequest?.RcImage.Path == photoPath)
                return;

            // Don't continue showing the old picture
            pictureBox.Image = null;
            pictureBox.Tag = null;
            // If the old picture was not loaded yet, don't show it when it finishes loading.
            oldRequest?.CancellationTokenSource.Cancel();
            // We don't need the picture anymore, dispose of the bitmap if it has no more holders.
            oldRequest?.RcImage.Unref();

            var newRequest = new ImageLoadRequest
            {
                RcImage = imageCache.LoadAsync(photoPath).Ref(),
                CancellationTokenSource = new CancellationTokenSource()
            };
            pictureBox.Tag = (ImageLoadRequest)newRequest;
            newRequest.WaitAndSetImage(this, pictureBox);
        }

        private void Tournament_NewPageEvent(TournamentController.IPageUIClient page)
        {
            UpdateGUI();
        }

        private void Tournament_TournamentFinishedEvent(List<PhotoPath> bestPhotos)
        {
            label1.Text = $"Best photos are: {String.Join(", ", bestPhotos.Select(x => x.ToString()))}";
        }

        private void Tournament_NewWinnerEvent(int place, PhotoPath item)
        {
            lblStatus.Text = $"The photo {item.ToString()} has made to the place #{place}";
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (controller.CurrentPage == null)
                return;

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.D1)
            {
                if (btnChooseA.Enabled)
                    controller.CurrentPage.Choose(TournamentController.PhotoChoice.PhotoAIsBetter);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D2)
            {
                if (btnChooseB.Enabled)
                    controller.CurrentPage.Choose(TournamentController.PhotoChoice.PhotoBIsBetter);
                e.Handled = true;
            }
        }

        private void btnChooseA_Click(object sender, EventArgs e)
        {
            controller.CurrentPage.Choose(TournamentController.PhotoChoice.PhotoAIsBetter);
        }

        private void btnChooseB_Click(object sender, EventArgs e)
        {
            controller.CurrentPage.Choose(TournamentController.PhotoChoice.PhotoBIsBetter);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            controller.SaveQuick();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            controller.LoadQuick();
        }

        private bool BothImagesLoaded
        {
            get
            {
                var requestA = ((ImageLoadRequest)imgPhotoA.Tag);
                var requestB = ((ImageLoadRequest)imgPhotoB.Tag);
                if (requestA == null || requestB == null)
                    return false;
                return requestA.RcImage.Task.IsCompleted && requestB.RcImage.Task.IsCompleted;
            }
        }

        private void UpdateGUI()
        {
            btnQuickLoad.Enabled = controller.QuickSaveExists;
            btnChooseA.Enabled = btnChooseB.Enabled = controller.CurrentPage != null && BothImagesLoaded;
            UpdatePictureBoxWithPhoto(imgPhotoA, controller.CurrentPage.PhotoA);
            UpdatePictureBoxWithPhoto(imgPhotoB, controller.CurrentPage.PhotoB);
        }
    }
}
