namespace WebApi.Test.Integration.Features.Outlets
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.Outlets.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreateOutletCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreateOutletCommandTestSuite(TestingFixture testingFixture)
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
            var command = new CreateOutletCommand
            {
                Title = "Electric Parking Brake",
                Code = "12",
                Description = "Outlet EPB",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("electric-parking-brake");
            result.Title.ShouldBe(command.Title);
            result.Code.ShouldBe(command.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.Outlets.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("electric-parking-brake");
            entities[0].Title.ShouldBe(command.Title);
            entities[0].Code.ShouldBe(command.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Theory]
        [InlineData("Electric Parking Brake")]
        [InlineData("electric parking brake")]
        [InlineData("EleCtric ParKIng BrakE")]
        public async Task Command_ShouldNormalizeEntityTitleIntoMoniker(string title)
        {
            var command = new CreateOutletCommand
            {
                Title = title,
                Code = "12",
                Description = "Outlet EPB",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("electric-parking-brake");
            result.Title.ShouldBe(command.Title);
            result.Code.ShouldBe(command.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.Outlets.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("electric-parking-brake");
            entities[0].Title.ShouldBe(command.Title);
            entities[0].Code.ShouldBe(command.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Theory]
        [InlineData("Electric Parking Brake")]
        [InlineData("electric parking brake")]
        public async Task Command_ShouldThrowBadRequestForDuplicateTitle(string title)
        {
            var existing = new[]
            {
                new Outlet
                {
                    Moniker = "electric-parking-brake",
                    Title = "Electric Parking Brake",
                    Code = "12",
                    Description = "Outlet EPB",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreateOutletCommand
            {
                Title = title,
                Code = "13",
                Description = "Outlet EPB 2",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyTitle(string title)
        {
            var command = new CreateOutletCommand
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
            var command = new CreateOutletCommand
            {
                Title = "actuation",
                Code = code,
                Description = "Outlet actuation",
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
            var command = new CreateOutletCommand
            {
                Title = "actuation",
                Code = "01",
                Description = description,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
