namespace WebApi.Data
{
    using System;

    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Token { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }
    }
}