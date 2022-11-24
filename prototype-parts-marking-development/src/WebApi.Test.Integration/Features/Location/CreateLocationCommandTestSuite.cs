namespace WebApi.Test.Integration.Features.Location
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.Locations.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreateLocationCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreateLocationCommandTestSuite(TestingFixture testingFixture)
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
            var command = new CreateLocationCommand
            {
                Title = "Zvolen",
                Code = "ZV",
                Description = "Location Zvolen",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("zvolen");
            result.Title.ShouldBe("Zvolen");
            result.Code.ShouldBe("ZV");
            result.Description.ShouldBe("Location Zvolen");

            var entities = await testingFixture.ExecuteAsync(c => c.Locations.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("zvolen");
            entities[0].Title.ShouldBe("Zvolen");
            entities[0].Code.ShouldBe("ZV");
            entities[0].Description.ShouldBe("Location Zvolen");
        }

        [Theory]
        [InlineData("Auburn Hills")]
        [InlineData("AubUrn HiLLs")]
        [InlineData("auburn hills")]
        public async Task Command_ShouldNormalizeEntityTitleIntoMoniker(string title)
        {
            var command = new CreateLocationCommand
            {
                Title = title,
                Code = "AU",
                Description = "Location Auburn Hills",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("auburn-hills");
            result.Title.ShouldBe(title);
            result.Code.ShouldBe("AU");
            result.Description.ShouldBe("Location Auburn Hills");

            var entities = await testingFixture.ExecuteAsync(c => c.Locations.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("auburn-hills");
            entities[0].Title.ShouldBe(title);
            entities[0].Code.ShouldBe("AU");
            entities[0].Description.ShouldBe("Location Auburn Hills");
        }

        [Theory]
        [InlineData("Zvolen")]
        [InlineData("zvolen")]
        public async Task Command_ShouldThrowBadRequestForDuplicateTitle(string title)
        {
            var existing = new[]
            {
                new Location
                {
                    Moniker = "zvolen",
                    Title = "Zvolen",
                    Code = "ZV",
                    Description = "Location Zvolen",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreateLocationCommand
            {
                Title = title,
                Code = "ZX",
                Description = "Location ZX",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyTitle(string title)
        {
            var command = new CreateLocationCommand
            {
                Title = title,
                Code = "ZV",
                Description = "Location X",
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
            var command = new CreateLocationCommand
            {
                Title = "zvolen",
                Code = code,
                Description = "Location Zvolen",
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
            var command = new CreateLocationCommand
            {
                Title = "zvolen",
                Code = "ZV",
                Description = description,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
