namespace WebApi.Common.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class MetadataCache<T>
    {
        private static readonly Type Type = typeof(T);

        public static class For<TAttribute>
            where TAttribute : Attribute
        {
            private static readonly TAttribute[] Cache;

            static For()
            {
                Cache = Type.GetCustomAttributes<TAttribute>().ToArray();
            }

            public static IReadOnlyList<TAttribute> Attributes => Cache;

            public static TAttribute AttributeOrDefault => Cache.Length == 1 ? Cache[0] : default;

            public static bool IsPresent => Cache.Length > 0;
        }
    }
}
