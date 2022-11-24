namespace WebApi.Features.Authentication.Services
{
    using Common.ActiveDirectory;
    using Utilities;

    public class ActiveDirectoryValidator : ICredentialsValidator
    {
        private readonly IActiveDirectory activeDirectory;

        public ActiveDirectoryValidator(IActiveDirectory activeDirectory)
        {
            Guard.NotNull(activeDirectory, nameof(activeDirectory));

            this.activeDirectory = activeDirectory;
        }

        public bool Validate(string username, string password)
        {
            return activeDirectory.ValidateCredentials(username, password);
        }
    }
}