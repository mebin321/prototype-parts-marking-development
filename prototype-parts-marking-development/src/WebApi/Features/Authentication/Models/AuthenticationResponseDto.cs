namespace WebApi.Features.Authentication.Models
{
    using WebApi.Features.Users.Models;

    public class AuthenticationResponseDto
    {
        public UserDto User { get; set; }

        public TokenDto AccessToken { get; set; }

        public TokenDto RefreshToken { get; set; }
    }
}