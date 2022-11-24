namespace WebApi.Features.GlobalProjects.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using WebApi.Data.Cobra;

    public interface ICobraRepository
    {
        IAsyncEnumerable<GlobalProject> GetGlobalProjects(CancellationToken cancellationToken);
    }
}