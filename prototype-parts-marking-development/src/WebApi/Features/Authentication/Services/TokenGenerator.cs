namespace WebApi.Features.Authentication.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using Utilities;
    using WebApi.Common;
    using WebApi.Configuration;

    public class TokenGenerator : ITokenGenerator
    {
        private readonly Authentication authenticationOptions;
        private readonly IDateTime dateTime;

        public TokenGenerator(IOptions<Authentication> authenticationOptions, IDateTime dateTime)
        {
            Guard.NotNull(authenticationOptions, nameof(authenticationOptions));
            Guard.NotNull(dateTime, nameof(dateTime));

            this.authenticationOptions = authenticationOptions.Value;
            this.dateTime = dateTime;
        }

        public GeneratedAccessToken CreateAccessToken(int userId, IEnumerable<string> roles)
        {
            var expiration = dateTime.Now.Add(authenticationOptions.AccessTokenExpiration);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    { "user", userId.ToString() },
                    { "role", roles },
                },
                Expires = expiration.UtcDateTime,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.TokenSigningKey)),
                        SecurityAlgorithms.HmacSha512Signature),
            };

            return new GeneratedAccessToken
            {
                Token = new JsonWebTokenHandler().CreateToken(tokenDescriptor),
                ExpiresAt = expiration,
            };
        }

        public GeneratedRefreshToken CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(128);

            return new GeneratedRefreshToken
            {
                Token = Convert.ToBase64String(bytes),
                CreatedAt = dateTime.Now,
                ExpiresAt = dateTime.Now.Add(authenticationOptions.RefreshTokenExpiration),
            };
        }
    }
}