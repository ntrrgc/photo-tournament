using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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
        }

        private void Controller_PreloadPhotosAdvicedEvent(List<string> nextPhotos)
        {
            Console.WriteLine("Advice");
            imageCache.ReplaceCache(nextPhotos.Take(4).ToList());
        }

        private void UpdatePictureBoxWithPhoto(PictureBox pictureBox, string photoPath)
        {
            pictureBox.Image = null;
            ImageCache.RefCountedImage previousImage = (ImageCache.RefCountedImage) pictureBox.Tag;
            previousImage?.Unref();
            pictureBox.Tag = null;
            
            imageCache.LoadAsync(photoPath).ContinueWith(image =>
            {
                ImageCache.RefCountedImage img = image.Result.Ref();
                pictureBox.Tag = img;
                pictureBox.Image = img.Image;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Tournament_NewPageEvent(TournamentController.IPageUIClient page)
        {
            UpdatePictureBoxWithPhoto(imgPhotoA, page.PhotoA);
            UpdatePictureBoxWithPhoto(imgPhotoB, page.PhotoB);
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
                controller.CurrentPage.Choose(TournamentController.PhotoChoice.PhotoAIsBetter);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D2)
            {
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
            controller.Save(Application.UserAppDataPath + @"\quick-save.xml");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            controller.Load(Application.UserAppDataPath + @"\quick-save.xml");
        }
    }
}
