namespace WebApi.Common.ActiveDirectory
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Microsoft.Extensions.Options;
    using Utilities;

    public class ActiveDirectoryProxy : IActiveDirectory
    {
        private readonly IEnumerable<IActiveDirectory> activeDirectories;

        public ActiveDirectoryProxy(IOptions<ActiveDirectoryDomains> activeDirectoryDomains)
        {
            Guard.NotNull(activeDirectoryDomains, nameof(activeDirectoryDomains));

            activeDirectories = activeDirectoryDomains.Value.DomainServices
                .Select(d => new ActiveDirectory(d))
                .ToList();
        }

        public AdUser FindUser(string username, out int count)
        {
            count = 0;

            foreach (var ad in activeDirectories)
            {
                var user = ad.FindUser(username, out int currentCount);

                count = currentCount;
                if (user is not null)
                {
                    return user;
                }

                if (currentCount > 1)
                {
                    break;
                }
            }

            return null;
        }

        public List<AdUser> FindUsers(string username, string email)
        {
            var users = new List<AdUser>();
            foreach (var ad in activeDirectories)
            {
                users.AddRange(ad.FindUsers(username, email));
            }

            return users;
        }

        public bool ValidateCredentials(string username, string password)
        {
            foreach (var ad in activeDirectories)
            {
                if (ad.ValidateCredentials(username, password))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
