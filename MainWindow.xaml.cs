using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using tetris_nowy;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private const int CellSize = 30;
        private Game game; // tworzenie obiektu gry
        public static DispatcherTimer timer;
        public static Stopwatch gameStopwatch { get; private set; } // liczy czas gry
        private int selectedLevel = 1; // Domyślny poziom trudności
        public static int userScore = 0;
        private LeaderboardWindow lbWindow = new LeaderboardWindow();


        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += GameTick;
            this.KeyDown += OnKeyDown;
            gameStopwatch = new Stopwatch();
        }

        private void OpenLeaderboard(object sender, EventArgs e)
        {
            if (lbWindow == null || !lbWindow.IsLoaded) // Jeśli okno zostało zamknięte, tworzysz je na nowo
            {
                lbWindow = new LeaderboardWindow();  // Utwórz nowe okno, jeśli zostało zamknięte
                lbWindow.Show();
            }
            else if (!lbWindow.IsVisible)
            {
                lbWindow.Show();  // Pokazuje okno, jeśli było ukryte
            }
            else
            {
                lbWindow.Hide();  // Ukrywa okno, jeśli jest widoczne
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
            this.Width = 650;
            this.Height = 700;
        }

        private void ChooseLvlButton_Click(object sender, RoutedEventArgs e)
        {
            // wybranie poziomu w innym oknie - domyślnie lvl 1
            var dialog = new ChooseLevelWindow();
            if (dialog.ShowDialog() == true)
            {
                selectedLevel = dialog.SelectedLevel;
            }
        }

        private void StartGame()
        {
            // Przełącz widok na ekran gry
            MenuPanel.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;

            // Inicjalizacja gry
            game = new Game(20, 10);
            ScoreTextBlock.Text = $"{game.GetScore()}";

            // Ustaw tempo w zależności od poziomu trudności
            int interval = 600 - (selectedLevel-1) * 100; // 600 ms na poziomie 1, 300 ms na poziomie 4
            game.selectedLevel = selectedLevel;
            timer.Interval = TimeSpan.FromMilliseconds(interval);

            // Rozpocznij grę
            timer.Start();
            gameStopwatch.Restart();

            DrawGame();
            DrawNextBlock();
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (!game.MoveBlock(0, 1))
            {
                game.PlaceBlock();
                DrawNextBlock();
            }

            DrawGame();
            UpdateTime();
            UpdateScore(userScore);
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (GameGrid.Visibility != Visibility.Visible)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    game.MoveBlock(-1, 0);
                    break;
                case Key.Right:
                    game.MoveBlock(1, 0);
                    break;
                case Key.Down:
                    game.MoveBlock(0, 1);
                    break;
                case Key.Up:
                    game.RotateBlock();
                    break;
                case Key.Space:
                    while (game.MoveBlock(0, 1)) ;
                    break;
            }

            DrawGame();
        }

        private void DrawGame()
        {
            GameCanvas.Children.Clear();
            var board = game.GetBoardState();

            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    if (board[y, x] != Colors.Transparent)
                    {
                        DrawCell(GameCanvas, x, y, board[y, x]);
                    }
                }
            }

            // Rysowanie bieżącego bloku
            foreach (var point in game.CurrentBlock.Shape)
            {
                int x = (int)(point.X + game.CurrentPosition.X);
                int y = (int)(point.Y + game.CurrentPosition.Y);

                if (y >= 0)
                {
                    DrawCell(GameCanvas, x, y, game.CurrentBlock.Color);
                }
            }
        }

        private void DrawNextBlock()
        {
            NextBlockCanvas.Children.Clear();

            foreach (var point in game.NextBlock.Shape)
            {
                int x = (int)point.X + 2; // Pozycjonowanie na środku NextBlockCanvas
                int y = (int)point.Y;

                DrawCell(NextBlockCanvas, x, y, game.NextBlock.Color);
            }
        }

        private void DrawCell(Canvas canvas, int x, int y, Color color)
        {
            Rectangle rect = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = new SolidColorBrush(color),
                Stroke = Brushes.Black
            };

            Canvas.SetLeft(rect, x * CellSize);
            Canvas.SetTop(rect, y * CellSize);
            canvas.Children.Add(rect);
        }

        private void UpdateScore(int score)
        {
            ScoreTextBlock.Text = score.ToString();
        }

        private void UpdateTime()
        {
            var time = gameStopwatch.Elapsed;
            TimeTextBlock.Text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public void ReturnToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Zatrzymaj grę i przejdź do menu
            timer.Stop();
            gameStopwatch.Stop();
            this.Height = 600;
            this.Width = 400;
            GameGrid.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
        }
    }
}
