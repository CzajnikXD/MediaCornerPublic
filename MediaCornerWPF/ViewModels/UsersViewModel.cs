using MediaCornerWPF.Commands;
using MediaCornerWPF.Lib;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Lib.MongoDB.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.API.Calls;
using MediaCornerWPF.View;


namespace MediaCornerWPF.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private int _currentPage = 0;
        private const int PageSize = 20;

        private ObservableCollection<UsersModel> _users;
        public ObservableCollection<UsersModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText)); 
            }
        }

        public ICommand SearchFriendCommand { get; }
        public ICommand AddFriendCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        private bool _isNextPageAvailable;
        public bool IsNextPageAvailable
        {
            get { return _isNextPageAvailable; }
            set
            {
                _isNextPageAvailable = value;
                OnPropertyChanged(nameof(IsNextPageAvailable)); 
            }
        }

        private bool _isPreviousPageAvailable;
        public bool IsPreviousPageAvailable
        {
            get { return _isPreviousPageAvailable; }
            set
            {
                _isPreviousPageAvailable = value;
                OnPropertyChanged(nameof(IsPreviousPageAvailable)); 
            }
        }

        public UsersViewModel()
        {
            Users = new ObservableCollection<UsersModel>();

            // Initialize commands
            SearchFriendCommand = new RelayCommand(SearchUsers);
            AddFriendCommand = new RelayCommand<UsersModel>(AddFriend);
            NextPageCommand = new RelayCommand(LoadNextPage);
            PreviousPageCommand = new RelayCommand(LoadPreviousPage);

            Mouse.OverrideCursor = Cursors.Wait;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await Task.Run(() => DB.GetUsers(_currentPage, PageSize, _searchText, LoggedUser.Id));

            IsNextPageAvailable = users.Count == PageSize;
            IsPreviousPageAvailable = _currentPage > 0;

            Users.Clear();
            foreach (var user in users)
            {
                if (user.username != LoggedUser.Username)
                {
                    Users.Add(user);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void LoadNextPage()
        {
            _currentPage++;
            LoadUsers();
        }

        private void LoadPreviousPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                LoadUsers();
            }
        }

        private void SearchUsers()
        {
            _currentPage = 0;
            LoadUsers();
        }

        private void AddFriend(UsersModel user)
        {
            DB.AddFriend(LoggedUser.Id, user._id.ToString());
            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Friend added successfully" };
            msgBox.ShowDialog();
        }
    }
}
