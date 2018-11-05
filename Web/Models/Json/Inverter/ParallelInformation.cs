using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Inverter
{
    public class ParallelInformation
    {
        public bool IsExist { get; set; }
        public string SerialNumber { get; set; }
        public string WorkMode { get; set; }
        public string FaultCode { get; set; }
        public float GridVoltage { get; set; }
        public float GridFrequency { get; set; }
        public float ACOutputVoltage { get; set; }
        public float ACOutputFrequency { get; set; }
        public float ACOutputApparentPower { get; set; }
        public float ACOutputActivePower { get; set; }
        public float LoadPercentage { get; set; }
        public float BatteryVoltage { get; set; }
        public float BatteryChargingCurrent { get; set; }
        public float BatteryCapacity { get; set; }
        public float PV_InputVoltage { get; set; }
        public float TotalChargingCurrent { get; set; }
        public float Total_AC_OutputApparentPower { get; set; }
        public float TotalOutputActivePower { get; set; }
        public float Total_AC_OutputPercentage { get; set; }
        public  InverterStatus InverterStatus { get; set; }
        public string OutputMode { get; set; }
        public string ChargerSourcePriority { get; set; }
        public float MaxChargerCurrent { get; set; }
        public float MaxChargerRange { get; set; }
        public float Max_AC_ChargerCurrent { get; set; }
        public float PV_InputCurrentForBattery { get; set; }
        public float BatteryDischargeCurrent { get; set; }

    }
}