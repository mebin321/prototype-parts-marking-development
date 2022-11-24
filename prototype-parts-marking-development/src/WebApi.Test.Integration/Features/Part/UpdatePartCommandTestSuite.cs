namespace WebApi.Test.Integration.Features.Part
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Parts.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class UpdatePartCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public UpdatePartCommandTestSuite(TestingFixture testingFixture)
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
            var existing = new Part
            {
                Moniker = "complete-prototype",
                Title = "Complete Prototype",
                Code = "00",
                Description = "Part Complete Prototype",
            };

            await testingFixture.AddAsync(existing);

            var command = new UpdatePartCommand
            {
                Moniker = "complete-prototype",
                Description = "Part Complete Prototype, Edited",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe(existing.Moniker);
            result.Title.ShouldBe(existing.Title);
            result.Code.ShouldBe(existing.Code);
            result.Description.ShouldBe("Part Complete Prototype, Edited");

            var entities = await testingFixture.ExecuteAsync(c => c.Parts.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe(existing.Moniker);
            entities[0].Title.ShouldBe(existing.Title);
            entities[0].Code.ShouldBe(existing.Code);
            entities[0].Description.ShouldBe("Part Complete Prototype, Edited");
        }

        [Fact]
        public async Task Command_ShouldThrowNotFoundExceptionForPartThatDoesNotExist()
        {
            var existing = new Part
            {
                Moniker = "complete-prototype",
                Title = "Complete Prototype",
                Code = "00",
                Description = "Part Complete Prototype",
            };

            await testingFixture.AddAsync(existing);

            var query = new UpdatePartCommand
            {
                Moniker = "anchor",
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
            var command = new UpdatePartCommand
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
            var command = new UpdatePartCommand
            {
                Moniker = "complete-prototype",
                Description = description,
            };
            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
