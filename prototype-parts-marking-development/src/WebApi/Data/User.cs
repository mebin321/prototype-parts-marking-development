namespace WebApi.Data
{
    using System;
    using System.Collections.Generic;

    public class User : IAuditableEntity
    {
        public User()
        {
            RefreshTokens = new List<RefreshToken>();
            UserRoles = new List<UserRole>();
        }

        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string DomainIdentity { get; set; }

        public bool ServiceAccount { get; set; }

        public List<RefreshToken> RefreshTokens { get; }

        public List<UserRole> UserRoles { get; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}