using Newtonsoft.Json;
using NLog;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Json;

namespace Web.Controllers
{
    public class DataInputController : Controller
    {
        private ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();
        public schemaJson JsonObject = new schemaJson();
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();//Log檔

        // GET: DataInput
        public ActionResult Index()
        {
            return View();
        }

        #region Json
        public ActionResult Json(schemaJson json)
        {
            try
            {
                if (json.stationUUID == null)
                {
                    JsonObject = (schemaJson)JsonConvert.DeserializeObject(JsonString2, typeof(schemaJson));
                    ViewBag.json = "Get";
                    logger.Trace("Json Get");
                    saveSQL(JsonObject, "");
                }
                else
                {
                    ModelState.Clear();
                    JsonObject = json;
                    ViewBag.json = "Post In";
                    logger.Info(json);

                }
                return View(JsonObject);
            }
            catch (Exception ex)
            {
                logger.Fatal("Json Get error");
                logger.Fatal(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Json(string json)
        {
            try
            {
                ViewBag.json = "Post";
                logger.Trace("Json Post Input");
                JsonObject = (schemaJson)JsonConvert.DeserializeObject(json, typeof(schemaJson));
                //寫入資料庫
                saveSQL(JsonObject, json);
                return null;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error Json Post");
                logger.Fatal(json);
                logger.Fatal(ex.ToString());
                logger.Fatal("Error END");
                return null;
            }
        }

        #endregion

        #region saveSQL
        /// <summary>
        /// json寫入資料庫
        /// </summary>
        /// <param name="JsonObject"></param>
        private void saveSQL(schemaJson JsonObject, string json)
        {
            #region 多工 Task
            Task.Run(() =>
            {
                try
                {
                    Guid gridPowerID = new Guid();
                    Guid loadPowerID = new Guid();
                    Guid generatorID = new Guid();
                    Guid inverterID = new Guid();
                    Guid batteryID = new Guid();
                    Guid ESSId = new Guid();

                    string gridPowerIDs = null;
                    string loadPowerIDs = null;
                    string inverterIDs = null;
                    string batteryIDs = null;
                    string generatorIDs = null;

                    #region gridPower
                    GridPower gridPower = new GridPower();
                    if (JsonObject.GridPowers.Count > 0)
                    {
                        foreach (var Json in JsonObject.GridPowers)
                        {
                            gridPower.version = Json.version;
                            gridPower.index = Json.index;
                            gridPower.modelSerial = Json.modelSerial;
                            gridPower.serialNO = Json.serialNO;
                            gridPower.name = Json.name;
                            // gridPower.updateTime = Json.updateTime;
                            gridPower.date_time = Json.date_time;
                            gridPower.VA = Json.VA;
                            gridPower.VB = Json.VB;
                            gridPower.VC = Json.VC;
                            gridPower.Vavg = Json.Vavg;
                            gridPower.Ia = Json.Ia;
                            gridPower.Ib = Json.Ib;
                            gridPower.Ic = Json.Ic;
                            gridPower.In = Json.In;
                            gridPower.Isum = Json.Isum;
                            gridPower.Watt_a = Json.Watt_a;
                            gridPower.Watt_b = Json.Watt_b;
                            gridPower.Watt_c = Json.Watt_c;
                            gridPower.Watt_t = Json.Watt_t;
                            gridPower.Var_a = Json.Var_a;
                            gridPower.Var_b = Json.Var_b;
                            gridPower.Var_c = Json.Var_c;
                            gridPower.Var_t = Json.Var_t;
                            gridPower.VA_a = Json.VA_a;
                            gridPower.VA_b = Json.VA_b;
                            gridPower.VA_c = Json.VA_c;
                            gridPower.VA_t = Json.VA_t;
                            gridPower.PF_a = Json.PF_a;
                            gridPower.PF_b = Json.PF_b;
                            gridPower.PF_c = Json.PF_c;
                            gridPower.PF_t = Json.PF_t;
                            gridPower.Angle_Va = Json.Angle_Va;
                            gridPower.Angle_Vb = Json.Angle_Vb;
                            gridPower.Angle_Vc = Json.Angle_Vc;
                            gridPower.Angle_Ia = Json.Angle_Ia;
                            gridPower.Angle_Ib = Json.Angle_Ib;
                            gridPower.Angle_Ic = Json.Angle_Ic;
                            gridPower.Frequency = Json.Frequency;
                            gridPower.Vab = Json.Vab;
                            gridPower.Vbc = Json.Vbc;
                            gridPower.Vca = Json.Vca;
                            gridPower.VIIavg = Json.VIIavg;
                            gridPower.kWHt = Json.kWHt;
                            gridPower.kWHa = Json.kWHa;
                            gridPower.kWHb = Json.kWHb;
                            gridPower.kWHc = Json.kWHc;
                            gridPower.kVarHt = Json.kVarHt;
                            gridPower.kVarHa = Json.kVarHa;
                            gridPower.kVarHb = Json.kVarHb;
                            gridPower.kVarHc = Json.kVarHc;
                            gridPower.kVAHt = Json.kVAHt;
                            gridPower.kVAHa = Json.kVAHa;
                            gridPower.kVAHb = Json.kVAHb;
                            gridPower.kVAHc = Json.kVAHc;
                            gridPower.Demand = Json.Demand;
                            gridPower.Prev_Demand = Json.Prev_Demand;
                            gridPower.Prev_Demand2 = Json.Prev_Demand2;
                            gridPower.Prev_Demand3 = Json.Prev_Demand3;
                            gridPower.Max_Demand_CurrnetMonth = Json.Max_Demand_CurrnetMonth;
                            gridPower.Max_Demand_LastMonth = Json.Max_Demand_LastMonth;
                            gridPower.Remain_Time = Json.Remain_Time;
                            if (Json.events.ToList().Count == 0)
                            {
                                gridPower.ErrorMessage = "Not Message";
                                gridPower.event_info = 0;
                                gridPower.event_date_time = DateTime.Now;
                                gridPower.Alarm = 0;
                                gridPower.ELeve = 0;
                                gridPower.EType = 0;
                                gridPower.ELoop = 0;
                            }
                            else
                            {
                                foreach (var ev in Json.events.ToList())
                                {
                                    gridPower.ErrorMessage = ev.ErrorMessage == null ? "Not Message" : ev.ErrorMessage;
                                    gridPower.event_info = ev.event_info;
                                    gridPower.event_date_time = ev.event_date_time;
                                    gridPower.Alarm = ev.info.Alarm;
                                    gridPower.ELeve = ev.info.ELeve;
                                    gridPower.EType = ev.info.EType;
                                    gridPower.ELoop = ev.info.ELoop;
                                }
                            }

                            gridPowerID = GridPowerService.Create(gridPower);
                            
                            gridPowerIDs += gridPowerID.ToString() + "+";
                        }

                    }
                    else
                    {
                      //  gridPowerID = Guid.NewGuid();
                    }

                    #endregion

                    #region loadPower
                    LoadPower loadPower = new LoadPower();
                    if (JsonObject.LoadPowers.Count > 0)
                    {
                        foreach (var Json in JsonObject.LoadPowers)
                        {
                            loadPower.version = Json.version;
                            loadPower.index = Json.index;
                            loadPower.modelSerial = Json.modelSerial;
                            loadPower.serialNO = Json.serialNO;
                            loadPower.name = Json.name;
                            loadPower.connected = Json.connected;
                            loadPower.updateTime = Json.updateTime;
                            loadPower.date_Time = Json.date_Time;
                            loadPower.VA = Json.VA;
                            loadPower.VB = Json.VB;
                            loadPower.VC = Json.VC;
                            loadPower.Vavg = Json.Vavg;
                            loadPower.Ia = Json.Ia;
                            loadPower.Ib = Json.Ib;
                            loadPower.Ic = Json.Ic;
                            loadPower.In = Json.In;
                            loadPower.Isum = Json.Isum;
                            loadPower.Watt_a = Json.Watt_a;
                            loadPower.Watt_b = Json.Watt_b;
                            loadPower.Watt_c = Json.Watt_c;
                            loadPower.Watt_t = Json.Watt_t;
                            loadPower.Var_a = Json.Var_a;
                            loadPower.Var_b = Json.Var_b;
                            loadPower.Var_c = Json.Var_c;
                            loadPower.Var_t = Json.Var_t;
                            loadPower.VA_a = Json.VA_a;
                            loadPower.VA_b = Json.VA_b;
                            loadPower.VA_c = Json.VA_c;
                            loadPower.VA_t = Json.VA_t;
                            loadPower.PF_a = Json.PF_a;
                            loadPower.PF_b = Json.PF_b;
                            loadPower.PF_c = Json.PF_c;
                            loadPower.PF_t = Json.PF_t;
                            loadPower.Angle_Va = Json.Angle_Va;
                            loadPower.Angle_Vb = Json.Angle_Vb;
                            loadPower.Angle_Vc = Json.Angle_Vc;
                            loadPower.Angle_Ia = Json.Angle_Ia;
                            loadPower.Angle_Ib = Json.Angle_Ib;
                            loadPower.Angle_Ic = Json.Angle_Ic;
                            loadPower.Frequency = Json.Frequency;
                            loadPower.Vab = Json.Vab;
                            loadPower.Vbc = Json.Vbc;
                            loadPower.Vca = Json.Vca;
                            loadPower.VIIavg = Json.VIIavg;
                            loadPower.kWHt = Json.kWHt;
                            loadPower.kWHa = Json.kWHa;
                            loadPower.kWHb = Json.kWHb;
                            loadPower.kWHc = Json.kWHc;
                            loadPower.kVarHt = Json.kVarHt;
                            loadPower.kVarHa = Json.kVarHa;
                            loadPower.kVarHb = Json.kVarHb;
                            loadPower.kVarHc = Json.kVarHc;
                            loadPower.kVAHt = Json.kVAHt;
                            loadPower.kVAHa = Json.kVAHa;
                            loadPower.kVAHb = Json.kVAHb;
                            loadPower.kVAHc = Json.kVAHc;
                            loadPower.Demand = Json.Demand;
                            loadPower.prev_demand = Json.prev_demand;
                            loadPower.prev_demand2 = Json.prev_demand2;
                            loadPower.prev_demand3 = Json.prev_demand3;
                            loadPower.maxdemand_currnetmonth = Json.maxdemand_currnetmonth;
                            loadPower.maxdemand_lastmonth = Json.maxdemand_lastmonth;
                            loadPower.remain_time = Json.remain_time;
                            foreach (var ev in Json.events)
                            {
                                loadPower.ErrorMessage = ev.ErrorMessage;
                                loadPower.event_info = ev.event_info;
                                loadPower.event_date_time = ev.event_date_time;
                                loadPower.Alarm = ev.info.Alarm;
                                loadPower.ELeve = ev.info.ELeve;
                                loadPower.EType = ev.info.EType;
                                loadPower.ELoop = ev.info.ELoop;
                            }

                            loadPowerID = LoadPowerService.Create(loadPower);
                            loadPowerIDs += loadPowerID.ToString() + "+";
                        }


                    }
                    else
                    {
                       // loadPowerID = Guid.NewGuid();
                    }
                    #endregion

                    #region generator
                    Generator generator = new Generator();
                    if (JsonObject.Generators.Count > 0)
                    {
                        foreach (var GenJson in JsonObject.Generators)
                        {
                            generator.version = GenJson.version;
                            generator.index = GenJson.index;
                            generator.modelSerial = GenJson.modelSerial;
                            generator.serialNO = GenJson.serialNO;
                            generator.name = GenJson.name;
                            generator.connected = GenJson.connected;
                            generator.UpdateTime = GenJson.UpdateTime;
                            generator.OilPressure = GenJson.OilPressure;
                            generator.CoolantTemperature = GenJson.CoolantTemperature;
                            generator.OilTemperature = GenJson.OilTemperature;
                            generator.FuleLevel = GenJson.FuleLevel;
                            generator.InternalFlexibleSenderAnalogueInputType = GenJson.InternalFlexibleSenderAnalogueInputType;
                            generator.ChargeAlternatorVoltage = GenJson.ChargeAlternatorVoltage;
                            generator.EngineBatteryVoltage = GenJson.EngineBatteryVoltage;
                            generator.EngineSpeed = GenJson.EngineSpeed;
                            generator.EngineRunTime = GenJson.EngineRunTime;
                            generator.NumberOfStarts = GenJson.NumberOfStarts;
                            generator.frequency = GenJson.frequency;
                            generator.L1Nvoltage = GenJson.L1Nvoltage;
                            generator.L2Nvoltage = GenJson.L2Nvoltage;
                            generator.L3Nvoltage = GenJson.L3Nvoltage;
                            generator.L1L2voltage = GenJson.L1L2voltage;
                            generator.L2L3voltage = GenJson.L2L3voltage;
                            generator.L3L1voltage = GenJson.L3L1voltage;
                            generator.L1current = GenJson.L1current;
                            generator.L2current = GenJson.L2current;
                            generator.L3current = GenJson.L3current;
                            generator.earthcurrent = GenJson.earthcurrent;
                            generator.L1watts = GenJson.L1watts;
                            generator.L2watts = GenJson.L2watts;
                            generator.L3watts = GenJson.L3watts;
                            generator.currentlaglead = GenJson.currentlaglead;
                            generator.totalwatts = GenJson.totalwatts;
                            generator.L1VA = GenJson.L1VA;
                            generator.L2VA = GenJson.L2VA;
                            generator.L3VA = GenJson.L3VA;
                            generator.totalVA = GenJson.totalVA;
                            generator.L1Var = GenJson.L1Var;
                            generator.L2Var = GenJson.L2Var;
                            generator.L3Var = GenJson.L3Var;
                            generator.totalVar = GenJson.totalVar;
                            generator.powerfactorL1 = GenJson.powerfactorL1;
                            generator.powerfactorL2 = GenJson.powerfactorL2;
                            generator.powerfactorL3 = GenJson.powerfactorL3;
                            generator.averagepowerfactor = GenJson.averagepowerfactor;
                            generator.percentageoffullpower = GenJson.percentageoffullpower;
                            generator.percentageoffullVar = GenJson.percentageoffullVar;
                            generator.positiveKWhours = GenJson.positiveKWhours;
                            generator.negativeKWhours = GenJson.negativeKWhours;
                            generator.KVAhours = GenJson.KVAhours;
                            generator.KVArhours = GenJson.KVArhours;
                            generator.ControlStatus = GenJson.ControlStatus;
                            generator.NumberOfNamedAlarms = GenJson.Alarm.NumberOfNamedAlarms;
                            generator.EmergencyStop = GenJson.Alarm.EmergencyStop;
                            generator.LowOilPressure = GenJson.Alarm.LowOilPressure;
                            generator.HighCoolantTemperature = GenJson.Alarm.HighCoolantTemperature;
                            generator.LowCoolantTemperature = GenJson.Alarm.LowCoolantTemperature;
                            generator.UnderSpeed = GenJson.Alarm.UnderSpeed;
                            generator.OverSpeed = GenJson.Alarm.OverSpeed;
                            generator.GeneratorUnderFrequency = GenJson.Alarm.GeneratorUnderFrequency;
                            generator.GeneratorOverFrequency = GenJson.Alarm.GeneratorOverFrequency;
                            generator.GeneratorLowVoltage = GenJson.Alarm.GeneratorLowVoltage;
                            generator.GeneratorHighVoltage = GenJson.Alarm.GeneratorHighVoltage;
                            generator.BatteryLowVoltage = GenJson.Alarm.BatteryLowVoltage;
                            generator.BatteryHighVoltage = GenJson.Alarm.BatteryHighVoltage;
                            generator.ChargeAlternatorFailure = GenJson.Alarm.ChargeAlternatorFailure;
                            generator.FailToStart = GenJson.Alarm.FailToStart;
                            generator.FailToStop = GenJson.Alarm.FailToStop;
                            generator.GeneratorFailToClose = GenJson.Alarm.GeneratorFailToClose;
                            generator.MainsFailToClose = GenJson.Alarm.MainsFailToClose;
                            generator.OilPressureSenderFault = GenJson.Alarm.OilPressureSenderFault;
                            generator.LossOfMagneticPickUp = GenJson.Alarm.LossOfMagneticPickUp;
                            generator.MagneticPickUpOpenCircuit = GenJson.Alarm.MagneticPickUpOpenCircuit;
                            generator.GeneratorHighCurrent = GenJson.Alarm.GeneratorHighCurrent;
                            generator.NoneA = GenJson.Alarm.NoneA;
                            generator.LowFuelLevel = GenJson.Alarm.LowFuelLevel;
                            generator.CANECUWarning = GenJson.Alarm.CANECUWarning;
                            generator.CANECUShutdown = GenJson.Alarm.CANECUShutdown;
                            generator.CANECUDataFail = GenJson.Alarm.CANECUDataFail;
                            generator.LowOillevelSwitch = GenJson.Alarm.LowOillevelSwitch;
                            generator.HighTemperatureSwitch = GenJson.Alarm.HighTemperatureSwitch;
                            generator.LowFuelLevelSwitch = GenJson.Alarm.LowFuelLevelSwitch;
                            generator.ExpansionUnitWatchdogAlarm = GenJson.Alarm.ExpansionUnitWatchdogAlarm;
                            generator.kWOverloadAlarm = GenJson.Alarm.kWOverloadAlarm;
                            generator.NegativePhaseSequenceCurrentAlarm = GenJson.Alarm.NegativePhaseSequenceCurrentAlarm;
                            generator.EarthFaultTripAlarm = GenJson.Alarm.EarthFaultTripAlarm;
                            generator.GeneratorPhaseRotationAlarm = GenJson.Alarm.GeneratorPhaseRotationAlarm;
                            generator.AutoVoltageSenseFail = GenJson.Alarm.AutoVoltageSenseFail;
                            generator.MaintenanceAlarm = GenJson.Alarm.MaintenanceAlarm;
                            generator.LoadingFrequencyAlarm = GenJson.Alarm.LoadingFrequencyAlarm;
                            generator.LoadingVoltageAlarm = GenJson.Alarm.LoadingVoltageAlarm;
                            generator.NoneB = GenJson.Alarm.NoneB;
                            generator.NoneC = GenJson.Alarm.NoneC;
                            generator.NoneD = GenJson.Alarm.NoneD;
                            generator.NoneE = GenJson.Alarm.NoneE;
                            generator.GeneratorShortCircuit = GenJson.Alarm.GeneratorShortCircuit;
                            generator.MainsHighCurrent = GenJson.Alarm.MainsHighCurrent;
                            generator.MainsEarthFault = GenJson.Alarm.MainsEarthFault;
                            generator.MainsShortCircuit = GenJson.Alarm.MainsShortCircuit;
                            generator.ECUProtect = GenJson.Alarm.ECUProtect;
                            generator.NoneF = GenJson.Alarm.NoneF;
                            generator.Message = GenJson.Alarm.Message;
                            generator.AvailabilityEnergy = GenJson.AvailabilityEnergy;
                            generator.AvailabilityHour = GenJson.AvailabilityHour;

                            generatorID = GeneratorService.Create(generator);
                            generatorIDs += generatorID.ToString()+"+";
                        }

                    }
                    else
                    {
                        //generatorID = Guid.NewGuid();
                    }

                    #endregion

                    #region inverter
                    Inverter inverter = new Inverter();
                    if (JsonObject.Inverters.Count > 0)
                    {
                        foreach (var Json in JsonObject.Inverters)
                        {
                            inverter.version = Json.version;
                            inverter.index = Json.index;
                            inverter.modelSerial = Json.modelSerial;
                            inverter.serialNO = Json.serialNO;
                            inverter.name = Json.name;
                            inverter.connected = Json.connected;
                            inverter.UpdateTime = Json.UpdateTime;
                            inverter.DeviceMode = Json.DeviceMode;
                            //WarningStatus
                            inverter.BusOver = Json.WarningStatus.BusOver;
                            inverter.BusUnder = Json.WarningStatus.BusUnder;
                            inverter.BusSoftFail = Json.WarningStatus.BusSoftFail;
                            inverter.LINE_FAIL = Json.WarningStatus.LINE_FAIL;
                            inverter.OPVShort = Json.WarningStatus.OPVShort;
                            inverter.InverterVoltageTooLow = Json.WarningStatus.InverterVoltageTooLow;
                            inverter.InverterVoltageTooHigh = Json.WarningStatus.InverterVoltageTooHigh;
                            inverter.OverTemperature = Json.WarningStatus.OverTemperature;
                            inverter.FanLocked = Json.WarningStatus.FanLocked;
                            inverter.BatteryVoltageHigh = Json.WarningStatus.BatteryVoltageHigh;
                            inverter.BatteryLowAlarm = Json.WarningStatus.BatteryLowAlarm;
                            inverter.BatteryUnderShutdown = Json.WarningStatus.BatteryUnderShutdown;
                            inverter.OverLoad = Json.WarningStatus.OverLoad;
                            inverter.EepromFault = Json.WarningStatus.EepromFault;
                            inverter.InverterOverCurrent = Json.WarningStatus.InverterOverCurrent;
                            inverter.InverterSoftFail = Json.WarningStatus.InverterSoftFail;
                            inverter.SelfTestFail = Json.WarningStatus.SelfTestFail;
                            inverter.OP_DC_VoltageOver = Json.WarningStatus.OP_DC_VoltageOver;
                            inverter.BatOpen = Json.WarningStatus.BatOpen;
                            inverter.CurrentSensorFail = Json.WarningStatus.CurrentSensorFail;
                            inverter.BatteryShort = Json.WarningStatus.BatteryShort;
                            inverter.PowerLimit = Json.WarningStatus.PowerLimit;
                            inverter.PV_VoltageHigh = Json.WarningStatus.PV_VoltageHigh;
                            inverter.MPPT_OverloadFault = Json.WarningStatus.MPPT_OverloadFault;
                            inverter.MPPT_OverloadWarning = Json.WarningStatus.MPPT_OverloadWarning;
                            inverter.BatteryTooLowToCharge = Json.WarningStatus.BatteryTooLowToCharge;
                            inverter.Message = Json.WarningStatus.Message;
                            //ParallelInformation
                            foreach (var p in Json.ParallelInformation)
                            {
                                inverter.ParallelInformation_IsExist = p.IsExist;
                                inverter.ParallelInformation_SerialNumber = p.SerialNumber;
                                inverter.ParallelInformation_WorkMode = p.WorkMode;
                                inverter.ParallelInformation_FaultCode = p.FaultCode;
                                inverter.ParallelInformation_GridVoltage = p.GridVoltage;
                                inverter.ParallelInformation_GridFrequency = p.GridFrequency;
                                inverter.ParallelInformation_ACOutputVoltage = p.ACOutputVoltage;
                                inverter.ParallelInformation_ACOutputFrequency = p.ACOutputFrequency;
                                inverter.ParallelInformation_ACOutputApparentPower = p.ACOutputApparentPower;
                                inverter.ParallelInformation_ACOutputActivePower = p.ACOutputActivePower;
                                inverter.ParallelInformation_LoadPercentage = p.LoadPercentage;
                                inverter.ParallelInformation_BatteryVoltage = p.BatteryVoltage;
                                inverter.ParallelInformation_BatteryChargingCurrent = p.BatteryChargingCurrent;
                                inverter.ParallelInformation_BatteryCapacity = p.BatteryCapacity;
                                inverter.ParallelInformation_PV_InputVoltage = p.PV_InputVoltage;
                                inverter.ParallelInformation_TotalChargingCurrent = p.TotalChargingCurrent;
                                inverter.ParallelInformation_Total_AC_OutputApparentPower = p.Total_AC_OutputApparentPower;
                                inverter.ParallelInformation_TotalOutputActivePower = p.TotalOutputActivePower;
                                inverter.ParallelInformation_Total_AC_OutputPercentage = p.Total_AC_OutputPercentage;
                                //InverterStatus start
                                inverter.SCC_OK = p.InverterStatus.SCC_OK;
                                inverter.AC_Charging = p.InverterStatus.AC_Charging;
                                inverter.SCC_Charging = p.InverterStatus.SCC_Charging;
                                inverter.Battery = p.InverterStatus.Battery;
                                inverter.Line_OK = p.InverterStatus.Line_OK;
                                inverter.loadOn = p.InverterStatus.loadOn;
                                //end
                                inverter.ConfigurationChange = p.InverterStatus.ConfigurationChange;
                                inverter.ParallelInformation_OutputMode = p.OutputMode;
                                inverter.ParallelInformation_ChargerSourcePriority = p.ChargerSourcePriority;
                                inverter.ParallelInformation_MaxChargerCurrent = p.MaxChargerCurrent;
                                inverter.ParallelInformation_MaxChargerRange = p.MaxChargerRange;
                                inverter.ParallelInformation_Max_AC_ChargerCurrent = p.Max_AC_ChargerCurrent;
                                inverter.ParallelInformation_PV_InputCurrentForBattery = p.PV_InputCurrentForBattery;
                                inverter.ParallelInformation_BatteryDischargeCurrent = p.BatteryDischargeCurrent;
                            }
                            inverter.GridVoltage = Json.GridVoltage;
                            inverter.GridFrequency = Json.GridFrequency;
                            inverter.AC_OutputVoltage = Json.AC_OutputVoltage;
                            inverter.AC_OutputFrequency = Json.AC_OutputFrequency;
                            inverter.AC_OutputApparentPower = Json.AC_OutputApparentPower;
                            inverter.AC_OutputActivePower = Json.AC_OutputActivePower;
                            inverter.OutputLoadPercent = Json.OutputLoadPercent;
                            inverter.BUSVoltage = Json.BUSVoltage;
                            inverter.BatteryVoltage = Json.BatteryVoltage;
                            inverter.BatteryChargingCurrent = Json.BatteryChargingCurrent;
                            inverter.BatteryCapacity = Json.BatteryCapacity;
                            inverter.InverterHeatSinkTemperature = Json.InverterHeatSinkTemperature;
                            inverter.PV_InputCurrentForBattery = Json.PV_InputCurrentForBattery;
                            inverter.PV_InputVoltage = Json.PV_InputVoltage;
                            inverter.BatteryVoltageFrom_SCC = Json.BatteryVoltageFrom_SCC;
                            inverter.BatteryDischargeCurrent = Json.BatteryDischargeCurrent;
                            inverter.Has_SBU_PriorityVersion = Json.DeviceStatus.Has_SBU_PriorityVersion;
                            inverter.ConfigurationStatus_Change = Json.DeviceStatus.ConfigurationStatus_Change;
                            inverter.SCC_FirmwareVersion_Updated = Json.DeviceStatus.SCC_FirmwareVersion_Updated;
                            inverter.LoadStatus_On = Json.DeviceStatus.LoadStatus_On;
                            inverter.BatteryVoltageTOSteadyWhileCharging = Json.DeviceStatus.BatteryVoltageTOSteadyWhileCharging;
                            inverter.ChargingStatus_On = Json.DeviceStatus.ChargingStatus_On;
                            inverter.ChargingSstatus_SCC_Charging_On = Json.DeviceStatus.ChargingSstatus_SCC_Charging_On;
                            inverter.ChargingStatus_AC_Charging_On = Json.DeviceStatus.ChargingStatus_AC_Charging_On;
                            inverter.ChargingStatusCharging = Json.DeviceStatus.ChargingStatusCharging;
                            inverter.ChargingToFloatingMode = Json.DeviceStatus.ChargingToFloatingMode;
                            inverter.SwitchOn = Json.DeviceStatus.SwitchOn;
                            inverter.BatteryVoltageOffsetForFansOn = Json.BatteryVoltageOffsetForFansOn;
                            inverter.EEPROM_Version = Json.EEPROM_Version;
                            inverter.PV_ChargingPower = Json.PV_ChargingPower;
                            inverter.SPM90Voltage = Json.SPM90Voltage;
                            inverter.SPM90Current = Json.SPM90Current;
                            inverter.SPM90ActivePower = Json.SPM90ActivePower;
                            inverter.SPM90ActiveEnergy = Json.SPM90ActiveEnergy;
                            inverter.SPM90VoltageDirection = Json.SPM90VoltageDirection;

                            inverterID = InverterService.Create(inverter);
                            inverterIDs += inverterID.ToString() + "+";
                        }


                    }
                    else
                    {
                      //  inverterID = Guid.NewGuid();
                    }

                    #endregion

                    #region battery
                    Service.ESS.Model.Battery battery = new Service.ESS.Model.Battery();

                    if (JsonObject.Battery.Count > 0)
                    {
                        foreach (var Json in JsonObject.Battery)
                        {                          
                            battery.version = Json.version;
                            battery.index = Json.index;
                            battery.modelSerial = Json.modelSerial;
                            battery.serialNO = Json.serialNO;
                            battery.name = Json.name;
                            battery.connected = Json.connected;
                            //battery.updateTime = Json.updateTime;
                            battery.voltage = Json.voltage;
                            battery.charging_current = Json.charging_current;
                            battery.discharging_current = Json.discharging_current;
                            battery.charging_watt = Json.charging_watt;
                            battery.discharging_watt = Json.discharging_watt;
                            battery.SOC = Json.SOC;
                            battery.Cycle = Json.Cycle;
                            battery.charge_direction = Json.charge_direction;
                            battery.temperature = Json.temperature;
                            //清空資料
                            battery.cells_index = null;
                            battery.cells_voltage = null; 
                            foreach (var ce in Json.Cells)
                            {
                                battery.cells_index += ce.index.ToString() + "+";
                                battery.cells_voltage += ce.voltage.ToString() + "+";
                            }
                            battery.OV_DIS = Json.AlarmState.OV_DIS;
                            battery.UV_DIS = Json.AlarmState.UV_DIS;
                            battery.OC_DIS = Json.AlarmState.OC_DIS;
                            battery.SC_DIS = Json.AlarmState.SC_DIS;
                            battery.OT_DIS = Json.AlarmState.OT_DIS;
                            battery.UT_DIS = Json.AlarmState.UT_DIS;
                            battery.RV_DIS = Json.AlarmState.RV_DIS;
                            battery.OC0_DIS = Json.AlarmState.OC0_DIS;

                            batteryID = BatteryService.Create(battery);

                            batteryIDs += batteryID.ToString() + "+";
                        }                   
                    }
                    else
                    {
                      //  batteryID = Guid.NewGuid();
                    }


                    #endregion

                    ESSObject eSS = new ESSObject()
                    {
                        UpdateDate = JsonObject.updateTime,
                        stationUUID = JsonObject.stationUUID,
                        stationName = JsonObject.stationName,
                        GridPowerIDs = gridPowerIDs,
                        LoadPowerIDs = loadPowerIDs,
                        GeneratorIDs = generatorIDs,
                        InvertersIDs = inverterIDs,
                        BatteryIDs = batteryIDs,
                        CreateTime = DateTime.Now
                    };
                    ESSId = ESSObjecterService.Create(eSS);
                    logger.Info(ESSId);
                }
                catch (Exception ex)
                {
                    logger.Fatal("SQL ERROE START");
                    logger.Fatal(json);
                    logger.Fatal(ex);
                    logger.Fatal("SQL ERROE END");
                }
            });
            #endregion         
        }
        #endregion

        #region alart
        [HttpPost]
        public ActionResult Alart()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// Json示範碼
        /// </summary>
        #region JsonString
        private readonly string JsonString2 =
            @"{
                      'updateTime': '2018-10-17T14:46:06.851Z',
                      'stationName': '霧台鄉',
                      'stationUUID': 'd4788824-ba3e-11e8-96f8-529269fb1459',
                      'GridPowers': [
                        {
                          'version': 1,
                          'index': 0,
                          'modelSerial': '0000111122223333',
                          'serialNO': 'SCB-MC01',
                          'name': '市電',
                          'connected': true,
                          'date_time': '2018-10-17T14:45:53.668Z',
                          'va': 0,
                          'vb': 0,
                          'vc': 0,
                          'vavg': 0,
                          'ia': 0,
                          'ib': 0,
                          'ic': 0,
                          'in': 0,
                          'isum': 0,
                          'watt_a': 0,
                          'watt_b': 0,
                          'watt_c': 0,
                          'watt_t': 0,
                          'var_a': 0,
                          'var_b': 0,
                          'var_c': 0,
                          'var_t': 0,
                          'va_a': 0,
                          'va_b': 0,
                          'va_c': 0,
                          'va_t': 0,
                          'pf_a': 0,
                          'pf_b': 0,
                          'pf_c': 0,
                          'pf_t': 0,
                          'angle_va': 0,
                          'angle_vb': 96.4,
                          'angle_vc': 186,
                          'angle_ia': 55.5,
                          'angle_ib': 262.6,
                          'angle_ic': 271.3,
                          'frequency': 0,
                          'vab': 0,
                          'vbc': 0,
                          'vca': 0,
                          'vii_avg': 0,
                          'kwht': 0.1,
                          'kwha': 0,
                          'kwhb': 0,
                          'kwhc': 0,
                          'kvarht': 0,
                          'kvarha': 0,
                          'kvarhb': 0,
                          'kvarhc': 0,
                          'kvaht': 0.1,
                          'kvaha': 0,
                          'kvahb': 0,
                          'kvahc': 0,
                          'demand': 0,
                          'prev_demand': 0,
                          'prev_demand2': 0,
                          'prev_demand3': 0,
                          'maxdemand_currnetmonth': 0,
                          'maxdemand_lastmonth': 0,
                          'remain_time': 7,
                          'events': []
                        }
                      ],
                      'LoadPowers': [],
                      'Generators': [],
                      'Inverters': [],
                      'Battery': [
                        {
                          'version': 1,
                          'index': 0,
                          'modelSerial': 'FSP-BS4866',
                          'serialNO': '0000111122223333',
                          'name': 'FSP-BS4866',
                          'connected': false,
                          'voltage': 0,
                          'charging_current': 0,
                          'discharging_current': 0,
                          'charging_watt': 0,
                          'discharging_watt': 0,
                          'SOC': 0,
                          'Cycle': 1,
                          'charge_direction': 0,
                          'temperature': 0,
                          'Cells': [
                            {
                              'index': 1,
                              'voltage': 0
                            },
                            {
                              'index': 2,
                              'voltage': 0
                            },
                            {
                              'index': 3,
                              'voltage': 0
                            },
                            {
                              'index': 4,
                              'voltage': 0
                            },
                            {
                              'index': 5,
                              'voltage': 0
                            },
                            {
                              'index': 6,
                              'voltage': 0
                            },
                            {
                              'index': 7,
                              'voltage': 0
                            },
                            {
                              'index': 8,
                              'voltage': 0
                            },
                            {
                              'index': 9,
                              'voltage': 0
                            },
                            {
                              'index': 10,
                              'voltage': 0
                            },
                            {
                              'index': 11,
                              'voltage': 0
                            },
                            {
                              'index': 12,
                              'voltage': 0
                            },
                            {
                              'index': 13,
                              'voltage': 0
                            },
                            {
                              'index': 14,
                              'voltage': 0
                            }
                          ],
                          'AlarmState': {
                            'OV_DIS': false,
                            'UV_DIS': false,
                            'OC_DIS': false,
                            'SC_DIS': false,
                            'OT_DIS': false,
                            'UT_DIS': false,
                            'RV_DIS': false,
                            'OC0_DIS': false
                          }
                        },
                        {
                          'version': 1,
                          'index': 1,
                          'modelSerial': 'FSP-BS4866',
                          'serialNO': '0000111122223333',
                          'name': 'FSP-BS4866',
                          'connected': false,
                          'voltage': 0,
                          'charging_current': 0,
                          'discharging_current': 0,
                          'charging_watt': 0,
                          'discharging_watt': 0,
                          'SOC': 0,
                          'Cycle': 1,
                          'charge_direction': 0,
                          'temperature': 0,
                          'Cells': [
                            {
                              'index': 1,
                              'voltage': 0
                            },
                            {
                              'index': 2,
                              'voltage': 0
                            },
                            {
                              'index': 3,
                              'voltage': 0
                            },
                            {
                              'index': 4,
                              'voltage': 0
                            },
                            {
                              'index': 5,
                              'voltage': 0
                            },
                            {
                              'index': 6,
                              'voltage': 0
                            },
                            {
                              'index': 7,
                              'voltage': 0
                            },
                            {
                              'index': 8,
                              'voltage': 0
                            },
                            {
                              'index': 9,
                              'voltage': 0
                            },
                            {
                              'index': 10,
                              'voltage': 0
                            },
                            {
                              'index': 11,
                              'voltage': 0
                            },
                            {
                              'index': 12,
                              'voltage': 0
                            },
                            {
                              'index': 13,
                              'voltage': 0
                            },
                            {
                              'index': 14,
                              'voltage': 0
                            }
                          ],
                          'AlarmState': {
                            'OV_DIS': false,
                            'UV_DIS': false,
                            'OC_DIS': false,
                            'SC_DIS': false,
                            'OT_DIS': false,
                            'UT_DIS': false,
                            'RV_DIS': false,
                            'OC0_DIS': false
                          }
                        },
                        {
                          'version': 1,
                          'index': 2,
                          'modelSerial': 'FSP-BS4866',
                          'serialNO': '0000111122223333',
                          'name': 'FSP-BS4866',
                          'connected': false,
                          'voltage': 0,
                          'charging_current': 0,
                          'discharging_current': 0,
                          'charging_watt': 0,
                          'discharging_watt': 0,
                          'SOC': 0,
                          'Cycle': 1,
                          'charge_direction': 0,
                          'temperature': 0,
                          'Cells': [
                            {
                              'index': 1,
                              'voltage': 0
                            },
                            {
                              'index': 2,
                              'voltage': 0
                            },
                            {
                              'index': 3,
                              'voltage': 0
                            },
                            {
                              'index': 4,
                              'voltage': 0
                            },
                            {
                              'index': 5,
                              'voltage': 0
                            },
                            {
                              'index': 6,
                              'voltage': 0
                            },
                            {
                              'index': 7,
                              'voltage': 0
                            },
                            {
                              'index': 8,
                              'voltage': 0
                            },
                            {
                              'index': 9,
                              'voltage': 0
                            },
                            {
                              'index': 10,
                              'voltage': 0
                            },
                            {
                              'index': 11,
                              'voltage': 0
                            },
                            {
                              'index': 12,
                              'voltage': 0
                            },
                            {
                              'index': 13,
                              'voltage': 0
                            },
                            {
                              'index': 14,
                              'voltage': 0
                            }
                          ],
                          'AlarmState': {
                            'OV_DIS': false,
                            'UV_DIS': false,
                            'OC_DIS': false,
                            'SC_DIS': false,
                            'OT_DIS': false,
                            'UT_DIS': false,
                            'RV_DIS': false,
                            'OC0_DIS': false
                          }
                        },
                        {
                          'version': 1,
                          'index': 3,
                          'modelSerial': 'FSP-BS4866',
                          'serialNO': '0000111122223333',
                          'name': 'FSP-BS4866',
                          'connected': false,
                          'voltage': 0,
                          'charging_current': 0,
                          'discharging_current': 0,
                          'charging_watt': 0,
                          'discharging_watt': 0,
                          'SOC': 0,
                          'Cycle': 1,
                          'charge_direction': 0,
                          'temperature': 0,
                          'Cells': [
                            {
                              'index': 1,
                              'voltage': 0
                            },
                            {
                              'index': 2,
                              'voltage': 0
                            },
                            {
                              'index': 3,
                              'voltage': 0
                            },
                            {
                              'index': 4,
                              'voltage': 0
                            },
                            {
                              'index': 5,
                              'voltage': 0
                            },
                            {
                              'index': 6,
                              'voltage': 0
                            },
                            {
                              'index': 7,
                              'voltage': 0
                            },
                            {
                              'index': 8,
                              'voltage': 0
                            },
                            {
                              'index': 9,
                              'voltage': 0
                            },
                            {
                              'index': 10,
                              'voltage': 0
                            },
                            {
                              'index': 11,
                              'voltage': 0
                            },
                            {
                              'index': 12,
                              'voltage': 0
                            },
                            {
                              'index': 13,
                              'voltage': 0
                            },
                            {
                              'index': 14,
                              'voltage': 0
                            }
                          ],
                          'AlarmState': {
                            'OV_DIS': false,
                            'UV_DIS': false,
                            'OC_DIS': false,
                            'SC_DIS': false,
                            'OT_DIS': false,
                            'UT_DIS': false,
                            'RV_DIS': false,
                            'OC0_DIS': false
                          }
                        }
                      ]
                    }";




        private readonly string JsonString =
             @"
                {
                  'updateTime': '2018-09-17T10:53:53.909Z',
                  'stationUUID': '屏東站1',
                  'stationName': 'd4788824-ba3e-11e8-96f8-529269fb1459',
                  'GridPowers': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'SCB-MC02',
                      'serialNO': '0000123456789101',
                      'name': 'SIMSmartCB',
                      'connected': true,
                      'updateTime': '2018-09-17T10:53:53.909Z',
                      'date_time': '2018-09-17T10:53:53.909Z',
                      'va': 0,
                      'vb': 1,
                      'vc': 2,
                      'vavg': 3,
                      'ia': 4,
                      'ib': 5,
                      'ic': 6,
                      'in': 7,
                      'isum': 8,
                      'watt_a': 9,
                      'watt_b': 10,
                      'watt_c': 11,
                      'watt_t': 12,
                      'var_a': 13,
                      'var_b': 14,
                      'var_c': 15,
                      'var_t': 16,
                      'va_a': 17,
                      'va_b': 18,
                      'va_c': 19,
                      'va_t': 20,
                      'pf_a': 21,
                      'pf_b': 22,
                      'pf_c': 23,
                      'pf_t': 24,
                      'angle_va': 25,
                      'angle_vb': 26,
                      'angle_vc': 27,
                      'angle_ia': 28,
                      'angle_ib': 29,
                      'angle_ic': 30,
                      'frequency': 31,
                      'vab': 32,
                      'vbc': 33,
                      'vca': 34,
                      'vii_avg': 35,
                      'kwht': 36,
                      'kwha': 37,
                      'kwhb': 38,
                      'kwhc': 39,
                      'kvarht': 40,
                      'kvarha': 41,
                      'kvarhb': 42,
                      'kvarhc': 43,
                      'kvaht': 44,
                      'kvaha': 45,
                      'kvahb': 46,
                      'kvahc': 47,
                      'demand': 48,
                      'prev_demand': 49,
                      'prev_demand2': 50,
                      'prev_demand3': 51,
                      'maxdemand_currnetmonth': 52,
                      'maxdemand_lastmonth': 53,
                      'remain_time': 54,
                      'events': [{
                        'IsCurrent': true,
                        'ErrorMessage': 'TEST MESSAGE FOR EVENTS',
                        'event_info': 0,
                        'event_date_time': '2018-09-17T10:53:53.909Z',
                        'info': {
                            'Alarm': 0,
                            'ELeve': 0,
                            'EType': 0,
                            'ELoop': 0,
                        }
                      }]
                    }
                  ],
                  'LoadPowers': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'SCB-MC02',
                      'serialNO': '0000123456789101',
                      'name': 'SIMSmartCB',
                      'connected': true,
                      'updateTime': '2018-09-17T10:53:53.909Z',
                      'date_time': '2018-09-17T10:53:53.909Z',
                      'va': 0,
                      'vb': 1,
                      'vc': 2,
                      'vavg': 3,
                      'ia': 4,
                      'ib': 5,
                      'ic': 6,
                      'in': 7,
                      'isum': 8,
                      'watt_a': 9,
                      'watt_b': 10,
                      'watt_c': 11,
                      'watt_t': 12,
                      'var_a': 13,
                      'var_b': 14,
                      'var_c': 15,
                      'var_t': 16,
                      'va_a': 17,
                      'va_b': 18,
                      'va_c': 19,
                      'va_t': 20,
                      'pf_a': 21,
                      'pf_b': 22,
                      'pf_c': 23,
                      'pf_t': 24,
                      'angle_va': 25,
                      'angle_vb': 26,
                      'angle_vc': 27,
                      'angle_ia': 28,
                      'angle_ib': 29,
                      'angle_ic': 30,
                      'frequency': 31,
                      'vab': 32,
                      'vbc': 33,
                      'vca': 34,
                      'vii_avg': 35,
                      'kwht': 36,
                      'kwha': 37,
                      'kwhb': 38,
                      'kwhc': 39,
                      'kvarht': 40,
                      'kvarha': 41,
                      'kvarhb': 42,
                      'kvarhc': 43,
                      'kvaht': 44,
                      'kvaha': 45,
                      'kvahb': 46,
                      'kvahc': 47,
                      'demand': 48,
                      'prev_demand': 49,
                      'prev_demand2': 50,
                      'prev_demand3': 51,
                      'maxdemand_currnetmonth': 52,
                      'maxdemand_lastmonth': 53,
                      'remain_time': 54,
                      'events': [{
                        'IsCurrent': true,
                        'ErrorMessage': 'TEST MESSAGE FOR EVENTS',
                        'event_info': 0,
                        'event_date_time': '2018-09-17T10:53:53.909Z',
                        'info': {
                            'Alarm': 0,
                            'ELeve': 0,
                            'EType': 0,
                            'ELoop': 0,
                        }
                      }]
                    }
                  ],
                  'Generators': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'DSE100',
                      'serialNO': '000011112222333',
                      'name': 'DSE100',
                      'connected': true,
                      'updateTime': '2018-09-17T10:53:53.910Z',
                      'OilPressure': 0,
                      'CoolantTemperature': 1,
                      'OilTemperature': 2,
                      'FuleLevel': 3,
                      'InternalFlexibleSenderAnalogueInputType': 4,
                      'ChargeAlternatorVoltage': 5,
                      'EngineBatteryVoltage': 6,
                      'EngineSpeed': 7,
                      'EngineRunTime': 8,
                      'NumberOfStarts': 9,
                      'frequency': 10,
                      'L1Nvoltage': 11,
                      'L2Nvoltage': 12,
                      'L3Nvoltage': 13,
                      'L1L2voltage': 14,
                      'L2L3voltage': 15,
                      'L3L1voltage': 16,
                      'L1current': 17,
                      'L2current': 18,
                      'L3current': 19,
                      'earthcurrent': 20,
                      'L1watts': 21,
                      'L2watts': 22,
                      'L3watts': 23,
                      'currentlaglead': 24,
                      'totalwatts': 25,
                      'L1VA': 26,
                      'L2VA': 27,
                      'L3VA': 28,
                      'totalVA': 29,
                      'L1Var': 30,
                      'L2Var': 31,
                      'L3Var': 32,
                      'totalVar': 33,
                      'powerfactorL1': 34,
                      'powerfactorL2': 35,
                      'powerfactorL3': 36,
                      'averagepowerfactor': 37,
                      'percentageoffullpower': 38,
                      'percentageoffullVar': 39,
                      'positiveKWhours': 40,
                      'negativeKWhours': 41,
                      'KVAhours': 42,
                      'KVArhours': 43,
                      'ControlStatus': true,
                      'Alarm': {
                        'NumberOfNamedAlarms': 44,
                        'EmergencyStop': 3,
                        'LowOilPressure': 3,
                        'HighCoolantTemperature': 3,
                        'LowCoolantTemperature': 3,
                        'UnderSpeed': 3,
                        'OverSpeed': 3,
                        'GeneratorUnderFrequency': 3,
                        'GeneratorOverFrequency': 3,
                        'GeneratorLowVoltage': 3,
                        'GeneratorHighVoltage': 3,
                        'BatteryLowVoltage': 3,
                        'BatteryHighVoltage': 3,
                        'ChargeAlternatorFailure': 3,
                        'FailToStart': 3,
                        'FailToStop': 3,
                        'GeneratorFailToClose': 3,
                        'MainsFailToClose': 3,
                        'OilPressureSenderFault': 3,
                        'LossOfMagneticPickUp': 3,
                        'MagneticPickUpOpenCircuit': 3,
                        'GeneratorHighCurrent': 3,
                        'NoneA': 3,
                        'LowFuelLevel': 3,
                        'CANECUWarning': 3,
                        'CANECUShutdown': 3,
                        'CANECUDataFail': 3,
                        'LowOillevelSwitch': 3,
                        'HighTemperatureSwitch': 3,
                        'LowFuelLevelSwitch': 3,
                        'ExpansionUnitWatchdogAlarm': 3,
                        'kWOverloadAlarm': 3,
                        'NegativePhaseSequenceCurrentAlarm': 3,
                        'EarthFaultTripAlarm': 3,
                        'GeneratorPhaseRotationAlarm': 3,
                        'AutoVoltageSenseFail': 3,
                        'MaintenanceAlarm': 3,
                        'LoadingFrequencyAlarm': 3,
                        'LoadingVoltageAlarm': 3,
                        'NoneB': 3,
                        'NoneC': 3,
                        'NoneD': 3,
                        'NoneE': 3,
                        'GeneratorShortCircuit': 3,
                        'MainsHighCurrent': 3,
                        'MainsEarthFault': 3,
                        'MainsShortCircuit': 3,
                        'ECUProtect': 3,
                        'NoneF': 3,
                        'Message': 'TEST FOR SIM'
                      },
                      'AvailabilityEnergy': 45,
                      'AvailabilityHour': 46,
                        'FuelRelay':true,
                        'StartRelay':true,
                        'DigitalOutC':true,
                        'DigitalOutD':true,
                        'DigitalOutE':true,
                        'DigitalOutF':true,
                        'DigitalOutG':true,
                        'DigitalOutH':true,
                        'STOPLEDstatus':true,
                        'MANUALLEDstatus':true,
                        'TESTLEDstatus':true,
                        'AUTOLEDstatus':true,
                        'GENLEDstatus':true,
                        'GENBREAKERLEDstatus':true,
                        'MAINSLEDstatus':true,
                        'MAINSBREAKERLEDstatus':true,
                        'USERLED1status':true,
                        'USERLED2statu':true,
                        'USERLED3status':true,
                        'USERLED4status':true
                    }
                  ],
                  'Inverters': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'OPTI5K',
                      'serialNO': '1111222233334444',
                      'name': 'OPTI5K',
                      'connected': true,
                      'updateTime': '2018-09-17T10:53:53.911Z',
                      'DeviceMode': 'P',
                      'WarningStatus': {
                        'InverterFault': true,
                        'BusOver': true,
                        'BusUnder': true,
                        'BusSoftFail': true,
                        'LINE_FAIL': true,
                        'OPVShort': true,
                        'InverterVoltageTooLow': true,
                        'InverterVoltageTooHigh': true,
                        'OverTemperature': true,
                        'FanLocked': true,
                        'BatteryVoltageHigh': true,
                        'BatteryLowAlarm': true,
                        'BatteryUnderShutdown': true,
                        'OverLoad': true,
                        'EepromFault': true,
                        'InverterOverCurrent': true,
                        'InverterSoftFail': true,
                        'SelfTestFail': true,
                        'OP_DC_VoltageOver': true,
                        'BatOpen': true,
                        'CurrentSensorFail': true,
                        'BatteryShort': true,
                        'PowerLimit': true,
                        'PV_VoltageHigh': true,
                        'MPPT_OverloadFault': true,
                        'MPPT_OverloadWarning': true,
                        'BatteryTooLowToCharge': true,
                        'Message': 'TEST FOR ALL EVENTS'
                      },
                      'ParallelInformation': [
                        {
                          'IsExist': true,
                          'SerialNumber': '001',
                          'WorkMode': 'P',
                          'FaultCode': '00',
                          'GridVoltage': 0,
                          'GridFrequency': 1,
                          'ACOutputVoltage': 2,
                          'ACOutputFrequency': 3,
                          'ACOutputApparentPower': 4,
                          'ACOutputActivePower': 5,
                          'LoadPercentage': 6,
                          'BatteryVoltage': 7,
                          'BatteryChargingCurrent': 8,
                          'BatteryCapacity': 9,
                          'PV_InputVoltage': 10,
                          'TotalChargingCurrent': 11,
                          'Total_AC_OutputApparentPower': 12,
                          'TotalOutputActivePower': 13,
                          'Total_AC_OutputPercentage': 14,
                          'InverterStatus': {
                            'SCC_OK': true,
                            'AC_Charging': true,
                            'SCC_Charging': true,
                            'Battery': '02',
                            'Line_OK': true,
                            'loadOn': true,
                            'ConfigurationChange': true
                          },
                          'OutputMode': '4',
                          'ChargerSourcePriority': '3',
                          'MaxChargerCurrent': 15,
                          'MaxChargerRange': 16,
                          'Max_AC_ChargerCurrent': 17,
                          'PV_InputCurrentForBattery': 18,
                          'BatteryDischargeCurrent': 19
                        }
                      ],
                      'GridVoltage': 20,
                      'GridFrequency': 21,
                      'AC_OutputVoltage': 22,
                      'AC_OutputFrequency': 23,
                      'AC_OutputApparentPower': 24,
                      'AC_OutputActivePower': 25,
                      'OutputLoadPercent': 26,
                      'BUSVoltage': 27,
                      'BatteryVoltage': 28,
                      'BatteryChargingCurrent': 29,
                      'BatteryCapacity': 30,
                      'InverterHeatSinkTemperature': 31,
                      'PV_InputCurrentForBattery': 32,
                      'PV_InputVoltage': 33,
                      'BatteryVoltageFrom_SCC': 34,
                      'BatteryDischargeCurrent': 35,
                      'DeviceStatus': {
                        'Has_SBU_PriorityVersion': true,
                        'ConfigurationStatus_Change': true,
                        'SCC_FirmwareVersion_Updated': true,
                        'LoadStatus_On': true,
                        'BatteryVoltageTOSteadyWhileCharging': true,
                        'ChargingStatus_On': true,
                        'ChargingSstatus_SCC_Charging_On': true,
                        'ChargingStatus_AC_Charging_On': true,
                        'ChargingStatusCharging': '000',
                        'ChargingToFloatingMode': true,
                        'SwitchOn': true
                      },
                      'BatteryVoltageOffsetForFansOn': 36,
                      'EEPROM_Version': 37,
                      'PV_ChargingPower': 38
                    }
                  ],
                  'Battery': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'FSP10KW',
                      'serialNO': '1111222233334444',
                      'name': 'FSP10KW',
                      'connected': true,
                      'updateTime': '2018-09-17T10:53:53.911Z',
                      'voltage': 0,
                      'charging_current': 1,
                      'discharging_current': 2,
                      'charging_watt': 3,
                      'discharging_watt': 4,
                      'SOC': 5,
                      'Cycle': 6,
                      'charge_direction': 0,
                      'temperature': 7,
                      'Cells': [
                        {
                          'index': 8,
                          'voltage': 9
                        }
                      ],
                      'AlarmState': {
                        'OV_DIS': true,
                        'UV_DIS': true,
                        'OC_DIS': true,
                        'SC_DIS': true,
                        'OT_DIS': true,
                        'UT_DIS': true,
                        'RV_DIS': true,
                        'OC0_DIS': true
                      }
                    }
                  ]
                }";
        #endregion
    }
}