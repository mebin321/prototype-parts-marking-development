namespace WebApi.Common
{
    using System;

    public interface IDateTime
    {
        DateTimeOffset Now { get; }
    }
}