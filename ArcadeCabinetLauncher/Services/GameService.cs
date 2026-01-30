using ArcadeCabinetLauncher.Models;
using System.Text.Json;
using System.IO;

namespace ArcadeCabinetLauncher.Services
{
    public class GameService
    {
        private string appDataPath;
        private string gamesFilePath;

        public List<GameEntry> games = new();

        public GameService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ArcadeCabinetLauncher"); //Creates new folder in appData so that Windows does not ask for permission when writing files
            Directory.CreateDirectory(appDataPath);                                                                                                //Creates new folder in appData so that Windows does not ask for permission when writing files

            gamesFilePath = Path.Combine(appDataPath, "games.json");
        }

        public List<GameEntry> LoadGames()
        {
            if (!File.Exists(gamesFilePath))
                return new();

            var json = File.ReadAllText(gamesFilePath);
            return JsonSerializer.Deserialize<List<GameEntry>>(json) ?? new();
        }

        public void SaveGames(IEnumerable<GameEntry> games) 
        {
            string json = JsonSerializer.Serialize(
                    games,
                    new JsonSerializerOptions { WriteIndented = true }
                );
            File.WriteAllText(gamesFilePath, json);
        }
    }
}
