namespace WebApi.Test.Unit.Features.GlobalProjects
{
    using System.Linq;
    using Microsoft.Extensions.Options;
    using Shouldly;
    using WebApi.Features.GlobalProjects.Services;
    using Xunit;

    public class QueryPartitionerTestSuite
    {
        [Theory]
        [InlineData(100, 99, 1)]
        [InlineData(100, 100, 2)]
        [InlineData(100, 101, 2)]
        [InlineData(100, 999, 10)]
        [InlineData(100, 1000, 11)]
        [InlineData(100, 1001, 11)]
        public void CreatePartitionsFor_ShouldCreateTheCorrentNumberOfPartitions(int partitionSize, int rowCount, int expected)
        {
            var options = new OptionsWrapper<QueryPartitionerOptions>(new QueryPartitionerOptions { PartitionSize = partitionSize });
            var partitioner = new QueryPartitioner(options);

            var result = partitioner.CreatePartitionsFor(rowCount).ToList();

            result.Count.ShouldBe(expected);
        }
    }
}
