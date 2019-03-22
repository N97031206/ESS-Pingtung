using Newtonsoft.Json;
using NLog;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
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
        private IndexDataService IndexDataService = new IndexDataService();
        private InfoChartsService InfoChartsService = new InfoChartsService();
        private InfoDatasService InfoDatasService = new InfoDatasService();
        public schemaJson JsonObject = new schemaJson();
        private static Logger logger = LogManager.GetCurrentClassLogger();//Log檔

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
                    SaveSQL(JsonObject, "");
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
                if (JsonObject.stationUUID != null)
                {
                    SaveSQL(JsonObject, json);               
                }        
                
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
        private void SaveSQL(schemaJson JsonObject, string json)
        {
            #region 多工 Task
            Task.Run(() =>
            {
                try
                {
                    TimeSpan Start = new TimeSpan(DateTime.Now.Ticks);

                    Guid UID = Guid.Parse(JsonObject.stationUUID);
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
                            gridPower.uuid = UID;
                            gridPower.modelSerial = Json.modelSerial;
                            gridPower.serialNO = Json.serialNO;
                            gridPower.name = Json.name;
                            gridPower.date_time = JsonObject.updateTime;
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
                            gridPower.MinuskWHt = GridPowerService.MinuskHWt(JsonObject.updateTime, Json.kWHt, Json.index, UID);
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
                            if (Json.events.ToList().Count >0)
                            {
                                foreach (var ev in Json.events.ToList())
                                {
                                    gridPower.ErrorMessage = ev.ErrorMessage ?? "Not Message";
                                    gridPower.event_info = ev.event_info;
                                    gridPower.event_date_time = ev.event_date_time;
                                    gridPower.Alarm = ev.info.Alarm;
                                    gridPower.ELeve = ev.info.ELeve;
                                    gridPower.EType = ev.info.EType;
                                    gridPower.ELoop = ev.info.ELoop;
                                }
                            }
                            else
                            {
                                gridPower.ErrorMessage = "Not Message";
                                gridPower.event_info = 0;
                                gridPower.event_date_time = DateTime.Now;
                                gridPower.Alarm = 0;
                                gridPower.ELeve = 0;
                                gridPower.EType = 0;
                                gridPower.ELoop = 0;
                            }

                            gridPowerID = GridPowerService.Create(gridPower);
                            gridPowerIDs += gridPowerID.ToString() + "|";
                        }
                    }


                    #endregion

                    #region loadPower
                    LoadPower loadPower = new LoadPower();
                    if (JsonObject.LoadPowers.Count > 0)
                    {
                        foreach (var Json in JsonObject.LoadPowers)
                        {
                            loadPower.version = Json.version;
                            loadPower.uuid = UID;
                            loadPower.index = Json.index;
                            loadPower.modelSerial = Json.modelSerial;
                            loadPower.serialNO = Json.serialNO;
                            loadPower.name = Json.name;
                            loadPower.connected = Json.connected;
                            loadPower.date_Time = JsonObject.updateTime;
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
                            loadPower.MinuskWHt = LoadPowerService.MinuskHWt(JsonObject.updateTime, Json.kWHt, Json.index,UID);                      
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
                            generator.uuid = UID;
                            generator.index = GenJson.index;
                            generator.modelSerial = GenJson.modelSerial;
                            generator.serialNO = GenJson.serialNO;
                            generator.name = GenJson.name;
                            generator.connected = GenJson.connected;
                            generator.UpdateTime = JsonObject.updateTime;
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
                            generatorIDs += generatorID.ToString() + "|";

                            GeneratorAlart(generatorID, UID, GenJson);
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
                            inverter.uuid = UID;
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
                                inverter.ParallelInformation_FaultCode += p.FaultCode + "|";
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
                            foreach (var s in Json.SPM90s)
                            {
                                inverter.SPMid += s.id + "|";
                                inverter.SPMconnected = s.connected + "|";
                                inverter.SPM90Voltage += s.Voltage + "|";
                                inverter.SPM90Current += s.Current + "|";
                                inverter.SPM90ActivePower += s.ActivePower + "|";
                                inverter.SPM90ActiveEnergy += s.ActiveEnergy + "|";
                                if (s.id == 1)
                                {
                                    inverter.SPM90ActiveEnergyMinus1 = InverterService.MinusEnergy1(JsonObject.updateTime, s.ActiveEnergy, Convert.ToInt32(s.id), UID);
                                }
                                else if (s.id == 4)
                                {
                                    inverter.SPM90ActiveEnergyMinus2 = InverterService.MinusEnergy4(JsonObject.updateTime, s.ActiveEnergy, Convert.ToInt32(s.id), UID);
                                }
                                inverter.SPM90VoltageDirection += s.VoltageDirection + "|";
                            }
                            //時間
                            inverter.CreateTime = DateTime.UtcNow;
                            //新增資料
                            inverterID = InverterService.Create(inverter);
                            inverterIDs += inverterID.ToString() + "|";

                            InvertersAlart(inverterID, UID, Json);
                        }
                    }
                    #endregion

                    #region battery
                    Service.ESS.Model.Battery battery = new Service.ESS.Model.Battery();

                    if (JsonObject.Battery.Count > 0)
                    {
                        foreach (var Json in JsonObject.Battery)
                        {
                            battery.uuid = UID;
                            battery.version = Json.version;
                            battery.index = Json.index;
                            battery.modelSerial = Json.modelSerial;
                            battery.serialNO = Json.serialNO;
                            battery.name = Json.name;
                            battery.connected = Json.connected;
                            battery.updateTime = JsonObject.updateTime;
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

                            BatteryAlart(batteryID, UID, Json);
                        }
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


                    UpdataIndex(JsonObject);
                    UpdataInfoData(JsonObject);

                    int Moment = JsonObject.updateTime.Minute;
                    int[] mom = { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };
                    if (mom.Contains(Moment))
                    {
                        UpdataBattery(UID, JsonObject.updateTime);
                        UpdataLoadPowers(UID, JsonObject.updateTime);
                        UpdataGenerators(UID, JsonObject.updateTime);
                        UpdataSolar(UID, JsonObject.updateTime);
                        UpdataInverters(UID, JsonObject.updateTime);
                        UpdataGridPower(UID, JsonObject.updateTime);
                    }

                    TimeSpan End = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = Start.Subtract(End).Duration();
                    logger.Info("SaveSQL Finish, takes " + ts.TotalSeconds.ToString() + " Seconde");

                    Dispose();
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

        /// <summary>
        /// 首頁資訊
        /// </summary>
        /// <param name="JsonObject"></param>
        private void UpdataIndex(schemaJson JsonObject)
        {
            try
            {
                Guid UUID = Guid.Parse(JsonObject.stationUUID);

                int cd = 0;
                double SolarActivePower = 0, GirdPower = 0, Load = 0, BatteySOC = 0, BatteyPower = 0, GeneratorPower = 0;
                double GirdPowerVoltage = 0, LoadVoltage = 0, InveratorVoltage = 0, BatteryVoltage = 0, GeneratorVoltage = 0, SolarVoltage = 0, GeneratorEngineBatteryVoltage = 0;
                bool LoadConnected = false, SPM90MConnected = false, GeneratorConnected = false, BatteryConnected = false, GridPowerConnected = false;
                string BatteyModel = "離線";

                if (JsonObject.Battery.Count > 0)
                {
                    List<int> cdData = new List<int>();
                    List<float> Voltages = new List<float>();
                    List<double> ChargingWatt = new List<double>();
                    List<double> DischargingWatt = new List<double>();
                    List<double> Voltage = new List<double>();
                    List<bool> Connection = new List<bool>();

                    foreach (var Json in JsonObject.Battery)
                    {
                        if ((int)Json.charge_direction>0) {cdData.Add((int)Json.charge_direction);}
                        if (Json.voltage > 0) Voltages.Add(Json.voltage);
                        if (Json.charging_watt > 0) ChargingWatt.Add(Json.charging_watt);
                        if (Json.discharging_watt > 0) DischargingWatt.Add(Json.discharging_watt);
                        Connection.Add(Json.connected);
                    }

                    cd = cdData.Count == 0 ? 0 : cdData.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();//找出最多次的
                    BatteyModel = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                    BatteyPower =
                        (cd == 1) ? (ChargingWatt.Count>0?Math.Round(ChargingWatt.Sum() / 1000.0, 2) :0):
                        (cd == 2)?(DischargingWatt.Count > 0 ? Math.Round(DischargingWatt.Sum() / 1000.0, 2):0):0;//所有電池放電電流總和，單位KW
                    BatteySOC = Voltages.Count>0?Math.Round(BatteryService.EachSOC(Voltages.Min()), 2):0;//有值的最小SOC
                    BatteryVoltage = Voltages.Count>0? Math.Round(Voltages.Average()*BatteyPower, 2):0;//電池電壓*電池充電電流
                    BatteryConnected = Connection.Count>0?Connection.Contains(true):false;//只要有一顆連就算連
                }

                if (JsonObject.Inverters.Count > 0)
                {
                    List<float> SPM90ActivePowers = new List<float>();
                    List<float> SPM90Voltage = new List<float>();
                    List<bool> SPM90Connection = new List<bool>();


                    foreach (var Json in JsonObject.Inverters)
                    {
                        foreach (var s in Json.SPM90s)
                        {
                            SPM90ActivePowers.Add(s.ActivePower);
                            SPM90Voltage.Add(s.Voltage);
                            SPM90Connection.Add(s.connected);
                        }
                    }
                    SolarActivePower = Math.Round(SPM90ActivePowers.Sum() / 1000.0, 2);
                    SolarVoltage = Math.Round(SPM90Voltage.Sum(), 2);
                    SPM90MConnected = SPM90Connection.Contains(true);
                }

                if (JsonObject.GridPowers.Count > 0)
                {
                    float TotalWatt = 0;
                    float AvgVoltage = 0;
                    bool Connection = false;
                    foreach (var Json in JsonObject.GridPowers)
                    {
                        if (Json.index == 0)
                        {
                            TotalWatt = Json.Watt_t;
                            AvgVoltage = Json.Vavg;
                            //Connection = Json.Vca > 200.0 ? true : false;
                            Connection = Json.Vavg > 90.0 ? true : false;
                        }
                    }
                    GirdPower = Math.Round(TotalWatt / 1000.00, 2);
                    GirdPowerVoltage = Math.Round(AvgVoltage, 2);
                    GridPowerConnected = Connection;
                }

                if (JsonObject.LoadPowers.Count > 0)
                {
                    float TotalWatt = 0;
                    float AvgVoltage = 0;
                    foreach (var Json in JsonObject.LoadPowers)
                    {
                        if (Json.index == 2)
                        {
                            TotalWatt = Json.Watt_t;
                            AvgVoltage = Json.Vavg;
                            LoadConnected = Json.connected;
                        }
                    }
                    Load = Math.Round(TotalWatt / 1000.00, 2);
                    LoadVoltage = Math.Round(AvgVoltage, 2);
                }

                if (JsonObject.Generators.Count > 0)
                {
                    float TotalVar = 0;
                    float BatteryHighVoltage = 0;
                    foreach (var GenJson in JsonObject.Generators)
                    {
                        TotalVar = GenJson.totalVar;
                        BatteryHighVoltage = GenJson.Alarm.BatteryHighVoltage;
                        GeneratorConnected = GenJson.connected;
                        GeneratorEngineBatteryVoltage = GenJson.EngineBatteryVoltage;
                    }
                    GeneratorPower = Math.Round(TotalVar, 2);
                    GeneratorVoltage = Math.Round(BatteryHighVoltage, 0);
                }

                IndexData indexData = new IndexData()
                {
                    UUID = UUID,
                    Solar = SolarActivePower,
                    GirdPower = GirdPower,
                    Load = Load,
                    BatteySOC = BatteySOC,
                    BatteyModel = BatteyModel,
                    BatteyPower = BatteyPower,
                    Direction = cd,
                    GeneratorPower = GeneratorPower,

                    GirdPowerVoltage = GirdPowerVoltage,
                    LoadVoltage = LoadVoltage,
                    InveratorVoltage = InveratorVoltage,
                    BatteryVoltage = BatteryVoltage,
                    GeneratorVoltage = GeneratorVoltage,
                    SolarVoltage = SolarVoltage,

                    LoadConnected = LoadConnected,
                    SPM90MConnected = SPM90MConnected,
                    GeneratorConnected = GeneratorConnected,
                    GridPowerConnected = GridPowerConnected,
                    BatteryConnected = BatteryConnected,

                    GeneratorEngineBatteryVoltage = GeneratorEngineBatteryVoltage
                };

                bool updataIndex = IndexDataService.Updata(indexData);
                logger.Info("Updata IndexData :" + updataIndex.ToString());
            }
            catch (Exception ex)
            {
                logger.Fatal("UpdataIndex ERROE START");
                logger.Fatal(ex);
                logger.Fatal("UpdataIndex ERROE END");
            }
        }

        private void UpdataInfoData(schemaJson JsonObject)
        {
            try
            {
                Guid UUID = Guid.Parse(JsonObject.stationUUID);

                bool updata = false;
                string GridPower = "";
                string Inverters = "";
                string Solar = "";
                string Battery = "";
                string LoadPower = "";
                string Generator = "";
                double Demand = 0;
                double RemainTime = 0;

                #region GridPower
                string gridInfo1 = "", gridInfo2 = "0", gridInfo3 = "0", gridInfo4 = "0", gridInfo5 = "0", gridInfo6 = "0", gridInfo7 = "0", gridInfo8 = "0", gridInfo9 = "0";

                if (JsonObject.GridPowers.Count > 0)
                {
                    foreach (var gp in JsonObject.GridPowers)
                    {
                        if (gp.index == 0)
                        {
                            gridInfo1 = JsonObject.updateTime.AddHours(8).ToString();
                            gridInfo2 = Math.Round(gp.Vavg, 2).ToString();
                            gridInfo3 = Math.Round(gp.Isum, 2).ToString();
                            gridInfo4 = Math.Round(gp.Watt_t / 1000.0, 2).ToString();
                            gridInfo5 = Math.Round(gp.Var_t / 1000.00, 2).ToString();
                            gridInfo6 = Math.Round(gp.VA_t / 1000.0, 2).ToString();
                            gridInfo7 = Math.Round(gp.PF_t, 2).ToString();
                            gridInfo8 = Math.Round(gp.Frequency, 2).ToString();
                            gridInfo9 = Math.Round(GridPowerService.MinuskHWt(JsonObject.updateTime, gp.kWHt, gp.index, UUID),2).ToString();
                        }
                    }
                }

                GridPower += gridInfo1 + "|";
                GridPower += gridInfo2 + "|";
                GridPower += gridInfo3 + "|";
                GridPower += gridInfo4 + "|";
                GridPower += gridInfo5 + "|";
                GridPower += gridInfo6 + "|";
                GridPower += gridInfo7 + "|";
                GridPower += gridInfo8 + "|";
                GridPower += gridInfo9;

                #endregion

                #region Inverter 
                List<Inverter> inverInfo = new List<Inverter>();

                string InvInfo1 = "", InvInfo2 = "故障模式", InvInfo3 = "0", InvInfo4 = "0", InvInfo5 = "0", InvInfo6 = "0", InvInfo7 = "0", InvInfo8 = "0", InvInfo9 = "0", InvInfo10 = "0", InvInfo11 = "0";

                if (JsonObject.Inverters.Count > 0)
                {
                    foreach (var inv in JsonObject.Inverters)
                    {
                        InvInfo1 = JsonObject.updateTime.AddHours(8).ToString();
                        string model = inv.DeviceMode.Trim();
                        if (model == "P") { model = "電源模式"; }
                        else if (model == "S") { model = "待機模式"; }
                        else if (model == "L") { model = "市電模式"; }
                        else if (model == "B") { model = "電池模式"; }
                        else if (model == "F") { model = "故障模式"; }
                        else if (model == "H") { model = "省電模式"; }
                        else { model = "其他模式"; }
                        InvInfo2 = model;
                        InvInfo3 = string.Format("{0:#,0.0}", inv.GridVoltage);
                        InvInfo4 = string.Format("{0:#,0.0}", inv.GridFrequency);
                        InvInfo5 = string.Format("{0:#,0.0}", inv.AC_OutputVoltage);
                        InvInfo6 = string.Format("{0:#,0.0}", inv.AC_OutputFrequency);
                        InvInfo7 = string.Format("{0:#,0.0}", inv.ParallelInformation.Sum(x => x.TotalOutputActivePower/1000.0));
                        InvInfo8 = string.Format("{0:#,0.0}", inv.BatteryVoltage);
                        InvInfo9 = string.Format("{0:#,0.0}", inv.BatteryCapacity);
                        InvInfo10 = string.Format("{0:#,0}", inv.PV_InputVoltage);
                        InvInfo11 = string.Format("{0:#,0}", inv.ParallelInformation.Sum(x => x.TotalChargingCurrent));

                    }
                }

                Inverters += InvInfo1 + "|";
                Inverters += InvInfo2 + "|";
                Inverters += InvInfo3 + "|";
                Inverters += InvInfo4 + "|";
                Inverters += InvInfo5 + "|";
                Inverters += InvInfo6 + "|";
                Inverters += InvInfo7 + "|";
                Inverters += InvInfo8 + "|";
                Inverters += InvInfo9 + "|";
                Inverters += InvInfo10 + "|";
                Inverters += InvInfo11;
                #endregion

                #region Solar
                string solarInfo11 = "", solarInfo21 = "";
                double solarInfo12 = 0, solarInfo13 = 0, solarInfo14 = 0, solarInfo15 = 0;
                double solarInfo22 = 0, solarInfo23 = 0, solarInfo24 = 0, solarInfo25 = 0;

                if (JsonObject.Inverters.Count > 0)
                {
                    foreach (var inv in JsonObject.Inverters)
                    {
                        foreach (var SPM in inv.SPM90s)
                        {
                            if (SPM.id == 1)
                            {
                                solarInfo11 = JsonObject.updateTime.AddHours(8).ToString();
                                solarInfo12 = Math.Round(SPM.Voltage, 2);
                                solarInfo13 = Math.Round(SPM.Current, 2);
                                solarInfo14 = Math.Round(SPM.ActivePower / 1000.0, 2);
                                solarInfo15 = Math.Round(InverterService.MinusEnergy1(JsonObject.updateTime, SPM.ActiveEnergy, Convert.ToInt32(SPM.id), UUID), 2);
                            }
                            if (SPM.id == 4)
                            {
                                solarInfo21 = JsonObject.updateTime.AddHours(8).ToString();
                                solarInfo22 = Math.Round(SPM.Voltage, 2);
                                solarInfo23 = Math.Round(SPM.Current, 2);
                                solarInfo24 = Math.Round(SPM.ActivePower / 1000.0, 2);
                                solarInfo25 = Math.Round(InverterService.MinusEnergy4(JsonObject.updateTime, SPM.ActiveEnergy, Convert.ToInt32(SPM.id), UUID), 2);
                            }
                        }
                    }
                }

                Solar += solarInfo11 + "|";
                Solar += solarInfo12 + "|";
                Solar += solarInfo13 + "|";
                Solar += solarInfo14 + "|";
                Solar += solarInfo15 ;
                Solar +="$";
                Solar += solarInfo21 + "|";
                Solar += solarInfo22 + "|";
                Solar += solarInfo23 + "|";
                Solar += solarInfo24 + "|";
                Solar += solarInfo25;
                #endregion

                #region Battery
                string BatteryInfo11 = "", BatteryInfo12 = "0", BatteryInfo13 = "0", BatteryInfo14 = "0", BatteryInfo15 = "0", BatteryInfo16 = "0", BatteryInfo17 = "離線";
                string BatteryInfo21 ="", BatteryInfo22 = "0", BatteryInfo23 = "0", BatteryInfo24 = "0", BatteryInfo25 = "0", BatteryInfo26 = "0", BatteryInfo27 = "離線";
                string BatteryInfo31 = "", BatteryInfo32 = "0", BatteryInfo33 = "0", BatteryInfo34 = "0", BatteryInfo35 = "0", BatteryInfo36 = "0", BatteryInfo37 = "離線";
                string BatteryInfo41 ="", BatteryInfo42 = "0", BatteryInfo43 = "0", BatteryInfo44 = "0", BatteryInfo45 = "0", BatteryInfo46 = "0", BatteryInfo47 = "離線";

                if (JsonObject.Battery.Count > 0)
                {
                    foreach (var ba in JsonObject.Battery)
                    {
                        if (ba.index == 0)
                        {
                            BatteryInfo11 = JsonObject.updateTime.AddHours(8).ToString();
                            BatteryInfo12 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo13 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo14 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo15 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo16 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = ba.connected == false ? 0 : Convert.ToInt32(ba.charge_direction);
                            BatteryInfo17 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 1)
                        {
                            BatteryInfo21 = JsonObject.updateTime.AddHours(8).ToString();
                            BatteryInfo22 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo23 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo24 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo25 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo26 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = ba.connected == false ? 0 : Convert.ToInt32(ba.charge_direction);
                            BatteryInfo27 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 2)
                        {
                            BatteryInfo31 = JsonObject.updateTime.AddHours(8).ToString();
                            BatteryInfo32 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo33 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo34 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo35 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo36 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = ba.connected == false ? 0 : Convert.ToInt32(ba.charge_direction);
                            BatteryInfo37 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 3)
                        {
                            BatteryInfo41 = JsonObject.updateTime.AddHours(8).ToString();
                            BatteryInfo42 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo43 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo44 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo45 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo46 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = ba.connected == false ? 0 : Convert.ToInt32(ba.charge_direction);
                            BatteryInfo47 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }
                    }
                }

                Battery += BatteryInfo11 + "|";
                Battery += BatteryInfo12 + "|";
                Battery += BatteryInfo13 + "|";
                Battery += BatteryInfo14 + "|";
                Battery += BatteryInfo15 + "|";
                Battery += BatteryInfo16 + "|";
                Battery += BatteryInfo17;
                Battery += "$";
               Battery += BatteryInfo21 + "|";
                Battery += BatteryInfo22 + "|";
                Battery += BatteryInfo23 + "|";
                Battery += BatteryInfo24 + "|";
                Battery += BatteryInfo25 + "|";
                Battery += BatteryInfo26 + "|";
                Battery += BatteryInfo27;
                Battery += "$";
                Battery += BatteryInfo31 + "|";
                Battery += BatteryInfo32 + "|";
                Battery += BatteryInfo33 + "|";
                Battery += BatteryInfo34 + "|";
                Battery += BatteryInfo35 + "|";
                Battery += BatteryInfo36 + "|";
                Battery += BatteryInfo37;
                Battery += "$";
                Battery += BatteryInfo41 + "|";
                Battery += BatteryInfo42 + "|";
                Battery += BatteryInfo43 + "|";
                Battery += BatteryInfo44 + "|";
                Battery += BatteryInfo45 + "|";
                Battery += BatteryInfo46 + "|";
                Battery += BatteryInfo47;
                #endregion

                #region Load

                string LoadInfo1 = "";
                double LoadInfo2 = 0, LoadInfo3 = 0, LoadInfo4 = 0, LoadInfo5 = 0, LoadInfo6 = 0, LoadInfo7 = 0, LoadInfo8 = 0, LoadInfo9 = 0;

                if (JsonObject.LoadPowers.Count > 0)
                {
                    foreach (var lo in JsonObject.LoadPowers)
                    {
                        if (lo.index == 2)
                        {
                            LoadInfo1 = JsonObject.updateTime.AddHours(8).ToString();
                            LoadInfo2 = Math.Round(lo.Vavg, 2);
                            LoadInfo3 = Math.Round(lo.Isum, 2);
                            LoadInfo4 = Math.Round(lo.Watt_t / 1000.0, 2);
                            LoadInfo5 = Math.Round(lo.Var_t / 1000.00, 2);
                            LoadInfo6 = Math.Round(lo.VA_t / 1000.0, 2);
                            LoadInfo7 = Math.Round(lo.PF_t, 2);
                            LoadInfo8 = Math.Round(lo.Frequency, 2);
                            LoadInfo9 = Math.Round(LoadPowerService.MinuskHWt(JsonObject.updateTime, lo.kWHt, lo.index, UUID), 2);
                        }
                    }
                }
                LoadPower += LoadInfo1 + "|";
                LoadPower += LoadInfo2 + "|";
                LoadPower += LoadInfo3 + "|";
                LoadPower += LoadInfo4 + "|";
                LoadPower += LoadInfo5 + "|";
                LoadPower += LoadInfo6 + "|";
                LoadPower += LoadInfo7 + "|";
                LoadPower += LoadInfo8 + "|";
                LoadPower += LoadInfo9;
                #endregion

                #region Generator
                string Info1 = "", Info13 = "離線", Info2 = "0", Info3 = "0", Info4 = "0", Info5 = "0", Info6 = "0", Info7 = "0", Info8 = "0", Info9 = "0", Info10 = "0", Info11 = "0", Info12 = "0", Info14 = "0";

                if (JsonObject.Generators.Count > 0)
                {
                    foreach (var gen in JsonObject.Generators)
                    {
                        Info1 = JsonObject.updateTime.AddHours(8).ToString();
                        Info2 = string.Format("{0:#,0.0}", gen.FuleLevel);
                        Info3 = string.Format("{0:#,0.0}", gen.L1Nvoltage);
                        Info4 = string.Format("{0:#,0.0}", gen.L2Nvoltage);
                        Info5 = string.Format("{0:#,0.0}", gen.L3Nvoltage);
                        Info6 = string.Format("{0:#,0.0}", gen.L1current);
                        Info7 = string.Format("{0:#,0.0}", gen.L2current);
                        Info8 = string.Format("{0:#,0.0}", gen.L3current);
                        Info9 = string.Format("{0:#,0.0}", gen.totalwatts / 1000);
                        Info10 = string.Format("{0:#,0.0}", gen.averagepowerfactor);
                        Info11 = string.Format("{0:#,0.0}", gen.positiveKWhours);
                        Info12 = string.Format("{0:#,0.0}", gen.negativeKWhours);
                        Info13 = gen.ControlStatus.Equals("true") ? "啟動中" : "已關閉";
                        Info14 = string.Format("{0:#,0.0}", gen.EngineBatteryVoltage);
                    }
                }
                Generator += Info1 + "|";
                Generator += Info2 + "|";
                Generator += Info3 + "|";
                Generator += Info4 + "|";
                Generator += Info5 + "|";
                Generator += Info6 + "|";
                Generator += Info7 + "|";
                Generator += Info8 + "|";
                Generator += Info9 + "|";
                Generator += Info10 + "|";
                Generator += Info11 + "|";
                Generator += Info12 + "|";
                Generator += Info13 + "|";
                Generator += Info14;
                #endregion

                #region SOCLoadWatt
                //Right View
                double soc = 0;
                List<float> AverageVoltage = new List<float>();
                if (JsonObject.Battery.Count > 0)
                {
                    foreach (var ba in JsonObject.Battery)
                    {
                        if (ba.connected == true)
                        {
                            AverageVoltage.Add(ba.voltage);
                        }
                    }
                }
                soc = AverageVoltage.Count==0?0:AverageVoltage.Average() <= 48.0 ? 0 : AverageVoltage.Average() >= 53 ? 100 : (((AverageVoltage.Average() - 48) / (53 - 48) * 100.00) * 0.2);
                Demand = Math.Round(soc, 2);

                List<double> Watt =new List<double>();
                if (JsonObject.LoadPowers.Count > 0)
                {
                    foreach (var lo in JsonObject.LoadPowers)
                    {
                        if (lo.index == 2)
                        {
                            Watt.Add(lo.Watt_t / 1000.0);
                        }
                    }
                }
               double LoadWatt = Watt.Count==0?0 : Watt.Average()<=0?0:Watt.Average();

                if (soc == 0 || LoadWatt == 0)
                {
                    RemainTime = 0;
                }
                else
                {
                    RemainTime = Math.Round(soc / LoadWatt, 1);
                };
                #endregion

                InfoDatas Datas = new InfoDatas()
                {
                    UUID = UUID,
                    GridPower = GridPower,
                    Inverters = Inverters,
                    Solar = Solar,
                    Battery = Battery,
                    LoadPower = LoadPower,
                    Generator = Generator,
                    Demand = Demand,
                    RemainTime = RemainTime
                };

                updata = InfoDatasService.Updata(Datas);
                logger.Info("Updata Solar Info :" + updata.ToString());
                Dispose();
            }
            catch (Exception ex)
            {
                logger.Fatal(" ........ERROE START Updata InfoData");
                logger.Fatal(ex);
                logger.Fatal(" .......ERROE END UpdataInfoData");
            }
        }

        private void UpdataBattery(Guid UUID, DateTime UTC)
        {
            try
            {
                int Hour = UTC.Hour;
                int Minture = UTC.Minute;

                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                DateTime starttime = DateTime.UtcNow;

                starttime = UTC.AddDays(-1);
                #region APP BatteryChart
                //資料
                List<Service.ESS.Model.Battery> APPBatteryDate = BatteryService.ReadByInfoListForUpdata(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (APPBatteryDate.Count > 0)
                {
                    HourList = "";
                    double TotalVolt = 0;
                    for (int i = 0; i < 24; i++)
                    {
                        List<Service.ESS.Model.Battery> count = APPBatteryDate.Where(x => x.updateTime >= starttime && x.updateTime < starttime.AddHours(1)).ToList();
                        List<double> batteryVolt = new List<double>();
                        List<double> batteryTotalVolt = new List<double>();
                        if (count.Count() > 0)
                        {
                            int c = 0;
                            foreach (var B in count)
                            {
                                if (c < 4)
                                {
                                    if (B.connected == true)
                                    {
                                        batteryVolt.Add(B.voltage);
                                    }
                                    c++;
                                    if (c == 4)
                                    {
                                        if (batteryVolt.Count > 0)
                                        {
                                            batteryTotalVolt.Add(batteryVolt.Average() < 48 ? 0 : batteryVolt.Average() >= 53 ? 1 : ((batteryVolt.Average() - 48.0) / (53.0 - 48.0)));
                                        }
                                        else
                                        {
                                            batteryTotalVolt.Add(0);
                                        }
                                        c = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            batteryTotalVolt.Add(0);
                        }
                        TotalVolt = batteryTotalVolt.Average() == 0 ? 0 : batteryTotalVolt.Average() * 100.00;
                        HourList += string.Format("{0:N2},", Math.Round(TotalVolt, 2)).Trim();
                        starttime = starttime.AddHours(1);
                    }
                }
                #endregion  APP BatteryChart

                starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                #region Web BatteryChart 
                List<Service.ESS.Model.Battery> WebBatteryDate = BatteryService.ReadByInfoListForUpdata(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (WebBatteryDate.Count > 0)
                {
                    QuarterList = "";
                    double TotalVolt = 0;
                    for (int i = 0; i < 96; i++)
                    {
                        List<double> batteryVolt = new List<double>();
                        List<double> batteryTotalVolt = new List<double>();
                        List<Service.ESS.Model.Battery> BatteryCount = WebBatteryDate.Where(x => x.updateTime >= starttime && x.updateTime < starttime.AddMinutes(15)).ToList();

                        if (BatteryCount.Count() > 0)
                        {
                            int c = 0;
                            foreach (var BV in BatteryCount)
                            {
                                if (c < 4)
                                {
                                    if (BV.connected == true)
                                    {
                                        batteryVolt.Add(BV.voltage);
                                    }
                                    c++;
                                    if (c == 4)
                                    {
                                        if (batteryVolt.Count > 0)
                                        {
                                            batteryTotalVolt.Add(batteryVolt.Average() < 48 ? 0 : batteryVolt.Average() >= 53 ? 1 : ((batteryVolt.Average() - 48.0) / (53.0 - 48.0)));
                                        }
                                        else
                                        {
                                            batteryTotalVolt.Add(0);
                                        }
                                        c = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            batteryTotalVolt.Add(0);
                        }
                        TotalVolt = batteryTotalVolt.Average() == 0 ? 0.00 : batteryTotalVolt.Average() * 100.00;
                        QuarterList += Math.Round(TotalVolt, 2).ToString().Trim() + ",";
                        starttime = starttime.AddMinutes(15);
                    }
                }
                #endregion BatteryChart

                InfoCharts BatteryCharts = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "Battery",
                    Hour = Hour,
                    Minture = Minture,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                bool updata = InfoChartsService.Updata(BatteryCharts);
                logger.Info("Updata Battery Info :" + updata.ToString());

            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info Battery Chart.........");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info Battery Chart.........");
            }
        }

        private void UpdataGridPower(Guid UUID, DateTime UTC)
        {
            try
            {
                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                DateTime starttime = DateTime.UtcNow;
                bool updata = false;
                List<double> sum = new List<double>();
                List<double> sum1 = new List<double>();

                starttime = UTC.AddHours(-25);
                #region APP GridPowerChart                      
                List<Service.ESS.Model.GridPower> GPD = GridPowerService.ReadByInfoListForUpdata(starttime, starttime.AddHours(25)).Where(x => x.index == 0 && x.uuid == UUID).ToList();

                if (GPD.Count > 0)
                {
                    HourList = "";
                    for (int i = 0; i <= 24; i++)
                    {
                        List<Service.ESS.Model.GridPower> count = GPD.Where(x => x.date_time >= starttime && x.date_time < starttime.AddHours(1)).ToList();
                        if (i > 0)
                        {
                            double g1 = count.Count == 0 ? sum1.Last() : count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            HourList += Convert.ToString(Math.Round(gs1 < 0 ? 0 : gs1, 2));
                            HourList += (i < 24) ? "," : "";
                            sum1.Add(g1);
                        }
                        else//第一筆資料
                        {
                            int j = 0;
                            int c = 0;
                            while (count.Count < 1)
                            {
                                if (c == 0)
                                {
                                    int k1 = -1 * j;
                                    j++;
                                    int k2 = -1 * j;
                                    count = GPD.Where(x => x.date_time >= starttime.AddHours(k2) && x.date_time < starttime.AddHours(k1)).ToList();
                                }
                                else
                                {
                                    int k1 = -1 * j;
                                    j++;
                                    int k2 = -1 * j;
                                    count = GridPowerService.ReadByInfoListForUpdata(starttime.AddHours(k2), starttime.AddHours(k1)).Where(x => x.index == 0 && x.uuid == UUID).ToList();
                                }
                                c++;
                                if (j > 672) { break; }
                            }
                            double grid = count.Count == 0 ? 0 : count.Average(x => x.kWHt);
                            sum1.Add(grid);
                        }
                        starttime = starttime.AddHours(1);
                    }
                }
                #endregion GridPowerChart

                starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                #region Web GridPowerChart          
                sum1.Clear();
                starttime = starttime.AddMinutes(-15);
                List<Service.ESS.Model.GridPower> WebGridPowerDate = GridPowerService.ReadByInfoListForUpdata(starttime, starttime.AddDays(1)).Where(x => x.index == 0 && x.uuid == UUID).ToList();
                if (WebGridPowerDate.Count > 0)
                {
                    QuarterList = "";
                    for (int i = 0; i <= 96; i++)
                    {
                        List<Service.ESS.Model.GridPower> count = WebGridPowerDate.Where(x => x.date_time >= starttime && x.date_time < starttime.AddMinutes(15)).ToList();
                        if (i > 0)
                        {
                            double g1 = count.Count == 0 ? sum1.Last() : count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            QuarterList += Math.Round(gs1 < 0 ? 0 : gs1, 2);
                            QuarterList += (i < 96) ? "," : "";
                            sum1.Add(g1);
                        }
                        else//第一筆資料
                        {
                            int j = 0;
                            int c = 0;
                            while (count.Count < 1)
                            {
                                if (c == 0)
                                {
                                    int k1 = -15 * j;
                                    j++;
                                    int k2 = -15 * j;
                                    count = WebGridPowerDate.Where(x => x.date_time >= starttime.AddMinutes(k2) && x.date_time < starttime.AddMinutes(k1)).ToList();
                                }
                                else
                                {
                                    int k1 = -15 * j;
                                    j++;
                                    int k2 = -15 * j;
                                    count = GridPowerService.ReadByInfoListForUpdata(starttime.AddMinutes(k2), starttime.AddMinutes(k1)).Where(x => x.index == 0 && x.uuid == UUID).ToList();
                                }
                                c++;
                                if (c >2688) //4*24*7*4 一個月都沒資料
                                {
                                    break;
                                }
                            }
                            double g1 =count.Count==0?0:count.Average(x => x.kWHt);
                            sum1.Add(g1);
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                }
                #endregion GridPowerChart

                InfoCharts GridPowerChart = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "GridPower",
                    Hour = UTC.Hour,
                    Minture = UTC.Minute,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                updata = InfoChartsService.Updata(GridPowerChart);
                logger.Info("Updata GridPower Info :" + updata.ToString());
            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info GridPower Chart");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info GridPower Chart");
            }
        }

        private void UpdataInverters(Guid UUID, DateTime UTC)
        {
            try
            {
                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                DateTime starttime = DateTime.UtcNow;
                bool updata = false;
                List<Inverter> InvertersDate = new List<Inverter>();

                #region APP InvertersChart
                //資料
                starttime = UTC.AddDays(-1);
                InvertersDate = InverterService.ReadByInfoListForUpdata(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (InvertersDate.Count > 0)
                {
                    HourList = "";
                    for (int i = 0; i < 24; i++)
                    {
                        List<Inverter> count = InvertersDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddHours(1)).ToList();
                        HourList += string.Format("{0:N2},",
                            (count.Count == 0) ? 0 :
                            (count.Average(x => x.ParallelInformation_TotalOutputActivePower
                            .Split('|').ToList()
                            .Sum(y => y.IsEmpty() ? 0 : Convert.ToDouble(y) / 1000.0)))).Trim();
                        starttime = starttime.AddHours(1);
                    }
                }
                InvertersDate.Clear();
                #endregion Chart

                #region Web Inverters Chart
                starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                InvertersDate = InverterService.ReadByInfoListForUpdata(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (InvertersDate.Count > 0)
                {
                    QuarterList = "";
                    for (int i = 0; i < 96; i++)
                    {
                        List<Inverter> count = InvertersDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddMinutes(15)).ToList();
                        QuarterList += string.Format("{0:N2},",
                            (count.Count == 0) ? 0 :
                            (count.Average(x => x.ParallelInformation_TotalOutputActivePower
                            .Split('|').ToList()
                            .Sum(y => y.IsEmpty() ? 0 : Convert.ToDouble(y) / 1000.0)))).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                }
                InvertersDate.Clear();
                #endregion Chart

                InfoCharts InvertersCharts = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "Inverters",
                    Hour = UTC.Hour,
                    Minture = UTC.Minute,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                updata = InfoChartsService.Updata(InvertersCharts);
                logger.Info("Updata Inverters Info :" + updata.ToString());
                Dispose();
            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info Inverters Chart");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info Inverters Chart");
            }
        }

        private void UpdataLoadPowers(Guid UUID, DateTime UTC)
        {
            try
            {
                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                DateTime starttime = DateTime.UtcNow;
                bool updata = false;
                List<double> sum = new List<double>();
                List<double> sum1 = new List<double>();
                List<double> sum2 = new List<double>();

                #region Web Load Chart
                //資料       
                starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                starttime = starttime.AddMinutes(-15);
                List<LoadPower> LoadDate = LoadPowerService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.index == 2 && x.uuid == UUID).ToList();     //負載迴路一
                if (LoadDate.Count > 0)
                {
                    QuarterList = "";
                    for (int i = 0; i <= 96; i++)
                    {
                        List<LoadPower> count = LoadDate.Where(x => x.date_Time >= starttime && x.date_Time < starttime.AddMinutes(15)).ToList();
                        if (i > 0)
                        {
                            //負載迴路一
                            double g1 = count.Count == 0 ? sum1.Last() : count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            QuarterList += Math.Round(gs1 < 0 ? 0 : gs1, 2);
                            QuarterList += (i < 96) ? "," : "";
                            sum1.Add(g1);
                        }
                        else
                        {
                            //負載迴路一
                            int j = 1;
                            int c = 0;
                            count.Clear();
                            while (count.Count < 1)
                            {
                                if (c == 0)
                                {
                                    int k1 = -15 * j;
                                    j++;
                                    int k2 = -15 * j;
                                    count = LoadDate.Where(x => x.date_Time >= starttime.AddMinutes(k2) && x.date_Time < starttime.AddMinutes(k1)).ToList();
                                }
                                else
                                {
                                    int k1 = -15 * j;
                                    j++;
                                    int k2 = -15 * j;
                                    count = LoadPowerService.ReadByInfoList(starttime.AddMinutes(k2), starttime.AddMinutes(k1)).Where(x => x.index == 2 && x.uuid == UUID).ToList();
                                }
                                c++;
                                if (c > 2688) { break; }
                            }
                            double g1 = count.Count()==0 ? 0 : count.Average(x => x.kWHt);
                            sum1.Add(g1);
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                    LoadDate.Clear();
                }
                #endregion LoadChart

                #region APP LoadChart
                starttime = UTC.AddHours(-25);
                sum1.Clear();
                sum2.Clear();
                LoadDate = LoadPowerService.ReadByInfoList(starttime, starttime.AddHours(25)).Where(x => x.index == 2 && x.uuid == UUID).ToList();
                if (LoadDate.Count > 0)
                {
                    HourList = "";
                    for (int i = 0; i <= 24; i++)
                    {
                        List<LoadPower> count = LoadDate.Where(x => x.date_Time >= starttime && x.date_Time < starttime.AddHours(1)).ToList();
                        if (i > 0)
                        {
                            //負載迴路一
                            double g1 = count.Count == 0 ? sum1.Last() : count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            HourList += Convert.ToString(Math.Round(gs1 < 0 ? 0 : gs1, 2));
                            HourList += (i < 24) ? "," : "";
                            sum1.Add(g1);
                        }
                        else
                        {
                            //負載迴路一
                            int j = 1;
                            int c = 0;
                            count.Clear();
                            while (count.Count < 1)
                            {
                                if (c == 0)
                                {
                                    int k1 = -1 * j;
                                    j++;
                                    int k2 = -1 * j;
                                    count = LoadDate.Where(x => x.date_Time >= starttime.AddHours(k2) && x.date_Time < starttime.AddHours(k1)).ToList();
                                }
                                else
                                {
                                    int k1 = -1 * j;
                                    j++;
                                    int k2 = -1 * j;
                                    count = LoadPowerService.ReadByInfoList(starttime.AddHours(k2), starttime.AddHours(k1)).Where(x => x.index == 2 && x.uuid == UUID).ToList();
                                }
                                c++;
                                if (j > 672) { break; }//找前一個月
                            }
                            double g1 = count.Count == 0 ? 0 : count.Average(x => x.kWHt);
                            sum1.Add(g1);
                        }
                        starttime = starttime.AddHours(1);
                    }
                }
                LoadDate.Clear();
                #endregion LoadChart

                InfoCharts LoadChart = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "LoadPower",
                    Hour = UTC.Hour,
                    Minture = UTC.Minute,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                updata = InfoChartsService.Updata(LoadChart);
                logger.Info("Updata Load Info :" + updata.ToString());


                Dispose();
            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info LoadPower Chart");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info LoadPower Chart");
            }
        }

        private void UpdataGenerators(Guid UUID, DateTime UTC)
        {
            try
            {
                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                bool updata = false;
                DateTime starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                List<double> sum = new List<double>();
                List<double> sum1 = new List<double>();
                List<double> sum2 = new List<double>();

                #region web GeneratorChart
                //資料
                List<Generator> GeneratorDate = GeneratorService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (GeneratorDate.Count > 0)
                {
                    QuarterList = "";
                    for (int i = 0; i <= 96; i++)
                    {
                        List<Generator> count = GeneratorDate.Where(x => x.UpdateTime >= starttime && x.UpdateTime < starttime.AddMinutes(15) && x.index == 0).ToList();
                        if (i > 0)
                        {
                            if (count.Count > 0)
                            {
                                double g1 = count.Average(x => x.positiveKWhours) / 1000.00;
                                QuarterList += Math.Round((count.Average(x => x.positiveKWhours) / 1000.0) - sum1.Last(), 2) + ",";
                                sum1.Add(g1);
                            }
                            else
                            {
                                QuarterList += 0 + ",";
                            }
                        }
                        else
                        {
                            if (count.Count > 0)
                            {
                                double g1 = count.Average(x => x.positiveKWhours) / 1000.00;
                                sum1.Add(g1);
                            }
                            else
                            {
                                sum1.Add(0);
                            }
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                }
                GeneratorDate.Clear();
                #endregion GeneratorChart

                #region App GeneratorChart
                GeneratorDate = null;
                sum1.Clear();
                starttime = UTC.AddDays(-1);
                GeneratorDate = GeneratorService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                if (GeneratorDate.Count > 0)
                {
                    HourList = "";
                    for (int i = 0; i <= 24; i++)
                    {
                        var count = GeneratorDate.Where(x => x.UpdateTime >= starttime && x.UpdateTime < starttime.AddHours(1) && x.index == 0).ToList();
                        if (i > 0)
                        {
                            if (count.Count > 0)
                            {
                                double g1 = count.Average(x => x.positiveKWhours) / 1000.00;
                                HourList += Math.Round((count.Average(x => x.positiveKWhours) / 1000.0) - sum1.Last(), 2) + ",";
                                sum1.Add(g1);
                            }
                            else
                            {
                                HourList += 0 + ",";
                            }
                        }
                        else
                        {
                            if (count.Count > 0)
                            {
                                double g1 = count.Average(x => x.positiveKWhours) / 1000.00;
                                sum1.Add(g1);
                            }
                            else
                            {
                                sum1.Add(0);
                            }
                        }
                        starttime = starttime.AddHours(1);
                    }
                }
                GeneratorDate.Clear();
                #endregion Chart


                InfoCharts GeneratorChart = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "Generator",
                    Hour = UTC.Hour,
                    Minture =UTC.Minute,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                updata = InfoChartsService.Updata(GeneratorChart);
                logger.Info("Updata Generator Info :" + updata.ToString());

                Dispose();
            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info Generator Chart");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info Generator Chart");
            }
        }

        private  void  UpdataSolar(Guid UUID, DateTime UTC)
        {
            try
            {
                string QuarterList = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0|0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string HourList = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00|0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                DateTime starttime = DateTime.UtcNow;
                bool updata = false;
                List<double> sum = new List<double>();
                List<double> sum1 = new List<double>();
                List<double> sum2 = new List<double>();
                string sun0 = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                string sun1 = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";

                #region Web Solar Chart
                //資料
                starttime = DateTime.Today.AddHours(-8);//當日00:00(utc+8)
                List<Inverter> SolarDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();

                if (SolarDate.Count > 0)
                {
                    QuarterList = "";
                    sun0 = "";
                    sun1 = "";
                    for (int i = 0; i < 96; i++)
                    {
                        List<Inverter> count = SolarDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddMinutes(15)).ToList();
                        List<double> S0 = new List<double>();
                        List<double> S1 = new List<double>();
                        if (count.Count > 0)
                        {
                            foreach (var sun in count)
                            {
                                string[] sunPower = sun.SPM90ActivePower.Split('|');
                                S0.Add(sunPower[0].IsEmpty() ? 0.0 : (Convert.ToDouble(sunPower[0])));
                                S1.Add(sunPower[1].IsEmpty() ? 0.0 : (Convert.ToDouble(sunPower[1])));
                            }
                            sun0 += Math.Round(S0.Average() / 1000.0, 2) + ",";
                            sun1 += Math.Round(S1.Average() / 1000.0, 2) + ",";
                        }
                        else
                        {
                            sun0 += 0 + ",";
                            sun1 += 0 + ",";
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                }
                SolarDate.Clear();
                //組圖表資料
                QuarterList = sun0 + "|" + sun1;
                #endregion  SolarChart

                #region APP SolarChart
                sum.Clear();
                starttime = UTC.AddDays(-1);
                SolarDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.uuid == UUID).ToList();
                 sun0 = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                 sun1 = "0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00";
                if (SolarDate.Count > 0)
                {
                    HourList = "";
                    sun0 = "";
                    sun1 = "";
                    for (int i = 0; i < 24; i++)
                    {
                        var count = SolarDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddHours(1)).ToList();
                        List<double> S0 = new List<double>();
                        List<double> S1 = new List<double>();
                        if (count.Count > 0)
                        {
                            foreach (var sun in count)
                            {
                                string[] sunPower = sun.SPM90ActivePower.Split('|');
                                S0.Add(sunPower[0].IsEmpty() ? 0.0 : (Convert.ToDouble(sunPower[0])));
                                S1.Add(sunPower[1].IsEmpty() ? 0.0 : (Convert.ToDouble(sunPower[1])));
                            }
                            sun0 += Math.Round(S0.Average() / 1000.0, 2) + ",";
                            sun1 += Math.Round(S1.Average() / 1000.0, 2) + ",";
                        }
                        else
                        {
                            sun0 += 0 + ",";
                            sun1 += 0 + ",";
                        }
                        starttime = starttime.AddHours(1);
                    }
                }
                SolarDate.Clear();
                HourList = sun0 + "|" + sun1;
                #endregion  SolarChart

                InfoCharts SolarChart = new InfoCharts()
                {
                    UUID = UUID,
                    Equipment = "Solar",
                    Hour = UTC.Hour,
                    Minture = UTC.Minute,
                    QuarterList = QuarterList,
                    HourList = HourList
                };

                updata = InfoChartsService.Updata(SolarChart);
                logger.Info("Updata Solar Info :" + updata.ToString());
                Dispose();
            }
            catch (Exception ex)
            {
                logger.Fatal(" ERROE START Updata Info Solar Chart");
                logger.Fatal(ex);
                logger.Fatal(" ERROE END Updata Info Solar Chart");
            }
        }

        #endregion

        #region 異常
        private void BatteryAlart(Guid BarrryID, Guid Uid, Models.Json.Battery BJ)
        {
            try
            {
                StationService stationService = new StationService();
                AlartTypeService alartType = new AlartTypeService();
                ErrorCodesService errorCodesService = new ErrorCodesService();

                if (BJ.AlarmState.OV_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("OV_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.UV_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("UV_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.OC_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("OC_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.SC_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("SC_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.OT_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("OT_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.UT_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("UT_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.RV_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("RV_DIS"); AlartID(SID, ATID, BarrryID, context); }
                if (BJ.AlarmState.OC0_DIS) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(3).Id; string context = "電池組:" + BJ.index.ToString() + ", 警示:" + errorCodesService.ReadContext("OC0_DIS"); AlartID(SID, ATID, BarrryID, context); }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void GeneratorAlart(Guid GeneratorID, Guid Uid, Generators GenJ)
        {
            try
            {
                StationService stationService = new StationService();
                AlartTypeService alartType = new AlartTypeService();
                ErrorCodesService errorCodesService = new ErrorCodesService();

                if (GenJ.index == 0)
                {
                    if (Convert.ToInt16(GenJ.Alarm.NumberOfNamedAlarms) > 0)
                    {
                        if (GenJ.Alarm.EmergencyStop > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	EmergencyStop	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LowOilPressure > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LowOilPressure	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.HighCoolantTemperature > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	HighCoolantTemperature	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LowCoolantTemperature > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LowCoolantTemperature	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.UnderSpeed > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	UnderSpeed	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.OverSpeed > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	OverSpeed	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorUnderFrequency > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorUnderFrequency	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorOverFrequency > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorOverFrequency	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorLowVoltage > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorLowVoltage	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorHighVoltage > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorHighVoltage	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.BatteryLowVoltage > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryLowVoltage	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.BatteryHighVoltage > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryHighVoltage	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.ChargeAlternatorFailure > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	ChargeAlternatorFailure	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.FailToStart > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	FailToStart	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.FailToStop > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	FailToStop	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorFailToClose > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorFailToClose	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.MainsFailToClose > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MainsFailToClose	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.OilPressureSenderFault > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	OilPressureSenderFault	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LossOfMagneticPickUp > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LossOfMagneticPickUp	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.MagneticPickUpOpenCircuit > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MagneticPickUpOpenCircuit	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorHighCurrent > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorHighCurrent	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneA > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneA	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LowFuelLevel > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LowFuelLevel	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.CANECUWarning > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	CANECUWarning	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.CANECUShutdown > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	CANECUShutdown	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.CANECUDataFail > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	CANECUDataFail	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LowOillevelSwitch > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LowOillevelSwitch	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.HighTemperatureSwitch > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	HighTemperatureSwitch	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LowFuelLevelSwitch > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LowFuelLevelSwitch	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.ExpansionUnitWatchdogAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	ExpansionUnitWatchdogAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.kWOverloadAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	kWOverloadAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NegativePhaseSequenceCurrentAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NegativePhaseSequenceCurrentAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.EarthFaultTripAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	EarthFaultTripAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorPhaseRotationAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorPhaseRotationAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.AutoVoltageSenseFail > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	AutoVoltageSenseFail	"); AlartID(SID, ATID, GeneratorID, context); }

                        //if (GenJ.Alarm.MaintenanceAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MaintenanceAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        //20190316改蓄電池電壓==0
                        if (GenJ.EngineBatteryVoltage <= 0) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MaintenanceAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LoadingFrequencyAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LoadingFrequencyAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.LoadingVoltageAlarm > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	LoadingVoltageAlarm	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneB > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneB	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneC > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneC	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneD > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneD	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneE > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneE	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.GeneratorShortCircuit > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	GeneratorShortCircuit	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.MainsHighCurrent > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MainsHighCurrent	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.MainsEarthFault > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MainsEarthFault	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.MainsShortCircuit > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	MainsShortCircuit	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.ECUProtect > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	ECUProtect	"); AlartID(SID, ATID, GeneratorID, context); }
                        if (GenJ.Alarm.NoneF > 1) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:" + errorCodesService.ReadContext("	NoneF	"); AlartID(SID, ATID, GeneratorID, context); }
                        //  if (GenJ.Alarm.Message != null) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(6).Id; string context = "警示:"+ errorCodesService.ReadContext("	Message	"); AlartID(SID, ATID, GeneratorID, context); }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void InvertersAlart(Guid inverterID, Guid Uid, Inverters InvJ)
        {
            try
            {
                StationService stationService = new StationService();
                AlartTypeService alartType = new AlartTypeService();
                ErrorCodesService errorCodesService = new ErrorCodesService();



                if (InvJ.WarningStatus.InverterFault) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	InverterFault	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BusOver) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BusOver	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BusUnder) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BusUnder	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BusSoftFail) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BusSoftFail	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.LINE_FAIL) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	LINE_FAIL	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.LINE_FAIL) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	LINE_FAIL	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.OPVShort) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	OPVShort	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.InverterVoltageTooLow) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	InverterVoltageTooLow	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.InverterVoltageTooHigh) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	InverterVoltageTooHigh	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.OverTemperature) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	OverTemperature	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.FanLocked) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	FanLocked	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatteryVoltageHigh) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryVoltageHigh	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatteryLowAlarm) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryLowAlarm	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatteryUnderShutdown) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryUnderShutdown	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.OverLoad) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	OverLoad	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.EepromFault) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	EepromFault	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.InverterOverCurrent) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	InverterOverCurrent	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.InverterSoftFail) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	InverterSoftFail	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.SelfTestFail) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	SelfTestFail	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.OP_DC_VoltageOver) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	OP_DC_VoltageOver	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatOpen) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatOpen	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.CurrentSensorFail) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	CurrentSensorFail	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatteryShort) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryShort	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.PowerLimit) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	PowerLimit	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.PV_VoltageHigh) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	PV_VoltageHigh	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.MPPT_OverloadFault) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	MPPT_OverloadFault	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.MPPT_OverloadWarning) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	MPPT_OverloadWarning	"); AlartID(SID, ATID, inverterID, context); }
                if (InvJ.WarningStatus.BatteryTooLowToCharge) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context = "警示:" + errorCodesService.ReadContext("	BatteryTooLowToCharge	"); AlartID(SID, ATID, inverterID, context); }
                // if (InvJ.WarningStatus.Message != null) { Guid SID = stationService.ReadUUID(Uid).Id; Guid ATID = alartType.ID(7).Id; string context =  "逆變機:" + InvJ.index.ToString() + ":" + errorCodesService.ReadContext("	Message	"); AlartID(SID, ATID, inverterID, context); }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private Guid AlartID(Guid SID, Guid ATID, Guid EquipmentID, string Context)
        {
            try
            {
                AlartService alartService = new AlartService();
                Alart alart = new Alart()
                {
                    StationID = SID,
                    AlartTypeID = ATID,
                    EquipmentID = EquipmentID,
                    AlartContext = Context,
                    StartTimet = DateTime.Now,
                    EndTimet = DateTime.Now
                };
                var AID = alartService.Create(alart);
                return AID;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return Guid.Empty;
            }
        }
        #endregion

        public enum GeneratorsAlertType
        {
            Disabled =0 ,
            無警報 = 1,
            普通警報 = 2,
            停機警報 = 3,
            電氣跳閘 = 4,
            InactiveIndicationNoString = 8,
            InactiveIndicationDisplayedString = 9,
            ActiveIndication = 10
        }


        /// <summary>
        /// Json示範碼
        /// </summary>
        #region JsonString
        private readonly string JsonString = @"{
  'updateTime': '2018-11-13T07:00:00.000Z',
  'stationName': '1111',
  'stationUUID': 'd4788824-ba3e-11e8-96f8-529269fb1459',
  'GridPowers': [
    {
      'version': 1,
      'index': 0,
      'modelSerial': '0100000000110000',
      'serialNO': 'SCB-SC01',
      'name': '市電迴路1',
      'connected': false,
      'date_time': '2018-11-13T05:58:09.941Z',
      'va': 38.7,
      'vb': 0,
      'vc': 40.6,
      'vavg': 39.6,
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
      'angle_vb': 68.7,
      'angle_vc': 155.4,
      'angle_ia': 135.4,
      'angle_ib': 149.9,
      'angle_ic': 291.1,
      'frequency': 59.98,
      'vab': 38.7,
      'vbc': 38.7,
      'vca': 77.4,
      'vii_avg': 0,
      'kwht': 31.1,
      'kwha': 4.3,
      'kwhb': 0,
      'kwhc': 26.7,
      'kvarht': 52.8,
      'kvarha': 33,
      'kvarhb': 0,
      'kvarhc': 19.7,
      'kvaht': 67.8,
      'kvaha': 34,
      'kvahb': 0,
      'kvahc': 33.8,
      'demand': 0.1,
      'prev_demand': 0.3,
      'prev_demand2': 0.6,
      'prev_demand3': 0,
      'maxdemand_currnetmonth': 1.6,
      'maxdemand_lastmonth': 0,
      'remain_time': 828,
      'events': []
    },
    {
      'version': 1,
      'index': 1,
      'modelSerial': '0100000000120000',
      'serialNO': 'SCB-SC01',
      'name': '市電迴路2',
      'connected': false,
      'date_time': '2018-11-13T05:58:12.267Z',
      'va': 38.4,
      'vb': 0,
      'vc': 40.3,
      'vavg': 39.3,
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
      'angle_vb': 159.7,
      'angle_vc': 357,
      'angle_ia': 124.9,
      'angle_ib': 312.5,
      'angle_ic': 40.6,
      'frequency': 59.98,
      'vab': 38.4,
      'vbc': 38.4,
      'vca': 2.8,
      'vii_avg': 0,
      'kwht': 31,
      'kwha': 4.5,
      'kwhb': 0,
      'kwhc': 26.5,
      'kvarht': 52.4,
      'kvarha': 32.7,
      'kvarhb': 0,
      'kvarhc': 19.7,
      'kvaht': 67.4,
      'kvaha': 33.7,
      'kvahb': 0,
      'kvahc': 33.6,
      'demand': 0,
      'prev_demand': 0.7,
      'prev_demand2': 1.4,
      'prev_demand3': 0.8,
      'maxdemand_currnetmonth': 2.5,
      'maxdemand_lastmonth': 0,
      'remain_time': 65522,
      'events': []
    }
  ],
  'LoadPowers': [
    {
      'version': 1,
      'index': 2,
      'modelSerial': '0100000000130001',
      'serialNO': 'SCB-SC01',
      'name': '負載迴路1',
      'connected': false,
      'date_time': '2018-11-13T05:58:14.581Z',
      'va': 179.7,
      'vb': 0,
      'vc': 40.3,
      'vavg': 110,
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
      'angle_vb': 199.8,
      'angle_vc': 199.7,
      'angle_ia': 152,
      'angle_ib': 33.3,
      'angle_ic': 135.1,
      'frequency': 59.98,
      'vab': 179.7,
      'vbc': 179.7,
      'vca': 218,
      'vii_avg': 0,
      'kwht': 19.2,
      'kwha': 10.9,
      'kwhb': 0,
      'kwhc': 8.2,
      'kvarht': 33.6,
      'kvarha': 16.4,
      'kvarhb': 0,
      'kvarhc': 17.1,
      'kvaht': 39.1,
      'kvaha': 19.8,
      'kvahb': 0,
      'kvahc': 19.2,
      'demand': 0.1,
      'prev_demand': 0.2,
      'prev_demand2': 0.2,
      'prev_demand3': 0.2,
      'maxdemand_currnetmonth': 0.2,
      'maxdemand_lastmonth': 0,
      'remain_time': 823,
      'events': []
    },
    {
      'version': 1,
      'index': 3,
      'modelSerial': '0100000000140001',
      'serialNO': 'SCB-SC01',
      'name': '負載迴路2',
      'connected': false,
      'date_time': '2018-11-13T05:58:16.065Z',
      'va': 179.4,
      'vb': 0,
      'vc': 40.3,
      'vavg': 109.8,
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
      'angle_vb': 181.3,
      'angle_vc': 188.4,
      'angle_ia': 198.3,
      'angle_ib': 215,
      'angle_ic': 49.8,
      'frequency': 59.98,
      'vab': 179.4,
      'vbc': 179.4,
      'vca': 219.3,
      'vii_avg': 0,
      'kwht': 18.9,
      'kwha': 10.8,
      'kwhb': 0,
      'kwhc': 8.1,
      'kvarht': 33.2,
      'kvarha': 16.3,
      'kvarhb': 0,
      'kvarhc': 16.9,
      'kvaht': 38.6,
      'kvaha': 19.6,
      'kvahb': 0,
      'kvahc': 18.9,
      'demand': 0.1,
      'prev_demand': 0.2,
      'prev_demand2': 0.2,
      'prev_demand3': 0.2,
      'maxdemand_currnetmonth': 0.2,
      'maxdemand_lastmonth': 0,
      'remain_time': 821,
      'events': []
    }
  ],
  'Generators': [
    {
      'version': 1,
      'index': 0,
      'modelSerial': 'DSE-9000',
      'serialNO': '0000111122223333',
      'name': 'DSE 9000',
      'connected': true,
      'updateTime': '2018-10-30T09:10:48.302Z',
      'OilPressure': 496,
      'CoolantTemperature': 47,
      'OilTemperature': 32767,
      'FuleLevel': 83.5,
      'InternalFlexibleSenderAnalogueInputType': 0,
      'ChargeAlternatorVoltage': 10,
      'EngineBatteryVoltage': 12,
      'EngineSpeed': 1818,
      'EngineRunTime': 6396,
      'NumberOfStarts': 18,
      'frequency': 60.6,
      'L1Nvoltage': 106.5,
      'L2Nvoltage': 104.5,
      'L3Nvoltage': 106.6,
      'L1L2voltage': 184,
      'L2L3voltage': 184,
      'L3L1voltage': 183.1,
      'L1current': 0,
      'L2current': 0,
      'L3current': 0,
      'earthcurrent': 0,
      'L1watts': 0,
      'L2watts': 0,
      'L3watts': 0,
      'currentlaglead': 32767,
      'totalwatts': 0,
      'L1VA': 0,
      'L2VA': 0,
      'L3VA': 0,
      'totalVA': 0,
      'L1Var': 0,
      'L2Var': 0,
      'L3Var': 0,
      'totalVar': 0,
      'powerfactorL1': 0,
      'powerfactorL2': 0,
      'powerfactorL3': 0,
      'averagepowerfactor': 327.65,
      'percentageoffullpower': 0,
      'percentageoffullVar': 0,
      'positiveKWhours': 44.7,
      'negativeKWhours': 429496729.5,
      'KVAhours': 54.6,
      'KVArhours': 30.4,
      'ControlStatus': true,
      'Alarm': {
        'NumberOfNamedAlarms': 69,
        'EmergencyStop': 1,
        'LowOilPressure': 1,
        'HighCoolantTemperature': 1,
        'LowCoolantTemperature': 0,
        'UnderSpeed': 1,
        'OverSpeed': 1,
        'GeneratorUnderFrequency': 1,
        'GeneratorOverFrequency': 1,
        'GeneratorLowVoltage': 1,
        'GeneratorHighVoltage': 1,
        'BatteryLowVoltage': 1,
        'BatteryHighVoltage': 1,
        'ChargeAlternatorFailure': 1,
        'FailToStart': 1,
        'FailToStop': 1,
        'GeneratorFailToClose': 1,
        'MainsFailToClose': 0,
        'OilPressureSenderFault': 1,
        'LossOfMagneticPickUp': 1,
        'MagneticPickUpOpenCircuit': 1,
        'GeneratorHighCurrent': 1,
        'NoneA': 1,
        'LowFuelLevel': 0,
        'CANECUWarning': 1,
        'CANECUShutdown': 1,
        'CANECUDataFail': 1,
        'LowOillevelSwitch': 1,
        'HighTemperatureSwitch': 1,
        'LowFuelLevelSwitch': 1,
        'ExpansionUnitWatchdogAlarm': 1,
        'kWOverloadAlarm': 1,
        'NegativePhaseSequenceCurrentAlarm': 4,
        'EarthFaultTripAlarm': 1,
        'GeneratorPhaseRotationAlarm': 1,
        'AutoVoltageSenseFail': 1,
        'MaintenanceAlarm': 15,
        'LoadingFrequencyAlarm': 1,
        'LoadingVoltageAlarm': 1,
        'NoneB': 1,
        'NoneC': 1,
        'NoneD': 1,
        'NoneE': 1,
        'GeneratorShortCircuit': 1,
        'MainsHighCurrent': 1,
        'MainsEarthFault': 1,
        'MainsShortCircuit': 1,
        'ECUProtect': 1,
        'NoneF': 1,
        'Message': '[報警停用：發機低水?：],[報警停用：市電合閘失敗：],[報警停用：低油位：]'
      },
      'AvailabilityEnergy': 833.9145,
      'AvailabilityHour': 0,
      'FuelRelay': true,
      'StartRelay': false,
      'DigitalOutC': true,
      'DigitalOutD': false,
      'DigitalOutE': false,
      'DigitalOutF': false,
      'DigitalOutG': false,
      'DigitalOutH': false,
      'STOPLEDstatus': false,
      'MANUALLEDstatus': false,
      'TESTLEDstatus': false,
      'AUTOLEDstatus': true,
      'GENLEDstatus': false,
      'GENBREAKERLEDstatus': false,
      'MAINSLEDstatus': true,
      'MAINSBREAKERLEDstatus': true,
      'USERLED1status': false,
      'USERLED2statu': false,
      'USERLED3status': false,
      'USERLED4status': false
    }
  ],
  'Inverters': [
    {
      'version': 1,
      'index': 0,
      'modelSerial': 'OPTI-SP5000',
      'serialNO': '5000000000000002',
      'name': 'OPTI SP5000 Brilliant Ultra',
      'connected': false,
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
        'BatteryLowAlarm': true,
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
          'SerialNumber': '92931807102904',
          'WorkMode': 'B',
          'FaultCode': '00',
          'GridVoltage': 0,
          'GridFrequency': 0,
          'ACOutputVoltage': 219.8,
          'ACOutputFrequency': 60,
          'ACOutputApparentPower': 65,
          'ACOutputActivePower': 23,
          'LoadPercentage': 1,
          'BatteryVoltage': 51.1,
          'BatteryChargingCurrent': 0,
          'BatteryCapacity': 89,
          'PV_InputVoltage': 53.8,
          'TotalChargingCurrent': 0,
          'Total_AC_OutputApparentPower': 109,
          'TotalOutputActivePower': 25,
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
        },
        {
          'IsExist': true,
          'SerialNumber': '92931807102938',
          'WorkMode': 'B',
          'FaultCode': '00',
          'GridVoltage': 0,
          'GridFrequency': 0,
          'ACOutputVoltage': 220,
          'ACOutputFrequency': 60,
          'ACOutputApparentPower': 44,
          'ACOutputActivePower': 3,
          'LoadPercentage': 0,
          'BatteryVoltage': 51.2,
          'BatteryChargingCurrent': 0,
          'BatteryCapacity': 90,
          'PV_InputVoltage': 54.1,
          'TotalChargingCurrent': 0,
          'Total_AC_OutputApparentPower': 88,
          'TotalOutputActivePower': 21,
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
        }
      ],
      'GridVoltage': 0,
      'GridFrequency': 0,
      'AC_OutputVoltage': 220.1,
      'AC_OutputFrequency': 59.9,
      'AC_OutputApparentPower': 44,
      'AC_OutputActivePower': 13,
      'OutputLoadPercent': 0,
      'BUSVoltage': 357,
      'BatteryVoltage': 51,
      'BatteryChargingCurrent': 0,
      'BatteryCapacity': 88,
      'InverterHeatSinkTemperature': 36,
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
      'PV_ChargingPower': 9,
      'SPM90s': [
        {
          'id': 1,
          'connected': false,
          'Voltage': 0,
          'Current': 0,
          'ActivePower': 0,
          'ActiveEnergy': 3.38,
          'VoltageDirection': 0
        },
        {
          'id': 4,
          'connected': false,
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
      'SPM90ActiveEnergy': 3.38,
      'SPM90VoltageDirection': 0
    }
  ],
  'Battery': [
    {
      'version': 1,
      'index': 0,
      'modelSerial': 'FSP-BS4866',
      'serialNO': '4866000000010000',
      'name': 'FSP-BS4866',
      'connected': false,
      'voltage': 51,
      'charging_current': 0,
      'discharging_current': 0,
      'charging_watt': 0,
      'discharging_watt': 0,
      'SOC': 0,
      'Cycle': 1,
      'charge_direction': 0,
      'temperature': 30,
      'Cells': [
        {
          'index': 1,
          'voltage': 3.1958
        },
        {
          'index': 2,
          'voltage': 3.1909
        },
        {
          'index': 3,
          'voltage': 3.1933000000000002
        },
        {
          'index': 4,
          'voltage': 3.1982000000000004
        },
        {
          'index': 5,
          'voltage': 3.186
        },
        {
          'index': 6,
          'voltage': 3.1909
        },
        {
          'index': 7,
          'voltage': 3.1982000000000004
        },
        {
          'index': 8,
          'voltage': 3.1958
        },
        {
          'index': 9,
          'voltage': 3.1909
        },
        {
          'index': 10,
          'voltage': 3.1933000000000002
        },
        {
          'index': 11,
          'voltage': 3.1982000000000004
        },
        {
          'index': 12,
          'voltage': 3.186
        },
        {
          'index': 13,
          'voltage': 3.1909
        },
        {
          'index': 14,
          'voltage': 3.1982000000000004
        }
      ],
      'AlarmState': {
        'OV_DIS': true,
        'UV_DIS': false,
        'OC_DIS': false,
        'SC_DIS': false,
        'OT_DIS': false,
        'UT_DIS': true,
        'RV_DIS': false,
        'OC0_DIS': false
      },
      'updateTime': '2018-11-13T05:58:14.629Z'
    },
    {
      'version': 1,
      'index': 1,
      'modelSerial': 'FSP-BS4866',
      'serialNO': '4866000000020000',
      'name': 'FSP-BS4866',
      'connected': false,
      'voltage': 51,
      'charging_current': 0,
      'discharging_current': 0.665,
      'charging_watt': 0,
      'discharging_watt': 33.915,
      'SOC': 0,
      'Cycle': 1,
      'charge_direction': 2,
      'temperature': 29,
      'Cells': [
        {
          'index': 1,
          'voltage': 3.1811000000000003
        },
        {
          'index': 2,
          'voltage': 3.1933000000000002
        },
        {
          'index': 3,
          'voltage': 3.1958
        },
        {
          'index': 4,
          'voltage': 3.1884
        },
        {
          'index': 5,
          'voltage': 3.1811000000000003
        },
        {
          'index': 6,
          'voltage': 3.1958
        },
        {
          'index': 7,
          'voltage': 3.1933000000000002
        },
        {
          'index': 8,
          'voltage': 3.1811000000000003
        },
        {
          'index': 9,
          'voltage': 3.1933000000000002
        },
        {
          'index': 10,
          'voltage': 3.1958
        },
        {
          'index': 11,
          'voltage': 3.1884
        },
        {
          'index': 12,
          'voltage': 3.1811000000000003
        },
        {
          'index': 13,
          'voltage': 3.1958
        },
        {
          'index': 14,
          'voltage': 3.1933000000000002
        }
      ],
      'AlarmState': {
        'OV_DIS': true,
        'UV_DIS': false,
        'OC_DIS': false,
        'SC_DIS': true,
        'OT_DIS': false,
        'UT_DIS': false,
        'RV_DIS': false,
        'OC0_DIS': true
      },
      'updateTime': '2018-11-13T05:58:14.985Z'
    },
    {
      'version': 1,
      'index': 2,
      'modelSerial': 'FSP-BS4866',
      'serialNO': '4866000000030000',
      'name': 'FSP-BS4866',
      'connected': false,
      'voltage': 51,
      'charging_current': 0,
      'discharging_current': 0.704,
      'charging_watt': 0,
      'discharging_watt': 35.903999999999996,
      'SOC': 0,
      'Cycle': 1,
      'charge_direction': 2,
      'temperature': 30,
      'Cells': [
        {
          'index': 1,
          'voltage': 3.1909
        },
        {
          'index': 2,
          'voltage': 3.1958
        },
        {
          'index': 3,
          'voltage': 3.1933000000000002
        },
        {
          'index': 4,
          'voltage': 3.1909
        },
        {
          'index': 5,
          'voltage': 3.1835
        },
        {
          'index': 6,
          'voltage': 3.186
        },
        {
          'index': 7,
          'voltage': 3.1884
        },
        {
          'index': 8,
          'voltage': 3.1909
        },
        {
          'index': 9,
          'voltage': 3.1958
        },
        {
          'index': 10,
          'voltage': 3.1933000000000002
        },
        {
          'index': 11,
          'voltage': 3.1909
        },
        {
          'index': 12,
          'voltage': 3.1835
        },
        {
          'index': 13,
          'voltage': 3.186
        },
        {
          'index': 14,
          'voltage': 3.1884
        }
      ],
      'AlarmState': {
        'OV_DIS': false,
        'UV_DIS': false,
        'OC_DIS': false,
        'SC_DIS': false,
        'OT_DIS': true,
        'UT_DIS': false,
        'RV_DIS': false,
        'OC0_DIS': false
      },
      'updateTime': '2018-11-13T05:58:15.391Z'
    },
    {
      'version': 1,
      'index': 3,
      'modelSerial': 'FSP-BS4866',
      'serialNO': '4866000000040000',
      'name': 'FSP-BS4866',
      'connected': false,
      'voltage': 51,
      'charging_current': 0,
      'discharging_current': 0.9390000000000001,
      'charging_watt': 0,
      'discharging_watt': 47.889,
      'SOC': 0,
      'Cycle': 1,
      'charge_direction': 2,
      'temperature': 33,
      'Cells': [
        {
          'index': 1,
          'voltage': 3.1909
        },
        {
          'index': 2,
          'voltage': 3.1933000000000002
        },
        {
          'index': 3,
          'voltage': 3.1958
        },
        {
          'index': 4,
          'voltage': 3.1933000000000002
        },
        {
          'index': 5,
          'voltage': 3.1933000000000002
        },
        {
          'index': 6,
          'voltage': 3.1909
        },
        {
          'index': 7,
          'voltage': 3.1884
        },
        {
          'index': 8,
          'voltage': 3.1909
        },
        {
          'index': 9,
          'voltage': 3.1933000000000002
        },
        {
          'index': 10,
          'voltage': 3.1958
        },
        {
          'index': 11,
          'voltage': 3.1933000000000002
        },
        {
          'index': 12,
          'voltage': 3.1933000000000002
        },
        {
          'index': 13,
          'voltage': 3.1909
        },
        {
          'index': 14,
          'voltage': 3.1884
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
      },
      'updateTime': '2018-11-13T07:00:00.000Z'
    }
  ]
}";



        #endregion
    }
}