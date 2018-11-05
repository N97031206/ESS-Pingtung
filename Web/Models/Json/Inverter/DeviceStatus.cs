using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Inverter
{
    public class DeviceStatus
    {
        public bool Has_SBU_PriorityVersion { get; set; }
        public bool ConfigurationStatus_Change { get; set; }
        public bool SCC_FirmwareVersion_Updated { get; set; }
        public bool LoadStatus_On { get; set; }
        public bool BatteryVoltageTOSteadyWhileCharging { get; set; }
        public bool ChargingStatus_On { get; set; }
        public bool ChargingSstatus_SCC_Charging_On { get; set; }
        public bool ChargingStatus_AC_Charging_On { get; set; }
        public string ChargingStatusCharging { get; set; }
        public bool ChargingToFloatingMode { get; set; }
        public bool SwitchOn { get; set; }

    }
}