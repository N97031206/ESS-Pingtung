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
        //20181203新增
        public float ActiveEnergyMinus { get; set; } //相差總實功電能(與前一筆)

        public float SPM90ActiveEnergyMinus1 { get; set; } //相差總實功電能(與前一筆)
        public float SPM90ActiveEnergyMinus2 { get; set; } //相差總實功電能(與前一筆)

        public float VoltageDirection { get; set; }
    }
}