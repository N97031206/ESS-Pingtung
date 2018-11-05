using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Inverter
{
    public class WarningStatus
    {
        public bool InverterFault { get; set; }
        public bool BusOver { get; set; }
        public bool BusUnder { get; set; }
        public bool BusSoftFail { get; set; }
        public bool LINE_FAIL { get; set; }
        public bool OPVShort { get; set; }
        public bool InverterVoltageTooLow { get; set; }
        public bool InverterVoltageTooHigh { get; set; }
        public bool OverTemperature { get; set; }
        public bool FanLocked { get; set; }
        public bool BatteryVoltageHigh { get; set; }
        public bool BatteryLowAlarm { get; set; }
        public bool BatteryUnderShutdown { get; set; }
        public bool OverLoad { get; set; }
        public bool EepromFault { get; set; }
        public bool InverterOverCurrent { get; set; }
        public bool InverterSoftFail { get; set; }
        public bool SelfTestFail { get; set; }
        public bool OP_DC_VoltageOver { get; set; }
        public bool BatOpen { get; set; }
        public bool CurrentSensorFail { get; set; }
        public bool BatteryShort { get; set; }
        public bool PowerLimit { get; set; }
        public bool PV_VoltageHigh { get; set; }
        public bool MPPT_OverloadFault { get; set; }
        public bool MPPT_OverloadWarning { get; set; }
        public bool BatteryTooLowToCharge { get; set; }
        public string Message { get; set; }


    }
}