using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Service.ESS.Model
{
    public class Generator
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
        public float OilPressure { get; set; }  //	發動機油壓	Kpa
        public float CoolantTemperature { get; set; }   //	發動機水温	℃
        public float OilTemperature { get; set; }   //	發動機油温	
        public float FuleLevel { get; set; }    //	發動機燃油油位	%
        public float InternalFlexibleSenderAnalogueInputType { get; set; }  //	靈活傳感器	
        public float ChargeAlternatorVoltage { get; set; }  //	充電發電機電壓	V
        public float EngineBatteryVoltage { get; set; } //	蓄電池電壓	V
        public float EngineSpeed { get; set; }  //	發動機轉速	RPM
        public int EngineRunTime { get; set; }    //	發動機運行時間	S
        public int NumberOfStarts { get; set; }   //	啟動次數	次
        public float frequency { get; set; }    //	發電機頻率	Hz
        public float L1Nvoltage { get; set; }   //	L1-N相電壓	V
        public float L2Nvoltage { get; set; }   //	L2-N相電壓	V
        public float L3Nvoltage { get; set; }   //	L3-N相電壓	V
        public float L1L2voltage { get; set; }  //	L1-L2線電壓	V
        public float L2L3voltage { get; set; }  //	L2-L3線電壓	V
        public float L3L1voltage { get; set; }  //	L3-L1線電壓	V
        public float L1current { get; set; }    //	L1相電流	A
        public float L2current { get; set; }    //	L2相電流	A
        public float L3current { get; set; }    //	L3相電流	A
        public float earthcurrent { get; set; } //	接地電流	A
        public float L1watts { get; set; }  //	L1相有功功率	W
        public float L2watts { get; set; }  //	L2相有功功率	W
        public float L3watts { get; set; }  //	L3相有功功率	W
        public float currentlaglead { get; set; }   //	電流超前/滯後	
        public float totalwatts { get; set; }   //	總的有功功率	W
        public float L1VA { get; set; } //	L1相視在功率	VA
        public float L2VA { get; set; } //	L2相視在功率	VA
        public float L3VA { get; set; } //	L3相視在功率	VA
        public float totalVA { get; set; }  //	總視在功率	VA
        public float L1Var { get; set; }    //	L1相無功功率	VAR
        public float L2Var { get; set; }    //	L2相無功功率	VAR
        public float L3Var { get; set; }    //	L3相無功功率	VAR
        public float totalVar { get; set; } //	總無功功率	VAR
        public float powerfactorL1 { get; set; }    //	L1相功率因數	
        public float powerfactorL2 { get; set; }    //	L2相功率因數	
        public float powerfactorL3 { get; set; }    //	L3相功率因數	
        public float averagepowerfactor { get; set; }   //	平均功率因數	
        public float percentageoffullpower { get; set; }    //	總功率的百分比	%
        public float percentageoffullVar { get; set; }  //	總無功功率的百分比	%
        public float positiveKWhours { get; set; }  //	正的千瓦時	kWh
        public float negativeKWhours { get; set; }  //	負的千瓦時	kWh
        public float KVAhours { get; set; } //	正的KVA/h	kVAh
        public float KVArhours { get; set; }    //	正的KVAr/h	kVAh
        public string ControlStatus { get; set; }   //	控制狀態	
        //alarm start
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
        //alarm end
        public float AvailabilityEnergy { get; set; }
        public float AvailabilityHour { get; set; }
        public bool FuelRelay { get; set; }
        public bool StartRelay { get; set; }
        public bool DigitalOutC { get; set; }
        public bool DigitalOutD { get; set; }
        public bool DigitalOutE { get; set; }
        public bool DigitalOutF { get; set; }
        public bool DigitalOutG { get; set; }
        public bool DigitalOutH { get; set; }
        public bool STOPLEDstatus { get; set; }
        public bool MANUALLEDstatus { get; set; }
        public bool TESTLEDstatus { get; set; }
        public bool AUTOLEDstatus { get; set; }
        public bool GENLEDstatus { get; set; }
        public bool GENBREAKERLEDstatus { get; set; }
        public bool MAINSLEDstatus { get; set; }
        public bool MAINSBREAKERLEDstatus { get; set; }
        public bool USERLED1status { get; set; }
        public bool USERLED2statu { get; set; }
        public bool USERLED3status { get; set; }
        public bool USERLED4status { get; set; }

        //end
    }
}
