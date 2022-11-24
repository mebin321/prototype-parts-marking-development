namespace WebApi.Features.GlobalProjects.Requests
{
    using System.Collections.Generic;
    using WebApi.Data;

    public class ProjectNumberComparer : IEqualityComparer<GlobalProject>
    {
        public bool Equals(GlobalProject x, GlobalProject y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            return x.ProjectNumber == y.ProjectNumber;
        }

        public int GetHashCode(GlobalProject obj)
        {
            return obj.ProjectNumber != null ? obj.ProjectNumber.GetHashCode() : 0;
        }
    }
}