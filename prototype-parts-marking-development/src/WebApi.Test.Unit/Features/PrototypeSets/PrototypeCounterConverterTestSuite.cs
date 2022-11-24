namespace WebApi.Test.Unit.Features.PrototypeSets
{
    using System;
    using Shouldly;
    using WebApi.Common.PrototypeIdentifier;
    using Xunit;

    public class PrototypeCounterConverterTestSuite
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(1679616)]
        public void CounterToUniqueIdentifier_ShouldThrowForInputOutsideAcceptableRange(int input)
        {
            var converter = new Base36Converter();

            Should.Throw<ArgumentOutOfRangeException>(() => converter.IdentifierFrom(input));
        }

        [Theory]
        [InlineData(0, "0000")]
        [InlineData(1, "0001")]
        [InlineData(10, "000A")]
        [InlineData(36, "0010")]
        [InlineData(1679615, "ZZZZ")]
        public void CounterToUniqueIdentifier_ShouldConvertInputToExpectedOutput(int input, string result)
        {
            var converter = new Base36Converter();

            converter.IdentifierFrom(input).ShouldBe(result);
        }
    }
}
