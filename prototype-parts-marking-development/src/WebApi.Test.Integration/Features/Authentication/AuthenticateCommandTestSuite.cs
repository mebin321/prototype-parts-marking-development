namespace WebApi.Test.Integration.Features.Authentication
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Authentication.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class AuthenticateCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public AuthenticateCommandTestSuite(TestingFixture testingFixture)
        {
            this.testingFixture = testingFixture;
        }

        public async Task InitializeAsync()
        {
            await testingFixture.ResetState();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Command_ShouldRespondWithTokenUponSuccessfulAuthentication()
        {
            var existing = new User
            {
                Name = "Foo Bar",
                Email = "foo.bar@continental-corporation.com",
                DomainIdentity = "foobar",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existing);

            var command = new AuthenticateCommand
            {
                Username = "foobar",
                Password = "foobar"
            };

            var response = await testingFixture.SendAsync(command);

            response.User.Id.ShouldBe(existing.Id);
            response.User.Name.ShouldBe(existing.Name);
            response.User.Email.ShouldBe(existing.Email);
            response.User.Username.ShouldBe(existing.DomainIdentity);
            
            response.AccessToken.Token.ShouldNotBeNullOrWhiteSpace();
            response.RefreshToken.Token.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Command_ShouldThrowNotAuthorizedForNonExistentUser()
        {
            var existing = new[]
            {
                new User
                {
                    Name = "Foo Bar",
                    Email = "foo.bar@continental-corporation.com",
                    DomainIdentity = "foobar",
                    ServiceAccount = true,
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var command = new AuthenticateCommand
            {
                Username = "foobar2",
                Password = "foobar2"
            };

            await Should.ThrowAsync<NotAuthorizedException>(async () => await testingFixture.SendAsync(command));
        }

        [Fact]
        public async Task Command_ShouldThrowNotAuthorizedForServiceUser()
        {
            var existing = new []
            {
                new User
                {
                    Name = "Foo Bar",
                    Email = "foo.bar@continental-corporation.com",
                    DomainIdentity = "foobar",
                    ServiceAccount = true,
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var command = new AuthenticateCommand
            {
                Username = "foobar",
                Password = "foobar"
            };

            await Should.ThrowAsync<NotAuthorizedException>(async () => await testingFixture.SendAsync(command));
        }

        [Fact]
        public async Task Command_ShouldThrowNotAuthorizedForInvalidPassword()
        {
            var existing = new[]
            {
                new User
                {
                    Name = "Foo Bar",
                    Email = "foo.bar@continental-corporation.com",
                    DomainIdentity = "foobar",
                    ServiceAccount = true,
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var command = new AuthenticateCommand
            {
                Username = "foobar",
                Password = "invalid"
            };

            await Should.ThrowAsync<NotAuthorizedException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyUsername(string username)
        {
            var command = new AuthenticateCommand
            {
                Username = username,
                Password = "foobar"
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyPassword(string password)
        {
            var command = new AuthenticateCommand
            {
                Username = "foobar",
                Password = password
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
