namespace WebApi.Common.PrototypeIdentifier
{
    using System.Threading;
    using System.Threading.Tasks;
    using Utilities;

    public class PrototypeIdentifierCounterSynchronizationDecorator : IPrototypeIdentifierCounter
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly IPrototypeIdentifierCounter inner;

        public PrototypeIdentifierCounterSynchronizationDecorator(IPrototypeIdentifierCounter inner)
        {
            Guard.NotNull(inner, nameof(inner));

            this.inner = inner;
        }

        public async Task<int> IncrementCounterFor(int locationId, int evidenceYearId)
        {
            await Semaphore.WaitAsync();
            try
            {
                return await inner.IncrementCounterFor(locationId, evidenceYearId);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}