namespace WebApi.Common.PrototypeIdentifier
{
    using System.Threading.Tasks;

    public interface IPrototypeIdentifierGenerator
    {
        Task<string> GenerateIdentifierFor(int locationId, int evidenceYearId);
    }
}