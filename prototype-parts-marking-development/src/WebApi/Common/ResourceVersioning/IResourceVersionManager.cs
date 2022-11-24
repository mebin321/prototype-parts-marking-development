namespace WebApi.Common.ResourceVersioning
{
    using Data;

    public interface IResourceVersionManager
    {
        void CheckVersion(IAuditableEntity entity, bool allowWildcard = false);

        void SetEtag(IAuditableEntity entity);
    }
}
