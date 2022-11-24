namespace DataMigrator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CsvHelper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Serilog;
    using WebApi.Common;
    using WebApi.Data;

    class Program
    {
        static async Task Main()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/migration.json")
                .WriteTo.Console()
                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Migration.json")
                .AddEnvironmentVariables()
                .Build();

            
                Log.Information("Start migration");
                var result = await Run(configuration);
                Log.Information($"{result} was migrated.");
            }
            catch (Exception e)
            {
                Log.Error(e,"Error while migration.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        private static async Task<int> Run(IConfiguration configuration)
        { 
            var contextOptions = new DbContextOptionsBuilder<PrototypePartsDbContext>()
                .UseNpgsql(configuration.GetConnectionString("PPMT"))
                .Options;

            var utcDateTime = new UtcDateTime();
            var timestamp = utcDateTime.Now;

            await using var dbContext = new PrototypePartsDbContext(
                new NullLoggerFactory(),
                utcDateTime,
                contextOptions);

            var repository = new PrototypeRepository(dbContext);

            using var reader = new StreamReader(configuration.GetSection("SourceFile").Value);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<DataModel>();
            var currentSets = new List<PrototypeSet>();
            var addedRecords = 0;

            foreach (var record in records)
            {
                Log.Information($"Migrate: {record}");
                var set = await GetPrototypeSetAsync(repository, currentSets, record);
                if (set is null)
                {
                    set = await CreateSet(repository, record);
                    currentSets.Add(set);
                }

                var prototype = await CreatePrototype(repository, record, timestamp);
                PrototypeCheckIndexUniqueness(set, prototype);

                set.Prototypes.Add(prototype);
                addedRecords++;
            }

            await repository.AddPrototypeSetsAsync(currentSets);
            await repository.SaveAsync();
            return addedRecords;

        }


        private static async Task<PrototypeSet> GetPrototypeSetAsync(PrototypeRepository repository, IEnumerable<PrototypeSet> currentSets, DataModel record)
         {
             var set = await repository.GetPrototypeSetAsync(record.LocationCode, record.EvidenceYearCode, record.SetIdentifier);

            if (set is not null)
            {
                throw new EntityException(
                    $"PrototypeSet with LocationCode:{record.LocationCode} , EvidenceYearCode:{record.EvidenceYearCode} and SetIdentifier:{record.SetIdentifier} already exist.");
            }

            set = currentSets.AsQueryable().FirstOrDefault(
                s => s.LocationCode == record.LocationCode 
                     && s.EvidenceYearCode == record.EvidenceYearCode 
                     && s.SetIdentifier == record.SetIdentifier);
            if (set is not null)
            {
                if (set.ProductGroupCode != record.ProductGroupCode
                    || set.ProductGroupTitle != record.ProductGroup
                    || set.OutletCode != record.OutletCode
                    || set.OutletTitle != record.Outlet
                    || set.LocationTitle != record.Location
                    || set.GateLevelCode != record.GateLevelCode
                    || set.Customer != record.Customer
                    || set.ProjectNumber != record.ProjectNumber)
                {
                    throw new EntityException(
                        $"Record {record} does not match in all fields with already created PrototypeSet with same unique identifier." +
                        $"\n Created Set - Record" +
                        $"\n {set.ProductGroupCode} : {record.ProductGroupCode}" +
                        $"\n {set.ProductGroupTitle} : {record.ProductGroup}" +
                        $"\n {set.OutletCode} : {record.OutletCode}" +
                        $"\n {set.OutletTitle} : {record.Outlet}" +
                        $"\n {set.LocationCode} : {record.LocationCode}" +
                        $"\n {set.LocationTitle} : {record.LocationCode}" +
                        $"\n {set.SetIdentifier} : {record.SetIdentifier}" +
                        $"\n {set.GateLevelCode} : {record.GateLevelCode}" +
                        $"\n {set.Customer} : {record.Customer}" +
                        $"\n {set.ProjectNumber} : {record.ProjectNumber}");
                }
            }

            return set;
        }

         private static async Task<PrototypeSet> CreateSet(PrototypeRepository repository, DataModel record)
         {
             var user = await repository.FindUserAsync(record.CreatedBy);
             var evidenceYear = await repository.FindEvidenceYearAsync(record.EvidenceYearCode);
             var gateLevel = await repository.FindGateLevelAsync(record.GateLevelCode);
             var location = await repository.FindLocationAsync(record.LocationCode, record.Location);
             var outlet = await repository.FindOutletAsync(record.OutletCode, record.Outlet);
             var productGroup = await repository.FindProductGroupAsync(record.ProductGroupCode, record.ProductGroup);

             return new PrototypeSet
             {
                 Customer = record.Customer,
                 CreatedBy = user,
                 EvidenceYearCode = evidenceYear.Code,
                 EvidenceYearTitle = evidenceYear.Year,
                 GateLevelCode = gateLevel.Code,
                 GateLevelTitle = gateLevel.Title,
                 LocationCode = location.Code,
                 LocationTitle = location.Title,
                 ModifiedBy = user,
                 OutletCode = outlet.Code,
                 OutletTitle = outlet.Title,
                 ProductGroupCode = productGroup.Code,
                 ProductGroupTitle = productGroup.Title,
                 ProjectNumber = record.ProjectNumber,
                 SetIdentifier = record.SetIdentifier,
                 Prototypes = new List<Prototype>()
             };
         }

         private static async Task<Prototype> CreatePrototype(PrototypeRepository repository, DataModel record, DateTimeOffset timestamp)
         {
             var user = await repository.FindUserAsync(record.CreatedBy);
             var part = await repository.FindPartAsync(record.PartCode, record.Part);

             var prototype = new Prototype
             {
                 Comment = record.Comment,
                 CreatedBy = user,
                 DeletedAt = record.IsActive == -1 ? null : timestamp,
                 DeletedBy = record.IsActive == -1 ? null : user,
                 Index = record.Index,
                 ModifiedBy = user,
                 Owner = user,
                 PartTypeTitle = part.Title,
                 PartTypeCode = part.Code,
                 Type = PrototypeType.Original,
             };

            return prototype;
         }

         private static void PrototypeCheckIndexUniqueness(PrototypeSet set, Prototype prototype)
         {
             var existingPrototype =  set.Prototypes.Find(p => p.Index == prototype.Index);
             if (existingPrototype != null)
             {
                 throw new EntityException($"Original prototype with Index:{prototype.Index} already exist in set: {SetToPartCode(set)}");
             }
         }

         private static string SetToPartCode(PrototypeSet set)
         {
             return
                 $"{set.OutletCode}.{set.ProductGroupCode}.xx.{set.EvidenceYearCode}.{set.SetIdentifier}.{set.GateLevelCode}_xxx";
         }

    }
}
