namespace WebApi.Common.Sorting
{
    using System;
    using Autofac;

    public static class SortColumnMappingExtensions
    {
        public static void AddSortColumnMapping<T>(this ContainerBuilder builder, Action<SortColumnMapping<T>> action)
        {
            var mapping = new SortColumnMapping<T>();
            action(mapping);

            builder.RegisterInstance(mapping).As<ISortColumnMapping<T>>().SingleInstance();
        }
    }
}