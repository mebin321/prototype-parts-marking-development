namespace WebApi.Test.Integration.Features.Part
{
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Parts.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListPartsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListPartsQueryTestSuite(TestingFixture testingFixture)
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
                new Part
                {
                    Moniker = "complete-prototype",
                    Title = "Complete Prototype",
                    Code = "00",
                    Description = "Part Complete Prototype",
                },
                new Part
                {
                    Moniker = "anchor",
                    Title = "Anchor",
                    Code = "33",
                    Description = "Part Anchor",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListPartsQuery());
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
                new Part
                {
                    Moniker = "complete-prototype",
                    Title = "Complete Prototype",
                    Code = "00",
                    Description = "Part Complete Prototype",
                },
                new Part
                {
                    Moniker = "anchor",
                    Title = "Anchor",
                    Code = "33",
                    Description = "Part Anchor",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListPartsQuery { Search = "complete" });
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
                new Part
                {
                    Moniker = "complete-prototype",
                    Title = "Complete Prototype",
                    Code = "00",
                    Description = "Part Complete Prototype",
                },
                new Part
                {
                    Moniker = "anchor",
                    Title = "Anchor",
                    Code = "33",
                    Description = "Part Anchor",
                },
            };

            await testingFixture.AddRangeAsync(existing);

            var result = await testingFixture.SendAsync(new ListPartsQuery
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
