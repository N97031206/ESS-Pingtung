using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.ESS.Model;
using Service.ESS.Provider;
using Newtonsoft.Json;
using NLog;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Web.Models.Json;
using Battery = Service.ESS.Model.Battery;
using System.Net.Http.Formatting;

namespace Web.Controllers.Api
{
    public class TabController : ApiController
    {
        #region private
        //Tab
        private BulletinService bulletinService = new BulletinService();
        private StationService stationService = new StationService();
        private AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private OrginService orginService = new OrginService();
        //EMS
        private ESSObjecterService essObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService inverterService = new InverterService();
        #endregion

        [Route("api/Tab/Abnormal/List")]
        [HttpGet]
        public IHttpActionResult AbnormalList(int page = 1)
        {
            var alartList = alartService.ReadAll().ToList();    
            return Ok(alartList);
        }

        [Route("api/Tab/Abnormal/Detail")]
        [HttpGet]
        public IHttpActionResult AbnormalDetail(int id = 0)
        {
            return Ok();
        }


        /// <summary>
        /// 站點
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Station")]
        [HttpGet]
        public IHttpActionResult Station()
        {

            return Ok(JsonConvert.SerializeObject(stationService.ReadAll()));
        }

        /// <summary>
        /// 佈告欄
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Bulletin")]
        [HttpGet]
        public IHttpActionResult Bulletin()
        {
            string Data = null;
            var Bulletun = bulletinService.ReadAll().ToList();

            Data += "{";
            Bulletun.ForEach(x=> {
                Data += "{";
                Data += "'發佈日期':'" + x.CreateDate.ToString()+ "'" +",";
                Data += "'標題':'"+x.title+"'" + ",";
                Data += "'單位':'"+ orginService.ReadID(x.OrginID).OrginName.ToString().Trim() + "'" + ",";
                Data += "'內文':'"+x.context+"'";
                Data += "}";
            });
            Data += "}";

            return Ok(Data);
        }

        /// <summary>
        ///  異常警示
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Abnormal")]
        [HttpGet]
        public IHttpActionResult Abnormal()
        {
            List<Alart> alartList = alartService.ReadAll().ToList();
            return Ok(JsonConvert.SerializeObject(alartList));
        }

        /// <summary>
        /// 異常類型
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/AlartType")]
        [HttpGet]
        public IHttpActionResult AlartType()
        {
            return Ok(JsonConvert.SerializeObject(alarttypeService.ReadAll().ToList()));
        }


        /// <summary>
        /// 市電資訊
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/GridPower")]
        [HttpGet]
        public IHttpActionResult GridPower()
        {
            GridPower gridPowers = GridPowerService.ReadNow();
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            endTime = GridPowerService.ReadNow().date_time;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + gridPowers.date_time + "',";
            Data += "'電壓(V)" + "':'" + string.Format("{0:0.0}", gridPowers.Vavg) + "',";
            Data += "'電流(A)" + "':'" + string.Format("{0:#,0.0}", gridPowers.Isum) + "',";
            Data += "'實功率(W)" + "':'" + string.Format("{0:#,0.0}", gridPowers.Watt_t / 1000.0) + "',";
            Data += "'虛功率(VAR)" + "':'" + string.Format("{0:#,0.00}", gridPowers.Var_t / 1000.00) + "',";
            Data += "'視在功率(VA)" + "':'" + string.Format("{0:#,0.0}", gridPowers.VA_t / 1000.0) + "',";
            Data += "'功率因數(PF)" + "':'" + string.Format("{0:#,0.0}", gridPowers.PF_t) + "',";
            Data += "'頻率(Hz)" + "':'" + string.Format("{0:#,0.0}", gridPowers.Frequency) + "',";
            Data += "'用電量(kWh)" + "':'" + string.Format("{0:#,0.0}", gridPowers.kWHt) + "'";
            Data += "},";    

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00" + "':";
                Data += string.Format("'{0:N2}'", GridPowerService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.Vavg)).Trim();
                if (i < 5) {Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

        /// <summary>
        /// 電池
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Battery")]
        [HttpGet]
        public IHttpActionResult Battery()
        {
            Battery battery = BatteryService.ReadNow();
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            int cd = Convert.ToInt32(battery.charge_direction);
            string str = null;
            if (cd == 0) { str = "不充電"; } else if (cd == 1) { str = "充電"; } else if (cd == 2) { str = "放電"; }
            endTime = BatteryService.ReadNow().updateTime;
            start = endTime.AddHours(-6);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + battery.updateTime + "',";
            Data += "'電池電壓(Voltage)" + "':'" + string.Format("{0:0.0}", battery.voltage) + "',";
            Data += "'充電電流(Charging)" + "':'" + string.Format("{0:#,0.0}", battery.charging_current) + "',";
            Data += "'放電電流(Discharging)" + "':'" + string.Format("{0:#,0.0}", battery.discharging_current) + "',";
            Data += "'電池容量(Capacity)" + "':'" + string.Format("{0:#,0.00}", battery.SOC) + "',";
            Data += "'充電次數(Cycles)" + "':'" + string.Format("{0:#,0.0}", battery.Cycle) + "',";
            Data += "'充電方向(Charging Direction)" + "':'" + str + "',";
            Data += "'溫度(Temperature)" + "':'" + string.Format("{0:#,0.0}", battery.charge_direction)+"°C" + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", BatteryService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.SOC)).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

        /// <summary>
        ///  逆變器
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Inverters")]
        [HttpGet]
        public IHttpActionResult Inverters()
        {
            Inverter inverter = inverterService.ReadNow();
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            string mod = inverter.DeviceMode.Trim();
            if (mod == "P") { mod = "Power On Mode"; }
            else if (mod == "S") { mod = "Standby Mode"; }
            else if (mod == "L") { mod = "Line Mode"; }
            else if (mod == "B") { mod = "Battery Mode"; }
            else if (mod == "F") { mod = "Fault Mode"; }
            else if (mod == "H") { mod = "Power Saving Mode"; }
            else { mod = "Unknown Mode"; }
            endTime = inverterService.ReadNow().CreateTime;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + inverter.CreateTime + "',";
            Data += "'工作模式" + "':'" +mod +"',";
            Data += "'市電電壓(V)" + "':'" + string.Format("{0:#,0.0}", inverter.GridVoltage) + "',";
            Data += "'市電頻率  (Hz)" + "':'" + string.Format("{0:#,0.0}", inverter.GridVoltage) + "',";
            Data += "'輸出電壓  (V)" + "':'" + string.Format("{0:#,0.00}", inverter.AC_OutputVoltage) + "',";
            Data += "'輸出頻率  (Hz)" + "':'" + string.Format("{0:#,0.0}", inverter.AC_OutputFrequency) + "',";
            Data += "'總輸出實功率(kW)" + "':'" + string.Format("{0:#,0.0}", Convert.ToDouble(inverter.ParallelInformation_TotalOutputActivePower) / 1000.0) + "',";
            Data += "'電池電壓  (V)" + "':'" + string.Format("{0:#,0.0}", inverter.BatteryVoltage) + "',";
            Data += "'電池容量  (%)" + "':'" + string.Format("{0:#,0.0}", inverter.BatteryCapacity) + "',";
            Data += "'太陽能電壓  (V)" + "':'" + string.Format("{0:#,0.0}", inverter.PV_InputVoltage) + "',";
            Data += "'總充電電流  (A)" + "':'" + string.Format("{0:#,0.0}", inverter.ParallelInformation_TotalChargingCurrent) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", inverterService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => Convert.ToDouble(x.ParallelInformation_TotalOutputActivePower))).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

        /// <summary>
        /// 負載
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Load")]
        [HttpGet]
        public IHttpActionResult Load()
        {
            LoadPower loadPower = LoadPowerService.ReadNow();
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            endTime = LoadPowerService.ReadNow().date_Time;
            start = endTime.AddHours(-6);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + loadPower.date_Time + "',";
            Data += "'電壓(V)" + "':'" + string.Format("{0:0.0}", loadPower.Vavg) + "',";
            Data += "'電流(A)" + "':'" + string.Format("{0:#,0.0}", loadPower.Isum) + "',";
            Data += "'實功率(W)" + "':'" + string.Format("{0:#,0.0}", loadPower.Watt_t / 1000.0) + "',";
            Data += "'虛功率(VAR)" + "':'" + string.Format("{0:#,0.00}", loadPower.Var_t / 1000.00) + "',";
            Data += "'視在功率(VA)" + "':'" + string.Format("{0:#,0.0}", loadPower.VA_t / 1000.0) + "',";
            Data += "'功率因數(PF)" + "':'" + string.Format("{0:#,0.0}", loadPower.PF_t) + "',";
            Data += "'頻率(Hz)" + "':'" + string.Format("{0:#,0.0}", loadPower.Frequency) + "',";
            Data += "'用電量(kWh)" + "':'" + string.Format("{0:#,0.0}", loadPower.kWHt) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", LoadPowerService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.Vavg)).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

        /// <summary>
        /// 發電機
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Generator")]
        [HttpGet]
        public IHttpActionResult Generator()
        {
            Generator generator = GeneratorService.ReadNow();
            string status = generator.ControlStatus.Equals("true") ? "已啟動": "關閉中";
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            endTime = GeneratorService.ReadNow().UpdateTime;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + generator.UpdateTime + "',";
            Data += "'發電機油位(%)" + "':'" + string.Format("{0:0.0}", generator.FuleLevel) + "',";
            Data += "'L1-N相電壓(V)" + "':'" + string.Format("{0:#,0.0}", generator.L1Nvoltage) + "',";
            Data += "'L2-N相電壓(V)" + "':'" + string.Format("{0:#,0.0}", generator.L2Nvoltage) + "',";
            Data += "'L3-N相電壓(V)" + "':'" + string.Format("{0:#,0.00}", generator.L3Nvoltage) + "',";
            Data += "'L1相電流(A)" + "':'" + string.Format("{0:#,0.0}", generator.L1current) + "',";
            Data += "'L2相電流(A)" + "':'" + string.Format("{0:#,0.0}", generator.L2current) + "',";
            Data += "'L3相電流(A)" + "':'" + string.Format("{0:#,0.0}", generator.L3current) + "',";
            Data += "'總實功率(kW)" + "':'" + string.Format("{0:#,0.0}", generator.totalwatts / 1000) + "',";
            Data += "'平均功率因數" + "':'" + string.Format("{0:#,0.0}", generator.averagepowerfactor) + "',";
            Data += "'正的千瓦時(kWh)" + "':'" + string.Format("{0:#,0.0}", generator.positiveKWhours) + "',";
            Data += "'負的千瓦時(kWh)" + "':'" + string.Format("{0:#,0.0}", generator.negativeKWhours) + "',";
            Data += "'發電機狀態" + "':'" + status + "',";
            Data += "'可用總電量(kWh)" + "':'" + string.Format("{0:#,0.0}", generator.AvailabilityEnergy) + "',";
            Data += "'可用電時數(H)" + "':'" + string.Format("{0:#,0.0}", generator.AvailabilityEnergy) + "',";
            Data += "'可用電時數(H)" + "':'" + string.Format("{0:#,0.0}", generator.AvailabilityHour) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", GeneratorService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.FuleLevel)).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

        /// <summary>
        /// 太陽能
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Solar")]
        [HttpGet]
        public IHttpActionResult Solar()
        {
            Inverter inverter = inverterService.ReadNow();
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            endTime = inverterService.ReadNow().CreateTime;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + inverter.CreateTime + "',";
            Data += "'電壓(V)" + "':'" + string.Format("{0:0.0}", inverter.SPM90Voltage) + "',";
            Data += "'電流(A)" + "':'" + string.Format("{0:#,0.0}", inverter.SPM90Current) + "',";
            Data += "'功率(W)" + "':'" + string.Format("{0:#,0.0}", Convert.ToDouble(inverter.SPM90ActiveEnergy) / 1000.0) + "',";
            Data += "'發電量(kWh)" + "':'" + string.Format("{0:#,0.00}", inverter.SPM90ActivePower) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", inverterService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => Convert.ToDouble(x.SPM90ActiveEnergy))).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }


        [Route("api/Android")]
        [HttpGet]
        public IHttpActionResult Android()
        {
            string jsonStr = JsonConvert.SerializeObject(JsonString);
            Console.Write(jsonStr);
            return Json(jsonStr);
        }

        private readonly string JsonString =
         @"{
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

    }
}
