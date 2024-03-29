using System;
using Helios.Domain;

namespace Helios.Authentication.Domain
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }

        public string Subject { get; set; }

        public string ClientId { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public string ProtectedTicket { get; set; }
    }
}