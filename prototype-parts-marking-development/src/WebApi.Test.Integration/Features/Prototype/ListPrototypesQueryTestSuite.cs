namespace WebApi.Test.Integration.Features.Prototypes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Shouldly;
    using WebApi.Features.Prototypes.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListPrototypesQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListPrototypesQueryTestSuite(TestingFixture testingFixture)
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
        public async Task Query_NoFilter_ShouldReturnAllEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery();

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(2);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);

            result.Items[1].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[1].PartTypeCode.ShouldBe("33");
            result.Items[1].PartTypeTitle.ShouldBe("Anchor");
            result.Items[1].Index.ShouldBe(2);
            result.Items[1].Comment.ShouldBe("Second prototype comment.");
            result.Items[1].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[1].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }

        [Fact]
        public async Task Query_IsActive_ShouldReturnOnlyActiveEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                IsActive = true,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_IsActive_ShouldReturnOnlyInactiveEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                IsActive = false,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].Index.ShouldBe(2);
            result.Items[0].Comment.ShouldBe("Second prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }

        [Fact]
        public async Task Query_FullTextSearchInComment_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                Search = "First"
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }


        [Fact]
        public async Task Query_FilterByMultipleCriteria_ShouldReturnOnlyInactiveEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 3,
                    Comment = "Third prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                OutletCodes = new List<string>
                {
                    "11",
                },
                PartTypeCodes = new List<string>
                {
                    "33",
                }
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[1].Id);
            result.Items[0].PrototypeSet.OutletCode.ShouldBe("11");
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].Index.ShouldBe(3);
            result.Items[0].Comment.ShouldBe("Third prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }

        
        [Fact]
        public async Task Query_FilterByOutletCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                OutletCodes = new List<string>
                {
                    "10",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.OutletCode.ShouldBe("10");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }
        
        [Fact]
        public async Task Query_FilterByOutletTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                OutletTitles = new List<string>
                {
                    "Actuation",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.OutletTitle.ShouldBe("Actuation");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        
        [Fact]
        public async Task Query_FilterByProductGroupCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                ProductGroupCodes = new List<string>
                {
                    "30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.ProductGroupCode.ShouldBe("30");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByProductGroupTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                ProductGroupTitles = new List<string>
                {
                    "Fist caliper",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByLocationCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                LocationCodes = new List<string>
                {
                    "FR",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.LocationCode.ShouldBe("FR");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByLocationTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                LocationTitles = new List<string>
                {
                    "Frankfurt",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByGateLevelCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                GateLevelCodes = new List<string>
                {
                    "30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.GateLevelCode.ShouldBe("30");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByGateLevelTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                GateLevelTitles = new List<string>
                {
                    "Gate Level 30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByEvidenceYearCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                EvidenceYearCodes = new List<string>
                {
                    "20",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.EvidenceYearCode.ShouldBe("20");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByEvidenceYearTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                EvidenceYearTitles = new List<int>
                {
                    2020,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByEvidenceYearLowerLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                EvidenceYearLowerLimit = 2021,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[1].Id);
            result.Items[0].PrototypeSet.EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].Index.ShouldBe(2);
            result.Items[0].Comment.ShouldBe("Second prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByEvidenceYearUpperLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                EvidenceYearUpperLimit = 2020,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterBySetIdentifiers_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                SetIdentifiers = new List<string>
                {
                    "0000",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.SetIdentifier.ShouldBe("0000");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByCustomers_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                Customers = new List<string>
                {
                    "ACURA",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.Customer.ShouldBe("ACURA");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByProjects_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                Projects = new List<string>
                {
                    "EPB_NA_HUM_TLX_FNCM",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByProjectNumbers_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                ProjectNumbers = new List<string>
                {
                    "DG-045599",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PrototypeSet.ProjectNumber.ShouldBe("DG-045599");
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByPrototypeSets_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSets = new[]
            {
                new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypeSet
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "03",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    SetIdentifier = "0001",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[0].Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSets[1].Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                PrototypeSets = new List<int>
                {
                    existingPrototypeSets[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSets[0].Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByPartTypeCodes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                PartTypeCodes = new List<string>
                {
                   "00",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByPartTypeTitles_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                PartTypeTitles = new List<string>
                {
                   "Complete prototype",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByIndexes_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                Indexes = new List<int>
                {
                    1,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByIndexesLowerLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                IndexLowerLimit = 2,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].Index.ShouldBe(2);
            result.Items[0].Comment.ShouldBe("Second prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByIndexesUpperLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                IndexUpperLimit = 1,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByOwnerIds_ShouldReturnFilteredEntities()
        {
            var existingUsers = new[]
            {
                new User()
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User1",
                    ServiceAccount = false,
                },

                new User()
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User2",
                    ServiceAccount = false,
                },
            };

            await testingFixture.AddRangeAsync(existingUsers);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUsers[0].Id,
                ModifiedById = existingUsers[0].Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                Owners = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByCreatedByIds_ShouldReturnFilteredEntities()
        {
            var existingUsers = new[]
            {
                new User()
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User1",
                    ServiceAccount = false,
                },

                new User()
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User2",
                    ServiceAccount = false,
                },
            };

            await testingFixture.AddRangeAsync(existingUsers);

            var existingPrototypeSet = new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[1].Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[0].Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                CreatedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByModifiedByIds_ShouldReturnFilteredEntities()
        {
            var existingUsers = new[]
            {
                new User()
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User1",
                    ServiceAccount = false,
                },

                new User()
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User2",
                    ServiceAccount = false,
                },
            };

            await testingFixture.AddRangeAsync(existingUsers);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUsers[0].Id,
                ModifiedById = existingUsers[0].Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[0].Id,
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[1].Id,
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                ModifiedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].DeletedBy.Id.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByDeletedByIds_ShouldReturnFilteredEntities()
        {
            var existingUsers = new[]
            {
                new User()
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User1",
                    ServiceAccount = false,
                },

                new User()
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User2",
                    ServiceAccount = false,
                },
            };

            await testingFixture.AddRangeAsync(existingUsers);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUsers[0].Id,
                ModifiedById = existingUsers[0].Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                    DeletedById = existingUsers[0].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                    DeletedById = existingUsers[1].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                DeletedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].DeletedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }

        [Fact]
        public async Task Query_FilterByDeletedAtLowerLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "30",
                    ProductGroupTitle = "Fist caliper",
                    LocationCode = "FR",
                    LocationTitle = "Frankfurt",
                    GateLevelCode = "30",
                    GateLevelTitle = "Gate Level 30",
                    EvidenceYearCode = "20",
                    EvidenceYearTitle = 2020,
                    SetIdentifier = "0000",
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                DeletedAtLowerLimit = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].Index.ShouldBe(2);
            result.Items[0].Comment.ShouldBe("Second prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T20:00:00Z"));
        }

        [Fact]
        public async Task Query_FilterByDeletedAtUpperLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypeSet = new PrototypeSet
            {
                OutletCode = "10",
                OutletTitle = "Actuation",
                ProductGroupCode = "30",
                ProductGroupTitle = "Fist caliper",
                LocationCode = "FR",
                LocationTitle = "Frankfurt",
                GateLevelCode = "30",
                GateLevelTitle = "Gate Level 30",
                EvidenceYearCode = "20",
                EvidenceYearTitle = 2020,
                SetIdentifier = "0000",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
            };

            await testingFixture.AddAsync(existingPrototypeSet);

            var existingPrototypes = new[]
            {
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    Index = 1,
                    Comment = "First prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },
                new Prototype
                {
                    PrototypeSetId = existingPrototypeSet.Id,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    Index = 2,
                    Comment = "Second prototype comment.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypes);

            var query = new FilteredListPrototypesQuery
            {
                DeletedAtUpperLimit = DateTimeOffset.Parse("2021-01-01T10:00:00Z")
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].PrototypeSet.Id.ShouldBe(existingPrototypeSet.Id);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].Index.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototype comment.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }
    }
}