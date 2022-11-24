namespace WebApi.Test.Integration.Features.EvidenceYear
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using WebApi.Features.EvidenceYears.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CreateEvidenceYearCommandTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public CreateEvidenceYearCommandTestSuite(TestingFixture testingFixture)
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
            var command = new CreateEvidenceYearCommand
            {
                Year = 2020,
                Code = "20",
            };

            var result = await testingFixture.SendAsync(command);
            result.Year.ShouldBe(2020);
            result.Code.ShouldBe("20");

            var entities = await testingFixture.ExecuteAsync(c => c.EvidenceYears.ToListAsync());
            entities.Count.ShouldBe(1);
            entities[0].Year.ShouldBe(2020);
            entities[0].Code.ShouldBe("20");
        }

        [Fact]
        public async Task Command_ShouldThrowBadRequestForDuplicateYear()
        {
            var existing = new[]
            {
                new EvidenceYear
                {
                    Year = 2020,
                    Code = "20",
                },
            };
            await testingFixture.AddRangeAsync(existing);

            var command = new CreateEvidenceYearCommand
            {
                Year = 2020,
                Code = "20",
            };

            await Should.ThrowAsync<BadRequestException>(async () => await testingFixture.SendAsync(command));
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
            var command = new CreateEvidenceYearCommand
            {
                Year = 2020,
                Code = code,
            };

            await Should.ThrowAsync<ModelValidationFailedException>(async ()
                => await testingFixture.SendAsync(command));
        }
    }
}
