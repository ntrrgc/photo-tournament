using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoTournament
{
    public partial class NewTournamentDialog : Form
    {
        public NewTournamentDialog()
        {
            InitializeComponent();
        }

        public string SelectedSourceDir { get => txtSourceDir.Text; }
        public int SelectedTotalPlaces { get => (int) spnPicks.Value;  }
        public static string LatestPickedSourceDirectory = null;

        private void btnBrowseSourceDir_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            browser.SelectedPath = LatestPickedSourceDirectory;
            if (browser.ShowDialog() != DialogResult.OK)
                return;
            LatestPickedSourceDirectory = txtSourceDir.Text = browser.SelectedPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NewTournamentDialog_Shown(object sender, EventArgs e)
        {
            btnBrowseSourceDir_Click(this, null);
        }
    }
}
