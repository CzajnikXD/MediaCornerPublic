using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Commands;
using MediaCornerWPF.Lib.Services;
using MediaCornerWPF.View;
using MediaCornerWPF.Lib.MongoDB.Models;
using System.Collections.Concurrent;
using static MediaCornerWPF.ViewModels.HomeViewModel;
using MediaCornerWPF.Lib.API;
using MediaCornerWPF.Lib;
using System.Diagnostics;

namespace MediaCornerWPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly MovieService _movieService;
        public ObservableCollection<MovieModel> Movies { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        private bool _isMessagesLoading;

        public ICommand AddToWatchlistCommand { get; }

        public bool IsMessagesLoading
        {
            get => _isMessagesLoading;
            set
            {
                _isMessagesLoading = value;
                OnPropertyChanged(nameof(IsMessagesLoading));
            }
        }

        public class Message
        {
            public string msg { get; set; }
            public int movieId { get; set; }
        }

        public HomeViewModel()
        {
            _movieService = new MovieService();
            Movies = new ObservableCollection<MovieModel>();
            Messages = new ObservableCollection<Message>();

            AddToWatchlistCommand = new RelayCommand<MovieModel>(AddToWatchlist);

            Mouse.OverrideCursor = Cursors.Wait;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadPopularMoviesAsync();

            _ = Task.Run(async () => await LoadUserWatchlistMessagesAsync());
        }

        private async Task LoadPopularMoviesAsync()
        {
            ApiController.EnsureClientInitialized();
            var movies = await _movieService.GetPopularMoviesAsync();
            Movies.Clear();
            foreach (var movie in movies)
            {
                Movies.Add(movie);
            }
        }

        private async Task LoadUserWatchlistMessagesAsync()
        {
            ApiController.EnsureClientInitialized();

            try
            {
                var friendsList = DB.GetUserFriends(LoggedUser.Id);

                if (friendsList == null || !friendsList.Any())
                {
                    return; 
                }

                var watchlist = await DB.GetOthersWatchlistAsync(LoggedUser.Id);

                if (watchlist == null || !watchlist.Any())
                {
                    return; 
                }

                var userIds = watchlist.Select(movie => movie.UsersId).Distinct().ToList();

                var usernamesDict = await DB.GetUsernamesInBulkAsync(userIds);

                var messageList = new ConcurrentBag<Message>();

                var movieTasks = watchlist.Select(async movie =>
                {
                    var movieModel = await _movieService.GetMovieDetailsAsync(movie.MovieId);
                    var username = usernamesDict.ContainsKey(movie.UsersId) ? usernamesDict[movie.UsersId] : "Unknown User";

                    return new Message
                    {
                        msg = $"{username} added {movieModel.title}",
                        movieId = movieModel.id
                    };
                }).ToList();

                var messages = await Task.WhenAll(movieTasks);

                foreach (var message in messages)
                {
                    messageList.Add(message);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var message in messageList)
                    {
                        Messages.Insert(0, message);
                    }
                });
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                });
            }
        }

        private void AddToWatchlist(MovieModel movie)
        {
            _movieService.AddToWatchlist(LoggedUser.Id, movie.id);
            CustomMessageBox msgBox = new CustomMessageBox
            {
                DataContext = new { Message = "Added movie to watchlist successfully" }
            };
            msgBox.ShowDialog();
        }
    }
}