namespace WebApi.Test.Integration.Features.GateLevels
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.GateLevels.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreateGateLevelCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreateGateLevelCommandTestSuite(TestingFixture testingFixture)
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
            var command = new CreateGateLevelCommand
            {
                Title = "Gate Level 80",
                Code = "80",
                Description = "GateLevel description",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("gate-level-80");
            result.Title.ShouldBe(command.Title);
            result.Code.ShouldBe(command.Code);
            result.Description.ShouldBe(command.Description);

            var entities = await testingFixture.ExecuteAsync(c => c.GateLevels.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("gate-level-80");
            entities[0].Title.ShouldBe(command.Title);
            entities[0].Code.ShouldBe(command.Code);
            entities[0].Description.ShouldBe(command.Description);
        }

        [Theory]
        [InlineData("Gate level 80")]
        [InlineData("gate level 80")]
        [InlineData("gATe lEVel 80")]
        public async Task Command_ShouldNormalizeEntityTitleIntoMoniker(string title)
        {
            var command = new CreateGateLevelCommand
            {
                Title = title,
                Code = "80",
                Description = "Gate level 80.",
            };

            var result = await testingFixture.SendAsync(command);
            result.Moniker.ShouldBe("gate-level-80");
            result.Title.ShouldBe(title);
            result.Code.ShouldBe("80");
            result.Description.ShouldBe("Gate level 80.");

            var entities = await testingFixture.ExecuteAsync(c => c.GateLevels.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Moniker.ShouldBe("gate-level-80");
            entities[0].Title.ShouldBe(title);
            entities[0].Code.ShouldBe("80");
            entities[0].Description.ShouldBe("Gate level 80.");
        }

        [Theory]
        [InlineData("Gate Level 80")]
        [InlineData("gate level 80")]
        public async Task Command_ShouldThrowBadRequestForDuplicateTitle(string title)
        {
            var existing = new[]
            {
                new GateLevel
                {
                    Moniker = "gate-level-80",
                    Title = "Gate Level 80",
                    Code = "80",
                    Description = "GateLevel description",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreateGateLevelCommand
            {
                Title = title,
                Code = "81",
                Description = "GateLevel description other.",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Command_Validation_ShouldRejectEmptyTitle(string title)
        {
            var command = new CreateGateLevelCommand
            {
                Title = title,
                Code = "80",
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
            var command = new CreateGateLevelCommand
            {
                Title = "Gate Level",
                Code = code,
                Description = "GateLevel description",
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
            var command = new CreateGateLevelCommand
            {
                Title = "gate_level",
                Code = "10",
                Description = description,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
