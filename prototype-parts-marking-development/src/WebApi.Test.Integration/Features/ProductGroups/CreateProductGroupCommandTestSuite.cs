namespace WebApi.Test.Integration.Features.ProductGroups
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.ProductGroup.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreateProductGroupCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreateProductGroupCommandTestSuite(TestingFixture testingFixture)
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
            var command = new CreateProductGroupCommand
            {
                Title = "Drum Brake Assembly",
                Code = "51",
                Description = "Drum Brake Assembly description",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("drum-brake-assembly");
            result.Title.ShouldBe(command.Title);
            result.Code.ShouldBe(command.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.ProductGroups.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("drum-brake-assembly");
            entities[0].Title.ShouldBe(command.Title);
            entities[0].Code.ShouldBe(command.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Theory]
        [InlineData("Drum Brake Assembly")]
        [InlineData("drum brake assembly")]
        [InlineData("Drum BraKE AssEmbly")]
        public async Task Command_ShouldNormalizeEntityTitleIntoMoniker(string title)
        {
            var command = new CreateProductGroupCommand
            {
                Title = title,
                Code = "51",
                Description = "Drum Brake Assembly description",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("drum-brake-assembly");
            result.Title.ShouldBe(command.Title);
            result.Code.ShouldBe(command.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.ProductGroups.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("drum-brake-assembly");
            entities[0].Title.ShouldBe(command.Title);
            entities[0].Code.ShouldBe(command.Code);
            entities[0].Description.ShouldBe(command.Description);
        }


        [Theory]
        [InlineData("Drum Brake Assembly")]
        [InlineData("drum brake assembly")]
        public async Task Command_ShouldThrowBadRequestForDuplicateTitle(string title)
        {
            var existing = new[]
            {
                new ProductGroup
                {
                    Moniker = "drum-brake-assembly",
                    Title = "Drum Brake Assembly",
                    Code = "51",
                    Description = "Drum Brake Assembly description",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreateProductGroupCommand
            {
                Title = title,
                Code = "52",
                Description = "Drum Brake Assembly description 2",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyTitle(string title)
        {
            var command = new CreateProductGroupCommand
            {
                Title = title,
                Code = "01",
                Description = "Description X",
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1 ")]
        [InlineData(" 1")]
        [InlineData("123")]
        [InlineData("1 3")]
        [InlineData("AA")]
        [InlineData("AAA")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectMalformedCode(string code)
        {
            var command = new CreateProductGroupCommand
            {
                Title = "brake",
                Code = code,
                Description = "ProductGroup Brake",
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
            var command = new CreateProductGroupCommand
            {
                Title = "brake",
                Code = "01",
                Description = description,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
