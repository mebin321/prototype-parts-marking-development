namespace WebApi.Common
{
    using System;

    public class UtcDateTime : IDateTime
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
