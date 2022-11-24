namespace WebApi.Test.Integration.Features.Location
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Locations.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetLocationQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetLocationQueryTestSuite(TestingFixture testingFixture)
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
            var existing = new Location
            {
                Moniker = "auburn-hills",
                Title = "Auburn Hills",
                Code = "AU",
                Description = "Location Auburn Hills",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetLocationQuery { Moniker = "auburn-hills" };

            var result = await testingFixture.SendAsync(query);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(existing.Description);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForLocationThatDoesNotExist()
        {
            var existing = new Location
            {
                Moniker = "auburn-hills",
                Title = "Auburn Hills",
                Code = "AU",
                Description = "Location Auburn Hills",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetLocationQuery { Moniker = "zvolen" };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Query_Validation_ShouldRejectEmptyTitle(string title)
        {
            var query = new GetLocationQuery { Moniker = title };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(query));
        }
    }
}
