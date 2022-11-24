namespace WebApi.Test.Integration.Features.PrototypeSet
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Shouldly;
    using WebApi.Features.PrototypeSets.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListPrototypeSetsQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListPrototypeSetsQueryTestSuite(TestingFixture testingFixture)
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);
            var query = new ListPrototypeSetsQuery();

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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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
            result.Items[1].SetIdentifier.ShouldBe("0001");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0001");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    ProductGroupCode = "40",
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
                },

                new PrototypeSet
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
                EvidenceYearTitle = 2022,
                SetIdentifier = "0003",
                CreatedById = existingUser.Id,
                ModifiedById = existingUser.Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].EvidenceYearTitle.ShouldBe(2022);
            result.Items[0].SetIdentifier.ShouldBe("0003");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    ProductGroupCode = "40",
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
                },
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
            {
                EvidenceYearLowerLimit =2021,
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
            result.Items[0].SetIdentifier.ShouldBe("0001");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
            {
                SetIdentifiers = new List<string>
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
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

            var existingPrototypeSets = new List<PrototypeSet>
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

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
            result.Items[0].Customer.ShouldBe("ACURA");
            result.Items[0].Project.ShouldBe("EPB_NA_HUM_TLX_FNCM");
            result.Items[0].ProjectNumber.ShouldBe("DG-045599");
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
        public async Task Query_FilterByCreatedByIds_ShouldReturnFilteredEntities()
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
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
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
            result.Items[0].CreatedBy.Id.ShouldBe(existingUsers[0].Id);
            result.Items[0].CreatedBy.Email.ShouldBe("user1@test.com");
            result.Items[0].CreatedBy.Name.ShouldBe("User1");
            result.Items[0].CreatedBy.Username.ShouldBe("identity1");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[0].Id,
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
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[1].Id,
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    CreatedById = existingUsers[1].Id,
                    ModifiedById = existingUsers[1].Id,
                    DeletedById = existingUsers[0].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
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
                    CreatedById = existingUsers[0].Id,
                    ModifiedById = existingUsers[0].Id,
                    DeletedById = existingUsers[1].Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0001");
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

            var existingPrototypeSets = new List<PrototypeSet>
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T10:00:00Z"),
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
                    DeletedById = existingUser.Id,
                    DeletedAt = DateTimeOffset.Parse("2021-01-01T20:00:00Z"),
                }
            };

            await testingFixture.AddRangeAsync(existingPrototypeSets);

            var query = new ListPrototypeSetsQuery
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
            result.Items[0].SetIdentifier.ShouldBe("0000");
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
