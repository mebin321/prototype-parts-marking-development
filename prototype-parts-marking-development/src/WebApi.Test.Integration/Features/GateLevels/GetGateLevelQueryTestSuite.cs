namespace WebApi.Test.Integration.Features.GateLevels
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.GateLevels.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetGateLevelQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetGateLevelQueryTestSuite(TestingFixture testingFixture)
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
        public async Task Query_ShouldReturnExistingEntity()
        {
            var existing = new GateLevel
            {
                Moniker = "gate-level-80",
                Title = "Gate Level 80",
                Code = "80",
                Description = "Gate level 80.",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetGateLevelQuery { Moniker = "gate-level-80" };

            var result = await testingFixture.SendAsync(query);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(existing.Description);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForGateLevelThatDoesNotExist()
        {
            var existing = new GateLevel
            {
                Moniker = "gate-level-80",
                Title = "Gate Level 80",
                Code = "80",
                Description = "Gate level 80.",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetGateLevelQuery { Moniker = "gate-level-81" };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Query_Validation_ShouldRejectEmptyTitle(string title)
        {
            var query = new GetGateLevelQuery { Moniker = title };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(query));
        }
    }
}
