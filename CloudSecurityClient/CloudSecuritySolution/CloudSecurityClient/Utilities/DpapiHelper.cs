using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CloudSecurityClient.Utilities
{
    public static class DpapiHelper
    {
        public static byte[] Protect(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            return ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        }

        public static string Unprotect(byte[] protectedData)
        {
            if (protectedData == null || protectedData.Length == 0) return null;

            byte[] plainBytes = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public static void SecureWipeBytes(byte[] data)
        {
            if (data == null) return;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }
    }
}