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
    public partial class MainWindow : Form
    {
        UIPhoto photoA;
        UIPhoto photoB;
        TaskCompletionSource<int> betterPhotoPromise;

        class UIPhoto : IAsyncComparable<UIPhoto>
        {
            public UIPhoto(MainWindow mainWindow, int n)
            {
                MainWindow = mainWindow;
                N = n;
            }

            public MainWindow MainWindow { get; }
            public int N { get; }

            public async Task<int> CompareToAsync(UIPhoto other)
            {
                MainWindow.label1.Text = $"{this.ToString()} vs {other.ToString()}";
                MainWindow.photoA = this;
                MainWindow.photoB = other;
                MainWindow.betterPhotoPromise = new TaskCompletionSource<int>();
                return await MainWindow.betterPhotoPromise.Task;
            }

            public override string ToString() => $"{N}";
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var tournament = new Tournament<UIPhoto>(Enumerable.Range(1, 7).ToList().Shuffle().Select(x => new UIPhoto(this, x)).ToList(), 3);
            tournament.NewWinnerEvent += Tournament_NewWinnerEvent;
            tournament.CalculateTopN().ContinueWith(bestPhotos =>
                {
                    label1.Text = $"Best photos are: {String.Join(", ", bestPhotos.Result.Select(x => x.ToString()))}";
                }, scheduler);
        }

        private void Tournament_NewWinnerEvent(int place, UIPhoto item)
        {
            toolStripStatusLabel1.Text = $"The photo {item.ToString()} has made to the place #{place}";
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (betterPhotoPromise == null)
                return;

            if (e.KeyCode == Keys.Left)
                betterPhotoPromise.TrySetResult(1);
            else if (e.KeyCode == Keys.Right)
                betterPhotoPromise.TrySetResult(-1);
        }
    }
}
