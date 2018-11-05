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
            endTime = inverterService.ReadNow().UpdateTime;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + inverter.UpdateTime + "',";
            Data += "'工作模式" + "':'" +mod +"',";
            Data += "'市電電壓(V)" + "':'" + string.Format("{0:#,0.0}", inverter.GridVoltage) + "',";
            Data += "'市電頻率  (Hz)" + "':'" + string.Format("{0:#,0.0}", inverter.GridVoltage) + "',";
            Data += "'輸出電壓  (V)" + "':'" + string.Format("{0:#,0.00}", inverter.AC_OutputVoltage) + "',";
            Data += "'輸出頻率  (Hz)" + "':'" + string.Format("{0:#,0.0}", inverter.AC_OutputFrequency) + "',";
            Data += "'總輸出實功率(kW)" + "':'" + string.Format("{0:#,0.0}", inverter.ParallelInformation_TotalOutputActivePower / 1000.0) + "',";
            Data += "'電池電壓  (V)" + "':'" + string.Format("{0:#,0.0}", inverter.BatteryVoltage) + "',";
            Data += "'電池容量  (%)" + "':'" + string.Format("{0:#,0.0}", inverter.BatteryCapacity) + "',";
            Data += "'太陽能電壓  (V)" + "':'" + string.Format("{0:#,0.0}", inverter.PV_InputVoltage) + "',";
            Data += "'總充電電流  (A)" + "':'" + string.Format("{0:#,0.0}", inverter.ParallelInformation_TotalChargingCurrent) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", inverterService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.ParallelInformation_TotalOutputActivePower)).Trim();
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
            endTime = LoadPowerService.ReadNow().updateTime;
            start = endTime.AddHours(-6);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + loadPower.updateTime + "',";
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
            endTime = inverterService.ReadNow().UpdateTime;
            start = endTime.AddMinutes(-360);
            starttime = new DateTime(start.Year, start.Month, start.Day, start.AddHours(1).Hour, 00, 00, 00);
            //資料
            Data += "{'info':{";
            Data += "'資料時間(UTC)" + "':'" + inverter.UpdateTime + "',";
            Data += "'電壓(V)" + "':'" + string.Format("{0:0.0}", inverter.SPM90Voltage) + "',";
            Data += "'電流(A)" + "':'" + string.Format("{0:#,0.0}", inverter.SPM90Current) + "',";
            Data += "'功率(W)" + "':'" + string.Format("{0:#,0.0}", inverter.SPM90ActiveEnergy / 1000.0) + "',";
            Data += "'發電量(kWh)" + "':'" + string.Format("{0:#,0.00}", inverter.SPM90ActivePower) + "'";
            Data += "},";

            Data += "chart:{";
            for (int i = 0; i < 6; i++)
            {
                Data += "'" + starttime.Hour + ":00':";
                Data += string.Format("'{0:N2}'", inverterService.ReadByInfoList(starttime, starttime.AddHours(1)).Average(x => x.SPM90ActiveEnergy)).Trim();
                if (i < 5) { Data += ","; }
                starttime = starttime.AddHours(1);
            }
            Data += "}}";
            return Ok(Data);
        }

    }
}
