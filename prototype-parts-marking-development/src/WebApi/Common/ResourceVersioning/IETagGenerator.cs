namespace WebApi.Common.ResourceVersioning
{
    using WebApi.Data;

    public interface IETagGenerator
    {
        string ETagFrom(IAuditableEntity entity);
    }
}