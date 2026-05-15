using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;

namespace ArcadeCabinetLauncher.Models
{
    public class GameEntry : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }
        private string _gameMaker;
        public string GameMaker
        {
            get => _gameMaker;
            set { _gameMaker = value; OnPropertyChanged(nameof(GameMaker)); }
        }

        private string _controller;
        public string Controller
        {
            get => _controller;
            set { _controller = value; OnPropertyChanged(nameof(Controller)); }
        }

        private string _executablePath;
        public string ExecutablePath
        {
            get => _executablePath;
            set { _executablePath = value; OnPropertyChanged(nameof(ExecutablePath)); }
        }

        private string _thumbnailPath;
        public string ThumbnailPath
        {
            get => _thumbnailPath;
            set { _thumbnailPath = value; OnPropertyChanged(nameof(ThumbnailPath)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string _year;
        public string Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(nameof(Year)); }
        }

        private bool _isHidden;
        public bool isHidden
        {
            get => _isHidden;
            set { _isHidden = value; OnPropertyChanged(nameof(isHidden)); }
        }

        public string DisplayThumbnailPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ThumbnailPath) && File.Exists(ThumbnailPath))
                    return ThumbnailPath;

                return "/Resources/Images/UnrealEngine.png";
            }
        }
    }
}
