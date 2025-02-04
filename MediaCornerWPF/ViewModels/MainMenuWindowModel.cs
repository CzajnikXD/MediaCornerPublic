using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FontAwesome.Sharp;
using System.Windows.Input;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Lib;
using MediaCornerWPF.Lib.API;
using System.Diagnostics;

namespace MediaCornerWPF.ViewModels
{
    public class MainMenuWindowModel : ViewModelBase
    {
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;
        private string _username;
        private string _profileImageSource;

        public ViewModelBase CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ProfileImageSource
        {
            get => _profileImageSource;
            set
            {
                _profileImageSource = value;
                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }

        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowWatchlistViewCommand { get; }
        public ICommand ShowMovieViewCommand { get; }
        public ICommand ShowUsersViewCommand { get; }
        public ICommand ShowFriendsViewCommand { get; }
        public ICommand ShowSettingsViewCommand { get; }

        public MainMenuWindowModel()
        {
            LoggedUser.OnUserChanged += OnUserChanged;

            // Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowWatchlistViewCommand = new ViewModelCommand(ExecuteShowWatchlistCommand);
            ShowMovieViewCommand = new ViewModelCommand(ExecuteShowMovieViewCommand);
            ShowUsersViewCommand = new ViewModelCommand(ExecuteShowUsersViewCommand);
            ShowFriendsViewCommand = new ViewModelCommand(ExecuteShowFriendsViewCommand);
            ShowSettingsViewCommand = new ViewModelCommand(ExecuteShowSettingsViewCommand);

            // Default view
            ExecuteShowHomeViewCommand(null);

            Username = LoggedUser.Username;
            if (string.IsNullOrEmpty(LoggedUser.ProfilePicture))
            {
                ProfileImageSource = "pack://application:,,,/Images/profile-picture.png";
            }
            else
            {
                ProfileImageSource = LoggedUser.ProfilePicture;
            }
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void ExecuteShowWatchlistCommand(object obj)
        {
            CurrentChildView = new WatchlistViewModel();
            Caption = "Watchlist";
            Icon = IconChar.Tv;
        }

        private void ExecuteShowMovieViewCommand(object obj)
        {
            CurrentChildView = new MovieViewModel();
            Caption = "Movie";
            Icon = IconChar.Film;
        }

        private void ExecuteShowUsersViewCommand(object obj)
        {
            CurrentChildView = new UsersViewModel();
            Caption = "Users";
            Icon = IconChar.PeopleGroup;
        }

        private void ExecuteShowFriendsViewCommand(object obj)
        {
            CurrentChildView = new FriendsViewModel();
            Caption = "Friends";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowSettingsViewCommand(object obj)
        {
            CurrentChildView = new SettingsViewModel();
            Caption = "Settings";
            Icon = IconChar.Tools;
        }

        private void OnUserChanged(string propertyName)
        {
            if (propertyName == nameof(LoggedUser.Username))
            {
                Username = LoggedUser.Username; 
            }
            else if (propertyName == nameof(LoggedUser.ProfilePicture))
            {
                ProfileImageSource = LoggedUser.ProfilePicture;
            }
        }
    }
}