namespace WebApi.Test.Integration.Features.PrototypesPackage
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Shouldly;
    using WebApi.Features.PrototypesPackages.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListPrototypesPackagesQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListPrototypesPackagesQueryTestSuite(TestingFixture testingFixture)
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);
            var query = new ListPrototypesPackagesQuery();

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(2);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);

            result.Items[1].OutletCode.ShouldBe("11");
            result.Items[1].OutletTitle.ShouldBe("Drum brake");
            result.Items[1].ProductGroupCode.ShouldBe("03");
            result.Items[1].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[1].LocationCode.ShouldBe("ZV");
            result.Items[1].LocationTitle.ShouldBe("Zvolen");
            result.Items[1].GateLevelCode.ShouldBe("20");
            result.Items[1].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[1].EvidenceYearCode.ShouldBe("21");
            result.Items[1].EvidenceYearTitle.ShouldBe(2021);
            result.Items[1].PartTypeCode.ShouldBe("33");
            result.Items[1].PartTypeTitle.ShouldBe("Anchor");
            result.Items[1].PackageIdentifier.ShouldBe("0001");
            result.Items[1].InitialCount.ShouldBe(10);
            result.Items[1].ActualCount.ShouldBe(11);
            result.Items[1].Comment.ShouldBe("Second prototypes package.");
            result.Items[1].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[1].Owner.Email.ShouldBe("user1@test.com");
            result.Items[1].Owner.Name.ShouldBe("User1");
            result.Items[1].Owner.Username.ShouldBe("identity1");
            result.Items[1].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[1].CreatedBy.Name.ShouldBe("User1");
            result.Items[1].CreatedBy.Username.ShouldBe("identity1");
            result.Items[1].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[1].ModifiedBy.Name.ShouldBe("User1");
            result.Items[1].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[1].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[1].DeletedBy.Email.ShouldBe("user1@test.com");
            result.Items[1].DeletedBy.Name.ShouldBe("User1");
            result.Items[1].DeletedBy.Username.ShouldBe("identity1");
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                IsActive = true,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                IsActive = false,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("11");
            result.Items[0].OutletTitle.ShouldBe("Drum brake");
            result.Items[0].ProductGroupCode.ShouldBe("03");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0001");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Second prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].DeletedBy.Name.ShouldBe("User1");
            result.Items[0].DeletedBy.Username.ShouldBe("identity1");
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                Search = "first",
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
                {
                    OutletCode = "11",
                    OutletTitle = "Drum brake",
                    ProductGroupCode = "40",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },
                new PrototypesPackage
                {
                    OutletCode = "10",
                    OutletTitle = "Actuation",
                    ProductGroupCode = "40",
                    ProductGroupTitle = "Brake caliper",
                    LocationCode = "ZV",
                    LocationTitle = "Zvolen",
                    GateLevelCode = "20",
                    GateLevelTitle = "Gate Level 20",
                    EvidenceYearCode = "21",
                    EvidenceYearTitle = 2021,
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0002",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Third prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            { 
                OutletCodes = new List<string> 
                {
                    "10",
                },
                ProductGroupCodes = new List<string>
                {
                    "40",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("40");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0002");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Third prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                OutletCodes = new List<string>
                {
                    "10",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                OutletTitles = new List<string>
                {
                    "Actuation",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ProductGroupCodes = new List<string>
                {
                    "30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ProductGroupTitles = new List<string>
                {
                    "Fist caliper",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                LocationCodes = new List<string>
                {
                    "FR",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                LocationTitles = new List<string>
                {
                    "Frankfurt",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                GateLevelCodes = new List<string>
                {
                    "30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                GateLevelTitles = new List<string>
                {
                    "Gate Level 30",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                EvidenceYearCodes = new List<string>
                {
                    "20",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                EvidenceYearTitles = new List<int>
                {
                    2020,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                EvidenceYearLowerLimit = 2021,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("11");
            result.Items[0].OutletTitle.ShouldBe("Drum brake");
            result.Items[0].ProductGroupCode.ShouldBe("03");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0001");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Second prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                EvidenceYearUpperLimit = 2020,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                PartTypeCodes = new List<string>
                {
                    "00",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                PartTypeTitles = new List<string>
                {
                    "Complete prototype",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByPackageIdentifiers_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,

                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                PackageIdentifiers = new List<string>
                {
                    "0000",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                Customers = new List<string>
                {
                    "ACURA",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                Projects = new List<string>
                {
                    "EPB_NA_HUM_TLX_FNCM",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    Customer = "ACURA",
                    Project = "EPB_NA_HUM_TLX_FNCM",
                    ProjectNumber = "DG-045599",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    Customer = "CADILLAC",
                    Project = "GM GMT166/8 OPD",
                    ProjectNumber = "DG-009284",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ProjectNumbers = new List<string>
                {
                    "DG-045599",
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByInitialCounts_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                InitialCounts = new List<int>
                {
                    2,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByInitialCountLowerLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                InitialCountLowerLimit = 10,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("11");
            result.Items[0].OutletTitle.ShouldBe("Drum brake");
            result.Items[0].ProductGroupCode.ShouldBe("03");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0001");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Second prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByInitialCountUpperLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                InitialCountUpperLimit = 2,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByActualCounts_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ActualCounts = new List<int>
                {
                    1,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByActualCountLowerLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ActualCountLowerLimit = 11,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("11");
            result.Items[0].OutletTitle.ShouldBe("Drum brake");
            result.Items[0].ProductGroupCode.ShouldBe("03");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0001");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Second prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByActualCountUpperLimit_ShouldReturnFilteredEntities()
        {
            var existingUser = new User
            {
                DomainIdentity = "identity1",
                Email = "user1@test.com",
                Name = "User1",
                ServiceAccount = false,
            };

            await testingFixture.AddAsync(existingUser);

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ActualCountUpperLimit = 1,
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
        }

        [Fact]
        public async Task Query_FilterByOwnerIds_ShouldReturnFilteredEntities()
        {
            var existingUsers = new []
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                Owners = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User2");
            result.Items[0].CreatedBy.Username.ShouldBe("identity2");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User2");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity2");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[1].Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[0].Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                CreatedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].Owner.Email.ShouldBe("user2@test.com");
            result.Items[0].Owner.Name.ShouldBe("User2");
            result.Items[0].Owner.Username.ShouldBe("identity2");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User2");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity2");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[0].Id,
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[1].Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                ModifiedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].Owner.Email.ShouldBe("user2@test.com");
            result.Items[0].Owner.Name.ShouldBe("User2");
            result.Items[0].Owner.Username.ShouldBe("identity2");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User2");
            result.Items[0].CreatedBy.Username.ShouldBe("identity2");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(null);
            result.Items[0].DeletedBy.Email.ShouldBe(null);
            result.Items[0].DeletedBy.Name.ShouldBe(null);
            result.Items[0].DeletedBy.Username.ShouldBe(null);
            result.Items[0].DeletedAt.ShouldBe(null);
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUsers[1].Id,
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                    DeletedById = existingUsers[0].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUsers[0].Id,
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                    DeletedById = existingUsers[1].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                DeletedBy = new List<int>
                {
                    existingUsers[0].Id,
                },
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].Owner.Email.ShouldBe("user2@test.com");
            result.Items[0].Owner.Name.ShouldBe("User2");
            result.Items[0].Owner.Username.ShouldBe("identity2");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User2");
            result.Items[0].CreatedBy.Username.ShouldBe("identity2");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUsers[1].Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user2@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User2");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity2");
            result.Items[0].DeletedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].DeletedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].DeletedBy.Name.ShouldBe("User1");
            result.Items[0].DeletedBy.Username.ShouldBe("identity1");
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                DeletedAtLowerLimit = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("11");
            result.Items[0].OutletTitle.ShouldBe("Drum brake");
            result.Items[0].ProductGroupCode.ShouldBe("03");
            result.Items[0].ProductGroupTitle.ShouldBe("Brake caliper");
            result.Items[0].LocationCode.ShouldBe("ZV");
            result.Items[0].LocationTitle.ShouldBe("Zvolen");
            result.Items[0].GateLevelCode.ShouldBe("20");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 20");
            result.Items[0].EvidenceYearCode.ShouldBe("21");
            result.Items[0].EvidenceYearTitle.ShouldBe(2021);
            result.Items[0].PartTypeCode.ShouldBe("33");
            result.Items[0].PartTypeTitle.ShouldBe("Anchor");
            result.Items[0].PackageIdentifier.ShouldBe("0001");
            result.Items[0].InitialCount.ShouldBe(10);
            result.Items[0].ActualCount.ShouldBe(11);
            result.Items[0].Comment.ShouldBe("Second prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].DeletedBy.Name.ShouldBe("User1");
            result.Items[0].DeletedBy.Username.ShouldBe("identity1");
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

            var existingPrototypesPackages = new List<PrototypesPackage>
            {
                new PrototypesPackage
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
                    PartTypeCode = "00",
                    PartTypeTitle = "Complete prototype",
                    PackageIdentifier = "0000",
                    InitialCount = 2,
                    ActualCount = 1,
                    Comment = "First prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                },

                new PrototypesPackage
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
                    PartTypeCode = "33",
                    PartTypeTitle = "Anchor",
                    PackageIdentifier = "0001",
                    InitialCount = 10,
                    ActualCount = 11,
                    Comment = "Second prototypes package.",
                    OwnerId = existingUser.Id,
                    CreatedById = existingUser.Id,
                    ModifiedById = existingUser.Id,
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypesPackages);

            var query = new ListPrototypesPackagesQuery
            {
                DeletedAtUpperLimit = DateTimeOffset.Parse("2021-01-01T10:00:00Z")
            };

            var result = await testingFixture.SendAsync(query);
            result.Items.Count.ShouldBe(1);
            result.Items[0].OutletCode.ShouldBe("10");
            result.Items[0].OutletTitle.ShouldBe("Actuation");
            result.Items[0].ProductGroupCode.ShouldBe("30");
            result.Items[0].ProductGroupTitle.ShouldBe("Fist caliper");
            result.Items[0].LocationCode.ShouldBe("FR");
            result.Items[0].LocationTitle.ShouldBe("Frankfurt");
            result.Items[0].GateLevelCode.ShouldBe("30");
            result.Items[0].GateLevelTitle.ShouldBe("Gate Level 30");
            result.Items[0].EvidenceYearCode.ShouldBe("20");
            result.Items[0].EvidenceYearTitle.ShouldBe(2020);
            result.Items[0].PartTypeCode.ShouldBe("00");
            result.Items[0].PartTypeTitle.ShouldBe("Complete prototype");
            result.Items[0].PackageIdentifier.ShouldBe("0000");
            result.Items[0].InitialCount.ShouldBe(2);
            result.Items[0].ActualCount.ShouldBe(1);
            result.Items[0].Comment.ShouldBe("First prototypes package.");
            result.Items[0].Owner.Id.ShouldBe(existingUser.Id);
            result.Items[0].Owner.Email.ShouldBe("user1@test.com");
            result.Items[0].Owner.Name.ShouldBe("User1");
            result.Items[0].Owner.Username.ShouldBe("identity1");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
            result.Items[0].ModifiedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].ModifiedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].ModifiedBy.Name.ShouldBe("User1");
            result.Items[0].ModifiedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedBy.Id.ShouldBe(existingUser.Id);
            result.Items[0].DeletedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].DeletedBy.Name.ShouldBe("User1");
            result.Items[0].DeletedBy.Username.ShouldBe("identity1");
            result.Items[0].DeletedAt.ShouldBe(DateTimeOffset.Parse("2021-01-01T10:00:00Z"));
        }
    }
}
