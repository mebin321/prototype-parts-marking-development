namespace WebApi.Features.Authentication.Services
{
    using System;

    public class StubCredentialsValidator : ICredentialsValidator
    {
        public bool Validate(string username, string password)
        {
            return string.Equals(username, password, StringComparison.OrdinalIgnoreCase);
        }
    }
}