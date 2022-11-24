namespace WebApi.Common.PrototypeIdentifier
{
    using System.Threading.Tasks;

    public interface IPrototypeIdentifierCounter
    {
        Task<int> IncrementCounterFor(int locationId, int evidenceYearId);
    }
}