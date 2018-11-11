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
                    JsonObject = (schemaJson)JsonConvert.DeserializeObject(JsonString, typeof(schemaJson));
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
                logger.Info(json);
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
        ///  
        /// </summary>
        /// <param name="JsonObject"></param>
        /// <param name="json"></param>
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
                            
                            gridPowerIDs += gridPowerID.ToString() + "|";
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
                                loadPower.IsCurrent += ev.IsCurrent + "|";
                                loadPower.ErrorMessage += ev.ErrorMessage + "|";
                                loadPower.event_info += ev.event_info + "|";
                                loadPower.event_date_time += ev.event_date_time + "|";
                                loadPower.Alarm += ev.info.Alarm + "|";
                                loadPower.ELeve += ev.info.ELeve + "|";
                                loadPower.EType += ev.info.EType + "|";
                                loadPower.ELoop += ev.info.ELoop + "|";
                            }
                            //新增資料
                            loadPowerID = LoadPowerService.Create(loadPower);
                            loadPowerIDs += loadPowerID.ToString() + "|";
                        }
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
                            inverter.DeviceMode = Json.DeviceMode;
                            //WarningStatus
                            inverter.InverterFault = Json.WarningStatus.InverterFault;
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
                                inverter.ParallelInformation_IsExist += p.IsExist + "|";
                                inverter.ParallelInformation_SerialNumber += p.SerialNumber + "|";
                                inverter.ParallelInformation_WorkMode += p.WorkMode + "|";
                                inverter.ParallelInformation_FaultCode+= p.FaultCode + "|";
                                inverter.ParallelInformation_GridVoltage += p.GridVoltage + "|";
                                inverter.ParallelInformation_GridFrequency += p.GridFrequency + "|";
                                inverter.ParallelInformation_ACOutputVoltage += p.ACOutputVoltage + "|";
                                inverter.ParallelInformation_ACOutputFrequency += p.ACOutputFrequency + "|";
                                inverter.ParallelInformation_ACOutputApparentPower += p.ACOutputApparentPower + "|";
                                inverter.ParallelInformation_ACOutputActivePower += p.ACOutputActivePower + "|";
                                inverter.ParallelInformation_LoadPercentage += p.LoadPercentage + "|";
                                inverter.ParallelInformation_BatteryVoltage += p.BatteryVoltage + "|";
                                inverter.ParallelInformation_BatteryChargingCurrent += p.BatteryChargingCurrent + "|";
                                inverter.ParallelInformation_BatteryCapacity += p.BatteryCapacity + "|";
                                inverter.ParallelInformation_PV_InputVoltage += p.PV_InputVoltage + "|";
                                inverter.ParallelInformation_TotalChargingCurrent += p.TotalChargingCurrent + "|";
                                inverter.ParallelInformation_Total_AC_OutputApparentPower += p.Total_AC_OutputApparentPower + "|";
                                inverter.ParallelInformation_TotalOutputActivePower += p.TotalOutputActivePower + "|";
                                inverter.ParallelInformation_Total_AC_OutputPercentage += p.Total_AC_OutputPercentage + "|";
                                //InverterStatus start
                                inverter.SCC_OK += p.InverterStatus.SCC_OK + "|";
                                inverter.AC_Charging += p.InverterStatus.AC_Charging + "|";
                                inverter.SCC_Charging += p.InverterStatus.SCC_Charging + "|";
                                inverter.Battery += p.InverterStatus.Battery + "|";
                                inverter.Line_OK += p.InverterStatus.Line_OK + "|";
                                inverter.loadOn += p.InverterStatus.loadOn + "|";
                                //end
                                inverter.ConfigurationChange += p.InverterStatus.ConfigurationChange + "|";
                                inverter.ParallelInformation_OutputMode += p.OutputMode + "|";
                                inverter.ParallelInformation_ChargerSourcePriority += p.ChargerSourcePriority + "|";
                                inverter.ParallelInformation_MaxChargerCurrent += p.MaxChargerCurrent + "|";
                                inverter.ParallelInformation_MaxChargerRange += p.MaxChargerRange + "|";
                                inverter.ParallelInformation_Max_AC_ChargerCurrent += p.Max_AC_ChargerCurrent + "|";
                                inverter.ParallelInformation_PV_InputCurrentForBattery += p.PV_InputCurrentForBattery + "|";
                                inverter.ParallelInformation_BatteryDischargeCurrent += p.BatteryDischargeCurrent + "|";
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
                            //Solar 
                            inverter.SPM90Voltage += Json.SPM90Voltage + "|";
                            inverter.SPM90Current += Json.SPM90Current + "|";
                            inverter.SPM90ActivePower += Json.SPM90ActivePower + "|";
                            inverter.SPM90ActiveEnergy += Json.SPM90ActiveEnergy + "|";
                            inverter.SPM90VoltageDirection += Json.SPM90VoltageDirection + "|";
                            //時間
                            inverter.CreateTime=DateTime.UtcNow;
                            //新增資料
                            inverterID = InverterService.Create(inverter);
                            inverterIDs += inverterID.ToString() + "|";
                        }
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
                                battery.cells_index += ce.index.ToString() + "|";
                                battery.cells_voltage += ce.voltage.ToString() + "|";
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
                            batteryIDs += batteryID.ToString() + "|";
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
        private readonly string JsonString =@"{
                  'updateTime': '2018-11-08T02:43:37.205Z',
                  'stationName': '大武社區',
                  'stationUUID': 'd4788824-ba3e-11e8-96f8-529269fb1459',
                  'GridPowers': [],
                  'LoadPowers': [
                    {
                      'version': 1,
                      'index': 2,
                      'modelSerial': '0000111122223333',
                      'serialNO': 'SCB-SC01',
                      'name': '負載迴路1',
                      'connected': true,
                      'date_time': '2018-11-08T02:43:30.288Z',
                      'va': 220.1,
                      'vb': 0,
                      'vc': 0,
                      'vavg': 220.1,
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
                      'angle_vb': 247.8,
                      'angle_vc': 180.4,
                      'angle_ia': 47.5,
                      'angle_ib': 253.5,
                      'angle_ic': 18.1,
                      'frequency': 60,
                      'vab': 220.1,
                      'vbc': 220.1,
                      'vca': 220.1,
                      'vii_avg': 0,
                      'kwht': 0.4,
                      'kwha': 0.4,
                      'kwhb': 0,
                      'kwhc': 0,
                      'kvarht': 0,
                      'kvarha': 0,
                      'kvarhb': 0,
                      'kvarhc': 0,
                      'kvaht': 0.4,
                      'kvaha': 0.4,
                      'kvahb': 0,
                      'kvahc': 0,
                      'demand': 0,
                      'prev_demand': 0,
                      'prev_demand2': 0,
                      'prev_demand3': 0,
                      'maxdemand_currnetmonth': 0,
                      'maxdemand_lastmonth': 0,
                      'remain_time': 812,
                      'events': []
                    },
                    {
                      'version': 1,
                      'index': 3,
                      'modelSerial': '0000111122223333',
                      'serialNO': 'SCB-SC01',
                      'name': '負載迴路2',
                      'connected': true,
                      'date_time': '2018-11-08T02:43:33.646Z',
                      'va': 220,
                      'vb': 0,
                      'vc': 0,
                      'vavg': 220,
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
                      'angle_vb': 200.9,
                      'angle_vc': 211.5,
                      'angle_ia': 250.3,
                      'angle_ib': 261.6,
                      'angle_ic': 181.7,
                      'frequency': 59.96,
                      'vab': 220,
                      'vbc': 220,
                      'vca': 220,
                      'vii_avg': 0,
                      'kwht': 60.4,
                      'kwha': 60.4,
                      'kwhb': 0,
                      'kwhc': 0,
                      'kvarht': 29.8,
                      'kvarha': 28.7,
                      'kvarhb': 0.5,
                      'kvarhc': 0.4,
                      'kvaht': 113.3,
                      'kvaha': 75.1,
                      'kvahb': 19.1,
                      'kvahc': 19,
                      'demand': 0,
                      'prev_demand': 0,
                      'prev_demand2': 0,
                      'prev_demand3': 0,
                      'maxdemand_currnetmonth': 0,
                      'maxdemand_lastmonth': 0,
                      'remain_time': 808,
                      'events': []
                    }
                  ],
                  'Generators': [],
                  'Inverters': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'OPTI-SP5000',
                      'serialNO': '0000111122223333',
                      'name': 'OPTI SP5000 Brilliant Ultra',
                      'connected': true,
                      'DeviceMode': 'B',
                      'WarningStatus': {
                        'InverterFault': false,
                        'BusOver': false,
                        'BusUnder': false,
                        'BusSoftFail': false,
                        'LINE_FAIL': true,
                        'OPVShort': false,
                        'InverterVoltageTooLow': false,
                        'InverterVoltageTooHigh': false,
                        'OverTemperature': false,
                        'FanLocked': false,
                        'BatteryVoltageHigh': false,
                        'BatteryLowAlarm': false,
                        'BatteryUnderShutdown': false,
                        'OverLoad': false,
                        'EepromFault': false,
                        'InverterOverCurrent': false,
                        'InverterSoftFail': false,
                        'SelfTestFail': false,
                        'OP_DC_VoltageOver': false,
                        'BatOpen': false,
                        'CurrentSensorFail': false,
                        'BatteryShort': false,
                        'PowerLimit': false,
                        'PV_VoltageHigh': false,
                        'MPPT_OverloadFault': false,
                        'MPPT_OverloadWarning': false,
                        'BatteryTooLowToCharge': false,
                        'Message': '[LINE_FAIL]'
                      },
                      'ParallelInformation': [
                        {
                          'IsExist': true,
                          'SerialNumber': '92931807102938',
                          'WorkMode': 'B',
                          'FaultCode': '00',
                          'GridVoltage': 0,
                          'GridFrequency': 0,
                          'ACOutputVoltage': 220,
                          'ACOutputFrequency': 59.99,
                          'ACOutputApparentPower': 44,
                          'ACOutputActivePower': 8,
                          'LoadPercentage': 0,
                          'BatteryVoltage': 52.4,
                          'BatteryChargingCurrent': 0,
                          'BatteryCapacity': 100,
                          'PV_InputVoltage': 54.3,
                          'TotalChargingCurrent': 0,
                          'Total_AC_OutputApparentPower': 87,
                          'TotalOutputActivePower': 41,
                          'Total_AC_OutputPercentage': 0,
                          'InverterStatus': {
                            'SCC_OK': false,
                            'AC_Charging': false,
                            'SCC_Charging': false,
                            'Battery': '00',
                            'Line_OK': false,
                            'loadOn': true,
                            'ConfigurationChange': false
                          },
                          'OutputMode': '1',
                          'ChargerSourcePriority': '3',
                          'MaxChargerCurrent': 20,
                          'MaxChargerRange': 80,
                          'Max_AC_ChargerCurrent': 20,
                          'PV_InputCurrentForBattery': 0,
                          'BatteryDischargeCurrent': 0
                        },
                        {
                          'IsExist': true,
                          'SerialNumber': '92931807102904',
                          'WorkMode': 'B',
                          'FaultCode': '00',
                          'GridVoltage': 0,
                          'GridFrequency': 0,
                          'ACOutputVoltage': 219.9,
                          'ACOutputFrequency': 59.97,
                          'ACOutputApparentPower': 43,
                          'ACOutputActivePower': 35,
                          'LoadPercentage': 0,
                          'BatteryVoltage': 52.4,
                          'BatteryChargingCurrent': 0,
                          'BatteryCapacity': 100,
                          'PV_InputVoltage': 54.4,
                          'TotalChargingCurrent': 0,
                          'Total_AC_OutputApparentPower': 87,
                          'TotalOutputActivePower': 35,
                          'Total_AC_OutputPercentage': 0,
                          'InverterStatus': {
                            'SCC_OK': false,
                            'AC_Charging': false,
                            'SCC_Charging': false,
                            'Battery': '00',
                            'Line_OK': false,
                            'loadOn': true,
                            'ConfigurationChange': false
                          },
                          'OutputMode': '1',
                          'ChargerSourcePriority': '2',
                          'MaxChargerCurrent': 20,
                          'MaxChargerRange': 80,
                          'Max_AC_ChargerCurrent': 20,
                          'PV_InputCurrentForBattery': 0,
                          'BatteryDischargeCurrent': 1
                        }
                      ],
                      'GridVoltage': 0,
                      'GridFrequency': 0,
                      'AC_OutputVoltage': 219.9,
                      'AC_OutputFrequency': 59.9,
                      'AC_OutputApparentPower': 43,
                      'AC_OutputActivePower': 4,
                      'OutputLoadPercent': 0,
                      'BUSVoltage': 367,
                      'BatteryVoltage': 52.4,
                      'BatteryChargingCurrent': 0,
                      'BatteryCapacity': 100,
                      'InverterHeatSinkTemperature': 35,
                      'PV_InputCurrentForBattery': 0,
                      'PV_InputVoltage': 0,
                      'BatteryVoltageFrom_SCC': 0,
                      'BatteryDischargeCurrent': 0,
                      'DeviceStatus': {
                        'Has_SBU_PriorityVersion': false,
                        'ConfigurationStatus_Change': false,
                        'SCC_FirmwareVersion_Updated': false,
                        'LoadStatus_On': true,
                        'BatteryVoltageTOSteadyWhileCharging': false,
                        'ChargingStatus_On': false,
                        'ChargingSstatus_SCC_Charging_On': false,
                        'ChargingStatus_AC_Charging_On': false,
                        'ChargingStatusCharging': '000',
                        'ChargingToFloatingMode': false,
                        'SwitchOn': true
                      },
                      'BatteryVoltageOffsetForFansOn': 0,
                      'EEPROM_Version': 0,
                      'PV_ChargingPower': 8,
                      'SPM90s': [
                        {
                          'id': 1,
                          'connected': true,
                          'Voltage': 0,
                          'Current': 0,
                          'ActivePower': 0,
                          'ActiveEnergy': 0.14,
                          'VoltageDirection': 0
                        },
                        {
                          'id': 4,
                          'connected': true,
                          'Voltage': 0,
                          'Current': 0,
                          'ActivePower': 0,
                          'ActiveEnergy': 0,
                          'VoltageDirection': 0
                        }
                      ],
                      'SPM90Voltage': 0,
                      'SPM90Current': 0,
                      'SPM90ActivePower': 0,
                      'SPM90ActiveEnergy': 0.14,
                      'SPM90VoltageDirection': 0
                    }
                  ],
                  'Battery': [
                    {
                      'version': 1,
                      'index': 0,
                      'modelSerial': 'FSP-BS4866',
                      'serialNO': '0000000000010000',
                      'name': 'FSP-BS4866',
                      'connected': true,
                      'updateTime': '2018-11-08T02:43:27.791Z',
                      'voltage': 52,
                      'charging_current': 0,
                      'discharging_current': 0,
                      'charging_watt': 0,
                      'discharging_watt': 0,
                      'SOC': 15,
                      'Cycle': 1,
                      'charge_direction': 0,
                      'temperature': 28,
                      'Cells': [
                        {
                          'index': 1,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 2,
                          'voltage': 3.2885
                        },
                        {
                          'index': 3,
                          'voltage': 3.2885
                        },
                        {
                          'index': 4,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 5,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 6,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 7,
                          'voltage': 3.2885
                        },
                        {
                          'index': 8,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 9,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 10,
                          'voltage': 3.2885
                        },
                        {
                          'index': 11,
                          'voltage': 3.2885
                        },
                        {
                          'index': 12,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 13,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 14,
                          'voltage': 3.2861000000000002
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
                      'serialNO': '0000000000020000',
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
                      'serialNO': '0000000000030000',
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
                      'serialNO': '0000000000040000',
                      'name': 'FSP-BS4866',
                      'connected': true,
                      'updateTime': '2018-11-08T02:43:14.457Z',
                      'voltage': 52,
                      'charging_current': 0,
                      'discharging_current': 0.9390000000000001,
                      'charging_watt': 0,
                      'discharging_watt': 48.828,
                      'SOC': 10,
                      'Cycle': 1,
                      'charge_direction': 2,
                      'temperature': 30,
                      'Cells': [
                        {
                          'index': 1,
                          'voltage': 3.2885
                        },
                        {
                          'index': 2,
                          'voltage': 3.2885
                        },
                        {
                          'index': 3,
                          'voltage': 3.291
                        },
                        {
                          'index': 4,
                          'voltage': 3.2885
                        },
                        {
                          'index': 5,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 6,
                          'voltage': 3.2836000000000003
                        },
                        {
                          'index': 7,
                          'voltage': 3.2885
                        },
                        {
                          'index': 8,
                          'voltage': 3.2885
                        },
                        {
                          'index': 9,
                          'voltage': 3.2885
                        },
                        {
                          'index': 10,
                          'voltage': 3.2885
                        },
                        {
                          'index': 11,
                          'voltage': 3.291
                        },
                        {
                          'index': 12,
                          'voltage': 3.2885
                        },
                        {
                          'index': 13,
                          'voltage': 3.2861000000000002
                        },
                        {
                          'index': 14,
                          'voltage': 3.2836000000000003
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


        #endregion
    }
}