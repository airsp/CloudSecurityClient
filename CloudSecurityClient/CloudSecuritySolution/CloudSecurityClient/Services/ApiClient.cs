using CloudSecurityClient.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CloudSecurityClient.Services
{
    //REST API客户端
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public void SetAuthToken(string token)
        {
            _accessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public void ClearAuthToken()
        {
            _accessToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<ApiResponse<T>> SecureApiCallAsync<T>(Func<Task<HttpResponseMessage>> apiCall)
        {
            var response = await apiCall();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // 令牌刷新逻辑
                var newToken = await RefreshTokenAsync();
                if (newToken != null)
                {
                    SetAuthToken(newToken.AccessToken);
                    response = await apiCall(); // 重试请求
                }
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        }

        private async Task<AuthToken> RefreshTokenAsync()
        {
            var refreshToken = SecureStorage.RetrieveRefreshToken();
            if (string.IsNullOrEmpty(refreshToken)) return null;

            var response = await PostAsync<AuthToken>("api/auth/refresh",
                new { RefreshToken = refreshToken });

            if (response != null && !string.IsNullOrEmpty(response.AccessToken))
            {
                SecureStorage.SaveRefreshToken(response.RefreshToken);
                return response;
            }

            return null;
        }
    }
}