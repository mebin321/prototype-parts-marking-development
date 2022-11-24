namespace WebApi.Test.Integration.Features.ProductGroups
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.ProductGroup.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class UpdateProductGroupCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public UpdateProductGroupCommandTestSuite(TestingFixture testingFixture)
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
            var existing = new ProductGroup
            {
                Moniker = "drum-brake-assembly",
                Title = "Drum Brake Assembly",
                Code = "51",
                Description = "Drum Brake Assembly description",
            };
            await testingFixture.AddAsync(existing);

            var command = new UpdateProductGroupCommand
            {
                Moniker = "drum-brake-assembly",
                Description = "updated",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.ProductGroups.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe(existing.Moniker);
            entities[0].Title.ShouldBe(existing.Title);
            entities[0].Code.ShouldBe(existing.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Fact]
        public async Task Command_ShouldThrowNotFoundExceptionForOutletThatDoesNotExist()
        {
            var existing = new ProductGroup
            {
                Moniker = "drum-brake-assembly",
                Title = "Drum Brake Assembly",
                Code = "51",
                Description = "Drum Brake Assembly description",
            };
            await testingFixture.AddAsync(existing);

            var query = new UpdateProductGroupCommand
            {
                Moniker = "mgu",
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
            var command = new UpdateProductGroupCommand
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
            var command = new UpdateProductGroupCommand
            {
                Moniker = "mgu",
                Description = description,
            };
            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
