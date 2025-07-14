using CloudSecurityClient.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CloudSecurityClient.Services
{
    //2FA认证服务
    public class AuthService
    {
        private readonly ApiClient _apiClient;
        private AuthToken _currentToken;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> Authenticate(string username, string password, string totpCode)
        {
            try
            {
                var request = new
                {
                    Username = username,
                    Password = password,
                    TotpCode = totpCode
                };

                var response = await _apiClient.PostAsync<AuthToken>("api/auth/login", request);

                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                {
                    return false;
                }

                _currentToken = response;
                _apiClient.SetAuthToken(_currentToken.AccessToken);
                SecureStorage.SaveRefreshToken(_currentToken.RefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"认证失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool TrySilentLogin()
        {
            try
            {
                var refreshToken = SecureStorage.RetrieveRefreshToken();
                if (string.IsNullOrEmpty(refreshToken)) return false;

                var request = new { RefreshToken = refreshToken };
                var response = _apiClient.PostAsync<AuthToken>("api/auth/refresh", request).Result;

                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                {
                    return false;
                }

                _currentToken = response;
                _apiClient.SetAuthToken(_currentToken.AccessToken);
                SecureStorage.SaveRefreshToken(_currentToken.RefreshToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            _currentToken = null;
            _apiClient.ClearAuthToken();
            SecureStorage.WipeSensitiveData();
        }
    }
}