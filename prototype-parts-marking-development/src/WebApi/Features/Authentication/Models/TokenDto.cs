namespace WebApi.Features.Authentication.Models
{
    using System;

    public class TokenDto
    {
        public string Token { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }
    }
}