namespace WebApi.Test.Integration.Features.Location
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Locations.Requests;
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
            var existing = new Location
            {
                Moniker = "auburn-hills",
                Title = "Auburn Hills",
                Code = "AU",
                Description = "Location Auburn Hills",
            };

            await testingFixture.AddAsync(existing);

            var command = new UpdateLocationCommand
            {
                Moniker = "auburn-hills",
                Description = "Location Auburn Hills, USA",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe("Location Auburn Hills, USA");

            var entities = await testingFixture.ExecuteAsync(c => c.Locations.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe(existing.Moniker);
            entities[0].Title.ShouldBe(existing.Title);
            entities[0].Code.ShouldBe(existing.Code);
            entities[0].Description.ShouldBe("Location Auburn Hills, USA");
        }

        [Fact]
        public async Task Command_ShouldThrowNotFoundExceptionForLocationThatDoesNotExist()
        {
            var existing = new Location
            {
                Moniker = "auburn-hills",
                Title = "Auburn Hills",
                Code = "AU",
                Description = "Location Auburn Hills",
            };

            await testingFixture.AddAsync(existing);

            var query = new UpdateLocationCommand
            {
                Moniker = "zvolen",
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
            var command = new UpdateLocationCommand
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
            var command = new UpdateLocationCommand
            {
                Moniker = "frankfurt",
                Description = description,
            };
            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
