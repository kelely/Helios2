using Helios.Domain;

namespace Helios.Authentication.Domain
{
    public class Client : BaseEntity
    {
        public string Key { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public ApplicationTypes ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }
    }
}
