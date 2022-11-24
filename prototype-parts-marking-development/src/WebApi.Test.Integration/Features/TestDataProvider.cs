namespace WebApi.Test.Integration.Features
{
    using System.Collections.Generic;

    public static class TestDataProvider
    {
        public static IEnumerable<Data.GateLevel> SeedGateLevels()
        {
            return new[]
            {
                new Data.GateLevel
                {
                    Title = "gate_level_30",
                    Code = "30",
                    Description = "description for gate level 30",
                },
                new Data.GateLevel
                {
                    Title = "gate_level_40",
                    Code = "40",
                    Description = "description for gate level 40",
                },
                new Data.GateLevel
                {
                    Title = "gate_level_50",
                    Code = "50",
                    Description = "description for gate level 50",
                },
                new Data.GateLevel
                {
                    Title = "gate_level_60",
                    Code = "60",
                    Description = "description for gate level 60",
                },
                new Data.GateLevel
                {
                    Title = "gate_level_70",
                    Code = "70",
                    Description = "description for gate level 30",
                },
            };
        }

        public static IEnumerable<Data.Location> SeedLocations()
        {
            return new[]
            {
                new Data.Location
                {
                    Title = "frankfurt",
                    Code = "FR",
                    Description = "HBS location Frankfurt",
                },
                new Data.Location
                {
                    Title = "zvolen",
                    Code = "ZV",
                    Description = "HBS location Zvolen",
                },
                new Data.Location
                {
                    Title = "yokohama",
                    Code = "YO",
                    Description = "HBS location Yokohama",
                },
                new Data.Location
                {
                    Title = "jicin",
                    Code = "JC",
                    Description = "HBS location Jicin",
                },
                new Data.Location
                {
                    Title = "auburn_hills",
                    Code = "AH",
                    Description = "HBS location Auburn Hills",
                },
            };
        }

        public static IEnumerable<Data.Outlet> SeedOutlets()
        {
            return new[]
            {
                new Data.Outlet
                {
                    Title = "actuation",
                    Code = "10",
                    Description = "Actuation description",
                },
                new Data.Outlet
                {
                    Title = "drum_brake",
                    Code = "11",
                    Description = "drum brake description",
                },
                new Data.Outlet
                {
                    Title = "electric_parking_brake",
                    Code = "12",
                    Description = "electric parking brake description",
                },
                new Data.Outlet
                {
                    Title = "foundation",
                    Code = "13",
                    Description = "foundation description",
                },
                new Data.Outlet
                {
                    Title = "disk_brake",
                    Code = "14",
                    Description = "disk brake description",
                },
            };
        }

        public static IEnumerable<Data.Part> SeedParts()
        {
            return new[]
            {
                new Data.Part
                {
                    Title = "complete_prototype",
                    Code = "00",
                    Description = "complete prototype description",
                },
                new Data.Part
                {
                    Title = "anchor",
                    Code = "33",
                    Description = "anchor description",
                },
                new Data.Part
                {
                    Title = "brake_pad",
                    Code = "05",
                    Description = "brake pad description",
                },
                new Data.Part
                {
                    Title = "housing",
                    Code = "52",
                    Description = "housing description",
                },
                new Data.Part
                {
                    Title = "brake_disc",
                    Code = "01",
                    Description = "brake disc",
                },
            };
        }

        public static IEnumerable<Data.ProductGroup> SeedProductGroups()
        {
            return new[]
            {
                new Data.ProductGroup
                {
                    Title = "fist_caliper",
                    Code = "30",
                    Description = "fist caliper description",
                },
                new Data.ProductGroup
                {
                    Title = "brake_caliper",
                    Code = "03",
                    Description = "brake_caliper description",
                },
                new Data.ProductGroup
                {
                    Title = "brake",
                    Code = "32",
                    Description = "brake description",
                },
                new Data.ProductGroup
                {
                    Title = "wheel",
                    Code = "56",
                    Description = "wheel description",
                },
                new Data.ProductGroup
                {
                    Title = "tire",
                    Code = "77",
                    Description = "tire description",
                },
            };
        }

        public static IEnumerable<Data.User> SeedUsers()
        {
            return new[]
            {
                new Data.User
                {
                     Name = "Robb Powlowski",
                     Email = "Cristal_Lemke97@hotmail.com",
                     DomainIdentity = "Virgil.Heathcote",
                     ServiceAccount = false,
                },
                new Data.User
                {
                    Name = "Earnest Bode",
                    Email = "Ona47@gmail.com",
                    DomainIdentity = "Mariano77",
                    ServiceAccount = false,
                },
                new Data.User
                {
                    Name = "Pete Bergstrom",
                    Email = "Sammie13@yahoo.com",
                    DomainIdentity = "Celestine19",
                    ServiceAccount = false,
                },
                new Data.User
                {
                    Name = "Lori Nader",
                    Email = "Liam67@hotmail.com",
                    DomainIdentity = "Alfredo59",
                    ServiceAccount = false,
                },
                new Data.User
                {
                    Name = "John Brzenk",
                    Email = "John345@gmail.com",
                    DomainIdentity = "ArmJon",
                    ServiceAccount = false,
                },
            };
        }
    }
}
