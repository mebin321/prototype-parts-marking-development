namespace WebApi.Test.Integration.Features.Maintenance
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Maintenance;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class RevokeRefreshTokenCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public RevokeRefreshTokenCommandTestSuite(TestingFixture testingFixture)
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
        public async Task Command_ShouldDoNothingIfTokenDoesNotExist()
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

            var command = new RevokeRefreshTokenCommand { Token = "token2" };

            await testingFixture.SendAsync(command);

            var updated = await testingFixture.ExecuteAsync(async ctx =>
            {
                return await ctx.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == existing.Id);
            });

            updated.RefreshTokens.Count.ShouldBe(1);
            updated.RefreshTokens[0].Token.ShouldBe("token1");
        }

        [Fact]
        public async Task Command_ShouldRemoveRevokedToken()
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
            existing.RefreshTokens.Add(new RefreshToken
            {
                Token = "token2",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(5)
            });

            await testingFixture.AddAsync(existing);

            var command = new RevokeRefreshTokenCommand{Token = "token2"};

            await testingFixture.SendAsync(command);

            var updated = await testingFixture.ExecuteAsync(async ctx =>
            {
                return await ctx.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == existing.Id);
            });

            updated.RefreshTokens.Count.ShouldBe(1);
            updated.RefreshTokens[0].Token.ShouldBe("token1");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyToken(string token)
        {
            var command = new RevokeRefreshTokenCommand
            {
                Token = token
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}