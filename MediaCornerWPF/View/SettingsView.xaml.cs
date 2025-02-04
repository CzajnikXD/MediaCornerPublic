using MediaCornerWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using MediaCornerWPF.Commands;
using System.Windows.Input;

namespace MediaCornerWPF.View
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public bool IsUsernameGuidelinesEnabled { get; set; } = true;
        public bool IsPasswordGuidelinesEnabled { get; set; } = true;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(); 
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Get the ViewModel and update the password
            if (this.DataContext is SettingsViewModel viewModel)
            {
                viewModel.Password = txtPassword.Password;
                ((RelayCommand<object>)viewModel.ChangePasswordCommand).RaiseCanExecuteChanged();
            }
        }

        private void textUserGuide_Click(object sender, RoutedEventArgs e)
        {
            if (IsUsernameGuidelinesEnabled)
            {
                CustomMessageBox msgBox = new CustomMessageBox(230, 420, TextAlignment.Left);
                msgBox.DataContext = new { Message = "Username Guidelines:\n- New username can't be empty\n- New username can't be your old username\n- New username must be unique" };
                msgBox.ShowDialog();
            }
        }

        private void textPassGuide_Click(object sender, RoutedEventArgs e)
        {
            if (IsPasswordGuidelinesEnabled)
            {
                CustomMessageBox msgBox = new CustomMessageBox(230, 420, TextAlignment.Left);
                msgBox.DataContext = new { Message = "Password Guidelines:\n- New password can't be empty\n- New password can't be your old password" };
                msgBox.ShowDialog();
            }
        }

        private void textUserGuide_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void textUserGuide_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null; 
        }

    }
}
