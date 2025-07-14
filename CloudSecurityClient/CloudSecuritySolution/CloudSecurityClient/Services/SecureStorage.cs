using CloudSecurityClient.Utilities;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace CloudSecurityClient.Services
{
    public static class SecureStorage
    {
        private static readonly string StoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CloudSecurityClient",
            "secure.dat");

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("CloudSecuritySalt");

        public static void SaveRefreshToken(string token)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StoragePath));

                byte[] encrypted = ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(token),
                    Entropy,
                    DataProtectionScope.CurrentUser);

                File.WriteAllBytes(StoragePath, encrypted);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"安全存储错误: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string RetrieveRefreshToken()
        {
            if (!File.Exists(StoragePath)) return null;

            try
            {
                byte[] encrypted = File.ReadAllBytes(StoragePath);
                byte[] decrypted = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return null;
            }
        }

        public static void WipeSensitiveData()
        {
            try
            {
                if (File.Exists(StoragePath))
                {
                    // 安全覆盖文件内容
                    byte[] randomData = new byte[1024];
                    new Random().NextBytes(randomData);
                    File.WriteAllBytes(StoragePath, randomData);

                    File.Delete(StoragePath);
                }
            }
            catch { }
        }
    }
}