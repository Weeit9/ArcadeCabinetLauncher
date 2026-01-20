using System.Windows;
using System.Windows.Controls;
using ArcadeCabinetLauncher.Models;
using Microsoft.Win32;

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
        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog gameToAdd = new OpenFileDialog();
            gameToAdd.ShowDialog();

            bool? success = gameToAdd.ShowDialog();
            if (success == true)
            {

                var game = new GameEntry
                {
                    Name = gameToAdd.FileName,
                    ExecutablePath = gameToAdd.FileName
                };

            }

            else
            {
                //no fill picked
            }
        }
    }
}
