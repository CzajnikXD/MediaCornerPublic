using MediaCornerWPF.Lib.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace MediaCornerWPF.View
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        public bool IsReturnLoginEnabled { get; set; } = true;
        public RegisterView()
        {
            InitializeComponent();
            DB.InitDB();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            bool isCreated = DB.CreateUser(username, password);

            if (isCreated)
            {
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Account created succesfully!" };
                bool? dialogResult = msgBox.ShowDialog();

                if (dialogResult == true)
                {
                    LoginView loginView = new LoginView();
                    loginView.Show();
                    this.Close();
                }
            }
            else
            {
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Username already exists. Please try again with a new username." };
                bool? dialogResult = msgBox.ShowDialog();
            }
        }

        private void textReturnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (IsReturnLoginEnabled)
            {
                LoginView loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }
    }
}