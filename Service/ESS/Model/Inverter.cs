﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Service.ESS.Model
{
    public class Inverter
    {       
        [Display(Name = "帳戶編號")]
        public Guid Id { get; set; }
        public int version { get; set; }    //	model版本	
        public int index { get; set; }  //	陣列編號	
        public string modelSerial { get; set; } //	型號	
        public string serialNO { get; set; }    //	序號	
        public string name { get; set; }    //	名稱	
        public bool connected { get; set; } //	連線通訊狀態	
        public DateTime UpdateTime { get; set; }
        public string DeviceMode { get; set; }
        //WarningStatus start
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
        //WarningStatus end

        //ParallelInformation start
        public bool ParallelInformation_IsExist { get; set; }
        public string ParallelInformation_SerialNumber { get; set; }
        public string ParallelInformation_WorkMode { get; set; }
        public string ParallelInformation_FaultCode { get; set; }
        public float ParallelInformation_GridVoltage { get; set; }
        public float ParallelInformation_GridFrequency { get; set; }
        public float ParallelInformation_ACOutputVoltage { get; set; }
        public float ParallelInformation_ACOutputFrequency { get; set; }
        public float ParallelInformation_ACOutputApparentPower { get; set; }
        public float ParallelInformation_ACOutputActivePower { get; set; }
        public float ParallelInformation_LoadPercentage { get; set; }
        public float ParallelInformation_BatteryVoltage { get; set; }
        public float ParallelInformation_BatteryChargingCurrent { get; set; }
        public float ParallelInformation_BatteryCapacity { get; set; }
        public float ParallelInformation_PV_InputVoltage { get; set; }
        public float ParallelInformation_TotalChargingCurrent { get; set; }
        public float ParallelInformation_Total_AC_OutputApparentPower { get; set; }
        public float ParallelInformation_TotalOutputActivePower { get; set; }
        public float ParallelInformation_Total_AC_OutputPercentage { get; set; }
        //InverterStatus start
        public bool SCC_OK { get; set; }
        public bool AC_Charging { get; set; }
        public bool SCC_Charging { get; set; }
        public string Battery { get; set; }
        public bool Line_OK { get; set; }
        public bool loadOn { get; set; }
        public bool ConfigurationChange { get; set; }
        //InverterStatus end
        public string ParallelInformation_OutputMode { get; set; }
        public string ParallelInformation_ChargerSourcePriority { get; set; }
        public float ParallelInformation_MaxChargerCurrent { get; set; }
        public float ParallelInformation_MaxChargerRange { get; set; }
        public float ParallelInformation_Max_AC_ChargerCurrent { get; set; }
        public float ParallelInformation_PV_InputCurrentForBattery { get; set; }
        public float ParallelInformation_BatteryDischargeCurrent { get; set; }
        //ParallelInformation end
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
        //DeviceStatus start
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
        //DeviceStatus end
        public float BatteryVoltageOffsetForFansOn { get; set; }
        public float EEPROM_Version { get; set; }
        public float PV_ChargingPower { get; set; }
        //107/10/01 新增
        [Display(Name = "SPM90 電表量測電壓")]
        public float SPM90Voltage { get; set; }
        [Display(Name = " SPM90 電表量測電流")]
        public float SPM90Current { get; set; }
        [Display(Name = "SPM90 電表量測瓦特")]
        public float SPM90ActivePower { get; set; }
        [Display(Name = "SPM90 電表量測度數")]
        public float SPM90ActiveEnergy { get; set; }
        [Display(Name = "SPM90 電表量測 方向 0 正向 1 反向")]
        public float SPM90VoltageDirection { get; set; }

        //end
    }
}

