using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Service.ESS.Model;
using Service.ESS.Provider;
using PagedList;
using NLog;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Web.Controllers
{
    public class MonitorController : Controller
    {
        #region private
        //Tab
        private AccountService accountService = new AccountService();
        private BulletinService bulletinService = new BulletinService();
        private StationService stationService = new StationService();
        private AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private OrginService orginService = new OrginService();
        //EMS
        private ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();
        //分頁
        private int PageSizes() { if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out int s)) { s = 10; } return s; }
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

  
       [HttpPost]
        public JsonResult ReadframeList()
        {
            Battery battery = BatteryService.ReadNow();
            int cd = Convert.ToInt32(battery.charge_direction);

            //Random random = new Random();
            //int direct = Convert.ToInt32(Math.Round(random.Next(0, 20) / 10.0));
            //FrameData frames = new FrameData()
            //{
            //    Solar = random.Next(-10, 30) / 10.0f,
            //    GirdPower = random.Next(20, 40) / 10.0f,
            //    Load = random.Next(50, 100) / 10.0f,
            //    BatteyMode = (direct == 0) ? "離線" : (direct == 1) ? "充電" : "放電",
            //    BatteySOC = random.Next(800, 990) / 10.0f,
            //    BatteyPower = random.Next(20, 30) / 10.0f,
            //    GeneratorPower = random.Next(1050, 1150) / 10.0f,
            //    Direction = direct
            //};

            FrameData frames = new FrameData()
            {
                Solar = InverterService.ReadNow().SPM90ActivePower,
                GirdPower = GridPowerService.ReadNow().Watt_t,
                Load = LoadPowerService.ReadNow().Watt_t,
                BatteyMode = (cd == 0) ? "離線" : (cd == 1) ? "充電" : "放電",
                BatteySOC = battery.SOC,
                BatteyPower = battery.discharging_watt,
                Direction = cd
            };

            return Json(frames);
        }

        public class FrameData
        {
            public float Solar { get; set; }
            public float GirdPower { get; set; }
            public float Load { get; set; }
            public string BatteyMode { get; set; }
            public float BatteySOC { get; set; }
            public float BatteyPower { get; set; }
            public float GeneratorPower { get; set; }
            public int Direction { get; set; }
        }
    }
}