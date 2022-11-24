namespace WebApi.Test.Integration.Features.EvidenceYear
{
    using System.Threading.Tasks;
    using Shouldly;
    using Data;
    using WebApi.Features.EvidenceYears.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class GetEvidenceYearQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public GetEvidenceYearQueryTestSuite(TestingFixture testingFixture)
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
        public async Task Query_ShouldReturnExistingEntity()
        {
            var existing = new EvidenceYear
            {
                Year = 2020,
                Code = "AU",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetEvidenceYearQuery { Year = 2020 };

            var result = await testingFixture.SendAsync(query);
            result.Year.ShouldBe(existing.Year);
            result.Code.ShouldBe(existing.Code);
        }

        [Fact]
        public async Task Query_ShouldThrowNotFoundExceptionForEvidenceYearThatDoesNotExist()
        {
            var existing = new EvidenceYear
            {
                Year = 2020,
                Code = "20",
            };

            await testingFixture.AddAsync(existing);

            var query = new GetEvidenceYearQuery { Year = 2021 };

            await Should.ThrowAsync<NotFoundException>(async () => await testingFixture.SendAsync(query));
        }
    }
}
