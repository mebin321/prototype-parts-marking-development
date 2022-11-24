namespace WebApi.Test.Integration.Features.GateLevels
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.GateLevels.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListGateLevelsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListGateLevelsQueryTestSuite(TestingFixture testingFixture)
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
                new GateLevel
                {
                    Moniker = "gate-level-70",
                    Title = "Gate Level 70",
                    Code = "70",
                    Description = "Gate Level 70",
                },
                new GateLevel
                {
                    Moniker = "gate-level-80",
                    Title = "Gate Level 80",
                    Code = "80",
                    Description = "Gate Level 80",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListGateLevelsQuery());
            result.Items.Count.ShouldBe(2);

            result.Items[0].Moniker.ShouldBe(existing[0].Moniker);
            result.Items[0].Title.ShouldBe(existing[0].Title);
            result.Items[0].Code.ShouldBe(existing[0].Code);
            result.Items[0].Description.ShouldBe(existing[0].Description);

            result.Items[1].Moniker.ShouldBe(existing[1].Moniker);
            result.Items[1].Title.ShouldBe(existing[1].Title);
            result.Items[1].Code.ShouldBe(existing[1].Code);
            result.Items[1].Description.ShouldBe(existing[1].Description);
        }

        [Fact]
        public async Task Query_ShouldPerformFullTextSearchWithTheProvidedParameter()
        {
            var existing = new[]
            {
                new GateLevel
                {
                    Moniker = "gate-level-70",
                    Title = "Gate Level 70",
                    Code = "70",
                    Description = "Gate Level 70",
                },
                new GateLevel
                {
                    Moniker = "gate-level-80",
                    Title = "Gate Level 80",
                    Code = "80",
                    Description = "Gate Level 80",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListGateLevelsQuery { Search = "level 80" });
            result.Items.Count.ShouldBe(1);

            result.Items[0].Moniker.ShouldBe(existing[1].Moniker);
            result.Items[0].Title.ShouldBe(existing[1].Title);
            result.Items[0].Code.ShouldBe(existing[1].Code);
            result.Items[0].Description.ShouldBe(existing[1].Description);
        }

        [Fact]
        public async Task Query_ShouldReturnTheRequestedPage()
        {
            var existing = new[]
            {
                new GateLevel
                {
                    Moniker = "gate-level-70",
                    Title = "Gate Level 70",
                    Code = "70",
                    Description = "Gate Level 70",
                },
                new GateLevel
                {
                    Moniker = "gate-level-80",
                    Title = "Gate Level 80",
                    Code = "80",
                    Description = "Gate Level 80",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListGateLevelsQuery
            {
                PageSize = 1,
                Page = 2
            });
            result.Items.Count.ShouldBe(1);

            result.Items[0].Moniker.ShouldBe(existing[1].Moniker);
            result.Items[0].Title.ShouldBe(existing[1].Title);
            result.Items[0].Code.ShouldBe(existing[1].Code);
            result.Items[0].Description.ShouldBe(existing[1].Description);

            result.Pagination.TotalPages.ShouldBe(2);
            result.Pagination.PageSize.ShouldBe(1);
            result.Pagination.Page.ShouldBe(2);
            result.Pagination.TotalCount.ShouldBe(2);
        }
    }
}
