using ArcadeCabinetLauncher.Commands;
using ArcadeCabinetLauncher.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows.Input;
using System.IO;

namespace ArcadeCabinetLauncher.ViewModels
{
    class AddGameViewModel : ViewModelBase
    {
        private GameEntry? _editingGame;
        public bool isEditMode => _editingGame != null;
        public string Name { get; set; } = "";
        public string GameMaker { get; set; } = "";
        public string Controller { get; set; } = "";
        public string ExecutablePath { get; set; } = "";
        public string ThumbnailPath { get; set; } = "";
        public string Description { get; set; } = "";
        public string Year { get; set; } = ""; 

        public ICommand BrowseExeCommand { get; }
        public ICommand BrowseThumbnailCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddGameViewModel(GameEntry? gameToEdit = null)
        {
            BrowseExeCommand = new RelayCommand(BrowseExe);
            BrowseThumbnailCommand = new RelayCommand(BrowseThumbnail);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            if (gameToEdit != null)
            {
                _editingGame = gameToEdit;

                Name = gameToEdit.Name;
                GameMaker = gameToEdit.GameMaker;
                Controller = gameToEdit.Controller;
                ExecutablePath = gameToEdit.ExecutablePath;
                ThumbnailPath = gameToEdit.ThumbnailPath;
                Description = gameToEdit.Description;
                Year = gameToEdit.Year;
            }
        }

        public GameEntry? Result { get; private set; }

        private void BrowseExe()
        {
            OpenFileDialog gameToAdd = new OpenFileDialog()
            {
                Filter = "Executables (*.exe)|*.exe"
            };

            bool? success = gameToAdd.ShowDialog();
            if (success == true)
            {
                ExecutablePath = gameToAdd.FileName;
                OnPropertyChanged(nameof(ExecutablePath));
            }

         }

        private void BrowseThumbnail()
        {
            OpenFileDialog gameToAdd = new OpenFileDialog()
            {
                Filter = "Images (*.png)|*.png"
            };

            bool? success = gameToAdd.ShowDialog();
            if (success == true)
            {
                ThumbnailPath = gameToAdd.FileName;
                OnPropertyChanged(nameof(ThumbnailPath));
            }
        }

        //private bool CanSave()
        //{
        //    return !string.IsNullOrWhiteSpace(Name)
        //        && File.Exists(ExecutablePath);
        //}

        public event Action<bool>? RequestClose;
        private void Save()
        {
            if (isEditMode && _editingGame != null)
            {
                // Update existing object
                _editingGame.Name = Name;
                _editingGame.GameMaker = GameMaker;
                _editingGame.Controller = Controller;
                _editingGame.ExecutablePath = ExecutablePath;
                _editingGame.ThumbnailPath = ThumbnailPath;
                _editingGame.Description = Description;
                _editingGame.Year = Year;

                Result = _editingGame;
            }
            else
            {
                // Create new
                Result = new GameEntry
                {
                    Name = Name,
                    GameMaker = GameMaker,
                    Controller = Controller,
                    ExecutablePath = ExecutablePath,
                    ThumbnailPath = ThumbnailPath,
                    Description = Description,
                    Year = Year
                };
            }

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(true);
        }

    }
}
