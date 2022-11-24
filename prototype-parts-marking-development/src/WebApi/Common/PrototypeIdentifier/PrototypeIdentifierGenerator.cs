namespace WebApi.Common.PrototypeIdentifier
{
    using System.Threading.Tasks;
    using Utilities;

    public sealed class PrototypeIdentifierGenerator : IPrototypeIdentifierGenerator
    {
        private readonly IPrototypeIdentifierCounter prototypeIdentifierCounter;
        private readonly IPrototypeCounterConverter prototypeCounterConverter;

        public PrototypeIdentifierGenerator(IPrototypeIdentifierCounter prototypeIdentifierCounter, IPrototypeCounterConverter prototypeCounterConverter)
        {
            Guard.NotNull(prototypeIdentifierCounter, nameof(prototypeIdentifierCounter));
            Guard.NotNull(prototypeCounterConverter, nameof(prototypeCounterConverter));

            this.prototypeIdentifierCounter = prototypeIdentifierCounter;
            this.prototypeCounterConverter = prototypeCounterConverter;
        }

        public async Task<string> GenerateIdentifierFor(int locationId, int evidenceYearId)
        {
            var counterValue = await prototypeIdentifierCounter.IncrementCounterFor(locationId, evidenceYearId);

            return prototypeCounterConverter.IdentifierFrom(counterValue);
        }
    }
}
