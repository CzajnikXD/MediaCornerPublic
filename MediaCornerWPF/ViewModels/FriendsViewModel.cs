using MediaCornerWPF.Commands;
using MediaCornerWPF.Lib;
using MediaCornerWPF.Lib.MongoDB;
using MediaCornerWPF.Lib.MongoDB.Models;
using MediaCornerWPF.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MediaCornerWPF.ViewModels
{
    public class FriendsViewModel : ViewModelBase
    {
        private ObservableCollection<UsersModel> _friends;
        public ObservableCollection<UsersModel> Friends
        {
            get { return _friends; }
            set
            {
                _friends = value;
                OnPropertyChanged(nameof(Friends));
            }
        }

        public ICommand RemoveFriendCommand { get; }

        public FriendsViewModel()
        {
            Friends = new ObservableCollection<UsersModel>();

            RemoveFriendCommand = new RelayCommand<UsersModel>(RemoveFriend);

            Mouse.OverrideCursor = Cursors.Wait;
            LoadFriends();
        }

        private async void LoadFriends()
        {
            var friends = await Task.Run(() => DB.GetFriends(LoggedUser.Id));

            Friends.Clear();
            foreach (var friend in friends)
            {
                Friends.Add(friend);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void RemoveFriend(UsersModel friend)
        {
            DB.RemoveFriend(LoggedUser.Id, friend._id.ToString());

            Friends.Remove(friend);

            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.DataContext = new { Message = "Removed friend successfully" };
            msgBox.ShowDialog();
        }
    }
}
