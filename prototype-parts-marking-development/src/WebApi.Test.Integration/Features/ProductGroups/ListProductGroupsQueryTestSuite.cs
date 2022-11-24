namespace WebApi.Test.Integration.Features.ProductGroups
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.ProductGroup.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListProductGroupsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListProductGroupsQueryTestSuite(TestingFixture testingFixture)
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
                new ProductGroup
                {
                    Moniker = "drum-brake-assembly",
                    Title = "Drum Brake Assembly",
                    Code = "51",
                    Description = "Drum Brake Assembly description",
                },
                new ProductGroup
                {
                    Moniker = "mgu",
                    Title = "MGU",
                    Code = "25",
                    Description = "MGU description",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListProductGroupsQuery());
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
                new ProductGroup
                {
                    Moniker = "drum-brake-assembly",
                    Title = "Drum Brake Assembly",
                    Code = "51",
                    Description = "Drum Brake Assembly description",
                },
                new ProductGroup
                {
                    Moniker = "mgu",
                    Title = "MGU",
                    Code = "25",
                    Description = "MGU description",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListProductGroupsQuery { Search = "brake" });
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
                new ProductGroup
                {
                    Moniker = "drum-brake-assembly",
                    Title = "Drum Brake Assembly",
                    Code = "51",
                    Description = "Drum Brake Assembly description",
                },
                new ProductGroup
                {
                    Moniker = "mgu",
                    Title = "MGU",
                    Code = "25",
                    Description = "MGU description",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListProductGroupsQuery
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
