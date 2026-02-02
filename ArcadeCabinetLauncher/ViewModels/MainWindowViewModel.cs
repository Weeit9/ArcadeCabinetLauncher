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
    internal class MainWindowViewModel : ViewModelBase
    {
        public bool inAdminMode { get; set; }
        public string adminButtonText { get; set; }
        public ICommand AddGameCommand { get; }
        public ICommand StartGameCommand { get; }
    
        public ICommand SwitchAdminCommand { get; }
        public ICommand ExitCommand { get; }



        public MainWindowViewModel()
        {

            inAdminMode = false;

            adminButtonText = "Enter Admin Mode";

            //AddGameCommand = new RelayCommand(AddGame);

            

            

            SwitchAdminCommand = new RelayCommand(SwitchAdmin);

            ExitCommand = new RelayCommand(Exit);

        }

        //private void AddGame()
        //{
        //    var vm = new AddGameViewModel();
        //    var window = new AddGameWindow
        //    {
        //        DataContext = vm,
        //        Owner = Application.Current.MainWindow
        //    };

        //    if (window.ShowDialog() == true && vm.Result != null)
        //    {
        //        Games.Add(vm.Result);
        //        _gameService.SaveGames(Games);
        //    }
        //}

        

        

        private void SwitchAdmin()
        {
            if (inAdminMode == false)
            {
                var vm = new AdminLoginViewModel();
                var window = new AdminLoginWindow
                {
                    DataContext = vm,
                    Owner = Application.Current.MainWindow
                };

                if (window.ShowDialog() == true && vm.IsAuthenticated == true)
                {
                    inAdminMode = true;
                    adminButtonText = "Exit Admin Mode";

                    OnPropertyChanged(nameof(inAdminMode));
                    OnPropertyChanged(nameof(adminButtonText));
                }
            }
            else
            {
                inAdminMode = false;
                adminButtonText = "Enter Admin Mode";

                OnPropertyChanged(nameof(inAdminMode));
                OnPropertyChanged(nameof(adminButtonText));
            }

        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
