namespace WebApi.Test.Integration.Features.Authentication
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Authentication.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class RefreshAccessTokenCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public RefreshAccessTokenCommandTestSuite(TestingFixture testingFixture)
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
        public async Task Command_ShouldThrowBadRequestIfProvidedTokenDoesNotExist()
        {
            var existing = new User
            {
                Name = "Foo Bar",
                Email = "foo.bar@continental-corporation.com",
                DomainIdentity = "foobar",
                ServiceAccount = false,
            };
            existing.RefreshTokens.Add(new RefreshToken
            {
                Token = "token1",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(5)
            });

            await testingFixture.AddAsync(existing);

            var command = new RefreshAccessTokenCommand { Token = "token2" };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Fact]
        public async Task Command_ShouldThrowBadRequestIfProvidedTokenIsExpired()
        {
            var existing = new User
            {
                Name = "Foo Bar",
                Email = "foo.bar@continental-corporation.com",
                DomainIdentity = "foobar",
                ServiceAccount = false,
            };
            existing.RefreshTokens.Add(new RefreshToken
            {
                Token = "token1",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1)
            });

            await testingFixture.AddAsync(existing);

            var command = new RefreshAccessTokenCommand { Token = "token1" };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Fact]
        public async Task Command_ShouldGenerateANewSetOfTokens()
        {
            var existing = new User
            {
                Name = "Foo Bar",
                Email = "foo.bar@continental-corporation.com",
                DomainIdentity = "foobar",
                ServiceAccount = false,
            };

            existing.RefreshTokens.Add(new RefreshToken
            {
                Token = "token1",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(5)
            });

            await testingFixture.AddAsync(existing);

            var command = new RefreshAccessTokenCommand { Token = "token1" };
            var response = await testingFixture.SendAsync(command);

            response.User.Id.ShouldBe(existing.Id);
            response.User.Name.ShouldBe(existing.Name);
            response.User.Email.ShouldBe(existing.Email);
            response.User.Username.ShouldBe(existing.DomainIdentity);

            response.AccessToken.Token.ShouldNotBeNullOrWhiteSpace();
            response.RefreshToken.Token.ShouldNotBeNullOrWhiteSpace();
            response.RefreshToken.Token.ShouldNotBe("token1");

            var updated = await testingFixture.ExecuteAsync(async ctx =>
            {
                return await ctx.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == existing.Id);
            });

            updated.RefreshTokens.Count.ShouldBe(1);
            updated.RefreshTokens[0].Token.ShouldNotBe("token1");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyToken(string token)
        {
            var command = new RefreshAccessTokenCommand
            {
                Token = token
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}