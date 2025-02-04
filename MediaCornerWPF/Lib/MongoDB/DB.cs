using MediaCornerWPF.Lib.MongoDB.Models;
using Microsoft.VisualBasic.ApplicationServices;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System;

namespace MediaCornerWPF.Lib.MongoDB
{
    public class DB
    {
        public static string ConnectionString { get; set; }

        public static IMongoDatabase DbName { get; set; }
        public static IMongoCollection<UsersModel> UsersCollection { get; set; }
        public static IMongoCollection<WatchlistedModel> WatchlistCollection { get; set; }

        /// <summary>
        /// Funkcja inicjalizująca połączenie z bazą danych MongoDB.
        /// </summary>
        public static void InitDB()
        {
            Debug.WriteLine("DB INIT");

            ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("Connection string is missing in environment variables.");
            }

            var settings = MongoClientSettings.FromConnectionString(ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            DbName = client.GetDatabase("MediaCorner");
            UsersCollection = DbName.GetCollection<UsersModel>("Users");
            WatchlistCollection = DbName.GetCollection<WatchlistedModel>("Watchlist");
        }

        /// <summary>
        /// Funkcja autoryzująca użytkownika na podstawie nazwy użytkownika i hasła.
        /// </summary>
        /// <param name="username">
        ///     Nazwa użytkownika do autoryzacji.
        /// </param>
        /// <param name="password">
        ///     Hasło użytkownika do autoryzacji.
        /// </param>
        /// <returns>
        ///     True - jeśli autoryzacja się powiodła.
        ///     False - jeśli autoryzacja się nie powiodła.
        /// </returns>
        public static bool AuthorizeUser(string username, string password)
        {
            var user = UsersCollection.Find(x => x.username == username && x.password == password).FirstOrDefault();

            if (user != null)
            {
                LoggedUser.InitUser(user._id.ToString(), user.username, user.password, user.profilePicture);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Funkcja tworząca nowego użytkownika i dodająca go do kolekcji użytkowników.
        /// </summary>
        /// <param name="username">Nazwa użytkownika.</param>
        /// <param name="password">Hasło użytkownika.</param>
        /// <returns>True, jeśli użytkownik został pomyślnie utworzony, false jeśli nazwa użytkownika już istnieje.</returns>
        public static bool CreateUser(string username, string password)
        {
            var existingUser = UsersCollection.Find(x => x.username == username).FirstOrDefault();

            if (existingUser != null)
            {
                return false;
            }

            var newUser = new UsersModel
            {
                _id = ObjectId.GenerateNewId(),
                username = username,
                password = password
            };

            UsersCollection.InsertOne(newUser);
            return true;
        }

        /// <summary>
        /// Funkcja wylogowująca aktualnie zalogowanego użytkownika.
        /// </summary>
        public static void LogoutUser()
        {
            LoggedUser.ClearUser();
            Debug.WriteLine("User logged out successfully.");
        }

        /// <summary>
        /// Funkcja pobierająca nazwę użytkownika na podstawie jego identyfikatora.
        /// </summary>
        /// <param name="UserId">Identyfikator użytkownika.</param>
        /// <returns>Nazwa użytkownika.</returns>
        public static string GetUsername(string UserId)
        {
            var user = UsersCollection.Find(x => x._id.ToString() == UserId).FirstOrDefault();

            return user.username;
        }

        /// <summary>
        /// Funkcja dodająca film do listy "do obejrzenia" użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="movieId">Identyfikator filmu.</param>
        public static void AddToWatchlist(string userId, int movieId)
        {
            var alreadyWatchlisted = WatchlistCollection.Find(x => x.UsersId == userId && x.MovieId == movieId).FirstOrDefault();

            if (alreadyWatchlisted != null)
            {
                return;
            }

            var watchlist = new WatchlistedModel
            {
                UsersId = userId,
                MovieId = movieId,
                Timestamp = DateTime.UtcNow 
            };

            WatchlistCollection.InsertOne(watchlist);
        }

        /// <summary>
        /// Funkcja usuwająca film z listy "do obejrzenia" użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="movieId">Identyfikator filmu.</param>
        public static void RemoveFromWatchlist(string userId, int movieId)
        {
            var watchlist = WatchlistCollection.Find(x => x.UsersId == userId && x.MovieId == movieId).FirstOrDefault();

            if (watchlist == null)
            {
                return;
            }

            WatchlistCollection.DeleteOne(x => x.UsersId == userId && x.MovieId == movieId);
        }

        /// <summary>
        /// Funkcja pobierająca listę "do obejrzenia" użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista filmów dodanych do obejrzenia przez użytkownika.</returns>
        public static List<WatchlistedModel> GetWatchlist(string userId)
        {
            var watchlist = WatchlistCollection.Find(x => x.UsersId == userId).ToList();

            return watchlist;
        }

        /// <summary>
        /// Funkcja pobierająca listę "do obejrzenia" znajomych użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista filmów dodanych do obejrzenia przez znajomych użytkownika.</returns>
        public static async Task<List<WatchlistedModel>> GetOthersWatchlistAsync(string userId)
        {
            var friendsList = GetUserFriends(userId);

            if (friendsList == null || !friendsList.Any())
            {
                return new List<WatchlistedModel>(); 
            }

            var indexKeys = Builders<WatchlistedModel>.IndexKeys
                            .Ascending(x => x.UsersId)
                            .Descending(x => x.Timestamp);
            await WatchlistCollection.Indexes.CreateOneAsync(new CreateIndexModel<WatchlistedModel>(indexKeys));

            var projection = Builders<WatchlistedModel>.Projection
                          .Include(x => x.MovieId)
                          .Include(x => x.UsersId)
                          .Include(x => x.Timestamp);

            var watchlist = await WatchlistCollection
                .Find(x => friendsList.Contains(x.UsersId)) 
                .SortByDescending(x => x.Timestamp)         
                .Limit(20)                                  
                .Project<WatchlistedModel>(projection)      
                .ToListAsync();

            return watchlist;
        }

        /// <summary>
        /// Funkcja masowego pobierania nazw użytkowników na podstawie listy identyfikatorów.
        /// </summary>
        /// <param name="userIds">Lista identyfikatorów użytkowników.</param>
        /// <returns>Słownik z identyfikatorami i odpowiadającymi im nazwami użytkowników.</returns>
        public static async Task<Dictionary<string, string>> GetUsernamesInBulkAsync(List<string> userIds)
        {
            var users = await UsersCollection
                .Find(x => userIds.Contains(x._id.ToString()))
                .Project(x => new { x._id, x.username })  
                .ToListAsync();

            return users.ToDictionary(user => user._id.ToString(), user => user.username);
        }

        /// <summary>
        /// Funkcja pobierająca użytkowników z paginacją i opcjonalnym filtrem wyszukiwania.
        /// </summary>
        /// <param name="page">Numer strony.</param>
        /// <param name="pageSize">Ilość użytkowników na stronie.</param>
        /// <param name="searchText">Opcjonalny tekst wyszukiwania.</param>
        /// <param name="loggedUserId">Identyfikator zalogowanego użytkownika.</param>
        /// <returns>Lista użytkowników pasujących do kryteriów wyszukiwania.</returns>
        public static List<UsersModel> GetUsers(int page, int pageSize, string searchText, string loggedUserId)
        {
            var filter = Builders<UsersModel>.Filter.Empty;

            if (!string.IsNullOrEmpty(searchText))
            {
                filter = Builders<UsersModel>.Filter.Regex("username", new BsonRegularExpression(searchText, "i"));
            }

            var excludeLoggedUserFilter = Builders<UsersModel>.Filter.Ne("_id", ObjectId.Parse(loggedUserId));
            var combinedFilter = Builders<UsersModel>.Filter.And(filter, excludeLoggedUserFilter);

            var users = UsersCollection.Find(combinedFilter)
                                       .Skip(page * pageSize)
                                       .Limit(pageSize)
                                       .ToList();

            return users;
        }

        /// <summary>
        /// Funkcja dodająca znajomego do listy znajomych użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="friendId">Identyfikator znajomego.</param>
        public static void AddFriend(string userId, string friendId)
        {
            var update = Builders<UsersModel>.Update.AddToSet("Friends", friendId);
            UsersCollection.UpdateOne(x => x._id.ToString() == userId, update);
        }

        /// <summary>
        /// Funkcja pobierająca listę znajomych zalogowanego użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista znajomych użytkownika.</returns>
        public static Task<List<UsersModel>> GetFriends(string userId)
        {
            return Task.Run(() =>
            {
                var user = UsersCollection.Find(x => x._id.ToString() == userId).FirstOrDefault();

                if (user == null || user.Friends == null)
                {
                    return new List<UsersModel>();
                }

                var friends = UsersCollection.Find(x => user.Friends.Contains(x._id.ToString())).ToList();
                return friends;
            });
        }

        /// <summary>
        /// Funkcja usuwająca znajomego z listy znajomych użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="friendId">Identyfikator znajomego.</param>
        public static void RemoveFriend(string userId, string friendId)
        {
            var update = Builders<UsersModel>.Update.Pull("Friends", friendId);
            UsersCollection.UpdateOne(x => x._id.ToString() == userId, update);
        }

        /// <summary>
        /// Funkcja aktualizująca nazwę użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="newUsername">Nowa nazwa użytkownika.</param>
        /// <returns>True, jeśli aktualizacja się powiodła, false jeśli nazwa użytkownika już istnieje.</returns>
        public static bool UpdateUsername(string userId, string newUsername)
        {
            var existingUser = UsersCollection.Find(x => x.username == newUsername).FirstOrDefault();
            if (existingUser != null)
            {
                return false; 
            }

            var update = Builders<UsersModel>.Update.Set("username", newUsername);
            UsersCollection.UpdateOne(x => x._id.ToString() == userId, update);
            return true;
        }

        /// <summary>
        /// Funkcja aktualizująca hasło użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="newPassword">Nowe hasło użytkownika.</param>
        public static void UpdatePassword(string userId, string newPassword)
        {
            var update = Builders<UsersModel>.Update.Set("password", newPassword);
            UsersCollection.UpdateOne(x => x._id.ToString() == userId, update);
        }

        /// <summary>
        /// Funkcja sprawdzająca, czy dana nazwa użytkownika jest już zajęta.
        /// </summary>
        /// <param name="username">Nazwa użytkownika do sprawdzenia.</param>
        /// <returns>True, jeśli nazwa użytkownika jest zajęta, false w przeciwnym razie.</returns>
        public static bool IsUsernameTaken(string username)
        {
            var existingUser = UsersCollection.Find(x => x.username == username).FirstOrDefault();
            return existingUser != null; 
        }

        /// <summary>
        /// Funkcja pobierająca listę identyfikatorów znajomych użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista identyfikatorów znajomych użytkownika.</returns>
        public static List<string> GetUserFriends(string userId)
        {
            var user = UsersCollection.Find(x => x._id.ToString() == userId).FirstOrDefault();

            if (user == null || user.Friends == null)
            {
                return new List<string>();
            }

            return user.Friends;
        }

        /// <summary>
        /// Funkcja aktualizująca zdjęcie profilowe użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="newProfilePicture">Nowy URL zdjęcia profilowego.</param>
        public static void UpdateProfilePicture(string userId, string newProfilePicture)
        {
            var update = Builders<UsersModel>.Update.Set("profilePicture", newProfilePicture);
            UsersCollection.UpdateOne(x => x._id.ToString() == userId, update);
        }

    }
}