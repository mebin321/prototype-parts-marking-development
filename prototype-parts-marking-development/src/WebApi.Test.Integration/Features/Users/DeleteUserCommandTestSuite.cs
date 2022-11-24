namespace WebApi.Test.Integration.Features.Users
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Shouldly;
    using Microsoft.EntityFrameworkCore;
    using WebApi.Data;
    using WebApi.Features.Users.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class DeleteUserCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public DeleteUserCommandTestSuite(TestingFixture testingFixture)
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
        public async Task Command_ShouldDeactivateExistingUser()
        {
            var fixture = new Fixture();
            var user = fixture
                .Build<User>()
                .With(u => u.DeletedAt, default(DateTimeOffset?))
                .Create();
            await testingFixture.AddAsync(user);
            var command = new DeleteUserCommand { UserId = user.Id };

            await testingFixture.SendAsync(command);

            var users = await testingFixture.ExecuteAsync(c => c.Users.ToListAsync());

            users.Count.ShouldBe(1);
            users[0].DeletedAt.ShouldNotBe(null);
        }

        [Fact]
        public async Task Command_ShouldThrowIfUserDoesNotExist()
        {
            var fixture = new Fixture();
            var user = fixture
                .Build<User>()
                .With(u => u.DeletedAt, default(DateTimeOffset?))
                .With(u => u.CreatedAt, DateTimeOffset.UtcNow)
                .With(u => u.ModifiedAt, DateTimeOffset.UtcNow)
                .Create();
            await testingFixture.AddAsync(user);
            var command = new DeleteUserCommand { UserId = user.Id + 1 };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(command));
        }
    }
}
