using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helios.Configuration;

namespace Helios.Wcf.Client
{
    public class FakeSetting : ISettings
    {
        public bool EnableOrderSystem { get; set; }

        public string ShortMessageServiceUrl { get; set; } 

    }
}
