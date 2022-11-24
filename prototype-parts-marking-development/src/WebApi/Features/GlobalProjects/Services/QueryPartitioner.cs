namespace WebApi.Features.GlobalProjects.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using Utilities;

    public class QueryPartitioner : IQueryPartitioner
    {
        private readonly QueryPartitionerOptions options;

        public QueryPartitioner(IOptions<QueryPartitionerOptions> options)
        {
            Guard.NotNull(options, nameof(options));

            this.options = options.Value;
        }

        public IEnumerable<Partition> CreatePartitionsFor(int rowCount)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), rowCount, "Value must be greater than 0.");
            }

            var offset = 0;

            while (offset <= rowCount)
            {
                yield return new Partition
                {
                    Skip = offset,
                    Take = options.PartitionSize,
                };

                offset += options.PartitionSize;
            }
        }
    }
}