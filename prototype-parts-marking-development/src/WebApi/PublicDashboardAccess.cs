namespace WebApi
{
    using Hangfire.Dashboard;

    public class PublicDashboardAccess : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}