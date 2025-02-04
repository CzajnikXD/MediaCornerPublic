using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using MediaCornerWPF.Lib;
using MediaCornerWPF.Lib.API.Calls;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Commands;
using MediaCornerWPF.Lib.Services;
using MediaCornerWPF.Lib.API;
using MediaCornerWPF.View;
using FontAwesome.Sharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MediaCornerWPF.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
        private readonly MovieService _movieService;
        private ObservableCollection<MovieModel> _movies;
        private string _searchText;
        private int _currentPage = 1;
        private bool _hasNextPage = true;
        private bool _isNextPageAvailable;
        private bool _isPreviousPageAvailable;

        public bool IsNextPageAvailable
        {
            get => _isNextPageAvailable;
            set
            {
                _isNextPageAvailable = value;
                OnPropertyChanged(nameof(IsNextPageAvailable));
            }
        }

        public bool IsPreviousPageAvailable
        {
            get => _isPreviousPageAvailable;
            set
            {
                _isPreviousPageAvailable = value;
                OnPropertyChanged(nameof(IsPreviousPageAvailable));
            }
        }

        public ObservableCollection<MovieModel> Movies
        {
            get => _movies;
            set
            {
                _movies = value;
                OnPropertyChanged(nameof(Movies));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public bool HasNextPage
        {
            get => _hasNextPage;
            set
            {
                _hasNextPage = value;
                OnPropertyChanged(nameof(HasNextPage));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddToWatchlistCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public MovieViewModel()
        {
            _movieService = new MovieService();
            Movies = new ObservableCollection<MovieModel>();

            // Initialize commands
            SearchCommand = new RelayCommand(async () => await SearchMoviesAsync());
            AddToWatchlistCommand = new RelayCommand<MovieModel>(AddToWatchlist);
            PreviousPageCommand = new RelayCommand(async () => await PreviousPageAsync(), CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(async () => await NextPageAsync(), CanGoToNextPage);

            Mouse.OverrideCursor = Cursors.Wait;
            _ = LoadPagedPopularMoviesAsync(CurrentPage);
        }

        private async Task LoadPagedPopularMoviesAsync(int page)
        {
            ApiController.EnsureClientInitialized();
            var movies = await _movieService.GetPagedPopularMoviesAsync(page);

            Movies.Clear();
            foreach (var movie in movies)
            {
                Movies.Add(movie);
            }

            int moviesPerPage = 0;
            HasNextPage = movies.Count > moviesPerPage;
            IsNextPageAvailable = HasNextPage;
            IsPreviousPageAvailable = CurrentPage > 1;

            ((RelayCommand)PreviousPageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextPageCommand).RaiseCanExecuteChanged();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async Task SearchMoviesAsync()
        {
            ApiController.EnsureClientInitialized();
            Movies.Clear();

            if (string.IsNullOrWhiteSpace(SearchText) || SearchText == "Wyszukaj film...")
            {
                await LoadPagedPopularMoviesAsync(1);
            }
            else
            {
                var movies = await _movieService.SearchMoviesAsync(SearchText);
                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }
            }
        }


        private async Task NextPageAsync()
        {
            if (HasNextPage)
            {
                CurrentPage++;;
                await LoadPagedPopularMoviesAsync(CurrentPage);
            }
        }

        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadPagedPopularMoviesAsync(CurrentPage);
            }
        }

        private bool CanGoToNextPage()
        {
            return HasNextPage;
        }

        private bool CanGoToPreviousPage()
        {
            return CurrentPage > 1;
        }


        private void AddToWatchlist(MovieModel movie)
        {
            _movieService.AddToWatchlist(LoggedUser.Id, movie.id);
            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Added movie to watchlist successfully" };
            msgBox.ShowDialog();
        }

    }

}

