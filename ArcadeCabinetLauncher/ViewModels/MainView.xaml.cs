using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using ArcadeCabinetLauncher.Models;
using Microsoft.Win32;
using ArcadeCabinetLauncher.Services;
using System.Collections.ObjectModel;

namespace ArcadeCabinetLauncher.ViewModels
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    

    public partial class MainView : Page
    {
        private List<GameEntry> games = new();
        private string appDataPath;
        private string gamesFilePath;

        public ObservableCollection<GameEntry> GamesList { get; } = new();

        public MainView()
        {
            InitializeComponent();
            DataContext = this;

            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ArcadeCabinetLauncher"); //Creates new folder in appData so that Windows does not ask for permission when writing files
            Directory.CreateDirectory(appDataPath);                                                                                                //Creates new folder in appData so that Windows does not ask for permission when writing files

            gamesFilePath = Path.Combine(appDataPath, "games.json");

            if (!File.Exists(gamesFilePath)) // Makes games.json in appData if does not already exist
            {
                var emptyList = new List<GameEntry>();

                string json = JsonSerializer.Serialize(
                    emptyList,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                File.WriteAllText(gamesFilePath, json);
            }
            

            string jsonContent = File.ReadAllText(gamesFilePath);
            games = JsonSerializer.Deserialize<List<GameEntry>>(jsonContent) ?? new List<GameEntry>(); //Get json data and deserialize

            foreach (var game in games)
                GamesList.Add(game);
        }


        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog gameToAdd = new OpenFileDialog();

            bool? success = gameToAdd.ShowDialog();
            if (success == true)
            {

                var game = new GameEntry
                {
                    Name = gameToAdd.FileName,
                    ExecutablePath = gameToAdd.FileName
                };
                
                games.Add(game);
                GamesList.Add(game);

                string json = JsonSerializer.Serialize(
                    games,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(gamesFilePath, json);
            }

            else
            {
                //no fill picked
            }
        }
    }
}
