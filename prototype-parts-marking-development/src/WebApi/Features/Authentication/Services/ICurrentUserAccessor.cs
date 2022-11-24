namespace WebApi.Features.Authentication.Services
{
    public interface ICurrentUserAccessor
    {
        int GetCurrentUser();
    }
}