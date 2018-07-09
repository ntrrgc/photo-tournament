using CompetititiveCullingAlgorithm;
using System;
using System.Windows.Forms;

namespace TournamentSort
{
    public partial class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
