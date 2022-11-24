namespace WebApi.Test.Integration.Features.Outlets
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Outlets.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class UpdateOutletCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public UpdateOutletCommandTestSuite(TestingFixture testingFixture)
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
            var existing = new Outlet
            {
                Moniker = "actuation",
                Title = "actuation",
                Code = "10",
                Description = "Actuation Outlet."
            };
            await testingFixture.AddAsync(existing);

            var command = new UpdateOutletCommand
            {
                Moniker = "actuation",
                Description = "updated",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.Outlets.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe(existing.Moniker);
            entities[0].Title.ShouldBe(existing.Title);
            entities[0].Code.ShouldBe(existing.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Fact]
        public async Task Command_ShouldThrowNotFoundExceptionForOutletThatDoesNotExist()
        {
            var existing = new Outlet
            {
                Moniker = "actuation",
                Title = "actuation",
                Code = "10",
                Description = "Actuation Outlet."
            };
            await testingFixture.AddAsync(existing);

            var query = new UpdateOutletCommand
            {
                Moniker = "foundation",
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
            var command = new UpdateOutletCommand
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
            var command = new UpdateOutletCommand
            {
                Moniker = "actuation",
                Description = description,
            };
            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
