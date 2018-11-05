using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Json.Batterys;

namespace Web.Models.Json
{
    public class Battery
    {
        public float version { get; set; }
        public float index { get; set; }
        public string modelSerial { get; set; }
        public string serialNO { get; set; }
        public string name { get; set; }
        public bool connected { get; set; }
        public DateTime updateTime { get; set; }
        public float voltage { get; set; }
        public float charging_current { get; set; }
        public float discharging_current { get; set; }
        public float charging_watt { get; set; }
        public float discharging_watt { get; set; }
        public float SOC { get; set; }
        public float Cycle { get; set; }
        public float charge_direction { get; set; }
        public float temperature { get; set; }
        public IList<cells> Cells { get; set; }
        public AlarmState AlarmState { get; set; }

    }
}