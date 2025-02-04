using MediaCornerWPF.Lib.API.Calls;
using MediaCornerWPF.Lib.API.Models;
using MediaCornerWPF.Lib.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib.Services
{
    public class MovieService
    {
        /// <summary>
        /// Pobiera listę popularnych filmów z The Movie Database API.
        /// </summary>
        /// <returns>
        /// Lista obiektów MovieModel reprezentujących popularne filmy.
        /// </returns>
        public async Task<List<MovieModel>> GetPopularMoviesAsync()
        {
            return await MovieController.GetPopular();
        }

        /// <summary>
        /// Pobiera listę popularnych filmów z podziałem na strony z The Movie Database API.
        /// </summary>
        /// <param name="page">
        /// Numer strony do pobrania.
        /// </param>
        /// <returns>
        /// Lista obiektów MovieModel reprezentujących popularne filmy z wybranej strony.
        /// </returns>
        public async Task<List<MovieModel>> GetPagedPopularMoviesAsync(int page)
        {
            return await MovieController.GetPopularPaged(page);
        }

        /// <summary>
        /// Wyszukuje filmy na podstawie zapytania w The Movie Database API.
        /// </summary>
        /// <param name="query">
        /// Wyszukiwane zapytanie
        /// </param>
        /// <returns>
        /// Lista obiektów MovieModel reprezentujących wyniki wyszukiwania.
        /// </returns>
        public async Task<List<MovieModel>> SearchMoviesAsync(string query)
        {
            return await MovieController.SearchMovie(query);
        }

        /// <summary>
        /// Dodaje film do listy "do obejrzenia" w bazie danych użytkownika.
        /// </summary>
        /// <param name="userId">
        /// Identyfikator użytkownika.
        /// </param>
        /// <param name="movieId">
        /// Identyfikator filmu do dodania do listy.
        /// </param>
        public void AddToWatchlist(string userId, int movieId)
        {
            DB.AddToWatchlist(userId, movieId);
        }

        /// <summary>
        /// Pobiera szczegóły filmu na podstawie jego identyfikatora z The Movie Database API.
        /// </summary>
        /// <param name="movieId">
        /// Identyfikator filmu, którego szczegóły mają zostać pobrane.
        /// </param>
        /// <returns>
        /// Obiekt MovieModel reprezentujący szczegóły filmu.
        /// </returns>
        public async Task<MovieModel> GetMovieDetailsAsync(int movieId)
        {
            return await MovieController.GetMovie(movieId);
        }
    }
}
