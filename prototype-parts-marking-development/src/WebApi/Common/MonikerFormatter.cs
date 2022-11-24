namespace WebApi.Common
{
    using System.Text.RegularExpressions;
    using Utilities;

    public class MonikerFormatter : IMonikerFormatter
    {
        public string Format(string value)
        {
            Guard.NotNullOrWhitespace(value, nameof(value));

            return Regex.Replace(value.Trim(), @"\s+", "-").ToLowerInvariant();
        }
    }
}