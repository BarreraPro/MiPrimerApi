using System.Text.Json;
using System.Net.Http.Headers;

namespace MiPrimerAPI_.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("https://reqres.in/api/users?page=1");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al consumir la API externa de usuarios"); 
    

        return await response.Content.ReadAsStringAsync(); // Devuelve el JSON como texto
        }
    }
}
