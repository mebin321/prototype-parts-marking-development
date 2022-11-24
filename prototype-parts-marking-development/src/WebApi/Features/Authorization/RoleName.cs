namespace WebApi.Features.Authorization
{
    using System.Collections.Generic;

    public static class RoleName
    {
        public const string TestEngineer = "test-engineer";

        public const string Coordinator = "coordinator";

        public const string ApplicationEngineer = "application-engineer";

        public const string PrototypeSup = "prototype-sup";

        public const string Tpm = "tpm";

        public const string TestTechnician = "test-technician";

        public const string FixturesTechnician = "fixtures-technician";

        public const string Admin = "admin";

        public const string SuperAdmin = "super-admin";

        public static IEnumerable<string> ListRoles()
        {
            yield return TestEngineer;
            yield return Coordinator;
            yield return ApplicationEngineer;
            yield return PrototypeSup;
            yield return Tpm;
            yield return TestTechnician;
            yield return FixturesTechnician;
            yield return Admin;
            yield return SuperAdmin;
        }
    }
}
