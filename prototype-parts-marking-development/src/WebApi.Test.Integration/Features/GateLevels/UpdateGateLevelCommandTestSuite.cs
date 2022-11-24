namespace WebApi.Test.Integration.Features.GateLevels
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.GateLevels.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class UpdateLocationCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public UpdateLocationCommandTestSuite(TestingFixture testingFixture)
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
        public async Task Command_ShouldUpdateDescription()
        {
            var existing = new GateLevel
            {
                Moniker = "gate-level-80",
                Title = "Gate Level 80",
                Code = "80",
                Description = "Gate Level 80",
            };

            await testingFixture.AddAsync(existing);

            var command = new UpdateGateLevelCommand
            {
                Moniker = "gate-level-80",
                Description = "New description.",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.GateLevels.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe(existing.Moniker);
            entities[0].Title.ShouldBe(existing.Title);
            entities[0].Code.ShouldBe(existing.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Fact]
        public async Task Command_ShouldThrowNotFoundExceptionForGateLevelThatDoesNotExist()
        {
            var existing = new GateLevel
            {
                Moniker = "gate-level-80",
                Title = "Gate Level 80",
                Code = "80",
                Description = "Gate Level 80",
            };

            await testingFixture.AddAsync(existing);

            var query = new UpdateGateLevelCommand
            {
                Moniker = "gate-level-70",
                Description = "New description",
            };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyMoniker(string moniker)
        {
            var command = new UpdateGateLevelCommand
            {
                Moniker = moniker,
                Description = "New description",
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyDescription(string description)
        {
            var command = new UpdateGateLevelCommand
            {
                Moniker = "gate-level-80",
                Description = description,
            };
            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
