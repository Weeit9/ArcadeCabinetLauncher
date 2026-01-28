using ArcadeCabinetLauncher.Commands;
using ArcadeCabinetLauncher.Models;
using ArcadeCabinetLauncher.Services;
using ArcadeCabinetLauncher.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace ArcadeCabinetLauncher.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly GameService _gameService = new();
        public ObservableCollection<GameEntry> Games { get; }
        public GameEntry SelectedGame { get; set; }
        public ICommand AddGameCommand { get; }
        public ICommand StartGameCommand { get; }

        public MainViewModel()
        {
            var loadedGames = _gameService.LoadGames();

            Games = new ObservableCollection<GameEntry>(loadedGames);

            AddGameCommand = new RelayCommand(AddGame);

            StartGameCommand = new RelayCommand(StartGame, () => true);

        }

        private void AddGame()
        {
            var vm = new AddGameViewModel();
            var window = new AddGameWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true && vm.Result != null)
            {
                Games.Add(vm.Result);
                _gameService.SaveGames(Games);
            }
            //OpenFileDialog gameToAdd = new OpenFileDialog();

            //bool? success = gameToAdd.ShowDialog();
            //if (success == true)
            //{

            //    var game = new GameEntry
            //    {
            //        Name = gameToAdd.FileName,
            //        ExecutablePath = gameToAdd.FileName,
            //        ThumbnailPath = "C:\\Users\\Wyatt\\Pictures\\Screenshots\\Screenshot 2025-05-27 014340.png",
            //    };

            //    Games.Add(game);
            //    _gameService.SaveGames(Games);
            //}
        }

        private void StartGame() 
        {
            if (SelectedGame == null)
                return;

            if (!File.Exists(SelectedGame.ExecutablePath))
            {
                // optional: handle missing exe
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = SelectedGame.ExecutablePath,
                UseShellExecute = true
            });
        }
    }
}
