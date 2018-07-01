using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompetititiveCullingAlgorithm
{
    using PhotoPath = String;

    public partial class MainWindow : Form
    {
        private TournamentController controller = new TournamentController();

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
            controller.StartNewTournament(@"V:\Photos\2018\2018-03-26 Fotos en Salamanca", 3);
        }

        private void Tournament_NewPageEvent(TournamentController.IPageUIClient page)
        {
            imgPhotoA.Image = null;
            imgPhotoB.Image = null;
            imgPhotoA.ImageLocation = page.PhotoA;
            imgPhotoB.ImageLocation = page.PhotoB;
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
    }
}
