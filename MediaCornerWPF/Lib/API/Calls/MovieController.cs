using MediaCornerWPF.Lib.API.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaCornerWPF.Lib.API.Calls
{
    internal class MovieController
    {
        /// <summary>
        /// Funkcja pobierająca listę popularnych filmów z The Movie Database API.
        /// </summary>
        /// <returns>
        ///     Lista obiektów MovieModel reprezentujących popularne filmy.
        /// </returns>
        /// <exception cref="Exception">
        ///     Rzuca wyjątek w przypadku nieudanej próby pobrania danych.
        /// </exception>
        public static async Task<List<MovieModel>> GetPopular()
        {
            string apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("TMDB API Key not found. Please set the environment variable TMDB_API_KEY.");
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.themoviedb.org/3/movie/popular?language=en-US&page=1"),
                Headers =
        {
            { "accept", "application/json" },
            { "Authorization", $"Bearer {apiKey}" },
        },
            };

            using (var response = await ApiController.ApiClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var movielist = JObject.Parse(result)["results"].ToString();

                    List<MovieModel> ret = JsonConvert.DeserializeObject<List<MovieModel>>(movielist);

                    return ret;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Funkcja pobierająca listę popularnych filmów z The Movie Database API z podziałem na strony.
        /// </summary>
        /// <param name="page">
        ///     Numer strony do pobrania.
        /// </param>
        /// <returns>
        ///     Lista obiektów MovieModel reprezentujących popularne filmy z wybranej strony.
        /// </returns>
        /// <exception cref="Exception">
        ///     Rzuca wyjątek w przypadku nieudanej próby pobrania danych.
        /// </exception>
        public static async Task<List<MovieModel>> GetPopularPaged(int page)
        {
            string apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("TMDB API Key not found. Please set the environment variable TMDB_API_KEY.");
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.themoviedb.org/3/movie/popular?language=en-US&page={page}"),
                Headers =
        {
            { "accept", "application/json" },
            { "Authorization", $"Bearer {apiKey}" },
        },
            };

            using (var response = await ApiController.ApiClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var movielist = JObject.Parse(result)["results"].ToString();

                    List<MovieModel> ret = JsonConvert.DeserializeObject<List<MovieModel>>(movielist);

                    return ret;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Funkcja pobierająca szczegóły filmu na podstawie jego identyfikatora z The Movie Database API.
        /// </summary>
        /// <param name="id">
        ///     Identyfikator filmu, którego szczegóły mają zostać pobrane.
        /// </param>
        /// <returns>
        ///     Obiekt MovieModel reprezentujący szczegóły filmu.
        /// </returns>
        /// <exception cref="Exception">
        ///     Rzuca wyjątek w przypadku nieudanej próby pobrania danych.
        /// </exception>
        public static async Task<MovieModel> GetMovie(int id)
        {
            string apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("TMDB API Key not found. Please set the environment variable TMDB_API_KEY.");
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.themoviedb.org/3/movie/{id}?language=en-US"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            };

            using (var response = await ApiController.ApiClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    MovieModel ret = JObject.Parse(result).ToObject<MovieModel>();

                    return ret;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Funkcja wyszukująca filmy na podstawie podanego zapytania w The Movie Database API.
        /// </summary>
        /// <param name="query">
        ///     Zapytanie wyszukiwane.
        /// </param>
        /// <returns>
        ///     Lista obiektów MovieModel reprezentujących wyniki wyszukiwania.
        /// </returns>
        /// <exception cref="Exception">
        ///     Rzuca wyjątek w przypadku nieudanej próby pobrania danych.
        /// </exception>
        public static async Task<List<MovieModel>> SearchMovie(string query)
        {
            string apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("TMDB API Key not found. Please set the environment variable TMDB_API_KEY.");
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.themoviedb.org/3/search/movie?query={query}&include_adult=false&language=en-US&page=1"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            };

            using (var response = await ApiController.ApiClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var movielist = JObject.Parse(result)["results"].ToString();

                    List<MovieModel> ret = JsonConvert.DeserializeObject<List<MovieModel>>(movielist);

                    return ret;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
