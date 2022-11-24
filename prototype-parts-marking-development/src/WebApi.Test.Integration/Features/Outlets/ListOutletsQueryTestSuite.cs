namespace WebApi.Test.Integration.Features.Outlets
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Outlets.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListLocationsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListLocationsQueryTestSuite(TestingFixture testingFixture)
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
                new Outlet
                {
                    Moniker = "electric-parking-brake",
                    Title = "Electric Parking Brake",
                    Code = "12",
                    Description = "EPB Outlet",
                },
                new Outlet
                {
                    Moniker = "actuation",
                    Title = "Actuation",
                    Code = "10",
                    Description = "Actuation outlet",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListOutletsQuery());
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
                new Outlet
                {
                    Moniker = "electric-parking-brake",
                    Title = "Electric Parking Brake",
                    Code = "12",
                    Description = "EPB Outlet",
                },
                new Outlet
                {
                    Moniker = "actuation",
                    Title = "Actuation",
                    Code = "10",
                    Description = "Actuation outlet",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListOutletsQuery { Search = "brake" });
            result.Items.Count.ShouldBe(1);

            result.Items[0].Moniker.ShouldBe(existing[0].Moniker);
            result.Items[0].Title.ShouldBe(existing[0].Title);
            result.Items[0].Code.ShouldBe(existing[0].Code);
            result.Items[0].Description.ShouldBe(existing[0].Description);
        }

        [Fact]
        public async Task Query_ShouldReturnTheRequestedPage()
        {
            var existing = new[]
            {
                new Outlet
                {
                    Moniker = "electric-parking-brake",
                    Title = "Electric Parking Brake",
                    Code = "12",
                    Description = "EPB Outlet",
                },
                new Outlet
                {
                    Moniker = "actuation",
                    Title = "Actuation",
                    Code = "10",
                    Description = "Actuation outlet",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListOutletsQuery
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
