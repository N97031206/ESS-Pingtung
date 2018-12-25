using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Service.ESS.Model;
using Service.ESS.Provider;
using PagedList;
using NLog;
using System.Web.WebPages;
using System.IO;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Domain = Repository.ESS.Domain;

namespace Web.Controllers
{
    [HandleError]
    public class TabController : Controller
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

        #region Index
        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            TagTitle();
            NavButtom("Index");
            return View();
        }
        #endregion

        #region QRCode
        [Authorize]
        public ActionResult QRCode()
        {
            TagTitle();
            NavButtom("QRCode");
            return View();
        }

        public ActionResult Download()
        {
            string file = Server.MapPath(@"~\Content\Aside\app-debug.apk");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        #endregion

        #region Bulletin
        /// <summary>
        /// 公告資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Bulletin(int page = 1)
        {
            TagTitle();
            NavButtom("Bulletin");
            //一開始抓取全部資料
            List<Bulletin> bulletins = bulletinService.ReadAllView().ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());

            return View(result);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Bulletin(FormCollection From, int page = 1)
        {
            TagTitle();
            NavButtom("Bulletin");

            //查詢時間區間
            var SE = Request.Form["range_date"];
            //分割字串
            char x = '-';
            string[] Day = SE.Split(x);
            //區間
            DateTime Start = Convert.ToDateTime(Day[0]);
            DateTime End = Convert.ToDateTime(Day[1]);
            //寫入表單
            List<Bulletin> bulletins = (Start == End) ? bulletinService.ReadAll().ToList() : bulletinService.ReadListBy(Start, End).ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());

            return View(result);
        }

        [Authorize]
        public ActionResult CreateBulletin()
        {
            TagTitle();
            StationList();
            AlartTypeList();
            OrginTypeList();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBulletin(FormCollection From)
        {
            var title = From["title"].Trim();
            var context = From["context"].Trim();
            var OrginID = Guid.Parse(From["OrginID"]);
            var AccountID = accountService.ReadByName(Session["UserName"].ToString()).Id;

            Bulletin bulletins = new Bulletin()
            {
                title = title,
                context = context,
                OrginID = OrginID,
                AccountID = AccountID
            };
            Guid BulletinID = bulletinService.Create(bulletins);
            return RedirectToAction("Maintain", "Tab");
        }

        [Authorize]
        public ActionResult ListBulletin(int page = 1)
        {
            List<Bulletin> bulletins = bulletinService.ReadAll();
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());
            return View(result);
        }

        [Authorize]
        public ActionResult EditBulletin(Guid id)
        {
            Bulletin bulletin = bulletinService.ReadByID(id);
            return View(bulletin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBulletin(FormCollection From)
        {
            var id = Guid.Parse(From["id"].Trim());
            var title = From["title"].Trim();
            var context = From["context"].Trim();
            string[] Disabled = From["Disabled"].Trim().Split(',');

            Bulletin bulletin = new Bulletin()
            {
                Id = id,
                title = title,
                context = context,
                Disabled = (Disabled.Length == 2) ? true : false,
                UpdateDate = DateTime.Now
            };

            Guid bulletinID = bulletinService.Update(bulletin);
            return RedirectToAction("ListBulletin", "Tab");
        }

        [Authorize]

        public ActionResult DisableBulletin(Guid id)
        {
            Bulletin bulletin = bulletinService.ReadByID(id);
            return View(bulletin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisableBulletin(FormCollection From)
        {
            Bulletin bulletin = new Bulletin()
            {
                Id = Guid.Parse(From["id"].Trim()),
                Disabled = (From["Disabled"].Trim().Split(',').Length == 2) ? true : false,
                UpdateDate = DateTime.Now
            };
            Guid BulletinID = bulletinService.UpdateDisable(bulletin);
            return RedirectToAction("ListBulletin", "Tab");
        }


        [Authorize]
        public ActionResult DeleteBulletin(Guid id)
        {
            Bulletin bulletin = bulletinService.ReadByID(id);
            return View(bulletin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBulletin(FormCollection From)
        {
            Guid Id = Guid.Parse(From["id"].Trim());
            bool BulletinID = bulletinService.Delete(Id);
            return RedirectToAction("ListBulletin", "Tab", new { del = BulletinID });
        }

        #endregion

        #region Info
        /// <summary>
        /// 及時資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Info(string tabType)
        {
            TagTitle();
            NavButtom("Info");
            Pills();
            StationList();

            tabType = string.IsNullOrEmpty(tabType) ? "GridPower" : tabType;
            InfoNav(tabType);
            ChartData(tabType);
            ViewBag.OnView = true;

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Info(FormCollection From, string tabType)
        {
            TagTitle();
            NavButtom("Info");
            Pills();
            StationList();

            var statonID = Guid.Parse(Request.Form["Staton"]);
            ViewBag.viewstation = stationService.ReadData(statonID).StationName.ToString().Trim();

            if (stationService.ReadData(statonID).StationName.Equals("霧台鄉大武社區活動中心") || stationService.ReadData(statonID).StationName.Equals("所有站別"))
            {
                tabType = string.IsNullOrEmpty(tabType) ? "GridPower" : tabType;
                InfoNav(tabType);
                ChartData(tabType);
                ViewBag.OnView = true;
            }
            else
            {
                ViewBag.OnView = false;
            }

            return View();
        }

        /// <summary>
        /// 更換Nav class
        /// </summary>
        /// <param name="tabType"></param>
        private void InfoNav(string tabType)
        {
            TempData["nav"] = tabType;
            TempData["navGridPower"] = "nav-link";
            TempData["navBattery"] = "nav-link";
            TempData["navSolar"] = "nav-link";
            TempData["navLoad"] = "nav-link";
            TempData["navGenerator"] = "nav-link";
            TempData["navInverters"] = "nav-link";
            TempData["tabGridPower"] = "tab-pane";
            TempData["tabBattery"] = "tab-pane ";
            TempData["tabSolar"] = "tab-pane ";
            TempData["tabLoad"] = "tab-pane ";
            TempData["tabGenerator"] = "tab-pane ";
            TempData["tabInverters"] = "tab-pane ";

            switch (tabType.Trim())
            {
                case "GridPower":
                    TempData["navGridPower"] = "nav-link active";
                    TempData["tabGridPower"] = "tab-pane active";
                    break;
                case "Inverters":
                    TempData["navInverters"] = "nav-link active";
                    TempData["tabInverters"] = "tab-pane active";
                    break;
                case "Solar":
                    TempData["navSolar"] = "nav-link active";
                    TempData["tabSolar"] = "tab-pane active";
                    break;
                case "Battery":
                    TempData["navBattery"] = "nav-link active";
                    TempData["tabBattery"] = "tab-pane active";
                    break;
                case "Load":
                    TempData["navLoad"] = "nav-link active";
                    TempData["tabLoad"] = "tab-pane active";
                    break;
                case "Generator":
                    TempData["navGenerator"] = "nav-link active";
                    TempData["tabGenerator"] = "tab-pane active";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// EChart資料
        /// </summary>
        /// <param name="tabType"></param>
        private void ChartData(string tabType)
        {
            DateTime starttime = DateTime.Today.AddHours(-8);
            string Data = null;
            List<double> sum = new List<double>();
            List<double> sum1 = new List<double>();
            List<double> sum2 = new List<double>();

            switch (tabType.Trim())
            {
                case "GridPower":
                    #region GridPowerChart          
                    //資料       
                    Data = null;
                    string Grid1 = null;
                    sum1.Clear();
                    sum2.Clear();
                    starttime = starttime.AddMinutes(-15);
                    var GridPowerDate = GridPowerService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.index == 0).ToList();
                    for (int i = 0; i <= 96; i++)
                    {
                        var count = GridPowerDate.Where(x => x.date_time >= starttime && x.date_time < starttime.AddMinutes(15)).ToList();
                        if (i > 0)
                        {
                            int j = 0;
                            while (count.Count < 1)
                            {
                                int k1 = -15 * j;
                                j++;
                                int k2 = -15 * j;
                                count = GridPowerDate.Where(x => x.date_time >= starttime.AddMinutes(k2) && x.date_time < starttime.AddMinutes(k1)).ToList();
                            }
                            double g1 = count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            Grid1 += Math.Round(gs1 < 0 ? 0 : gs1, 2) + ",";
                            sum1.Add(g1);
                        }
                        else//第一筆資料
                        {
                            int j = 0;
                            while (count.Count < 1)
                            {
                                int k1 = -15 * j;
                                j++;
                                int k2 = -15 * j;
                                count = GridPowerDate.Where(x => x.date_time >= starttime.AddMinutes(k2) && x.date_time < starttime.AddMinutes(k1)).ToList();
                            }
                            double g1 = count.Average(x => x.kWHt);
                            sum1.Add(g1);
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                    GridPowerDate.Clear();
                    //組圖表資料
                    TempData["Grid1"] = Grid1;
                    #endregion GridPowerChart
                    break;
                case "Inverters":
                    #region InvertersChart
                    //資料
                    Data = null;
                    sum.Clear();
                    var InvertersDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
                    for (int i = 0; i < 96; i++)
                    {
                        var count = InvertersDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddMinutes(15)).ToList();
                        Data += string.Format("{0:N2},",
                            (count.Count == 0) ? 0 :
                            (count.Average(x => x.ParallelInformation_TotalOutputActivePower
                            .Split('|').ToList()
                            .Sum(y => y.IsEmpty() ? 0 : Convert.ToDouble(y) / 1000.0)))).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    InvertersDate.Clear();
                    //組圖表資料
                    TempData["InverterData"] = Data;
                    #endregion Chart
                    break;
                case "Solar":
                    #region SolarChart
                    //資料
                    Data = null;
                    string sun0 = null, sun1 = null;
                    sum.Clear();
                    var SolarDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
                    for (int i = 0; i < 96; i++)
                    {
                        var count = SolarDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddMinutes(15)).ToList();
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
                    SolarDate.Clear();
                    //組圖表資料
                    TempData["Sun0"] = sun0;
                    TempData["Sun1"] = sun1;
                    #endregion  SolarChart
                    break;
                case "Battery":
                    #region BatteryChart
                    //資料
                    Data = null;
                    var BatteryDate = BatteryService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
                    for (int i = 0; i < 96; i++)
                    {
                        var count = BatteryDate.Where(x => x.updateTime >= starttime && x.updateTime < starttime.AddMinutes(15)).ToList();
                        List<double> batteryVolt = new List<double>();
                        List<double> batteryTotalVolt = new List<double>();
                        if (count.Count() > 0)
                        {
                            int c = 0;
                            foreach (var B in count)
                            {
                                if (c < 4)
                                {
                                    batteryVolt.Add(B.voltage);
                                    c++;
                                    if (c == 4)
                                    {
                                        batteryTotalVolt.Add((batteryVolt.Average() - 42) / (58 - 42));
                                        c = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            batteryTotalVolt.Add(0);
                        }
                        var TotalVolt = batteryTotalVolt.Average() == 0 ? 0 : batteryTotalVolt.Average() * 100.00;
                        Data += string.Format("{0:N2},", TotalVolt).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    BatteryDate.Clear();
                    //組圖表資料
                    TempData["BatteryData"] = Data;
                    #endregion BatteryChart
                    break;
                case "Load":
                    #region LoadChart
                    //資料       
                    Data = null;
                    string Load1 = null;
                    sum1.Clear();
                    sum2.Clear();
                    starttime = starttime.AddMinutes(-15);
                    var LoadDate = LoadPowerService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.index == 2).ToList();     //負載迴路一
                    for (int i = 0; i <= 96; i++)
                    {
                        var count = LoadDate.Where(x => x.date_Time >= starttime && x.date_Time < starttime.AddMinutes(15)).ToList();
                        if (i > 0)
                        {
                            //負載迴路一
                            int j = 0;
                            while (count.Count < 1)
                            {
                                int k1 = -15 * j;
                                j++;
                                int k2 = -15 * j;
                                count = LoadDate.Where(x => x.date_Time >= starttime.AddMinutes(k2) && x.date_Time < starttime.AddMinutes(k1)).ToList();
                            }
                            double g1 = count.Average(x => x.kWHt);
                            double gs1 = g1 - sum1.Last();
                            Load1 += Math.Round(gs1 < 0 ? 0 : gs1, 2) + ",";
                            sum1.Add(g1);
                        }
                        else
                        {
                            //負載迴路一
                            int j = 1;
                            while (count.Count < 1)
                            {
                                int k1 = -15 * j;
                                j++;
                                int k2 = -15 * j;
                                count = LoadDate.Where(x => x.date_Time >= starttime.AddMinutes(k2) && x.date_Time < starttime.AddMinutes(k1)).ToList();
                            }
                            double g1 = count.Average(x => x.kWHt);
                            sum1.Add(g1);
                        }
                        starttime = starttime.AddMinutes(15);
                    }
                    LoadDate.Clear();
                    //組圖表資料
                    TempData["Load1"] = Load1;
                    #endregion LoadChart
                    break;
                case "Generator":
                    #region GeneratorChart
                    //資料
                    Data = null;
                    string Generqtor1 = null;
                    sum1.Clear();
                    var GeneratorDate = GeneratorService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
                    for (int i = 0; i <= 96; i++)
                    {
                        var count = GeneratorDate.Where(x => x.UpdateTime >= starttime && x.UpdateTime < starttime.AddMinutes(15) && x.index==0).ToList();
                        if (i > 0)
                        {
                            if (count.Count > 0)
                            {
                                double g1 = count.Average(x => x.positiveKWhours) / 1000.00;
                                Generqtor1 += Math.Round((count.Average(x => x.positiveKWhours) / 1000.0) - sum1.Last(), 2) + ",";
                                sum1.Add(g1);
                            }
                            else
                            {
                                Generqtor1 += 0 + ",";
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
                    GeneratorDate.Clear();
                    //組圖表資料
                    TempData["Generator1"] = Generqtor1;
                    #endregion GeneratorChart
                    break;
                default:
                    break;
            }

            //Right View
            double soc = BatteryService.totalSOC() * 20.0;
            double LoadWatt = LoadPowerService.ReadNow().Watt_t;
            //可用總電量(度) = SOC(%) *20kWh(額定容量)
            ViewBag.Demand = string.Format("{0:#,0} kWh", soc);
            //可用電時數(H) = 可用總電量(度) / 負載實功率(kW)
            ViewBag.RemainTime = LoadWatt <= 0 ? "無負載" : string.Format("{0:#,0.0} 小時", (soc / LoadWatt));
        }
        #endregion

        #region Abnormal
        /// <summary>
        /// 異常警示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Abnormal(int page = 1)
        {
            TagTitle();
            NavButtom("Abnormal");
            StationList();
            AlartTypeList();

            List<Alart> alartList = alartService.ReadAll().OrderByDescending(x => x.StartTimet).ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = alartList.ToPagedList(currentPage, PageSizes());

            return View(result);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Abnormal(FormCollection From, int page = 1)
        {
            TagTitle();
            NavButtom("Abnormal");
            StationList();
            AlartTypeList();

            var datarange = Request.Form["range_date"];
            var stationID = Guid.Parse(Request.Form["Statons"]);
            var alarttypeID = Guid.Parse(Request.Form["AlartTypes"]);
            //分割字串
            char x = '-';
            string[] Day = datarange.Split(x);
            //區間
            DateTime Start = Convert.ToDateTime(Day[0]);
            DateTime End = Convert.ToDateTime(Day[1]);

            List<Alart> alartList = new List<Alart>();

            alartList = alartService.ReadListBy(Start, End, alarttypeID, stationID).OrderByDescending(y => y.StartTimet).ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = alartList.ToPagedList(currentPage, PageSizes());

            return View(result);
        }

        [Authorize]
        public ActionResult CreateAbnormal()
        {
            TagTitle();
            StationList();
            AlartTypeList();
            OrginTypeList();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAbnormal(FormCollection From)
        {
            string AlartContext = From["AlartContext"].Trim();
            Guid StationID = Guid.Parse(From["StationID"]);
            Guid AlartTypeID = Guid.Parse(From["AlartTypeID"]);
            Guid AccountID = accountService.ReadByName(Session["UserName"].ToString()).Id;

            Alart alart = new Alart()
            {
                AlartTypeID = AlartTypeID,
                StationID = StationID,
                AlartContext = AlartContext
            };

            Guid AlartID = alartService.Create(alart);

            return RedirectToAction("Maintain", "Tab");
        }
        #endregion

        #region History
        /// <summary>
        /// 歷史資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult History(string connStr, int? page)
        {
            string tabType = "GridPower";
            string sDay = null;
            string eDay = null;

            if (connStr != null)
            {
                string[] str = connStr.Trim().Split('+');
                tabType = str[0];
                if (!string.IsNullOrEmpty(str[1]) || !string.IsNullOrEmpty(str[2]))
                {
                    sDay = str[1];
                    eDay = str[2];
                }
            }

            TagTitle();
            NavButtom("History");
            StationList();
            AlartTypeList();
            Pills();

            TabAction(tabType);

            List<ESSObject> List = new List<ESSObject>();

            if (sDay == null || eDay == null)
            {
                ViewBag.startDay = DateTime.Now.AddDays(-1);
                ViewBag.endDay = DateTime.Now;
                //取得此時間點後1日的資料
                List = ESSObjecterService.ReadTimeInterval(DateTime.Now.AddDays(-1).AddHours(-8), DateTime.Now.AddHours(-8)).ToList();

            }
            else
            {
                ViewBag.startDay = DateTime.Parse(sDay);
                ViewBag.endDay = DateTime.Parse(eDay);
                //取得資料
                List = ESSObjecterService.ReadTimeInterval(DateTime.Parse(sDay).AddHours(-8), DateTime.Parse(eDay).AddHours(-8)).ToList();

            }

            ViewBag.Count = List.Count();
            ViewBag.StationName = "所有站別";
            int currentPage = page ?? 1;
            var result = List.ToPagedList(currentPage, 10);
            return View(result);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult History(FormCollection From, string tabType, int? page)
        {
            string date = Request.Form["datetimes"];
            Guid StationID = Guid.Parse(Request.Form["Statons"]);
            string[] data = date.ToString().Trim().Split('-');
            DateTime startday = Convert.ToDateTime(data[0]);
            DateTime endday = Convert.ToDateTime(data[1]);

            ViewBag.startDay = startday;
            ViewBag.endDay = endday;

            TagTitle();
            NavButtom("History");
            StationList();
            AlartTypeList();
            Pills();
            TabAction(tabType);

            List<ESSObject> List = new List<ESSObject>();
            if (stationService.ReadID(StationID).UUID.Equals(Guid.Empty))
            {
                List = ESSObjecterService.ReadTimeInterval(startday.AddHours(-8), endday.AddHours(-8)).ToList();
                ViewBag.StationName = "所有站別";
            }
            else
            {
                var Station = stationService.ReadID(StationID);
                List = ESSObjecterService.ReadTimeIntervalStation(startday.AddHours(-8), endday.AddHours(-8), Station.UUID.ToString()).ToList();
                ViewBag.StationName = Station.StationName;
            }
            ViewBag.Count = List.Count();

            int currentPage = page ?? 1;
            var result = List.ToPagedList(currentPage, PageSizes());
            return View(result);
        }

        #region Excel   
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult XLSX(FormCollection From)
        {
            try
            {
                var radio = Request.Form["inlineRadio"];
                string tabType = Request.Form["tabType"].Trim();
                DateTime startDay = Convert.ToDateTime(Request.Form["startDay"] + " " + Request.Form["startTime"]);
                DateTime endDay = Convert.ToDateTime(Request.Form["endDay"] + " " + Request.Form["endTime"]);

                if (radio.Equals("option1"))
                {
                    startDay = DateTime.Today;
                    endDay = DateTime.Now;
                }
                else if (radio.Equals("option2"))
                {
                    startDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDay = DateTime.Now;
                }

                string connStr = tabType + "+" + startDay + "+" + endDay;
                string reportName = tabType + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";

                List<ESSObject> data = ESSObjecterService.ReadTimeInterval(startDay, endDay);
                if (data.Count != 0)
                {
                    try
                    {
                        //ExcelPackage//https://blog.csdn.net/accountwcx/article/details/8144970
                        using (ExcelPackage excel = new ExcelPackage())
                        {
                            //加入 excel 工作表名為 `tabType`
                            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(tabType);
                            int colIdx = 1;
                            int rowIdx = 2;
                            int conlumnIndex = 1;
                            #region 選擇匯出類型
                            switch (tabType.Trim())
                            {
                                case "GridPower":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "實功率(kW)";
                                    sheet.Cells[1, colIdx++].Value = "虛功率(kVAR)";
                                    sheet.Cells[1, colIdx++].Value = "視在功率(kVA)";
                                    sheet.Cells[1, colIdx++].Value = "功因(PF)";
                                    sheet.Cells[1, colIdx++].Value = "頻率(Hz)";
                                    sheet.Cells[1, colIdx++].Value = "用電量(度)";
                                    rowIdx = 2;                //資料起始列位置       
                                    conlumnIndex = 1;   //每筆資料欄位起始位置
                                    foreach (var gps in data)
                                    {
                                        int count = 1;
                                        if (!string.IsNullOrEmpty(gps.GridPowerIDs))
                                        {
                                            DateTime time = gps.UpdateDate.AddHours(8);
                                            DateTime BaseTime = new DateTime(time.Year, time.Month, time.Day);
                                            string Timer = null;
                                            double T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0, T8 = 0;
                                            string[] IDs = gps.GridPowerIDs?.Trim().Split('|');
                                            if (IDs != null)
                                            {
                                                foreach (var gp in IDs)
                                                {
                                                    if (!gp.Trim().IsEmpty())
                                                    {
                                                        Guid ID = Guid.Parse(gp);
                                                        GridPower gridPowers = GridPowerService.ReadByID(ID);
                                                        if (gridPowers != null && gridPowers.index == 0)
                                                        {
                                                            Timer = gps.UpdateDate.AddHours(8).ToString();
                                                            T1 += gridPowers.Vavg;
                                                            T2 += gridPowers.Isum;
                                                            T3 += gridPowers.Watt_t / 1000.0;
                                                            T4 += gridPowers.Var_t / 1000.00;
                                                            T5 += gridPowers.VA_t / 1000.00;
                                                            T6 += gridPowers.PF_t;
                                                            T7 += gridPowers.Frequency;
                                                            T8 += gridPowers.MinuskWHt;
                                                            count++;
                                                        }
                                                    }
                                                }
                                                sheet.Cells[rowIdx, 1].Value = Timer.ToString();
                                                sheet.Cells[rowIdx, 2].Value = Math.Round(T1 / count, 2);
                                                sheet.Cells[rowIdx, 3].Value = Math.Round(T2, 2);
                                                sheet.Cells[rowIdx, 4].Value = Math.Round(T3, 2);
                                                sheet.Cells[rowIdx, 5].Value = Math.Round(T4, 2);
                                                sheet.Cells[rowIdx, 6].Value = Math.Round(T5, 2);
                                                sheet.Cells[rowIdx, 7].Value = Math.Round(T6 / count, 2);
                                                sheet.Cells[rowIdx, 8].Value = Math.Round(T7 / count, 2);
                                                sheet.Cells[rowIdx, 9].Value = Math.Round(T8, 2);
                                                conlumnIndex++;
                                                rowIdx++;
                                            }
                                        }
                                    }
                                    for (int i = 1; i <= 9; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                case "Inverters":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "工作模式";
                                    sheet.Cells[1, colIdx++].Value = "市電電壓  (V)";
                                    sheet.Cells[1, colIdx++].Value = "市電頻率  (Hz)";
                                    sheet.Cells[1, colIdx++].Value = "輸出電壓  (V)";
                                    sheet.Cells[1, colIdx++].Value = "輸出頻率  (Hz)";
                                    sheet.Cells[1, colIdx++].Value = "總輸出實功率(kW)";
                                    sheet.Cells[1, colIdx++].Value = "電池電壓  (V)";
                                    sheet.Cells[1, colIdx++].Value = "電池容量  (%)";
                                    sheet.Cells[1, colIdx++].Value = "太陽能電壓  (V)";
                                    sheet.Cells[1, colIdx++].Value = "總充電電流  (A)";
                                    rowIdx = 2;
                                    conlumnIndex = 1;
                                    foreach (var invs in data)
                                    {
                                        if (!string.IsNullOrEmpty(invs.InvertersIDs))
                                        {
                                            string Timer = null, T1 = null;
                                            double T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0, T8 = 0, T9 = 0, T10 = 0;
                                            string[] IDs = invs.InvertersIDs?.Trim().Split('|');
                                            if (IDs != null)
                                            {
                                                foreach (var inv in IDs)
                                                {
                                                    if (!inv.Trim().IsEmpty())
                                                    {
                                                        Timer = invs.UpdateDate.AddHours(8).ToString(); ;
                                                        Guid ID = Guid.Parse(inv);
                                                        Inverter inverter = InverterService.ReadByID(ID);
                                                        if (inverter != null)
                                                        {
                                                            string mod = inverter.DeviceMode.Trim();
                                                            if (mod == "P") { mod = "Power On Mode"; }
                                                            else if (mod == "S") { mod = "Standby Mode"; }
                                                            else if (mod == "L") { mod = "Line Mode"; }
                                                            else if (mod == "B") { mod = "Battery Mode"; }
                                                            else if (mod == "F") { mod = "Fault Mode"; }
                                                            else if (mod == "H") { mod = "Power Saving Mode"; }
                                                            else { mod = "Unknown Mode"; }
                                                            T1 += mod;
                                                            T2 += inverter.GridVoltage;
                                                            T3 += inverter.GridFrequency;
                                                            T4 += inverter.AC_OutputVoltage;
                                                            T5 += inverter.AC_OutputFrequency;
                                                            var toc = inverter.ParallelInformation_TotalOutputActivePower.Trim().Split('|');
                                                            int i = 1;
                                                            foreach (var d in toc)
                                                            {
                                                                if (!d.IsEmpty())
                                                                {
                                                                    T6 += Convert.ToDouble(d) / 1000.0;
                                                                    i++;
                                                                }
                                                            }
                                                            T7 += inverter.BatteryVoltage;
                                                            T8 += inverter.BatteryCapacity;
                                                            T9 += inverter.PV_InputVoltage;

                                                            var tcc = inverter.ParallelInformation_TotalChargingCurrent.Trim().Split('|');
                                                            int j = 1;
                                                            foreach (var d in tcc)
                                                            {
                                                                if (!d.IsEmpty())
                                                                {
                                                                    T10 += Convert.ToDouble(d);
                                                                    j++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                sheet.Cells[rowIdx, 1].Value = Timer;
                                                sheet.Cells[rowIdx, 2].Value = T1;
                                                sheet.Cells[rowIdx, 3].Value = Math.Round(T2, 2);
                                                sheet.Cells[rowIdx, 4].Value = Math.Round(T3, 2);
                                                sheet.Cells[rowIdx, 5].Value = Math.Round(T4, 2);
                                                sheet.Cells[rowIdx, 6].Value = Math.Round(T5, 2);
                                                sheet.Cells[rowIdx, 7].Value = Math.Round(T6, 2);
                                                sheet.Cells[rowIdx, 8].Value = Math.Round(T7, 2);
                                                sheet.Cells[rowIdx, 9].Value = Math.Round(T8, 2);
                                                sheet.Cells[rowIdx, 10].Value = Math.Round(T9, 2);
                                                sheet.Cells[rowIdx, 11].Value = Math.Round(T10, 2);
                                                conlumnIndex++;
                                                rowIdx++;
                                            }
                                        }
                                    }
                                    for (int i = 1; i <= 11; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                case "Solar":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "太陽能編號";
                                    sheet.Cells[1, colIdx++].Value = "電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "功率(kW)";
                                    sheet.Cells[1, colIdx++].Value = "發電量(度)";
                                    colIdx++;
                                    foreach (var invs in data)
                                    {
                                        DateTime time = invs.UpdateDate.AddHours(8);
                                        DateTime BaseTime = new DateTime(time.Year, time.Month, time.Day);
                                        if (!string.IsNullOrEmpty(invs.InvertersIDs))
                                        {
                                            string[] IDs = invs.InvertersIDs.Trim().Split('|');
                                            if (IDs != null)
                                            {
                                                foreach (var inv in IDs)
                                                {
                                                    if (!inv.Trim().IsEmpty())
                                                    {
                                                        Guid ID = Guid.Parse(inv);
                                                        Inverter inverter = InverterService.ReadByID(ID);
                                                        if (inverter != null)
                                                        {
                                                            var SolarID = inverter.SPMid.Split('|').ToList();
                                                            var volt = inverter.SPM90Voltage.Split('|').ToList();
                                                            var curent = inverter.SPM90Current.Split('|').ToList();
                                                            var activePower = inverter.SPM90ActivePower.Split('|').ToList();
                                                            for (int k = 0; k < SolarID.Count() - 1; k++)
                                                            {
                                                                sheet.Cells[rowIdx, 1].Value = time.ToString();
                                                                sheet.Cells[rowIdx, 2].Value = SolarID[k];
                                                                sheet.Cells[rowIdx, 3].Value = volt[k];
                                                                sheet.Cells[rowIdx, 4].Value = curent[k];
                                                                sheet.Cells[rowIdx, 5].Value = Math.Round(Convert.ToDouble(activePower[k]) / 1000.0, 2);
                                                                sheet.Cells[rowIdx, 6].Value = Math.Round(k == 0 ? inverter.SPM90ActiveEnergyMinus1 : inverter.SPM90ActiveEnergyMinus2, 2);
                                                                conlumnIndex++;
                                                                rowIdx++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    for (int i = 1; i <= 6; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                case "Battery":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "電池編號";
                                    sheet.Cells[1, colIdx++].Value = "電池電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "充電電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "放電電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "電池容量(%)";
                                    sheet.Cells[1, colIdx++].Value = "充電次數(次)";
                                    sheet.Cells[1, colIdx++].Value = "充電方向";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index1_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index2_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index3_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index4_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index5_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index6_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index7_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index8_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index9_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index10_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index11_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index12_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index13_Voltage";
                                    sheet.Cells[1, colIdx++].Value = "Cell_Index14_Voltage";
                                    rowIdx = 2;
                                    conlumnIndex = 1;
                                    List<string> IDList = new List<string>();
                                    foreach (var bas in data)
                                    {
                                        if (!string.IsNullOrEmpty(bas.BatteryIDs))
                                        {
                                            string[] IDs = bas.BatteryIDs?.Trim().Split('|');
                                            if (IDs != null)
                                            {
                                                foreach (var ba in IDs)
                                                {
                                                    if (!ba.Trim().IsEmpty())
                                                    {
                                                        Guid ID = Guid.Parse(ba);
                                                        Battery battery = BatteryService.ReadByID(ID);
                                                        if (battery != null)
                                                        {
                                                            int cd = Convert.ToInt32(battery.charge_direction);
                                                            IDList.Add(ba.Trim());
                                                            string Timer = bas.UpdateDate.AddHours(8).ToString();
                                                            double Item = Math.Round(battery.index, 0);
                                                            double T1 = Math.Round(battery.voltage, 2);
                                                            double T2 = Math.Round(battery.charging_current, 2);
                                                            double T3 = Math.Round(battery.discharging_current, 2);
                                                            double T4 = Math.Round(BatteryService.EachSOC(battery.voltage), 2);
                                                            double T5 = Math.Round(battery.Cycle, 0);
                                                            string T6 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                                                            var volta = battery.cells_voltage.Split('|').ToList();
                                                            sheet.Cells[rowIdx, 1].Value = Timer;
                                                            sheet.Cells[rowIdx, 2].Value = Item;
                                                            sheet.Cells[rowIdx, 3].Value = T1;
                                                            sheet.Cells[rowIdx, 4].Value = T2;
                                                            sheet.Cells[rowIdx, 5].Value = T3;
                                                            sheet.Cells[rowIdx, 6].Value = T4;
                                                            sheet.Cells[rowIdx, 7].Value = T5;
                                                            sheet.Cells[rowIdx, 8].Value = T6;
                                                            int v = 9;
                                                            foreach (var c in volta)
                                                            {
                                                                if (!c.IsEmpty())
                                                                {
                                                                    sheet.Cells[rowIdx, v].Value = c;
                                                                    v++;
                                                                }
                                                            }
                                                            conlumnIndex++;
                                                            rowIdx++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    for (int i = 1; i <= 22; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                case "Load":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "負載編號";
                                    sheet.Cells[1, colIdx++].Value = "電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "實功率(kW)";
                                    sheet.Cells[1, colIdx++].Value = "虛功率(kVAR)";
                                    sheet.Cells[1, colIdx++].Value = "視在功率(kVA)";
                                    sheet.Cells[1, colIdx++].Value = "功因(PF)";
                                    sheet.Cells[1, colIdx++].Value = "頻率(Hz)";
                                    sheet.Cells[1, colIdx++].Value = "用電量(度)";
                                    rowIdx = 2;
                                    conlumnIndex = 1;
                                    foreach (var los in data)
                                    {
                                        if (!string.IsNullOrEmpty(los.LoadPowerIDs))
                                        {
                                            DateTime time = los.UpdateDate.AddHours(8);
                                            DateTime BaseTime = new DateTime(time.Year, time.Month, time.Day);
                                            string Timer = null, Loadname = null;
                                            double T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0, T8 = 0;
                                            string[] IDs = los.LoadPowerIDs?.Trim().Split('|');
                                            if (IDs != null)
                                            {
                                                foreach (var lo in IDs)
                                                {
                                                    if (!lo.Trim().IsEmpty())
                                                    {
                                                        Guid ID = Guid.Parse(lo);
                                                        LoadPower loadPower = LoadPowerService.ReadByID(ID);
                                                        if (loadPower != null && loadPower.index == 2)
                                                        {
                                                            Timer = loadPower.date_Time.AddHours(8).ToString();
                                                            Loadname = loadPower.name;
                                                            T1 = Math.Round(loadPower.Vavg, 2);
                                                            T2 = Math.Round(loadPower.Isum, 2);
                                                            T3 = Math.Round(loadPower.Watt_t / 1000.0, 2);
                                                            T4 = Math.Round(loadPower.Var_t / 1000.00, 2);
                                                            T5 = Math.Round(loadPower.VA_t / 1000.00, 2);
                                                            T6 = Math.Round(loadPower.PF_t, 2);
                                                            T7 = Math.Round(loadPower.Frequency, 2);
                                                            T8 = Math.Round(loadPower.MinuskWHt, 2);

                                                            sheet.Cells[rowIdx, 1].Value = Timer;
                                                            sheet.Cells[rowIdx, 2].Value = Loadname;
                                                            sheet.Cells[rowIdx, 3].Value = T1;
                                                            sheet.Cells[rowIdx, 4].Value = T2;
                                                            sheet.Cells[rowIdx, 5].Value = T3;
                                                            sheet.Cells[rowIdx, 6].Value = T4;
                                                            sheet.Cells[rowIdx, 7].Value = T5;
                                                            sheet.Cells[rowIdx, 8].Value = T6;
                                                            sheet.Cells[rowIdx, 9].Value = T7;
                                                            sheet.Cells[rowIdx, 10].Value = T8;
                                                            conlumnIndex++;
                                                            rowIdx++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    for (int i = 1; i <= 10; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                case "Generator":
                                    colIdx = 1;
                                    sheet.Cells[1, colIdx++].Value = "資料時間";
                                    sheet.Cells[1, colIdx++].Value = "發電機油位(%)";
                                    sheet.Cells[1, colIdx++].Value = "L1-N相電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "L2-N相電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "L3-N相電壓(V)";
                                    sheet.Cells[1, colIdx++].Value = "L1相電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "L2相電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "L3相電流(A)";
                                    sheet.Cells[1, colIdx++].Value = "總實功率(kW)";
                                    sheet.Cells[1, colIdx++].Value = "平均功率因數";
                                    sheet.Cells[1, colIdx++].Value = "正的千瓦時(度)";
                                    sheet.Cells[1, colIdx++].Value = "負的千瓦時(度)";
                                    sheet.Cells[1, colIdx++].Value = "發電機 狀態";
                                    sheet.Cells[1, colIdx++].Value = "可用總電量(度)";
                                    sheet.Cells[1, colIdx++].Value = "可用電時數(H)";
                                    rowIdx = 2;
                                    conlumnIndex = 1;
                                    foreach (var gens in data)
                                    {
                                        DateTime time = gens.UpdateDate.AddHours(8);
                                        DateTime BaseTime = new DateTime(time.Year, time.Month, time.Day);
                                        string Timer = null;
                                        double T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0, T8 = 0, T9 = 0, T10 = 0, T11 = 0, T13 = 0, T14 = 0;
                                        string T12 = "離線";
                                        string[] IDs = gens.GeneratorIDs?.Trim().Split('|');
                                        if (!string.IsNullOrEmpty(gens.GeneratorIDs))
                                        {
                                            foreach (var gen in IDs)
                                            {
                                                if (!string.IsNullOrEmpty(gen.Trim()))
                                                {
                                                    Guid ID = Guid.Parse(gen);
                                                    Generator generator = GeneratorService.ReadByID(ID);
                                                    Timer = time.ToString();
                                                    T1 = Math.Round(generator.FuleLevel, 2);
                                                    T2 = Math.Round(generator.L1Nvoltage, 2);
                                                    T3 = Math.Round(generator.L2Nvoltage, 2);
                                                    T4 = Math.Round(generator.L3Nvoltage, 2);
                                                    T5 = Math.Round(generator.L1current, 2);
                                                    T6 = Math.Round(generator.L2current, 2);
                                                    T7 = Math.Round(generator.L3current, 2);
                                                    T8 = Math.Round(generator.totalwatts / 1000.0, 2);
                                                    T9 = Math.Round(generator.averagepowerfactor, 2);
                                                    T10 = Math.Round(generator.positiveKWhours, 2);
                                                    T11 = Math.Round(generator.negativeKWhours, 2);
                                                    T12 = generator.ControlStatus.Equals("true") ? "啟動" : "關閉";
                                                    T13 = Math.Round(generator.AvailabilityEnergy, 2);
                                                    T14 = Math.Round(generator.AvailabilityHour, 2);
                                                }
                                            }
                                            sheet.Cells[rowIdx, 1].Value = time.ToString(); ;
                                            sheet.Cells[rowIdx, 2].Value = T1;
                                            sheet.Cells[rowIdx, 3].Value = T2;
                                            sheet.Cells[rowIdx, 4].Value = T3;
                                            sheet.Cells[rowIdx, 5].Value = T4;
                                            sheet.Cells[rowIdx, 6].Value = T5;
                                            sheet.Cells[rowIdx, 7].Value = T6;
                                            sheet.Cells[rowIdx, 8].Value = T7;
                                            sheet.Cells[rowIdx, 9].Value = T8;
                                            sheet.Cells[rowIdx, 10].Value = T9;
                                            sheet.Cells[rowIdx, 11].Value = T10;
                                            sheet.Cells[rowIdx, 12].Value = T11;
                                            sheet.Cells[rowIdx, 13].Value = T12;
                                            sheet.Cells[rowIdx, 14].Value = T13;
                                            sheet.Cells[rowIdx, 15].Value = T14;
                                            conlumnIndex++;
                                            rowIdx++;
                                        }
                                        else
                                        {
                                            sheet.Cells[rowIdx, 1].Value = time.ToString(); ;
                                            sheet.Cells[rowIdx, 2].Value = T1;
                                            sheet.Cells[rowIdx, 3].Value = T2;
                                            sheet.Cells[rowIdx, 4].Value = T3;
                                            sheet.Cells[rowIdx, 5].Value = T4;
                                            sheet.Cells[rowIdx, 6].Value = T5;
                                            sheet.Cells[rowIdx, 7].Value = T6;
                                            sheet.Cells[rowIdx, 8].Value = T7;
                                            sheet.Cells[rowIdx, 9].Value = T8;
                                            sheet.Cells[rowIdx, 10].Value = T9;
                                            sheet.Cells[rowIdx, 11].Value = T10;
                                            sheet.Cells[rowIdx, 12].Value = T11;
                                            sheet.Cells[rowIdx, 13].Value = T12;
                                            sheet.Cells[rowIdx, 14].Value = T13;
                                            sheet.Cells[rowIdx, 15].Value = T14;
                                            conlumnIndex++;
                                            rowIdx++;
                                        }
                                    }
                                    for (int i = 1; i <= 15; i++)
                                    {
                                        sheet.Column(i).AutoFit();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            #endregion

                            using (MemoryStream ms = new MemoryStream())
                            {
                                Response.Buffer = true;
                                Response.Clear();
                                ms.Position = 0;//不重新將位置設為0，excel開啟後會出現錯誤
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlEncode(reportName)));
                                //寫入資料
                                excel.SaveAs(ms);
                                ms.WriteTo(Response.OutputStream);
                                excel.Dispose();
                                Response.Flush();
                                Response.End();
                            }
                        }

                        //TempData["message"] = "匯出" + reportName ;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        TempData["message"] = "匯出失敗";
                    }
                }
                else
                {
                    TempData["message"] = "無資料可供匯出";
                }
                return RedirectToAction("History", "Tab", new { connStr });
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                return RedirectToAction("History", "Tab");
            }
        }
        #endregion Excel

        private void TabAction(string tabType)
        {
            ViewBag.tabGridPower = "tab-pane";
            ViewBag.tabLoad = "tab-pane";
            ViewBag.tabSolar = "tab-pane";
            ViewBag.tabGenerator = "tab-pane";
            ViewBag.tabInverters = "tab-pane";
            ViewBag.tabBattery = "tab-pane";

            ViewBag.navGridPower = "nav-link";
            ViewBag.navLoad = "nav-link";
            ViewBag.navSolar = "nav-link";
            ViewBag.navGenerator = "nav-link";
            ViewBag.navInverters = "nav-link";
            ViewBag.navBattery = "nav-link";

            ViewBag.onTab = "GridPower";

            if (!string.IsNullOrEmpty(tabType))
            {
                tabType = tabType.Trim();
                switch (tabType)
                {
                    case "GridPower":
                        ViewBag.tabGridPower = "tab-pane active";
                        ViewBag.navGridPower = "nav-link active";
                        ViewBag.onTab = tabType;
                        break;
                    case "Load":
                        ViewBag.tabLoad = "tab-pane active";
                        ViewBag.navLoad = "nav-link active";
                        ViewBag.onTab = tabType;

                        break;
                    case "Solar":
                        ViewBag.tabSolar = "tab-pane active";
                        ViewBag.navSolar = "nav-link active";
                        ViewBag.onTab = tabType;
                        break;
                    case "Generator":
                        ViewBag.tabGenerator = "tab-pane active";
                        ViewBag.navGenerator = "nav-link active";
                        ViewBag.onTab = tabType;
                        break;
                    case "Inverters":
                        ViewBag.tabInverters = "tab-pane active";
                        ViewBag.navInverters = "nav-link active";
                        ViewBag.onTab = tabType;
                        break;
                    case "Battery":
                        ViewBag.tabBattery = "tab-pane active";
                        ViewBag.navBattery = "nav-link active";
                        ViewBag.onTab = tabType;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ViewBag.tabGridPower = "tab-pane active";
                ViewBag.navGridPower = "nav-link active";
            }

        }
        #endregion

        #region  Maintain
        /// <summary>
        /// 維護管理
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Maintain()
        {
            TagTitle();
            NavButtom("Maintain");
            return View();
        }

        [Authorize]
        public ActionResult CreateStation()
        {
            TagTitle();
            NavButtom("Maintain");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStation(FormCollection From)
        {
            Station station = new Station() { StationName = From["StationName"].Trim() };
            Guid StationID = stationService.Create(station);
            return RedirectToAction("Maintain", "Tab");
        }

        public ActionResult ListStation(int page = 1)
        {
            var Station = stationService.ReadAll();
            int currentPage = page < 1 ? 1 : page;
            var result = Station.ToPagedList(currentPage, PageSizes());
            return View(result);
        }

        [Authorize]
        public ActionResult DeleteStation(Guid id)
        {
            Station station = stationService.ReadID(id);
            return View(station);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStation(FormCollection From)
        {
            Guid Id = Guid.Parse(From["id"].Trim());
            bool StationID = stationService.Delete(Id);
            return RedirectToAction("ListStation", "Tab", new { del = StationID });
        }

        [Authorize]
        public ActionResult EditStation(Guid id)
        {
            Station station = stationService.ReadID(id);
            return View(station);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStation(FormCollection From)
        {
            var id = Guid.Parse(From["id"].Trim());
            var UUID = Guid.Parse(From["UUID"].Trim());
            var StationName = From["StationName"].Trim();

            Station station = new Station()
            {
                Id = id,
                UUID = UUID,
                StationName = StationName
            };
            Guid ID = stationService.Update(station);
            return RedirectToAction("ListStation", "Tab");
        }



        [Authorize]
        public ActionResult CreateOrgin()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrgin(FormCollection From)
        {
            Orgin orgin = new Orgin() { OrginName = From["OrginName"].Trim() };
            Guid OrginID = orginService.Create(orgin);
            return RedirectToAction("Maintain", "Tab");
        }

        public ActionResult ListOrgin(int page = 1)
        {
            var Orgin = orginService.ReadAll();
            int currentPage = page < 1 ? 1 : page;
            var result = Orgin.ToPagedList(currentPage, PageSizes());
            return View(result);
        }

        [Authorize]
        public ActionResult DeleteOrgin(Guid id)
        {
            Orgin orgin = orginService.ReadID(id);
            return View(orgin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrgin(FormCollection From)
        {
            Guid Id = Guid.Parse(From["id"].Trim());
            bool OrginID = orginService.Delete(Id);
            return RedirectToAction("ListOrgin", "Tab", new { del = OrginID });
        }

        [Authorize]
        public ActionResult EditOrgin(Guid id)
        {
            Orgin orgin = orginService.ReadID(id);
            return View(orgin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrgin(FormCollection From)
        {
            var id = Guid.Parse(From["id"].Trim());
            var orginName = From["OrginName"].Trim();

            Orgin orgin = new Orgin()
            {
                Id = id,
                OrginName = orginName
            };
            Guid ID = orginService.Update(orgin);
            return RedirectToAction("ListOrgin", "Tab");
        }

        #endregion

        #region 參考程式
        /// <summary>
        /// 查Seesion登入資訊
        /// </summary>
        protected internal void TagTitle()
        {
            ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
            if (Session["UserName"] != null)
            {
                string un = Session["UserName"].ToString();
                ViewBag.User = string.IsNullOrEmpty(un) ? null : un;
            }
        }

        /// <summary>
        /// 資源檔取名稱
        /// </summary>
        private void Pills()
        {
            ViewBag.Index = Resources.Resource.Index;
            ViewBag.GridPower = Resources.Resource.GridPower;
            ViewBag.Battery = Resources.Resource.Battery;
            ViewBag.Solar = Resources.Resource.Solar;
            ViewBag.Load = Resources.Resource.Load;
            ViewBag.Generator = Resources.Resource.Generator;
            ViewBag.Inverters = Resources.Resource.Inverters;
            ViewBag.QRCode = "QRCode";
        }

        /// <summary>
        /// 更換Buttom的class
        /// </summary>
        /// <param name="type"></param>
        private void NavButtom(string type)
        {
            TempData["ButtomIndex"] = "btn btn-outline-success btn-lg";
            TempData["ButtomBulletin"] = "btn btn-outline-success btn-lg";
            TempData["ButtomInfo"] = "btn btn-outline-success btn-lg";
            TempData["ButtomAbnormal"] = "btn btn-outline-success btn-lg";
            TempData["Buttomhistory"] = "btn btn-outline-success btn-lg";
            TempData["ButtomMaintain"] = "btn btn-outline-success btn-lg";
            TempData["ButtomQRCode"] = "btn btn-outline-success btn-lg";
            type = type.Trim();
            switch (type)
            {
                case "Index":
                    TempData["ButtomIndex"] = "btn btn-success btn-lg";
                    break;
                case "Bulletin":
                    TempData["ButtomBulletin"] = "btn btn-success btn-lg";
                    break;
                case "Info":
                    TempData["ButtomInfo"] = "btn btn-success btn-lg";
                    break;
                case "Abnormal":
                    TempData["ButtomAbnormal"] = "btn btn-success btn-lg";
                    break;
                case "History":
                    TempData["Buttomhistory"] = "btn btn-success btn-lg";
                    break;
                case "Maintain":
                    TempData["ButtomMaintain"] = "btn btn-success btn-lg";
                    break;
                case "QRCode":
                    TempData["ButtomQRCode"] = "btn btn-success btn-lg";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Station下拉式選單
        /// </summary>
        private void StationList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<Station> stations = stationService.ReadAll().OrderBy(x => x.StationCode).ToList();
            stations.ForEach(x => {
                items.Add(new SelectListItem()
                {
                    Text = x.StationName,
                    Value = x.Id.ToString()
                });
            });
            ViewBag.station = items;
        }

        /// <summary>
        /// Alart下拉式選單
        /// </summary>
        private void AlartTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<AlartType> AT = alarttypeService.ReadAll().OrderBy(x => x.AlartTypeCode).ToList();
            AT.ForEach(x => {
                items.Add(new SelectListItem()
                {
                    Text = x.AlartTypeName,
                    Value = x.Id.ToString()
                });
            });
            ViewBag.alartType = items;
        }

        /// <summary>
        /// orgin下拉式選單
        /// </summary>
        private void OrginTypeList()
        {
            List<Orgin> List = orginService.ReadAll().ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            List.ForEach(x => {
                items.Add(new SelectListItem()
                {
                    Text = x.OrginName.ToString().Trim(),
                    Value = x.Id.ToString().Trim()
                });
            });
            ViewBag.Orgin = items;
        }

        /// <summary>
        /// 分割使用者資訊，回傳權限名稱
        /// </summary>
        /// <returns></returns>
        private string UserRoletype()
        {
            string[] type = User.Identity.Name.ToString().Trim().Split(',');
            return type[2];
        }

        /// <summary>
        /// 圖表假資料
        /// </summary>
        private void EchartDataMethod()
        {
            //echart    yData  
            Random y = new Random();
            string yData = null;
            yData = "[";
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 60; j = j + 15)
                {
                    Double r = Convert.ToInt32(y.Next(100, 10000)) / 100;
                    yData += string.Format("{0:N2},", r);
                }
            }
            yData += "]";
            ViewBag.yData = yData;
            //  ViewBag.yData = " [56.14, 81.34, 70.95, 97.54, 7.51, 5.13, 38.62, 25.51, 77.09, 79.83, 17.28, 72.83, 65.89, 52.68, 4.15, 57.90, 13.28, 22.44, 39.44, 39.90, 89.57, 49.56, 73.67, 91.20, 75.15, 97.19, 35.31, 64.70, 93.17, 81.14, 85.73, 36.10, 46.48, 77.66, 89.15, 71.21, 77.43, 69.93, 87.33, 89.30, 99.95, 27.03, 54.89, 11.24, 65.23, 59.47, 74.92, 89.18, 53.68, 98.31, 77.56, 14.75, 24.17, 87.86, 22.28, 22.91, 91.36, 34.32, 26.14, 20.06, 53.63, 70.88, 77.88, 34.02, 33.31, 98.60, 83.55, 22.29, 38.80, 0.34, 69.03, 46.56, 49.02]";
        }
        #endregion
    }
}