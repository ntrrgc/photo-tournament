using PhotoTournament;
using System;
using System.Windows.Forms;

namespace PhotoTournament
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
