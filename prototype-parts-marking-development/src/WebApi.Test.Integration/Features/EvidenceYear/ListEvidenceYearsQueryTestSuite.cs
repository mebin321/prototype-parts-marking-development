namespace WebApi.Test.Integration.Features.EvidenceYear
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.EvidenceYears.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListEvidenceYearsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListEvidenceYearsQueryTestSuite(TestingFixture testingFixture)
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
        public async Task Query_ShouldReturnExistingEntities()
        {
            var existing = new[]
            {
                new EvidenceYear
                {
                    Year = 2020,
                    Code = "22",
                },
                new EvidenceYear
                {
                    Year = 2021,
                    Code = "21",
                }, 
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListEvidenceYearsQuery());
            result.Items.Count.ShouldBe(2);

            result.Items[0].Year.ShouldBe(existing[0].Year);
            result.Items[0].Code.ShouldBe(existing[0].Code);

            result.Items[1].Year.ShouldBe(existing[1].Year);
            result.Items[1].Code.ShouldBe(existing[1].Code);
        }

        /*[Fact]
        public async Task Query_ShouldPerformStartTextSearchWithTheProvidedParameter()
        {
            var existing = new[]
            {
                new EvidenceYear
                {
                    Year = 2019,
                    Code = "19",
                },
                new EvidenceYear
                {
                    Year = 2020,
                    Code = "ZV",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListEvidenceYearsQuery{Search = "slovakia"});
            result.Items.Count.ShouldBe(1);

            result.Items[0].Year.ShouldBe(existing[1].Year);
            result.Items[0].Code.ShouldBe(existing[1].Code);
        }*/

        [Fact]
        public async Task Query_ShouldReturnTheRequestedPage()
        {
            var existing = new[]
            {
                new EvidenceYear
                {
                    Year = 2019,
                    Code = "19",
                },
                new EvidenceYear
                {
                    Year = 2020,
                    Code = "20",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListEvidenceYearsQuery
            {
                PageSize = 1,
                Page = 2
            });
            result.Items.Count.ShouldBe(1);

            result.Items[0].Year.ShouldBe(existing[1].Year);
            result.Items[0].Code.ShouldBe(existing[1].Code);

            result.Pagination.TotalPages.ShouldBe(2);
            result.Pagination.PageSize.ShouldBe(1);
            result.Pagination.Page.ShouldBe(2);
            result.Pagination.TotalCount.ShouldBe(2);
        }
    }
}
