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
        }
    }
}
