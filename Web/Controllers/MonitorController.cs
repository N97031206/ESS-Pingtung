using NLog;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class MonitorController : Controller
    {
        #region private
        private readonly ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();
        private StationService stationService = new StationService();
        //Log檔
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public ActionResult Web()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            return View();
        }

        public ActionResult JiaSing()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            return View();
        }

        [HttpPost]
        public JsonResult ReadframeList()
        {
            Guid 霧台ID = stationService.UUID(2);
            List<int> cdData = new List<int>();
            var battery = BatteryService.ReadNow(霧台ID);
            battery.ForEach(x => cdData.Add((int)x.charge_direction));
            int cd = cdData.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();

            #region 正式資料
            FrameData frames = new FrameData()
            {
                Solar = Math.Round(InverterService.ReadNow(霧台ID).SPM90ActivePower.Split('|').ToList().Sum(x => string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x) / 1000.0), 2),
                GirdPower = Math.Round( GridPowerService.ReadNow(霧台ID).Watt_t/1000.00,2),
                Load = Math.Round(LoadPowerService.ReadNow(霧台ID).Watt_t   /1000.00,2),
                BatteySOC = BatteryService.EachSOC(battery.Average(x => x.voltage)),
                BatteyPower = (cd == 1) ? Math.Round(battery.Average(x => x.charging_watt) / 1000.0, 2) : (cd == 2) ? Math.Round(battery.Average(x => x.discharging_watt) / 1000.0, 2) : 0,
               Direction = cd,
               GeneratorPower= Math.Round(GeneratorService.ReadNow(霧台ID).totalVar, 2)
        };
            #endregion

            #region 測試資料
            //Random random = new Random();
            //int direct = Convert.ToInt32(Math.Round(random.Next(0, 20) / 10.0));
            //FrameData frames = new FrameData()
            //{
            //    Solar = random.Next(-10, 30) / 10.0f,
            //    GirdPower = random.Next(-20, 40) / 10.0f,
            //    Load = random.Next(-50, 100) / 10.0f,
            //    BatteyMode = (direct == 0) ? "離線" : (direct == 1) ? "充電" : "放電",
            //    BatteySOC = random.Next(800, 990) / 10.0f,
            //    BatteyPower = random.Next(20, 30) / 10.0f,
            //    GeneratorPower = random.Next(-1050, 1150) / 10.0f,
            //    Direction = direct
            //};
            #endregion

            return Json(frames);
        }

        [HttpPost]
        public JsonResult JiaSingList()
        {
            //Guid JiaSingID = stationService.UUID(6);
            //List<int> cdData = new List<int>();
            //var battery = BatteryService.ReadNow(JiaSingID);
            //battery.ForEach(x => cdData.Add((int)x.charge_direction));
            //int cd = cdData.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();

            //#region 正式資料
            //FrameData frames = new FrameData()
            //{
            //    Solar = Math.Round(InverterService.ReadNow(JiaSingID).SPM90ActivePower.Split('|').ToList().Sum(x => string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x) / 1000.0), 2),
            //    GirdPower = Math.Round(GridPowerService.ReadNow(JiaSingID).Watt_t / 1000.00, 2),
            //    Load = Math.Round(LoadPowerService.ReadNow(JiaSingID).Watt_t / 1000.00, 2),
            //    BatteySOC = BatteryService.EachSOC(battery.Average(x => x.voltage)),
            //    BatteyPower = (cd == 1) ? Math.Round(battery.Average(x => x.charging_watt) / 1000.0, 2) : (cd == 2) ? Math.Round(battery.Average(x => x.discharging_watt) / 1000.0, 2) : 0,
            //    Direction = cd,
            //    GeneratorPower = Math.Round(GeneratorService.ReadNow(JiaSingID).totalVar, 2)
            //};
            //#endregion

            #region 測試資料
            Random random = new Random();
            int direct = Convert.ToInt32(Math.Round(random.Next(0, 20) / 10.0));
            FrameData frames = new FrameData()
            {
                Solar = random.Next(-10, 30) / 10.0f,
                GirdPower = random.Next(-20, 40) / 10.0f,
                Load = random.Next(-50, 100) / 10.0f,
                BatteyMode = (direct == 0) ? "離線" : (direct == 1) ? "充電" : "放電",
                BatteySOC = random.Next(800, 990) / 10.0f,
                BatteyPower = random.Next(20, 30) / 10.0f,
                GeneratorPower = random.Next(-1050, 1150) / 10.0f,
                Direction = direct
            };
            #endregion

            return Json(frames);
        }

        //Angular首頁資料結構
        private class FrameData
        {
            public double Solar { get; set; }
            public double GirdPower { get; set; }
            public double Load { get; set; }
            public string BatteyMode { get; set; }
            public double BatteySOC { get; set; }
            public double BatteyPower { get; set; }
            public double GeneratorPower { get; set; }
            public int Direction { get; set; }
        }
    }
}