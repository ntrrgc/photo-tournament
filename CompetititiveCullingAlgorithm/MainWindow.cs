using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            var oldRequest = (ImageLoadRequest)pictureBox.Tag;
            if (oldRequest?.RcImage.Path == photoPath)
                return;

            // Don't continue showing the old picture
            pictureBox.Image = null;
            pictureBox.Tag = null;
            // If the old picture was not loaded yet, don't show it when it finishes loading.
            oldRequest?.CancellationTokenSource.Cancel();
            // We don't need the picture anymore, dispose of the bitmap if it has no more holders.
            oldRequest?.RcImage.Unref();

            if (photoPath == null)
                return; // leave the picture blank, by request

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
            lblHint.Text = "";
            lblStatus.Text = "";
            UpdateGUI();
        }

        private void Tournament_TournamentFinishedEvent(List<PhotoPath> bestPhotos)
        {
            UpdateGUI();
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
                    controller.DoChoosePhotoUndoable(TournamentController.PhotoChoice.PhotoAIsBetter);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D2)
            {
                if (btnChooseB.Enabled)
                    controller.DoChoosePhotoUndoable(TournamentController.PhotoChoice.PhotoBIsBetter);
                e.Handled = true;
            }
        }

        private void btnChooseA_Click(object sender, EventArgs e)
        {
            controller.DoChoosePhotoUndoable(TournamentController.PhotoChoice.PhotoAIsBetter);
        }

        private void btnChooseB_Click(object sender, EventArgs e)
        {
            controller.DoChoosePhotoUndoable(TournamentController.PhotoChoice.PhotoBIsBetter);
        }

        private bool TrySave()
        {
            if (controller.TournamentFilePath == null)
                return TrySaveAs();
            else
            {
                try
                {
                    controller.Save(controller.TournamentFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error while saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private bool TrySaveAs()
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "My selection.tournament",
                DefaultExt = ".tournament",
                InitialDirectory = NewTournamentDialog.LatestPickedSourceDirectory,
                Filter = "Tournament files (*.tournament)|*.*",
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return false;
            try
            {
                controller.Save(dialog.FileName);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error while saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TrySave();
            UpdateGUI();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                FileName = "My selection.tournament",
                DefaultExt = ".tournament",
                InitialDirectory = NewTournamentDialog.LatestPickedSourceDirectory,
                Filter = "Tournament files (*.tournament)|*.*",
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;
            try
            {
                controller.Load(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error while loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateGUI();
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
            bool hasTournament = controller.Tournament != null;
            if (hasTournament)
            {
                String fileName = controller.TournamentFilePath != null
                    ? Path.GetFileNameWithoutExtension(controller.TournamentFilePath)
                    : "New tournament";
                String star = controller.MadeAnyChanges ? "*" : "";
                this.Text = $"Photo Tournament - {fileName}{star}";
            }
            else
                this.Text = "Photo Tournament";

            if (hasTournament && controller.Tournament.Finished)
                lblHint.Text = $"A selection of the best {controller.Tournament.RankingWinners.Count} photos is complete. You can export it now.";
            else if (hasTournament)
                lblHint.Text = "Which picture fits better in the album?";
            else
                lblHint.Text = "Create a new tournament to start.";

            btnSave.Enabled = hasTournament && controller.MadeAnyChanges;
            btnSaveAs.Enabled = hasTournament;
            btnChooseA.Enabled = btnChooseB.Enabled = controller.CurrentPage != null && BothImagesLoaded;
            UpdatePictureBoxWithPhoto(imgPhotoA, controller.CurrentPage?.PhotoA);
            UpdatePictureBoxWithPhoto(imgPhotoB, controller.CurrentPage?.PhotoB);
            if (hasTournament)
            {
                pgrGlobal.Maximum = controller.Tournament.GlobalStepsMax;
                pgrGlobal.Value = controller.Tournament.Finished ? pgrGlobal.Maximum : controller.Tournament.GlobalStepsDone;
                pgrNextWinner.Maximum = controller.Tournament.NextWinnerStepsMax;
                pgrNextWinner.Value = controller.Tournament.Finished ? pgrNextWinner.Maximum : controller.Tournament.NextWinnerStepsDone;
            }
            else
            {
                pgrGlobal.Maximum = pgrNextWinner.Maximum = pgrGlobal.Value = pgrNextWinner.Value = 0;
            }
            btnUndo.Enabled = controller.UndoStack.CanUndo;
            btnRedo.Enabled = controller.UndoStack.CanRedo;
            btnExportUnsorted.Enabled = btnExportByRank.Enabled = hasTournament && controller.Tournament.RankingWinners.Count > 0;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            controller.UndoStack.Undo();
            UpdateGUI();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            controller.UndoStack.Redo();
            UpdateGUI();
        }

        private bool ExportPhotosWithPicker(Action<string> savePhotosFn)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select the folder the winner photos will be copied to."
            };
            var result = dialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return false;
            try
            {
                savePhotosFn(dialog.SelectedPath);
                Process.Start("explorer.exe", dialog.SelectedPath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error while saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnExportByRank_Click(object sender, EventArgs e)
        {
            ExportPhotosWithPicker(path => controller.ExportWinnerPhotosSorted(path));
        }

        private void btnExportUnsorted_Click(object sender, EventArgs e)
        {
            ExportPhotosWithPicker(path => controller.ExportWinnerPhotosUnsorted(path));
        }

        private void btnNewTournament_Click_1(object sender, EventArgs e)
        {
            var dialog = new NewTournamentDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;
            try
            {
                controller.StartNewTournament(dialog.SelectedSourceDir, dialog.SelectedTotalPlaces);
            }
            catch (TournamentController.NoPhotosException)
            {
                MessageBox.Show(this, "There are no photos in the provided folder.", "No photos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateGUI();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (controller.MadeAnyChanges)
            {
                var response = MessageBox.Show(this, "Do you want to save your changes?", "Save your changes", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (response == DialogResult.Yes)
                    e.Cancel = !TrySave();
                else if (response == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            TrySaveAs();
            UpdateGUI();
        }
    }
}