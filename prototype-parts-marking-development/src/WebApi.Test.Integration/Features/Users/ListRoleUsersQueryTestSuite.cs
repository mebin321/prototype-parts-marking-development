namespace WebApi.Test.Integration.Features.Users
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;
    using WebApi.Data;
    using WebApi.Features.Users.Requests;
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class ListRoleUsersQueryTestSuite : IClassFixture<TestingFixture>, IAsyncLifetime
    {
        private readonly TestingFixture testingFixture;

        public ListRoleUsersQueryTestSuite(TestingFixture testingFixture)
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
        public async Task Query_ShouldReturnUsersAssignedToTheGivenRole()
        {
            var users = new[]
            {
                new User
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User 1",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                },
            };

            var roles = new[]
            {
                new Role
                {
                    Moniker = "admin",
                    Title = "Admin",
                    Description = "Administrator",
                },
                new Role
                {
                    Moniker = "super-admin",
                    Title = "Super Admin",
                    Description = "Super Administrator"
                },
            };

            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[0],
            });
            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[1],
            });
            roles[1].UserRoles.Add(new UserRole
            {
                Role = roles[1],
                User = users[2],
            });

            await testingFixture.ExecuteAsync(async c =>
            {
                c.Roles.AddRange(roles);
                c.Users.AddRange(users);
                await c.SaveChangesAsync();
            });

            var result = await testingFixture.SendAsync(new ListRoleUsersQuery { Moniker = "admin"});

            result.Items.Count.ShouldBe(2);

            result.Items[0].Name.ShouldBe(users[0].Name);
            result.Items[0].Email.ShouldBe(users[0].Email);
            result.Items[0].Username.ShouldBe(users[0].DomainIdentity);

            result.Items[1].Name.ShouldBe(users[1].Name);
            result.Items[1].Email.ShouldBe(users[1].Email);
            result.Items[1].Username.ShouldBe(users[1].DomainIdentity);
        }

        [Fact]
        public async Task Query_ShouldThrowIfRoleDoesNotExist()
        {
            var users = new[]
            {
                new User
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User 1",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                },
            };

            var roles = new[]
            {
                new Role
                {
                    Moniker = "admin",
                    Title = "Admin",
                    Description = "Administrator",
                },
                new Role
                {
                    Moniker = "super-admin",
                    Title = "Super Admin",
                    Description = "Super Administrator"
                },
            };

            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[0],
            });
            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[1],
            });
            roles[1].UserRoles.Add(new UserRole
            {
                Role = roles[1],
                User = users[2],
            });

            await testingFixture.ExecuteAsync(async c =>
            {
                c.Roles.AddRange(roles);
                c.Users.AddRange(users);
                await c.SaveChangesAsync();
            });

            await Should.ThrowAsync<NotFoundException>(async ()
                => await testingFixture.SendAsync(new ListRoleUsersQuery { Moniker = "other" }));
        }

        [Fact]
        public async Task Query_IsActive_ShouldReturnAllUsers()
        {
            var users = new[]
            {
                new User
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User 1",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                    DeletedAt = DateTimeOffset.UtcNow,
                },
            };

            var roles = new[]
            {
                new Role
                {
                    Moniker = "admin",
                    Title = "Admin",
                    Description = "Administrator",
                },
            };

            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[0],
            });
            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[1],
            });

            await testingFixture.ExecuteAsync(async c =>
            {
                c.Roles.AddRange(roles);
                c.Users.AddRange(users);
                await c.SaveChangesAsync();
            });

            var result = await testingFixture.SendAsync(new ListRoleUsersQuery { Moniker = "admin" });

            result.Items.Count.ShouldBe(2);

            result.Items[0].Name.ShouldBe(users[0].Name);
            result.Items[0].Email.ShouldBe(users[0].Email);
            result.Items[0].Username.ShouldBe(users[0].DomainIdentity);

            result.Items[1].Name.ShouldBe(users[1].Name);
            result.Items[1].Email.ShouldBe(users[1].Email);
            result.Items[1].Username.ShouldBe(users[1].DomainIdentity);
        }

        [Fact]
        public async Task Query_IsActive_ShouldReturnOnlyActiveUsers()
        {
            var users = new[]
            {
                new User
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User 1",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                    DeletedAt = DateTimeOffset.UtcNow,
                },
            };

            var roles = new[]
            {
                new Role
                {
                    Moniker = "admin",
                    Title = "Admin",
                    Description = "Administrator",
                },
            };

            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[0],
            });
            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[1],
            });

            await testingFixture.ExecuteAsync(async c =>
            {
                c.Roles.AddRange(roles);
                c.Users.AddRange(users);
                await c.SaveChangesAsync();
            });

            var result = await testingFixture.SendAsync(new ListRoleUsersQuery
            {
                Moniker = "admin",
                IsActive = true,
            });

            result.Items.Count.ShouldBe(1);

            result.Items[0].Name.ShouldBe(users[0].Name);
            result.Items[0].Email.ShouldBe(users[0].Email);
            result.Items[0].Username.ShouldBe(users[0].DomainIdentity);
        }

        [Fact]
        public async Task Query_IsActive_ShouldReturnOnlyInactiveUsers()
        {
            var users = new[]
            {
                new User
                {
                    DomainIdentity = "identity1",
                    Email = "user1@test.com",
                    Name = "User 1",
                    ServiceAccount = false,
                },
                new User
                {
                    DomainIdentity = "identity2",
                    Email = "user2@test.com",
                    Name = "User 2",
                    ServiceAccount = false,
                    DeletedAt = DateTimeOffset.UtcNow,
                },
            };

            var roles = new[]
            {
                new Role
                {
                    Moniker = "admin",
                    Title = "Admin",
                    Description = "Administrator",
                },
            };

            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[0],
            });
            roles[0].UserRoles.Add(new UserRole
            {
                Role = roles[0],
                User = users[1],
            });

            await testingFixture.ExecuteAsync(async c =>
            {
                c.Roles.AddRange(roles);
                c.Users.AddRange(users);
                await c.SaveChangesAsync();
            });

            var result = await testingFixture.SendAsync(new ListRoleUsersQuery
            {
                Moniker = "admin",
                IsActive = false,
            });

            result.Items.Count.ShouldBe(1);

            result.Items[0].Name.ShouldBe(users[1].Name);
            result.Items[0].Email.ShouldBe(users[1].Email);
            result.Items[0].Username.ShouldBe(users[1].DomainIdentity);
        }
    }
}
