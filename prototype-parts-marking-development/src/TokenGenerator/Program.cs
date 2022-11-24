namespace TokenGenerator
{
    using System;
    using Microsoft.Extensions.Options;
    using WebApi.Common;
    using WebApi.Configuration;
    using WebApi.Features.Authentication.Services;
    using WebApi.Features.Authorization;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Authentication
            {
                AccessTokenExpiration = TimeSpan.FromDays(365),
                RefreshTokenExpiration = TimeSpan.FromDays(365),
                TokenSigningKey = "todochangethistodo",
            };

            var generator = new TokenGenerator(new OptionsWrapper<Authentication>(options), new UtcDateTime());
            var token = generator.CreateAccessToken(1, new[]
            {
                RoleName.SuperAdmin
            });

            Console.WriteLine(token.Token);
        }
    }
}
