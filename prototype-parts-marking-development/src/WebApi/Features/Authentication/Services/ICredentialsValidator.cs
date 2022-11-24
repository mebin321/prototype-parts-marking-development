namespace WebApi.Features.Authentication.Services
{
    public interface ICredentialsValidator
    {
        bool Validate(string username, string password);
    }
}
