namespace WebApi.Common.ActiveDirectory
{
    using System.Collections.Generic;

    public interface IActiveDirectory
    {
        AdUser FindUser(string username, out int count);

        List<AdUser> FindUsers(string username, string email);

        bool ValidateCredentials(string username, string password);
    }
}
