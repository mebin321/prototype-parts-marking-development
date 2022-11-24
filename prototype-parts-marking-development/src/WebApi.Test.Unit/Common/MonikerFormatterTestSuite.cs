namespace WebApi.Test.Unit.Common
{
    using System;
    using Shouldly;
    using WebApi.Common;
    using Xunit;

    public class MonikerFormatterTestSuite
    {
        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData(" ", typeof(ArgumentException))]
        [InlineData(null, typeof(ArgumentNullException))]
        public void Format_ShouldRejectEmptyInput(string input, Type exceptionType)
        {
            var formatter = new MonikerFormatter();

            Should.Throw(() => formatter.Format(input), exceptionType);
        }

        [Theory]
        [InlineData("Test", "test")]
        [InlineData("TestInput", "testinput")]
        public void Format_ShouldTransformInputToLowerCase(string input, string result)
        {
            var formatter = new MonikerFormatter();

            formatter.Format(input).ShouldBe(result);
        }

        [Theory]
        [InlineData(" test", "test")]
        [InlineData("test ", "test")]
        [InlineData(" test ", "test")]
        public void Format_ShouldTrimLeadingAndTrailingWhitespace(string input, string result)
        {
            var formatter = new MonikerFormatter();

            formatter.Format(input).ShouldBe(result);
        }

        [Theory]
        [InlineData("test input", "test-input")]
        [InlineData("test input test", "test-input-test")]
        [InlineData("test  input", "test-input")]
        public void Format_ShouldReplaceWhitespaceWithDash(string input, string result)
        {
            var formatter = new MonikerFormatter();

            formatter.Format(input).ShouldBe(result);
        }
    }
}
