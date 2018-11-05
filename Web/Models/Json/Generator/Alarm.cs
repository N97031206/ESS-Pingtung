using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Generator
{
    public class Alarm
    {
        public float NumberOfNamedAlarms { get; set; }
        public float EmergencyStop { get; set; }
        public float LowOilPressure { get; set; }
        public float HighCoolantTemperature { get; set; }
        public float LowCoolantTemperature { get; set; }
        public float UnderSpeed { get; set; }
        public float OverSpeed { get; set; }
        public float GeneratorUnderFrequency { get; set; }
        public float GeneratorOverFrequency { get; set; }
        public float GeneratorLowVoltage { get; set; }
        public float GeneratorHighVoltage { get; set; }
        public float BatteryLowVoltage { get; set; }
        public float BatteryHighVoltage { get; set; }
        public float ChargeAlternatorFailure { get; set; }
        public float FailToStart { get; set; }
        public float FailToStop { get; set; }
        public float GeneratorFailToClose { get; set; }
        public float MainsFailToClose { get; set; }
        public float OilPressureSenderFault { get; set; }
        public float LossOfMagneticPickUp { get; set; }
        public float MagneticPickUpOpenCircuit { get; set; }
        public float GeneratorHighCurrent { get; set; }
        public float NoneA { get; set; }
        public float LowFuelLevel { get; set; }
        public float CANECUWarning { get; set; }
        public float CANECUShutdown { get; set; }
        public float CANECUDataFail { get; set; }
        public float LowOillevelSwitch { get; set; }
        public float HighTemperatureSwitch { get; set; }
        public float LowFuelLevelSwitch { get; set; }
        public float ExpansionUnitWatchdogAlarm { get; set; }
        public float kWOverloadAlarm { get; set; }
        public float NegativePhaseSequenceCurrentAlarm { get; set; }
        public float EarthFaultTripAlarm { get; set; }
        public float GeneratorPhaseRotationAlarm { get; set; }
        public float AutoVoltageSenseFail { get; set; }
        public float MaintenanceAlarm { get; set; }
        public float LoadingFrequencyAlarm { get; set; }
        public float LoadingVoltageAlarm { get; set; }
        public float NoneB { get; set; }
        public float NoneC { get; set; }
        public float NoneD { get; set; }
        public float NoneE { get; set; }
        public float GeneratorShortCircuit { get; set; }
        public float MainsHighCurrent { get; set; }
        public float MainsEarthFault { get; set; }
        public float MainsShortCircuit { get; set; }
        public float ECUProtect { get; set; }
        public float NoneF { get; set; }
        public string Message { get; set; }

    }
}