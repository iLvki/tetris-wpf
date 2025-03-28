using System.Windows;

namespace Tetris
{
    public partial class ChooseLevelWindow : Window
    {
        public int SelectedLevel { get; private set; } = 1; // Domyślny poziom

        public ChooseLevelWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz wybrany poziom
            if (LevelComboBox.SelectedIndex >= 0)
            {
                SelectedLevel = LevelComboBox.SelectedIndex + 1;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a level.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
