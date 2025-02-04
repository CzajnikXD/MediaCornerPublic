using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib.API
{
    public static class ApiController
    {
        public static HttpClient? ApiClient { get; set; } = null;

        /// <summary>
        /// Inicjalizuje ApiClient z domyślnymi nagłówkami i konfiguracjami, jeśli nie został już zainicjalizowany.
        /// </summary>
        public static void InitializeClient()
        {
            if (ApiClient == null)
            {
                ApiClient = new HttpClient();
                ApiClient.DefaultRequestHeaders.Accept.Clear();
                ApiClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        /// <summary>
        /// Upewnia się, że ApiClient został zainicjalizowany przed wykonaniem jakichkolwiek żądań.
        /// </summary>
        public static void EnsureClientInitialized()
        {
            if (ApiClient == null)
            {
                throw new InvalidOperationException("ApiClient is not initialized. Call InitializeClient() before making any requests.");
            }
        }
    }
}