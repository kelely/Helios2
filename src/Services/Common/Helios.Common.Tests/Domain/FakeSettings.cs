using Helios.Configuration;

namespace Helios.Common.Domain
{
    public class FakeSettings : ISettings
    {
        public bool EnableOrderSystem { get; set; }

        public string ShortMessageServiceUrl { get; set; }

    }
}