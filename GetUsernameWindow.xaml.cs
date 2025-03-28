using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logika interakcji dla klasy GetUsernameWindow.xaml
    /// </summary>
    public partial class GetUsernameWindow : Window
    {
        private string username {  get; set; }

        public GetUsernameWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            username = (usernameTextBox.Text != String.Empty) ? usernameTextBox.Text : "Anon";
            DialogResult = true;
            Close();
        }

        public string GetUserName()
        {
            return username;
        }
    }
}
