using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib
{
    /// <summary>
    /// Klasa dla aktualnie zalogowanego użytkownika
    /// </summary>
    public static class LoggedUser
    {
        public static string Id { get; set; }

        private static string _username;
        public static string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnUserChanged?.Invoke(nameof(Username)); 
                }
            }
        }

        private static string _password;
        public static string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnUserChanged?.Invoke(nameof(Password)); 
                }
            }
        }

        private static string _profilePicture;
        public static string ProfilePicture
        {
            get => _profilePicture;
            set
            {
                if (_profilePicture != value)
                {
                    _profilePicture = value;
                    OnUserChanged?.Invoke(nameof(ProfilePicture)); 
                }
            }
        }

        public static event Action<string> OnUserChanged;

        public static void InitUser(string id, string username, string password, string profilePicture)
        {
            Id = id;
            Username = username;
            Password = password;
            ProfilePicture = profilePicture;
        }

        public static void ClearUser()
        {
            Id = null;
            Username = null;
            Password = null;
            OnUserChanged?.Invoke(null); 
        }
    }
}