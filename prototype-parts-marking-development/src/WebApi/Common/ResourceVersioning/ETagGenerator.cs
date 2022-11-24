namespace WebApi.Common.ResourceVersioning
{
    using System;
    using System.Text;
    using Utilities;
    using WebApi.Data;

    public class ETagGenerator : IETagGenerator
    {
        public string ETagFrom(IAuditableEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var value = entity.ModifiedAt?.ToString("o") ?? string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
    }
}