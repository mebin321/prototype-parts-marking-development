namespace WebApi.Data
{
    using System;
    using System.Collections.Generic;

    public interface IAuditableEntity
    {
        DateTimeOffset? CreatedAt { get; set; }

        DateTimeOffset? ModifiedAt { get; set; }

        DateTimeOffset? DeletedAt { get; set; }
    }
}