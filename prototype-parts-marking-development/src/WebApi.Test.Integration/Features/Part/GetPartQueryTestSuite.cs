namespace WebApi.Test.Integration.Features.Part
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Parts.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetPartQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetPartQueryTestSuite(TestingFixture testingFixture)
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
            var existing = new Part
            {
                Moniker = "complete-prototype",
                Title = "Complete Prototype",
                Code = "00",
                Description = "Part Complete Prototype",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetPartQuery { Moniker = "complete-prototype" };

            var result = await testingFixture.SendAsync(query);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(existing.Description);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForPartThatDoesNotExist()
        {
            var existing = new Part
            {
                Moniker = "complete-prototype",
                Title = "Complete Prototype",
                Code = "00",
                Description = "Part Complete Prototype",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetPartQuery { Moniker = "anchor" };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Query_Validation_ShouldRejectEmptyTitle(string title)
        {
            var query = new GetPartQuery { Moniker = title };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(query));
        }
    }
}
