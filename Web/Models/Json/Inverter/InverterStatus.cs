using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Inverter
{
    public class InverterStatus
    {
        public bool SCC_OK { get; set; }
        public bool AC_Charging { get; set; }
        public bool SCC_Charging { get; set; }
        public string Battery { get; set; }
        public bool Line_OK { get; set; }
        public bool loadOn { get; set; }
        public bool ConfigurationChange { get; set; }

    }
}