using MediaCornerWPF.Commands;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Lib;
using MediaCornerWPF.View;
using MediaCornerWPF.ViewModels;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace MediaCornerWPF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _profileImageSource;
        public string ProfileImageSource
        {
            get => _profileImageSource;
            set
            {
                _profileImageSource = value;
                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ((RelayCommand<object>)ChangeUsernameCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ((RelayCommand<object>)ChangePasswordCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand ChangeUsernameCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand ChangeProfilePictureCommand { get; }


        public SettingsViewModel()
        {
            ChangeUsernameCommand = new RelayCommand<object>(ChangeUsername, CanChangeUsername);
            ChangePasswordCommand = new RelayCommand<object>(ChangePassword, CanChangePassword);
            ChangeProfilePictureCommand = new RelayCommand<object>(ChangeProfilePicture);

            Username = LoggedUser.Username;
            Password = LoggedUser.Password;
            if (string.IsNullOrEmpty(LoggedUser.ProfilePicture))
            {
                ProfileImageSource = "pack://application:,,,/Images/profile-picture.png";  
            }
            else
            {
                ProfileImageSource = LoggedUser.ProfilePicture;
            }
        }

        private void ChangeUsername(object parameter)
        {
            var success = DB.UpdateUsername(LoggedUser.Id, Username);
            if (success)
            {
                LoggedUser.Username = Username;
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Username successfully updated." };
                msgBox.ShowDialog();

                ((RelayCommand<object>)ChangeUsernameCommand).RaiseCanExecuteChanged();
            }
            else
            {
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Username is already taken. Please choose a different one." };
                msgBox.ShowDialog();
            }
        }

        private bool CanChangeUsername(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && Username != LoggedUser.Username;
        }

        private void ChangePassword(object parameter)
        {
            DB.UpdatePassword(LoggedUser.Id, Password);
            LoggedUser.Password = Password;

            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Password successfully updated." };
            msgBox.ShowDialog();

            ((RelayCommand<object>)ChangePasswordCommand).RaiseCanExecuteChanged();
        }

        private bool CanChangePassword(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Password) && Password != LoggedUser.Password;
        }

        private void ChangeProfilePicture(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png, *.webp)|*.jpg;*.png;*.webp";
            openFileDialog.Title = "Select a Profile Picture";

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 1048576) // 1 MB in bytes
                {
                    CustomMessageBox msgBox = new CustomMessageBox();
                    msgBox.DataContext = new { Message = "The file is too large. Please select an image less than 1 MB." };
                    msgBox.ShowDialog();
                    return;
                }

                // Convert image to base64 string to store in database
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                string base64Image = Convert.ToBase64String(imageBytes);

                DB.UpdateProfilePicture(LoggedUser.Id, base64Image);
                LoggedUser.ProfilePicture = base64Image;
                ProfileImageSource = base64Image; 
            }
        }
    }
}