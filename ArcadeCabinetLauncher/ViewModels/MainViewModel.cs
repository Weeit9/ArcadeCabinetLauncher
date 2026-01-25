using ArcadeCabinetLauncher.Commands;
using ArcadeCabinetLauncher.Models;
using ArcadeCabinetLauncher.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ArcadeCabinetLauncher.ViewModels
{
    internal class MainViewModel
    {
        private readonly GameService _gameService = new();
        public ObservableCollection<GameEntry> Games { get; }
        public ICommand AddGameCommand { get; }
        public ICommand StartGameCommand { get; }

        public MainViewModel()
        {
            var loadedGames = _gameService.LoadGames();

            Games = new ObservableCollection<GameEntry>(loadedGames);

            AddGameCommand = new RelayCommand(AddGame);

        }

        private void AddGame()
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

                Games.Add(game);
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
    }
}
