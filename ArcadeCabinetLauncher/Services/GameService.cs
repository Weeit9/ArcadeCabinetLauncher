using ArcadeCabinetLauncher.Models;
using System.Text.Json;
using System.IO;

namespace ArcadeCabinetLauncher.Services
{
    public class GameService
    {
        private string appDataPath;
        private string gamesFilePath;
        private string adminFilePath;
        public string adminUsername;
        public string adminPassword;

        public List<GameEntry> games = new();

        public GameService()
        {
            appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); 
            Directory.CreateDirectory(appDataPath);                             

            gamesFilePath = Path.Combine(appDataPath, "games.json");
            adminFilePath = Path.Combine(appDataPath, "admin.txt");

            if (!File.Exists(adminFilePath))
            {
                adminUsername = "admin";
                adminPassword = "1234";
            }
            else
            {
                GetAdminInfo();
            }
        }

        public List<GameEntry> ScanGameFolder(string rootPath)
        {
            var games = new List<GameEntry>();

            if (!Directory.Exists(rootPath))
                return games;

            var gameDirectories = Directory.GetDirectories(rootPath);

            foreach (var gameDir in gameDirectories)
            {
                var windowsPath = Path.Combine(gameDir, "Windows");

                if (!Directory.Exists(windowsPath))
                    continue;

                var exe = Directory.GetFiles(windowsPath, "*.exe").FirstOrDefault();
                var thumbnail = Path.Combine(windowsPath, "thumbnail.png");
                var infoFile = Path.Combine(windowsPath, "info.txt");

                if (exe == null || !File.Exists(infoFile))
                    continue;

                var gameEntry = CreateGameEntryFromFiles(exe, thumbnail, infoFile);

                if (gameEntry != null)
                    games.Add(gameEntry);
            }

            return games;
        }

        private GameEntry? CreateGameEntryFromFiles(string exePath, string thumbnailPath, string infoPath)
        {
            var lines = File.ReadAllLines(infoPath);

            string name = "";
            string developer = "";
            string description = "";
            string controller = "";
            string year = "";

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                switch (parts[0].Trim())
                {
                    case "Name":
                        name = parts[1].Trim();
                        break;
                    case "Developer":
                        developer = parts[1].Trim();
                        break;
                    case "Controller":
                        controller = parts[1].Trim();
                        break;
                    case "Description":
                        description = parts[1].Trim();
                        break;
                    case "Year":
                        year = parts[1].Trim();
                        break;
                }
            }

            return new GameEntry
            {
                Name = name,
                GameMaker = developer,
                Controller = controller,
                Description = description,
                Year = year,
                ExecutablePath = exePath,
                ThumbnailPath = File.Exists(thumbnailPath) ? thumbnailPath : ""
            };

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

        public void GetAdminInfo()
        {
            var lines = File.ReadAllLines(adminFilePath);


            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                switch (parts[0].Trim())
                {
                    case "Username":
                        adminUsername = parts[1].Trim();
                        break;
                    case "Password":
                        adminPassword = parts[1].Trim();
                        break;

                }
            }
        }

    }
}
