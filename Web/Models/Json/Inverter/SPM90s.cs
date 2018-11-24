using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Inverter
{
    public class SPM90s
    {
        public float id { get; set; }
        public bool connected { get; set; }
        public float Voltage { get; set; }
        public float Current { get; set; }
        public float ActivePower { get; set; }
        public float ActiveEnergy { get; set; }
        public float VoltageDirection { get; set; }
    }
}