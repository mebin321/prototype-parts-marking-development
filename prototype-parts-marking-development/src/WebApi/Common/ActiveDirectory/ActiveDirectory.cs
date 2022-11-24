namespace WebApi.Common.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using Utilities;

    public class ActiveDirectory : IActiveDirectory
    {
        private const int SearchLimitUser = 10;

        private const int SearchLimitListUsers = 100;

        private readonly string domain;

        public ActiveDirectory(string domain)
        {
            Guard.NotNull(domain, nameof(domain));

            this.domain = domain;
        }

        public AdUser FindUser(string username, out int count)
        {
            using var context = new PrincipalContext(ContextType.Domain, domain);

            var query = new UserPrincipal(context)
            {
                Enabled = true,
                SamAccountName = username,
            };

            var results = Find(query, SearchLimitUser);

            count = results.Count;

            if (results.Count != 1)
            {
                return null;
            }

            return results[0];
        }

        public List<AdUser> FindUsers(string username, string email)
        {
            using var context = new PrincipalContext(ContextType.Domain, domain);

            var query = new UserPrincipal(context)
            {
                Enabled = true,
            };

            if (!string.IsNullOrWhiteSpace(username))
            {
                query.SamAccountName = $"{username}*";
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query.EmailAddress = $"{email}*";
            }

            return Find(query, SearchLimitListUsers);
        }

        public bool ValidateCredentials(string username, string password)
        {
            using var context = new PrincipalContext(ContextType.Domain, domain);
            return context.ValidateCredentials(username, password);
        }

        private static List<AdUser> Find(UserPrincipal query, int limit)
        {
            using var principalSearcher = new PrincipalSearcher(query);
            if (principalSearcher.GetUnderlyingSearcher() is DirectorySearcher ds)
            {
                // https://stackoverflow.com/questions/23176284/difference-between-principalsearcher-and-directorysearcher
                ds.SizeLimit = limit;
                ds.ClientTimeout = TimeSpan.FromSeconds(5);
                ds.ServerTimeLimit = TimeSpan.FromSeconds(3);
            }

            var results = principalSearcher
                .FindAll()
                .OfType<UserPrincipal>()
                .Select(
                    u => new AdUser
                    {
                        Name = $"{u.GivenName} {u.Surname}",
                        Email = u.EmailAddress,
                        Username = u.SamAccountName,
                    }).ToList();

            return results;
        }
    }
}