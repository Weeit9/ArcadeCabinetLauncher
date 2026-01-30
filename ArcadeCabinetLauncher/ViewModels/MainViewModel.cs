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
        public bool inAdminMode { get; set; }
        public string adminButtonText { get; set; }
        public ICommand AddGameCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand RemoveGameCommand { get; }
        public ICommand SwitchAdminCommand { get; }

        public MainViewModel()
        {
            var loadedGames = _gameService.LoadGames();

            Games = new ObservableCollection<GameEntry>(loadedGames);

            inAdminMode = false;
            adminButtonText = "Enter Admin Mode";

            AddGameCommand = new RelayCommand(AddGame);

            StartGameCommand = new RelayCommandGeneric<GameEntry>(StartGame);

            RemoveGameCommand = new RelayCommandGeneric<GameEntry>(RemoveGame);

            SwitchAdminCommand = new RelayCommand(SwitchAdmin);



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
        }

        private void StartGame(GameEntry game) 
        {
            if (game == null)
                return;

            if (!File.Exists(game.ExecutablePath))
            {
                // optional: handle missing exe
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = game.ExecutablePath,
                UseShellExecute = true
            });
        }

        private void RemoveGame(GameEntry game)
        {
            Games.Remove(game);
            _gameService.SaveGames(Games);
        }

        private void SwitchAdmin()
        {
            if (inAdminMode == false)
            {
                inAdminMode = true;
                adminButtonText = "Exit Admin Mode";

                OnPropertyChanged(nameof(inAdminMode));
                OnPropertyChanged(nameof(adminButtonText));
            }
            else if (inAdminMode == true)
            {
                inAdminMode = false;
                adminButtonText = "Enter Admin Mode";

                OnPropertyChanged(nameof(inAdminMode));
                OnPropertyChanged(nameof(adminButtonText));
            }

        }
    }
}
