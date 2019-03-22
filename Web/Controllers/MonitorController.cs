using NLog;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Configuration;
using System.Web.Mvc;


namespace Web.Controllers
{
    public class MonitorController : Controller
    {
        #region private
        private StationService stationService = new StationService();
        private MonitorService monitorService = new MonitorService();


        //Log檔
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public ActionResult Index()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            ViewBag.Local ="(大武社區)";
            return View();
        }

        public ActionResult JiaSing()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["LogoInfo"];
            ViewBag.Local = "(佳興社區)";
            return View();
        }

        [HttpPost]
        [OutputCache(CacheProfile = "Cache1Min")]
        public JsonResult WutaiList()
        {
            Guid WutaiID = stationService.UUID(2);

            var data = monitorService.IndexDatas(WutaiID);
            IndexData frames = new IndexData()
            {
                Solar = data.Solar,
                GirdPower = data.GirdPower,
                Load = data.Load,
                BatteySOC = data.BatteySOC,
                BatteyModel = data.BatteyModel.Trim(),
                BatteyPower = data.BatteyPower,
                Direction = data.Direction,
                GeneratorPower = data.GeneratorPower,
                LoadConnected=data.LoadConnected,
                SPM90MConnected= data.SPM90MConnected,
                GeneratorConnected = data.GeneratorConnected,
                GridPowerConnected = data.GridPowerConnected,
                BatteryConnected = data.BatteryConnected
            };

            #region 測試資料
            //Random random = new Random();
            //int direct = Convert.ToInt32(Math.Round(random.Next(0, 20) / 10.0));
            //IndexData frames = new IndexData()
            //{
            //    Solar = random.Next(-10, 30) / 10.0f,
            //    GirdPower = random.Next(-20, 40) / 10.0f,
            //    Load = random.Next(-50, 100) / 10.0f,
            //    BatteyModel = (direct == 0) ? "離線" : (direct == 1) ? "充電" : "放電",
            //    BatteySOC = random.Next(800, 990) / 10.0f,
            //    BatteyPower = random.Next(20, 30) / 10.0f,
            //    GeneratorPower = random.Next(-1050, 1150) / 10.0f,
            //    Direction = direct
            //};
            #endregion
            return Json(frames);
        }
      
        /// <summary>
        /// 佳興
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OutputCache(CacheProfile = "Cache1Min")]
        public JsonResult JiaSingList()
        {
            Guid JiaSingID = stationService.UUID(6);

            var data = monitorService.IndexDatas(JiaSingID);
            IndexData frames = new IndexData()
            {
                Solar = data.Solar,
                GirdPower = data.GirdPower,
                Load = data.Load,
                BatteySOC = data.BatteySOC,
                BatteyModel = data.BatteyModel.Trim(),
                BatteyPower = data.BatteyPower,
                Direction = data.Direction,
                GeneratorPower = data.GeneratorPower,
                LoadConnected = data.LoadConnected,
                SPM90MConnected = data.SPM90MConnected,
                GeneratorConnected = data.GeneratorConnected,
                GridPowerConnected = data.GridPowerConnected,
                BatteryConnected = data.BatteryConnected
            };

            #region 測試資料
            //Random random = new Random();
            //int direct = Convert.ToInt32(Math.Round(random.Next(0, 20) / 10.0));
            //IndexData frames = new IndexData()
            //{
            //    Solar = Math.Round(random.Next(-10, 30) / 10.0f, 2),
            //    GirdPower = Math.Round(random.Next(-20, 40) / 10.0f, 2),
            //    Load = Math.Round(random.Next(-50, 100) / 10.0f, 2),
            //    BatteyMode = (direct == 0) ? "離線" : (direct == 1) ? "充電" : "放電",
            //    BatteySOC = Math.Round(random.Next(800, 990) / 10.0f, 2),
            //    BatteyPower = Math.Round(random.Next(20, 30) / 10.0f, 2),
            //    GeneratorPower = Math.Round(random.Next(-1050, 1150) / 10.0f, 2),
            //    Direction = direct
            //};
            #endregion

            return Json(frames);
        }
    }
}