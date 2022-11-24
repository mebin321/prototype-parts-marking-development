namespace WebApi.Configuration
{
    using System;

    public class Authentication
    {
        public bool UseAd { get; set; }

        public string TokenSigningKey { get; set; }

        public TimeSpan RefreshTokenExpiration { get; set; }

        public TimeSpan AccessTokenExpiration { get; set; }
    }
}