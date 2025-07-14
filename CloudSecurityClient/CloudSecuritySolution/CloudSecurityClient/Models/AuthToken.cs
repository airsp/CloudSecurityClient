using System;

namespace CloudSecurityClient.Models
{
    public class AuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}