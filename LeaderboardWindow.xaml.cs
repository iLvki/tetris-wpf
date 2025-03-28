using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tetris_nowy
{
    /// <summary>
    /// Logika interakcji dla klasy LeaderboardWindow.xaml
    /// </summary>
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();
            ShowLeaderboard();
        }

        string? appDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string fileName = "wyniki.txt";

        private void ShowLeaderboard()
        {
            string appFilePath = System.IO.Path.Combine(appDirectory, fileName);
            StreamReader sr = new StreamReader(appFilePath);
            string line = sr.ReadLine();

            while (line != null)
            {
                LeaderboardTextBlock.Text += $"{line} \n";
                line = sr.ReadLine();
            }

            sr.Close();
        }
    }
}
