using ArcadeCabinetLauncher.Commands;
using ArcadeCabinetLauncher.Models;
using ArcadeCabinetLauncher.Services;
using ArcadeCabinetLauncher.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Runtime.InteropServices;


namespace ArcadeCabinetLauncher.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_RESTORE = 9;


        private readonly GameService _gameService = new();
        public ObservableCollection<GameEntry> Games { get; }


        private bool _inAdminMode;
        public bool inAdminMode
        {
            get => _inAdminMode;
            set
            {
                _inAdminMode = value;
                OnPropertyChanged();
                GamesView.Refresh();
            }
        }


        private ViewMode _currentView;
        public ViewMode CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private ViewMode OldView;

        private GameEntry? _selectedGame;
        public GameEntry? SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged();
            }
        }

        private GameEntry? _oneViewGame;
        public GameEntry? OneViewGame
        {
            get => _oneViewGame;
            set
            {
                _oneViewGame = value;
                OnPropertyChanged();
            }
        }

        private bool _gameRunning;
        public bool GameRunning
        {
            get => _gameRunning;
            set
            {
                _gameRunning = value;
                OnPropertyChanged();
            }
        }

        public string adminButtonText { get; set;  }

        public bool isGameNameVisible { get; set; }
        public bool isDeveloperNameVisible { get; set; }
        public bool isControllerVisible { get; set; }
        public bool isDescriptionVisible { get; set; }
        public bool isYearVisible { get; set; }
        private bool LaunchingGame = false;

        public ICommand AddGameCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand RemoveGameCommand { get; }
        public ICommand ClearGamesCommand { get; }
        public ICommand SwitchAdminCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ScanGamesCommand { get; }
        public ICommand HideGameCommand { get; }
        public ICommand HideAllGamesCommand { get; }
        public ICommand ShowAllGamesCommand { get; }
        public ICommand OneGameViewCommand { get; }
        public ICommand EditGameCommand { get; }
        public ICommand ShowListViewCommand { get; }
        public ICommand ShowCardViewCommand { get; }
        public ICommand SwitchGameNameVisibilityCommand {  get; }
        public ICommand SwitchDeveloperNameVisibilityCommand { get; }
        public ICommand SwitchControllerVisibilityCommand { get; }
        public ICommand SwitchDescriptionVisibilityCommand { get; }
        public ICommand SwitchYearVisibilityCommand { get; }
        public ICommand LeaveFeedbackCommand {  get; }
        public ICommand BackCommand => new RelayCommand(() =>
        {
            CurrentView = OldView;

        });


        public ICollectionView GamesView { get; }

        private bool FilterGames(object obj)
        {
            if (obj is not GameEntry game)
                return false;

            // If admin → show everything
            if (inAdminMode)
                return true;

            // If not admin → hide hidden games
            return !game.isHidden;
        }

        public MainViewModel()
        {
            var loadedGames = _gameService.LoadGames();

            Games = new ObservableCollection<GameEntry>(loadedGames);

            GamesView = CollectionViewSource.GetDefaultView(Games);
            GamesView.Filter = FilterGames;

            inAdminMode = false;
            adminButtonText = "Enter Admin Mode";


            CurrentView = ViewMode.Card;
            isGameNameVisible = true;
            isDeveloperNameVisible = true;
            isControllerVisible = true;
            isDescriptionVisible = true;
            isYearVisible = true;

            GameRunning = false;

            AddGameCommand = new RelayCommand(AddGame);
            StartGameCommand = new RelayCommandGeneric<GameEntry>(StartGame);
            RemoveGameCommand = new RelayCommandGeneric<GameEntry>(RemoveGame);
            ClearGamesCommand = new RelayCommand(ClearGames);
            SwitchAdminCommand = new RelayCommand(SwitchAdmin);
            ExitCommand = new RelayCommand(Exit);
            ScanGamesCommand = new RelayCommand(ScanGames);
            HideGameCommand = new RelayCommandGeneric<GameEntry>(HideGame);
            HideAllGamesCommand = new RelayCommand(HideAllGames);
            ShowAllGamesCommand = new RelayCommand(ShowAllGames);
            OneGameViewCommand = new RelayCommandGeneric<GameEntry>(OneGameView);
            EditGameCommand = new RelayCommandGeneric<GameEntry>(EditGame);
            LeaveFeedbackCommand = new RelayCommandGeneric<GameEntry>(LeaveFeedback);

            ShowListViewCommand = new RelayCommand(ShowListView);
            ShowCardViewCommand = new RelayCommand(ShowCardView);
            SwitchGameNameVisibilityCommand = new RelayCommand(SwitchGameNameVisibility);
            SwitchDeveloperNameVisibilityCommand = new RelayCommand(SwitchDeveloperNameVisibility);
            SwitchControllerVisibilityCommand = new RelayCommand(SwitchControllerVisibility);
            SwitchDescriptionVisibilityCommand = new RelayCommand(SwitchDescriptionVisibility);
            SwitchYearVisibilityCommand = new RelayCommand(SwitchYearVisibility);

        }

        private void AddGame()
        {
            var vm = new AddGameViewModel();
            var window = new AddGameWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true && vm.Result != null)
            {
                Games.Add(vm.Result);
                _gameService.SaveGames(Games);
            }
        }
        private void ScanGames()
        {
            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games");

            var scannedGames = _gameService.ScanGameFolder(rootPath);

            foreach (var game in scannedGames)
            {
                if (!Games.Any(g => g.ExecutablePath == game.ExecutablePath))
                    Games.Add(game);
            }

            _gameService.SaveGames(Games);
        }

        private async void StartGame(GameEntry game)
        {
            if (GameRunning || LaunchingGame)
                return;

            LaunchingGame = true;

            if (!File.Exists(game.ExecutablePath))
            {
                var dialog = new MessageWindow("Executable Not Found");
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();

                LaunchingGame = false;
                return;
            }

            GameRunning = true;

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = game.ExecutablePath,
                UseShellExecute = true
            });

            

            if (process != null)
            {

                var gameWindow = new GameRunningWindow(process);
                gameWindow.Owner = Application.Current.MainWindow;

                _ = Task.Run(() =>
                {
                    process.WaitForExit();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GameRunning = false;

                        gameWindow.Close();

                        LaunchingGame = false;
                    });
                });

                gameWindow.ShowDialog();
            }

            else
            {
                LaunchingGame = false;
            }

            SetForegroundWindow(process.MainWindowHandle);

        }

        private void HideGame(GameEntry game)
        {
            game.isHidden = !game.isHidden;
            _gameService.SaveGames(Games);
            GamesView.Refresh();
        }

        private void HideAllGames()
        {
            foreach (GameEntry game in Games)
            {
                game.isHidden = true;
            }
        }
        private void ShowAllGames()
        {
            foreach (GameEntry game in Games)
            {
                game.isHidden = false;
            }
        }

        private void RemoveGame(GameEntry game)
        {
            Games.Remove(game);
            _gameService.SaveGames(Games);
        }

        private void ClearGames()
        {
            Games.Clear();
            _gameService.SaveGames(Games);
        }

        private void OneGameView(GameEntry game)
        {
            OneViewGame = game;
            OldView = CurrentView;
            CurrentView = ViewMode.SingleGame;
        }

        private void EditGame(GameEntry game)
        {
            var vm = new AddGameViewModel(game);
            var window = new AddGameWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true && vm.Result != null)
            {
                _gameService.SaveGames(Games);
            }


        }

        private void LeaveFeedback(GameEntry game)
        {
            var vm = new LeaveFeedbackViewModel(game.Name);
            var window = new FeedbackWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();

        }

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

                System.Media.SystemSounds.Asterisk.Play();
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

        private void ShowListView()
        {
            CurrentView = ViewMode.List;
        }

        private void ShowCardView()
        {
            CurrentView = ViewMode.Card;
        }

        private void SwitchGameNameVisibility()
        {
            isGameNameVisible = !isGameNameVisible;
            OnPropertyChanged(nameof(isGameNameVisible));
        }

        private void SwitchDeveloperNameVisibility()
        {
            isDeveloperNameVisible = !isDeveloperNameVisible;
            OnPropertyChanged(nameof(isDeveloperNameVisible));
        }

        private void SwitchControllerVisibility()
        {
            isControllerVisible = !isControllerVisible;
            OnPropertyChanged(nameof(isControllerVisible));
        }

        private void SwitchDescriptionVisibility()
        {
            isDescriptionVisible = !isDescriptionVisible;
            OnPropertyChanged(nameof(isDescriptionVisible));
        }

        private void SwitchYearVisibility()
        {
            isYearVisible = !isYearVisible;
            OnPropertyChanged(nameof(isYearVisible));
        }

        private void Exit() 
        {
            Application.Current.Shutdown();
        }
    }
}
