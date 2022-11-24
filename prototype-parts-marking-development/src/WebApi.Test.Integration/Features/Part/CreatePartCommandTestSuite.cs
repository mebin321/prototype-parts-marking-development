namespace WebApi.Test.Integration.Features.Part
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.Parts.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreatePartCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreatePartCommandTestSuite(TestingFixture testingFixture)
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
        public async Task Command_ShouldCreateNewEntity()
        {
            var command = new CreatePartCommand
            {
                Title = "Complete Prototype",
                Code = "00",
                Description = "Part Complete Prototype",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("complete-prototype");
            result.Title.ShouldBe("Complete Prototype");
            result.Code.ShouldBe("00");
            result.Description.ShouldBe("Part Complete Prototype");

            var entities = await testingFixture.ExecuteAsync(c => c.Parts.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("complete-prototype");
            entities[0].Title.ShouldBe("Complete Prototype");
            entities[0].Code.ShouldBe("00");
            entities[0].Description.ShouldBe("Part Complete Prototype");
        }

        [Theory]
        [InlineData("Complete Prototype")]
        [InlineData("ComplEte ProTOType")]
        [InlineData("complete prototype")]
        public async Task Command_ShouldNormalizeEntityTitleIntoMoniker(string title)
        {
            var command = new CreatePartCommand
            {
                Title = title,
                Code = "00",
                Description = "Part Complete Prototype",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("complete-prototype");
            result.Title.ShouldBe(title);
            result.Code.ShouldBe("00");
            result.Description.ShouldBe("Part Complete Prototype");

            var entities = await testingFixture.ExecuteAsync(c => c.Parts.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("complete-prototype");
            entities[0].Title.ShouldBe(title);
            entities[0].Code.ShouldBe("00");
            entities[0].Description.ShouldBe("Part Complete Prototype");
        }

        [Theory]
        [InlineData("Anchor")]
        [InlineData("anchor")]
        public async Task Command_ShouldThrowBadRequestForDuplicateTitle(string title)
        {
            var existing = new[]
            {
                new Part
                {
                    Moniker = "anchor",
                    Title = "Anchor",
                    Code = "33",
                    Description = "Part Anchor",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreatePartCommand
            {
                Title = title,
                Code = "44",
                Description = "Part 44",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyTitle(string title)
        {
            var command = new CreatePartCommand
            {
                Title = title,
                Code = "33",
                Description = "Part X",
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("A ")]
        [InlineData(" A")]
        [InlineData("  ")]
        [InlineData("AAA")]
        [InlineData("123")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectMalformedCode(string code)
        {
            var command = new CreatePartCommand
            {
                Title = "anchor",
                Code = code,
                Description = "Part Anchor",
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
            var command = new CreatePartCommand
            {
                Title = "anchor",
                Code = "33",
                Description = description,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
