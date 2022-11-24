namespace WebApi.Common.Sorting
{
    using System.Collections.Generic;

    public interface ISortColumnMapping
    {
        IEnumerable<string> MappedValues { get; }

        string this[string input] { get; }
    }
}