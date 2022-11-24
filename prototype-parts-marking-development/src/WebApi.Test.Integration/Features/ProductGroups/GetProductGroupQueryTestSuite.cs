namespace WebApi.Test.Integration.Features.ProductGroups
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.ProductGroup.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetProductGroupQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetProductGroupQueryTestSuite(TestingFixture testingFixture)
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
            var existing = new ProductGroup
            {
                Moniker = "drum-brake-assembly",
                Title = "Drum Brake Assembly",
                Code = "51",
                Description = "Drum Brake Assembly description",
            };
            await testingFixture.AddAsync(existing);

            var query = new GetProductGroupQuery { Moniker = "drum-brake-assembly" };

            var result = await testingFixture.SendAsync(query);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(existing.Description);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForOutletThatDoesNotExist()
        {
            var existing = new ProductGroup
            {
                Moniker = "drum-brake-assembly",
                Title = "Drum Brake Assembly",
                Code = "51",
                Description = "Drum Brake Assembly description",
            };
            await testingFixture.AddAsync(existing);

            var query = new GetProductGroupQuery { Moniker = "mgu" };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Query_Validation_ShouldRejectEmptyTitle(string moniker)
        {
            var query = new GetProductGroupQuery
            {
                Moniker = moniker,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(query));
        }
    }
}
