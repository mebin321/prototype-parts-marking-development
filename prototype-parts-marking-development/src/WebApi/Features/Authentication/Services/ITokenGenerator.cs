namespace WebApi.Features.Authentication.Services
{
    using System.Collections.Generic;
    using WebApi.Data;

    public interface ITokenGenerator
    {
        GeneratedAccessToken CreateAccessToken(int userId, IEnumerable<string> roles);

        GeneratedRefreshToken CreateRefreshToken();
    }
}