namespace WebApi.Features.GlobalProjects.Services
{
    using System.Collections.Generic;

    public interface IQueryPartitioner
    {
        IEnumerable<Partition> CreatePartitionsFor(int rowCount);
    }
}