using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace tetris_nowy
{
    internal class SaveGame
    {
        public string Name { get; set; }
        public TimeSpan gameTime { get; set; }
        public int gameScore { get; set; }
        public string OS = Environment.OSVersion.ToString();

        public SaveGame(string name, TimeSpan gameTime, int gameScore)
        {
            Name = name;
            this.gameTime = gameTime;
            this.gameScore = gameScore;
        }

        public void SaveScore()
        {
            string? appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string solutionDirectory = GetSolutionDirectory();
            string fileName = "wyniki.txt";

            string appFilePath = Path.Combine(appDirectory, fileName);
            string? solutionFilePath = solutionDirectory != null ? Path.Combine(solutionDirectory, fileName) : null;

            string content = $"{Name} | {gameScore}pkt | {gameTime.Minutes:D2}:{gameTime.Seconds:D2} | {OS}";

            // Zapis do katalogu aplikacji
            File.AppendAllText(appFilePath, content + Environment.NewLine);

            // Zapis do katalogu rozwiązania (jeśli znaleziono plik .sln)
            if (solutionFilePath != null)
            {
                File.AppendAllText(solutionFilePath, content + Environment.NewLine);
            }

            MessageBox.Show(
                $"Wynik zapisano w:\n- Katalog aplikacji: {appFilePath}" +
                (solutionFilePath != null ? $"\n- Katalog rozwiązania: {solutionFilePath}" : ""),
                "Tetris",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private string GetSolutionDirectory()
        {
            string appDirectory = Directory.GetCurrentDirectory();

            // Przechodzenie do nadrzędnych katalogów w poszukiwaniu pliku .sln
            DirectoryInfo directoryInfo = new DirectoryInfo(appDirectory);
            while (directoryInfo != null)
            {
                var solutionFiles = directoryInfo.GetFiles("*.sln");
                if (solutionFiles.Length > 0)
                {
                    return directoryInfo.FullName; // Znaleziono katalog rozwiązania
                }
                directoryInfo = directoryInfo.Parent; // Przejdź do katalogu nadrzędnego
            }

            return null; // Nie znaleziono katalogu rozwiązania
        }
    }
}
