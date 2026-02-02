using ArcadeCabinetLauncher.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace ArcadeCabinetLauncher.ViewModels
{
    public class AdminLoginViewModel : ViewModelBase
    {
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
                MessageBox.Show("Invalid username or password",
                    "Login Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // 🔒 TEMP: replace later with hash or config file
            return username == "admin" && password == "1234";
        }

        private void Cancel()
        {
            RequestClose?.Invoke(true);
        }
    }
}