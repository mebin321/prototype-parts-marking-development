namespace WebApi.Features.Authorization.Policies
{
    public interface IPermissionHandler
    {
        string Name { get; }

        bool AllowedFor(string role);
    }
}