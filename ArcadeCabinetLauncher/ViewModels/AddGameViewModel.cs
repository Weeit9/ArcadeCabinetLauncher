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
        public string Name { get; set; } = "";
        public string GameMaker { get; set; } = "";
        public string ExecutablePath { get; set; } = "";
        public string ThumbnailPath { get; set; } = "";
        public string Description { get; set; } = "";

        public ICommand BrowseExeCommand { get; }
        public ICommand BrowseThumbnailCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddGameViewModel()
        {
            BrowseExeCommand = new RelayCommand(BrowseExe);
            BrowseThumbnailCommand = new RelayCommand(BrowseThumbnail);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
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

            Result = new GameEntry
            {
                Name = Name,
                GameMaker = GameMaker,
                ExecutablePath = ExecutablePath,
                ThumbnailPath = ThumbnailPath,
                Description = Description
            };

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(true);
        }

    }
}
