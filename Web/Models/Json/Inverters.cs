using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Json.Inverter;

namespace Web.Models.Json
{
    public class Inverters
    {
        //start
        public int version { get; set; }    //	model版本	
        public int index { get; set; }  //	陣列編號	
        public string modelSerial { get; set; } //	型號	
        public string serialNO { get; set; }    //	序號	
        public string name { get; set; }    //	名稱	
        public bool connected { get; set; } //	連線通訊狀態	
        public DateTime UpdateTime { get; set; }
        public string DeviceMode { get; set; }
        public WarningStatus WarningStatus { get; set; }
        public IList< ParallelInformation >ParallelInformation { get; set; }
        public float GridVoltage { get; set; }
        public float GridFrequency { get; set; }
        public float AC_OutputVoltage { get; set; }
        public float AC_OutputFrequency { get; set; }
        public float AC_OutputApparentPower { get; set; }
        public float AC_OutputActivePower { get; set; }
        public float OutputLoadPercent { get; set; }
        public float BUSVoltage { get; set; }
        public float BatteryVoltage { get; set; }
        public float BatteryChargingCurrent { get; set; }
        public float BatteryCapacity { get; set; }
        public float InverterHeatSinkTemperature { get; set; }
        public float PV_InputCurrentForBattery { get; set; }
        public float PV_InputVoltage { get; set; }
        public float BatteryVoltageFrom_SCC { get; set; }
        public float BatteryDischargeCurrent { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
        public float BatteryVoltageOffsetForFansOn { get; set; }
        public float EEPROM_Version { get; set; }
        public float PV_ChargingPower { get; set; }

        //107/10/01 新增
        public float SPM90Voltage { get; set; }
        public float SPM90Current { get; set; }
        public float SPM90ActivePower { get; set; }
        public float SPM90ActiveEnergy { get; set; }
        public float SPM90VoltageDirection { get; set; }

    }
}