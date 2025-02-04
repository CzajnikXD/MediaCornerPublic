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
using System.Runtime.InteropServices;
using System.Runtime;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.API.Calls;
using MediaCornerWPF.Lib.API;
using MediaCornerWPF.Lib.MongoDB;

namespace MediaCornerWPF.View
{
    public partial class MainMenuWindow : Window
    {
        public ObservableCollection<MovieModel> PopularMovies = new();
        public MainMenuWindow()
        {
            DataContext = this;
            InitializeComponent();
            DB.InitDB();
            ApiController.InitializeClient();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ll = await MovieController.GetPopular();

            foreach (var item in ll)
            {
                PopularMovies.Add(item);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseButtonEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void pnlControlBar_MouseEnter_1(object sender, MouseEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DB.LogoutUser();
            Application.Current.Shutdown();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ShowLogoutPopup(object sender, RoutedEventArgs e)
        {
            LogoutPopup.IsOpen = !LogoutPopup.IsOpen;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DB.LogoutUser();

            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Logged out succesfully!" };
            bool? dialogResult = msgBox.ShowDialog();

            if (dialogResult == true)
            {
                LoginView loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }
    }
}