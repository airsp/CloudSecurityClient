using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CloudSecurityClient.Utilities
{
    public static class SecurityValidator
    {
        private static readonly string ExpectedHash = "EXPECTED_ASSEMBLY_HASH"; // 应替换为实际哈希值

        public static bool VerifyAssemblyIntegrity()
        {
#if DEBUG
            return true; // 调试模式跳过验证
#endif

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("CloudSecurityClient.manifest.sha256");
                if (stream == null) return false;

                using var reader = new StreamReader(stream);
                var manifestHash = reader.ReadToEnd().Trim();

                if (!string.IsNullOrEmpty(ExpectedHash) && manifestHash != ExpectedHash)
                    return false;

                // 计算实际程序集哈希
                using var sha256 = SHA256.Create();
                using var fileStream = File.OpenRead(assembly.Location);
                byte[] hashBytes = sha256.ComputeHash(fileStream);
                string actualHash = BitConverter.ToString(hashBytes).Replace("-", "");

                return actualHash == manifestHash;
            }
            catch
            {
                return false;
            }
        }
    }
}