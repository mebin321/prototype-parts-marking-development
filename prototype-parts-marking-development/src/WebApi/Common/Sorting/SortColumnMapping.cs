namespace WebApi.Common.Sorting
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    public class SortColumnMapping<T> : ISortColumnMapping<T>
    {
        private readonly Dictionary<string, string> mapping = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> MappedValues => mapping.Keys;

        public string this[string external]
        {
            get => GetInternalNameFrom(external?.Trim());
            set
            {
                Guard.NotNullOrWhitespace(external, nameof(external));
                Guard.NotNullOrWhitespace(value, nameof(value));

                mapping[external] = value;
            }
        }

        public bool ContainsMappingFor(string externalName) => mapping.ContainsKey(externalName);

        private string GetInternalNameFrom(string externalName)
        {
            return mapping.TryGetValue(externalName, out var result)
                ? result
                : throw new InvalidSortPropertyException($"[{externalName}] is not a valid sorting property.");
        }
    }
}
