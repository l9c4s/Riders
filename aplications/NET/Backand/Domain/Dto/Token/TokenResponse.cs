using System;

namespace Domain.Dto.Token
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime expiration { get; set; }
    }
}


