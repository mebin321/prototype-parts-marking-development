namespace WebApi.Data
{
    using System.Collections.Generic;

    public class Role
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }

        public int Id { get; set; }

        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<UserRole> UserRoles { get; }
    }
}