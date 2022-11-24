namespace WebApi
{
    using System;
    using System.Threading.Tasks;
    using Utilities;

    public static class Extensions
    {
        public static async Task<TValue> ThrowIfNullAsync<TValue, TException>(this Task<TValue> task, TException exception)
            where TException : Exception
        {
            Guard.NotNull(task, nameof(task));
            Guard.NotNull(exception, nameof(exception));

            var value = await task;

            return value is null ? throw exception : value;
        }
    }
}
