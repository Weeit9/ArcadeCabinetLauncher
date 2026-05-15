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
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListView list && list.DataContext is MainViewModel vm)
            {
                if (!vm.GameRunning && vm.SelectedGame != null)
                {
                    vm.StartGameCommand.Execute(vm.SelectedGame);
                }
            }
        }
    }
}
