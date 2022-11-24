namespace WebApi.Test.Integration.Features.Outlets
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Outlets.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetOutletQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetOutletQueryTestSuite(TestingFixture testingFixture)
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
            var existing = new Outlet
            {
                Moniker = "actuation",
                Title = "actuation",
                Code = "10",
                Description = "Actuation Outlet."
            };
            await testingFixture.AddAsync(existing);

            var query = new GetOutletQuery { Moniker = "actuation" };

            var result = await testingFixture.SendAsync(query);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(existing.Description);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForOutletThatDoesNotExist()
        {
            var existing = new Outlet
            {
                Moniker = "actuation",
                Title = "actuation",
                Code = "10",
                Description = "Actuation Outlet."
            };
            await testingFixture.AddAsync(existing);

            var query = new GetOutletQuery { Moniker = "foundation" };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Query_Validation_ShouldRejectEmptyTitle(string moniker)
        {
            var query = new GetOutletQuery
            {
                Moniker = moniker,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(query));
        }
    }
}
