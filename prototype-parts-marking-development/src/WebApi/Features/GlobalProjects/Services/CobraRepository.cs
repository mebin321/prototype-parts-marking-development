namespace WebApi.Features.GlobalProjects.Services
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Oracle.ManagedDataAccess.Client;
    using Utilities;
    using WebApi.Data.Cobra;

    public sealed class CobraRepository : ICobraRepository
    {
        private const string CountQuery = @"
                            SELECT COUNT(*)
                            FROM TBL_GLOBAL_PS t";

        private const string DataQuery = @"
                            SELECT 
                                t.PROJECT_PK ProjectPk,
                                t.PROJECTNUMBER ProjectNumber,
                                t.PROJECTTEXT ProjectText,
                                t.MANUFACTURER Manufacturer                        
                            FROM TBL_GLOBAL_PS t
                            ORDER BY t.PROJECT_PK
                            OFFSET :Skip ROWS FETCH NEXT :Take ROWS ONLY";

        private readonly IConfiguration configuration;
        private readonly IQueryPartitioner queryPartitioner;

        public CobraRepository(IConfiguration configuration, IQueryPartitioner queryPartitioner)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(queryPartitioner, nameof(queryPartitioner));

            this.configuration = configuration;
            this.queryPartitioner = queryPartitioner;
        }

        public async IAsyncEnumerable<GlobalProject> GetGlobalProjects([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await using var connection = new OracleConnection(configuration.GetConnectionString("Cobra"));

            var rowCount = await connection.ExecuteScalarAsync<int>(CountQuery);

            foreach (var partition in queryPartitioner.CreatePartitionsFor(rowCount))
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var project in await connection.QueryAsync<GlobalProject>(DataQuery, partition))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return project;
                }
            }
        }
    }
}