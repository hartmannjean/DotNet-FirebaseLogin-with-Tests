using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FirebaseLogin.Network
{
    public class FirebaseService{
        private static readonly string API_KEY = "";
        private readonly HttpClient _httpClient;

        public FirebaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
            public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}";
            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseData);
                return result.idToken;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception("Não autenticado");
            }

        }
        public class FirebaseAuthResponse {
            public string idToken { get; set; }
            public string email { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }
        }
    }
}
