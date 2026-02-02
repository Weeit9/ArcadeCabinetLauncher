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

        public ICommand RemoveGameCommand { get; }

        public ICommand StartGameCommand { get; }

        public MainWindowViewModel App { get; }

        public MainViewModel(MainWindowViewModel app)
        {
            App = app;

            var loadedGames = _gameService.LoadGames();

            Games = new ObservableCollection<GameEntry>(loadedGames);

            RemoveGameCommand = new RelayCommandGeneric<GameEntry>(RemoveGame);

            StartGameCommand = new RelayCommandGeneric<GameEntry>(StartGame);
        }

        private void RemoveGame(GameEntry game)
        {
            Games.Remove(game);
            _gameService.SaveGames(Games);
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

    }
}
