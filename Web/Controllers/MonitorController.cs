using NLog;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class MonitorController : Controller
    {
        #region private
        private ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();
        //Log檔
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        // GET: Monitor
        public ActionResult Index()
        {
            Random random = new Random();
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            TempData["solar"]= random.Next(20,30)/10.0;
            TempData["GirdPower"] = random.Next(20, 40) / 10.0;
            TempData["Load"] = random.Next(50, 100) / 10.0;
            TempData["BatteyModa"] = "充電";
            TempData["BatteySOC"] = random.Next(800, 990) / 10.0;
            TempData["BatteyPower"] = random.Next(20, 30) / 10.0;
            return View();
        }

        public ActionResult Frames()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];          
            return View();
        }
        
        public ActionResult Web()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            return View();
        }

        [HttpPost]
        public JsonResult ReadframeList()
        {
            Battery battery = BatteryService.ReadNow();
            int cd = Convert.ToInt32(battery.charge_direction);

            #region 正式資料
            FrameData frames = new FrameData()
            {
                Solar = Math.Round(InverterService.ReadNow().SPM90ActivePower.Split('|').ToList().Sum(x => string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x) / 1000.0), 2),
                GirdPower = Math.Round( GridPowerService.ReadNow().Watt_t/1000.00,2),
                Load = Math.Round(LoadPowerService.ReadNow().Watt_t/1000.00,2),
                BatteyMode = (cd == 1)?"充電": (cd == 2) ? "放電": "離線",
                BatteySOC =  BatteryService.EachSOC(battery.voltage),
                BatteyPower = (cd == 1) ?  Math.Round(battery.charging_watt/1000.0,2): (cd == 2) ?Math.Round(battery.discharging_watt/1000.0,2) : 0,
                Direction = cd
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