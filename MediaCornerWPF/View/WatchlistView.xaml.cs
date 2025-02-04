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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Lib;
using MediaCornerWPF.Lib.MongoDB.Models;
using System.Collections.ObjectModel;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.API.Calls;

namespace MediaCornerWPF.View
{
    /// <summary>
    /// Logika interakcji dla klasy WatchlistView.xaml
    /// </summary>
    public partial class WatchlistView : UserControl
    {
        public ObservableCollection<MovieModel> _movies = new ObservableCollection<MovieModel>();
        public ObservableCollection<MovieModel> Movies
        {
            get { return _movies; }
            set { _movies = value; }
        }
        public WatchlistView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var watchlist = await Task.Run(() => DB.GetWatchlist(LoggedUser.Id));

            foreach (WatchlistedModel movie in watchlist)
            {
                var movieModel = await MovieController.GetMovie(movie.MovieId);
                _movies.Add(movieModel);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public async void btnClick(object sender, RoutedEventArgs e)
        {
            MovieModel movieToRemove = (MovieModel)((Button)e.Source).DataContext;

            DB.RemoveFromWatchlist(LoggedUser.Id, movieToRemove.id);

            _movies.Remove(movieToRemove);

            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Removed movie from the watchlist!" };
            msgBox.ShowDialog();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
