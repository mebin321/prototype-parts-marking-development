namespace WebApi.Features.Prototypes.Services
{
    public interface IUrlCreator
    {
        string CreateUrl(string routeName, object values);
    }
}
