using ArcadeCabinetLauncher.Commands;
using ArcadeCabinetLauncher.Services;
using ArcadeCabinetLauncher.Windows;
using System.CodeDom;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace ArcadeCabinetLauncher.ViewModels
{
    public class AdminLoginViewModel : ViewModelBase
    {
        private readonly GameService _gameService = new();
        public string Username { get; set; } = "";


        public ICommand CancelCommand { get; }

        public bool IsAuthenticated { get; private set; }

        
        public AdminLoginViewModel()
        {
            CancelCommand = new RelayCommand(Cancel);
        }

        public event Action<bool>? RequestClose;

        public void Login(string password)
        {
            if (ValidateCredentials(Username, password))
            {
                IsAuthenticated = true;
                RequestClose?.Invoke(true);
            }
            else
            {
                var dialog = new MessageWindow("Invalid username or password");
                dialog.Owner = Application.Current.MainWindow;
                System.Media.SystemSounds.Asterisk.Play();

                dialog.ShowDialog();

                if (!dialog.Result) 
                {
                    RequestClose?.Invoke(true);
                }
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // 🔒 TEMP: replace later with hash or config file

            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(rootPath);

            return username == _gameService.adminUsername && password == _gameService.adminPassword;
        }

        private void Cancel()
        {
            RequestClose?.Invoke(true);
        }
    }
}