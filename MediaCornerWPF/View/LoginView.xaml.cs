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
using System.IO;
using System.Reflection;

namespace MediaCornerWPF.View
{
    /// <summary>
    /// Logika interakcji dla klasy LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public bool IsCreateAccountEnabled { get; set; } = true;

        public LoginView()
        {
            InitializeComponent();
            Console.WriteLine(Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING"));
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
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            bool isAuthorized = DB.AuthorizeUser(username, password);

            if (isAuthorized)
            {
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Logged in successfully" };
                bool? dialogResult = msgBox.ShowDialog(); 

                if (dialogResult == true)
                {
                    MainMenuWindow mainMenuWindow = new MainMenuWindow();
                    mainMenuWindow.Show();
                    this.Close(); 
                }
            }
            else
            {
                CustomMessageBox msgBox = new CustomMessageBox();
                msgBox.DataContext = new { Message = "Invalid username or password. Please try again." };
                msgBox.ShowDialog();
            }
        }

        private void textCreate_Click(object sender, RoutedEventArgs e)
        {
            if (IsCreateAccountEnabled)
            {
                RegisterView registerView = new RegisterView();
                registerView.Show();
                this.Close();
            }
        }
    }
}