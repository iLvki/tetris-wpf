using System;
using System.Windows;
using System.Windows.Media;
using Tetris;
using tetris_nowy;

public class Game
{
    public int score { get; private set; }

    private int Rows;
    private int Columns;
    private Color[,] Board;
    public Block CurrentBlock { get; private set; }
    public Block NextBlock { get; private set; }
    public Point CurrentPosition;
    public int selectedLevel = 1;
    string username;
    private bool isPlayable = true;

    private readonly Random random = new Random();

    public Game(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Board = new Color[rows, columns];
        for (int y = 0; y < rows; y++) // Inicjalizacja planszy
        {
            for (int x = 0; x < columns; x++)
            {
                Board[y, x] = Colors.Transparent;
            }
        }

        NextBlock = GenerateRandomBlock(); // Tylko `NextBlock` jest generowany na początku
        SpawnBlock(); // Dopiero teraz ustawiamy `CurrentBlock` na początku gry
    }

    private void SpawnBlock()
    {
        if (!isPlayable) return;

        CurrentBlock = NextBlock; // Przypisz następny blok jako bieżący
        NextBlock = GenerateRandomBlock(); // Wygeneruj nowy blok na przyszłość
        CurrentPosition = new Point(Columns / 2 - 1, 0); // Ustaw początkową pozycję

        // sprawdzanie czy da się postawić nowy blok - jeśli nie, to koniec gry
        if(!CanMove(CurrentPosition, CurrentBlock.Shape)) GameOver();
    }

    private void GameOver()
    {
        isPlayable = false;
        MainWindow.timer.Stop();
        MainWindow.gameStopwatch.Stop();
        var time = MainWindow.gameStopwatch.Elapsed;

        MessageBox.Show($" Koniec gry! \n Wynik: {score} \n Czas: {time.Minutes:D2}:{time.Seconds:D2}");

        var userNameWindow = new GetUsernameWindow();
        if (userNameWindow.ShowDialog() == true) username = userNameWindow.GetUserName();

        SaveGame save = new SaveGame(username, time, score);
        save.SaveScore();
    }

    private Block GenerateRandomBlock()
    {
        var blocks = new[]
        {
            new Block(new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(2, 0) }, Colors.Cyan),    // I
            new Block(new Point[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) }, Colors.Yellow),  // O
            new Block(new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, 1) }, Colors.Purple), // T
            new Block(new Point[] { new Point(0, 0), new Point(-1, 0), new Point(0, 1), new Point(1, 1) }, Colors.Green),  // S
            new Block(new Point[] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(-1, 1) }, Colors.Red),    // Z
            new Block(new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(1, 0) }, Colors.Blue),  // J
            new Block(new Point[] { new Point(0, 0), new Point(1, 0), new Point(-1, 0), new Point(1, 1) }, Colors.Orange)  // L
        };

        return blocks[random.Next(blocks.Length)];
    }

    public void PlaceBlock()
    {
        if (!isPlayable) return;

        foreach (var point in CurrentBlock.Shape)
        {
            int x = (int)(point.X + CurrentPosition.X);
            int y = (int)(point.Y + CurrentPosition.Y);

            if (x >= 0 && x < Columns && y >= 0 && y < Rows) Board[y, x] = CurrentBlock.Color;
        }

        ClearLines();
        SpawnBlock();
    }

    public int GetScore()
    {
        return score;
    }

    private void ClearLines()
    {
        int linesCleared = 0;

        for (int y = Rows - 1; y >= 0; y--)
        {
            bool isFull = true;

            for (int x = 0; x < Columns; x++)
            {
                if (Board[y, x] == Colors.Transparent)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                linesCleared++;
                for (int ny = y; ny > 0; ny--)
                {
                    for (int x = 0; x < Columns; x++)
                    {
                        Board[ny, x] = Board[ny - 1, x];
                    }
                }

                // Wyczyść górny wiersz
                for (int x = 0; x < Columns; x++)
                {
                    Board[0, x] = Colors.Transparent;
                }

                y++; // Sprawdź ten sam wiersz ponownie
            }
        }

        if (linesCleared > 0)
        {
            switch (linesCleared)
            {
                case 1: score += 40 * (selectedLevel + 1); break;
                case 2: score += 100 * (selectedLevel + 1); break;
                case 3: score += 300 * (selectedLevel + 1); break;
                case 4: score += 1200 * (selectedLevel + 1); break;
                default: score += 10 * (selectedLevel + 1); break;
            }
        }

        MainWindow.userScore = score;
    }

    public bool MoveBlock(int dx, int dy)
    {
        var newPosition = new Point(CurrentPosition.X + dx, CurrentPosition.Y + dy);
        if (CanMove(newPosition, CurrentBlock.Shape))
        {
            CurrentPosition = newPosition;
            return true;
        }

        return false;
    }

    public void RotateBlock()
    {
        if (CurrentBlock.Color == Colors.Yellow) return;

        var rotatedShape = CurrentBlock.GetRotatedShape();
        if (CanMove(CurrentPosition, rotatedShape))
        {
            CurrentBlock.Shape = rotatedShape; // Zastosowanie obrotu, jeśli możliwy
        }
    }

    private bool CanMove(Point position, Point[] shape)
    {
        foreach (var point in shape)
        {
            int x = (int)(point.X + position.X);
            int y = (int)(point.Y + position.Y);

            // Sprawdź, czy współrzędne są poza zakresem
            if (x < 0 || x >= Columns || y >= Rows)
                return false;

            // Sprawdź, czy miejsce jest już zajęte
            if (y >= 0 && Board[y, x] != Colors.Transparent)
                return false;
        }

        return true;
    }


    public Color[,] GetBoardState()
    {
        return Board;
    }
}
