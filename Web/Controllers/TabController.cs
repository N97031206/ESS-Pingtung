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
        private int PageSizes(){ if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out int s)) { s = 10; } return s; }
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
            List<Bulletin> bulletins = bulletinService.ReadAll().ToList();
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
            var AccountID = accountService.ReadByName( Session["UserName"].ToString()).Id;

            Bulletin bulletins = new Bulletin()
            {
                title = title,
                context = context,
                OrginID = OrginID,
                AccountID = AccountID
            };

            Guid BulletinID=bulletinService.Create(bulletins);

            return RedirectToAction("Maintain", "Tab");
        }


        [Authorize]
        public ActionResult DisableBulletin()
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
        public ActionResult DisableBulletin(FormCollection From)
        {
            return View();
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

            tabType=string.IsNullOrEmpty(tabType) ? "GridPower" : tabType;
            InfoNav(tabType);
            ChartData(tabType);

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Info(FormCollection From,string tabType)
        {
            TagTitle();
            NavButtom("Info");
            Pills();
            StationList();

            tabType = string.IsNullOrEmpty(tabType) ? "GridPower" : tabType;
            InfoNav(tabType);
            ChartData(tabType);

            var staton = Guid.Parse(Request.Form["Staton"]);
            ViewBag.viewstation = stationService.ReadID(staton).StationName.ToString().Trim();

            return View();
        }

        /// <summary>
        /// 更換Nav class
        /// </summary>
        /// <param name="tabType"></param>
        private void InfoNav(string tabType)
        {
            TempData["nav"] = tabType;
            TempData["navGridPower"]= "nav-link";
            TempData["navBattery"] = "nav-link";
            TempData["navSolar"]= "nav-link";
            TempData["navLoad"]= "nav-link";
            TempData["navGenerator"]= "nav-link";
            TempData["navInverters"] = "nav-link";
            TempData["tabGridPower"] = "tab-pane";
            TempData["tabBattery"]= "tab-pane ";
            TempData["tabSolar"]= "tab-pane ";
            TempData["tabLoad"]= "tab-pane ";
            TempData["tabGenerator"]= "tab-pane ";
            TempData["tabInverters"]= "tab-pane ";

            switch (tabType.Trim())
            {
                case "GridPower":
                    TempData["navGridPower"] ="nav-link active" ;
                    TempData["tabGridPower"] = "tab-pane active";
                    break;
                case "Inverters":
                    TempData["navInverters"] = "nav-link active";
                    TempData["tabInverters"] ="tab-pane active" ;
                    break;
                case "Solar":
                    TempData["navSolar"] ="nav-link active";
                    TempData["tabSolar"] = "tab-pane active" ;
                    break;
                case "Battery":
                    TempData["navBattery"] = "nav-link active";
                    TempData["tabBattery"] =  "tab-pane active";
                    break;
                case "Load":
                    TempData["navLoad"] ="nav-link active";
                    TempData["tabLoad"] = "tab-pane active" ;
                    break;
                case "Generator":
                    TempData["navGenerator"] ="nav-link active";
                    TempData["tabGenerator"] = "tab-pane active" ;
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
            DateTime start = new DateTime();
            DateTime starttime = new DateTime();
            DateTime endTime = new DateTime();
            string Data = null;
            int min = 0;

            switch (tabType.Trim())
            {
                case "GridPower":
                    #region GridPowerChart
                    var now = GridPowerService.ReadNow();
                    endTime = GridPowerService.ReadNow().date_time;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; } else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料                
                    for (int i = 0; i < 96; i ++)
                    {
                        var count = GridPowerService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count == 0 ?0:count.Average(x => x.Vavg)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["GridPowerData"] = Data;
                    TempData["GridPowerhh"] = start.Hour;
                    TempData["GridPowermm"] = min;
                    #endregion GridPowerChart
                    break;
                case "Inverters":
                    #region InvertersChart
                    endTime = InverterService.ReadNow().UpdateTime;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; } else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料
                    for (int i = 0; i < 96; i++)
                    {
                        var count = InverterService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count==0?0:count.Average(x => x.ParallelInformation_TotalOutputActivePower)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["InverterData"] = Data;
                    TempData["Inverterhh"] = start.Hour;
                    TempData["Invertermm"] = min;
                    #endregion Chart
                    break;
                case "Solar":
                    #region SolarChart
                    endTime = InverterService.ReadNow().UpdateTime;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; } else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料
                    for (int i = 0; i < 96; i++)
                    {
                        var count = InverterService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count==0?0:count.Average(x => x.SPM90ActiveEnergy)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["SolarData"] = Data;
                    TempData["Solarhh"] = start.Hour;
                    TempData["Solarmm"] = min;
                    #endregion  SolarChart
                    break;
                case "Battery":
                    #region BatteryChart
                    endTime = BatteryService.ReadNow().updateTime;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; } else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料
                    for (int i = 0; i < 96; i++)
                    {
                        var count = BatteryService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count==0?0:count.Average(x => x.SOC)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["BatteryData"] = Data;
                    TempData["Batteryhh"] = start.Hour;
                    TempData["Batterymm"] = min;
                    #endregion BatteryChart
                    break;
                case "Load":
                    #region LoadChart
                    endTime = LoadPowerService.ReadNow().updateTime;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; }else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料
                    for (int i = 0; i < 96; i++)
                    {
                        var count = LoadPowerService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count == 0 ? 0: count.Average(x => x.Vavg)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["LoadData"] = Data;
                    TempData["Loadhh"] = start.Hour;
                    TempData["Loadmm"] = min;
                    #endregion LoadChart
                    break;
                case "Generator":
                    #region GeneratorChart
                    endTime = GeneratorService.ReadNow().UpdateTime;
                    start = endTime.AddMinutes(-1440);
                    min = Math.Abs(start.Minute / 15);
                    if (min.Equals(0)) { min = 0; } else if (min.Equals(1)) { min = 15; } else if (min.Equals(2)) { min = 30; } else if (min.Equals(3)) { min = 45; }else { min = 0; }
                    starttime = new DateTime(start.Year, start.Month, start.Day, start.Hour, min, 00, 00);
                    //資料
                    //count = GeneratorService.Count(starttime, endTime);
                    //for (int i = 0; i < count; i += 15)
                    //{
                    for (int i = 0; i < 96; i++)
                    {
                        var count = GeneratorService.ReadByInfoList(starttime, starttime.AddMinutes(15));
                        Data += string.Format("{0:N2},", count.Count == 0 ? 0 : count.Average(x => x.FuleLevel)).Trim();
                        starttime = starttime.AddMinutes(15);
                    }
                    //組圖表資料
                    TempData["GeneratorData"] = Data;
                    TempData["Generatorhh"] = start.Hour;
                    TempData["Generatormm"] = min;
                    #endregion GeneratorChart
                    break;
                default:
                    break;
            }
            ViewBag.Demand = "555.1";//可用總電量</div>
            ViewBag.RemainTime = "200";//可用電時數</div>
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

            List<Alart> alartList = alartService.ReadAll().ToList();
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

            alartList = alartService.ReadListBy(Start, End, alarttypeID, stationID).ToList();

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
                StationID= StationID,
                AlartContext= AlartContext
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
        public ActionResult History( string connStr ,int? page)
        {
            string tabType = "GridPower";
            string sDay = null;
            string eDay = null;

            if (connStr != null)
            {
                string[] str = connStr.Trim().Split('+');
                tabType = str[0];
                if (!string.IsNullOrEmpty(str[1] )|| !string.IsNullOrEmpty(str[2]))
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
                List = ESSObjecterService.ReadTimeInterval(DateTime.Now.AddDays(-1), DateTime.Now).ToList();
                ViewBag.Count = List.Count();
            }
            else
            {
                ViewBag.startDay = DateTime.Parse(sDay);
                ViewBag.endDay = DateTime.Parse(eDay);
                //取得資料
                List = ESSObjecterService.ReadTimeInterval(DateTime.Parse(sDay), DateTime.Parse(eDay)).ToList();
                ViewBag.Count = List.Count();
            }

            int currentPage = page ?? 1;
            var result = List.ToPagedList(currentPage, 10);
            return View(result);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult History(FormCollection From, string tabType, int? page)
        {
            var date= Request.Form["datetimes"];
            var station =  Request.Form["Statons"];
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

            List<ESSObject> List = ESSObjecterService.ReadTimeInterval(startday, endday).ToList();
            ViewBag.Count = List.Count();

            int currentPage = page ?? 1;
            var result = List.ToPagedList(currentPage, PageSizes());
            return View(result);
        }

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
            Station station = new Station(){StationName=From["StationName"].Trim()};
            Guid StationID = stationService.Create(station);
            return RedirectToAction("Maintain", "Tab");
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
            Orgin orgin = new Orgin() {OrginName= From["OrginName"].Trim()};
            Guid OrginID = orginService.Create(orgin);
            return RedirectToAction("Maintain", "Tab");
        }

        #endregion

        #region Excel   
        #region XLSX
        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="From"></param>
        /// <returns></returns>
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

                string reportPath = "C:\\DownLoads\\";
                string reportName = tabType + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
                //查資料夾有無建立
                bool exists = System.IO.Directory.Exists(reportPath);
                if (!exists) { System.IO.Directory.CreateDirectory(reportPath); }

                List<ESSObject> ESSList = ESSObjecterService.ReadTimeInterval(startDay, endDay);
                if (ESSList.Count!=0)
                {
                    Task.Run(() => {                     
                        //xlsx
                        var xlsx = Export(ESSList, tabType);
                        //存檔至指定位置
                        xlsx.SaveAs(reportPath + reportName);
                    });
                    TempData["message"] = "匯出"+ reportName+"中";
                    return RedirectToAction("History", "Tab", new { connStr });
                }
                TempData["message"] = "無資料可供匯出";
                return RedirectToAction("History", "Tab", new { connStr });
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                return RedirectToAction("History","Tab");
            }
        }

        //使用 C# 將資料匯出成 Excel (.xlsx)//https://blog.yowko.com/2018/04/list-to-excel.html
        /// <summary>
        /// 產生 excel
        /// </summary>
        /// <typeparam name="ESSObject">傳入的物件型別</typeparam>
        /// <param name="data">物件資料集</param>
        /// <returns></returns>
        public XLWorkbook Export(List<ESSObject> data, string tabType)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();

            //加入 excel 工作表名為 `tabType`
            var sheet = workbook.Worksheets.Add(tabType);
            int colIdx = 1;
            int rowIdx = 2;                
            int conlumnIndex = 1;
            //使用 reflection 將物件屬性取出當作工作表欄位名稱
            #region 選擇匯出類型
            switch (tabType.Trim())
            {
                case "GridPower":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "電流(A)";
                    sheet.Cell(1, colIdx++).Value = "實功率(kW)";
                    sheet.Cell(1, colIdx++).Value = "虛功率(kVAR)";
                    sheet.Cell(1, colIdx++).Value = "視在功率(kVA)";
                    sheet.Cell(1, colIdx++).Value = "功因(PF)";
                    sheet.Cell(1, colIdx++).Value = "頻率(Hz)";
                    sheet.Cell(1, colIdx++).Value = "用電量(kWh)";
                    rowIdx = 2;                //資料起始列位置       
                    conlumnIndex = 1;   //每筆資料欄位起始位置
                    foreach (var gps in data)
                    {
                        string[] IDs = gps.GridPowerIDs.Trim().Split('+');
                        foreach (var gp in IDs)
                        {
                            Guid ID = Guid.Parse(gp);
                            GridPower gridPowers = GridPowerService.ReadByID(ID);
                            sheet.Cell(rowIdx, 1).Value = gridPowers.date_time.ToString();
                            sheet.Cell(rowIdx, 2).Value = string.Format("{0:#,0.0}", gridPowers.Vavg);
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", gridPowers.Isum);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", gridPowers.Watt_t / 1000.0);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.00}", gridPowers.Var_t / 1000.00);
                            sheet.Cell(rowIdx, 6).Value = string.Format("{0:#,0.00}", gridPowers.VA_t / 1000.0);
                            sheet.Cell(rowIdx, 7).Value = string.Format("{0:#,0.0}", gridPowers.PF_t);
                            sheet.Cell(rowIdx, 8).Value = string.Format("{0:#,0.0}", gridPowers.Frequency);
                            sheet.Cell(rowIdx, 9).Value = string.Format("{0:#,0.0}", gridPowers.kWHt);
                            conlumnIndex++;
                            rowIdx++;
                        }
                    }
                    break;
                case "Inverters":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "工作模式";
                    sheet.Cell(1, colIdx++).Value = "市電電壓  (V)";
                    sheet.Cell(1, colIdx++).Value = "市電頻率  (Hz)";
                    sheet.Cell(1, colIdx++).Value = "輸出電壓  (V)";
                    sheet.Cell(1, colIdx++).Value = "輸出頻率  (Hz)";
                    sheet.Cell(1, colIdx++).Value = "總輸出實功率(kW)";
                    sheet.Cell(1, colIdx++).Value = "電池電壓  (V)";
                    sheet.Cell(1, colIdx++).Value = "電池容量  (%)";
                    sheet.Cell(1, colIdx++).Value = "太陽能電壓  (V)";
                    sheet.Cell(1, colIdx++).Value = "總充電電流  (A)";
                    rowIdx = 2;
                    conlumnIndex = 1;
                    foreach (var invs in data)
                    {
                        string[] IDs = invs.GridPowerIDs.Trim().Split('+');
                        foreach (var inv in IDs)
                        {
                            Guid ID = Guid.Parse(inv);
                            Inverter inverter = InverterService.ReadByID(ID);
                            string mod = inverter.DeviceMode.Trim();
                            if (mod == "P") { mod = "Power On Mode"; }
                            else if (mod == "S") { mod = "Standby Mode"; }
                            else if (mod == "L") { mod = "Line Mode"; }
                            else if (mod == "B") { mod = "Battery Mode"; }
                            else if (mod == "F") { mod = "Fault Mode"; }
                            else if (mod == "H") { mod = "Power Saving Mode"; }
                            else { mod = "Unknown Mode"; }
                            sheet.Cell(rowIdx, 1).Value = inverter.UpdateTime.ToString().Trim();
                            sheet.Cell(rowIdx, 2).Value = mod;
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", inverter.GridVoltage);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", inverter.GridFrequency);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.0}", inverter.AC_OutputVoltage);
                            sheet.Cell(rowIdx, 6).Value = string.Format("{0:#,0.0}", inverter.AC_OutputFrequency);
                            sheet.Cell(rowIdx, 7).Value = string.Format("{0:#,0.0}", inverter.ParallelInformation_TotalOutputActivePower / 1000.0);
                            sheet.Cell(rowIdx, 8).Value = string.Format("{0:#,0.0}", inverter.BatteryVoltage);
                            sheet.Cell(rowIdx, 9).Value = string.Format("{0:#,0.0}", inverter.BatteryCapacity);
                            sheet.Cell(rowIdx, 10).Value = string.Format("{0:#,0.0}", inverter.PV_InputVoltage);
                            sheet.Cell(rowIdx, 11).Value = string.Format("{0:#,0.0}", inverter.ParallelInformation_TotalChargingCurrent);
                            conlumnIndex++;
                            rowIdx++;
                        }
                    }
                    break;
                case "Solar":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "電流(A)";
                    sheet.Cell(1, colIdx++).Value = "功率(kW)";
                    sheet.Cell(1, colIdx++).Value = "發電量(kWh)";
                    colIdx++;
                    foreach (var invs in data)
                    {
                        string[] IDs = invs.GridPowerIDs.Trim().Split('+');
                        foreach (var inv in IDs)
                        {
                            Guid ID = Guid.Parse(inv);
                            Inverter inverter = InverterService.ReadByID(ID);
                            sheet.Cell(rowIdx, 1).Value = inverter.UpdateTime.ToString().Trim();
                            sheet.Cell(rowIdx, 2).Value = string.Format("{0:#,0.0}", inverter.SPM90Voltage);
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", inverter.SPM90Current);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", inverter.SPM90ActiveEnergy / 1000.0);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.0}", inverter.SPM90ActivePower);
                            conlumnIndex++;
                            rowIdx++;
                        }
                    }
                    break;
                case "Battery":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "電池電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "充電電流(A)";
                    sheet.Cell(1, colIdx++).Value = "放電電流(A)";
                    sheet.Cell(1, colIdx++).Value = "電池容量(%)";
                    sheet.Cell(1, colIdx++).Value = "充電次數(次)";
                    sheet.Cell(1, colIdx++).Value = "充電方向";
                    sheet.Cell(1, colIdx++).Value = "溫度(°C)";
                    rowIdx = 2;
                    conlumnIndex = 1;
                    foreach (var bas in data)
                    {
                        string[] IDs = bas.GridPowerIDs.Trim().Split('+');
                        foreach (var ba in IDs)
                        {
                            Guid ID = Guid.Parse(ba);
                            Battery battery = BatteryService.ReadByID(ID);
                            int cd = Convert.ToInt32(battery.charge_direction);
                            sheet.Cell(rowIdx, 1).Value = battery.updateTime.ToString().Trim();
                            sheet.Cell(rowIdx, 2).Value = string.Format("{0:#,0.0}", battery.voltage);
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", battery.charging_current);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", battery.discharging_current);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.0}", battery.SOC);
                            sheet.Cell(rowIdx, 6).Value = string.Format("{0:#,0}", battery.Cycle);
                            sheet.Cell(rowIdx, 7).Value = (cd == 0)? "離線":(cd == 1)?"充電": "放電"; 
                            sheet.Cell(rowIdx, 8).Value = string.Format("{0:#,0.0}", battery.temperature);
                            conlumnIndex++;
                            rowIdx++;
                        }
                    }
                    break;
                case "Load":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "電流(A)";
                    sheet.Cell(1, colIdx++).Value = "實功率(kW)";
                    sheet.Cell(1, colIdx++).Value = "虛功率(kVAR)";
                    sheet.Cell(1, colIdx++).Value = "視在功率(kVA)";
                    sheet.Cell(1, colIdx++).Value = "功因(PF)";
                    sheet.Cell(1, colIdx++).Value = "頻率(Hz)";
                    sheet.Cell(1, colIdx++).Value = "用電量(kWh)";
                    rowIdx = 2;
                    conlumnIndex = 1;
                    foreach (var los in data)
                    {
                        string[] IDs = los.GridPowerIDs.Trim().Split('+');
                        foreach (var lo in IDs)
                        {
                            Guid ID = Guid.Parse(lo);
                            LoadPower loadPower = LoadPowerService.ReadByID(ID);
                            sheet.Cell(rowIdx, 1).Value = loadPower.updateTime.ToString();
                            sheet.Cell(rowIdx, 2).Value = string.Format("{0:#,0.0}", loadPower.Vavg);
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", loadPower.Isum);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", loadPower.Watt_t / 1000.0);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.00}", loadPower.Var_t / 1000.00);
                            sheet.Cell(rowIdx, 6).Value = string.Format("{0:#,0.0}", loadPower.PF_t);
                            sheet.Cell(rowIdx, 7).Value = string.Format("{0:#,0.0}", loadPower.Frequency);
                            sheet.Cell(rowIdx, 8).Value = string.Format("{0:#,0.0}", loadPower.kWHt);
                            conlumnIndex++;
                            rowIdx++;
                        }
                    }
                    break;
                case "Generator":
                    colIdx = 1;
                    sheet.Cell(1, colIdx++).Value = "資料時間(UTC)";
                    sheet.Cell(1, colIdx++).Value = "發電機油位(%)";
                    sheet.Cell(1, colIdx++).Value = "L1-N相電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "L2-N相電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "L3-N相電壓(V)";
                    sheet.Cell(1, colIdx++).Value = "L1相電流(A)";
                    sheet.Cell(1, colIdx++).Value = "L2相電流(A)";
                    sheet.Cell(1, colIdx++).Value = "L3相電流(A)";
                    sheet.Cell(1, colIdx++).Value = "總實功率(kW)";
                    sheet.Cell(1, colIdx++).Value = "平均功率因數";
                    sheet.Cell(1, colIdx++).Value = "正的千瓦時(kWh)";
                    sheet.Cell(1, colIdx++).Value = "負的千瓦時(kWh)";
                    sheet.Cell(1, colIdx++).Value = "發電機 狀態";
                    sheet.Cell(1, colIdx++).Value = "可用總電量(kWh)";
                    sheet.Cell(1, colIdx++).Value = "可用電時數(H)";
                    rowIdx = 2;
                    conlumnIndex = 1;
                    foreach (var gens in data)
                    {
                        string[] IDs = gens.GridPowerIDs.Trim().Split('+');
                        foreach (var lo in IDs)
                        {
                            Guid ID = Guid.Parse(lo);
                            Generator generator = GeneratorService.ReadByID(ID);
                            sheet.Cell(rowIdx, 1).Value = generator.UpdateTime.ToString().Trim();
                            sheet.Cell(rowIdx, 2).Value = string.Format("{0:#,0.0}", generator.FuleLevel);
                            sheet.Cell(rowIdx, 3).Value = string.Format("{0:#,0.0}", generator.L1Nvoltage);
                            sheet.Cell(rowIdx, 4).Value = string.Format("{0:#,0.0}", generator.L2Nvoltage);
                            sheet.Cell(rowIdx, 5).Value = string.Format("{0:#,0.0}", generator.L3Nvoltage);
                            sheet.Cell(rowIdx, 6).Value = string.Format("{0:#,0.0}", generator.L1current);
                            sheet.Cell(rowIdx, 7).Value = string.Format("{0:#,0.0}", generator.L2current);
                            sheet.Cell(rowIdx, 8).Value = string.Format("{0:#,0.0}", generator.L3current);
                            sheet.Cell(rowIdx, 9).Value = string.Format("{0:#,0.0}", generator.totalwatts / 1000.0);
                            sheet.Cell(rowIdx, 10).Value = string.Format("{0:#,0.00}", generator.averagepowerfactor);
                            sheet.Cell(rowIdx, 11).Value = string.Format("{0:#,0.0}", generator.positiveKWhours);
                            sheet.Cell(rowIdx, 12).Value = string.Format("{0:#,0.0}", generator.negativeKWhours);
                            sheet.Cell(rowIdx, 13).Value = generator.ControlStatus == "true" ? "啟動" : "關閉";
                            sheet.Cell(rowIdx, 14).Value = string.Format("{0:#,0.0}", generator.AvailabilityEnergy);
                            sheet.Cell(rowIdx, 15).Value = string.Format("{0:#,0.0}", generator.AvailabilityHour);
                            conlumnIndex++;
                            rowIdx++;
                        }                       
                    }
                    break;
                default:
                    break;
            }
            #endregion
            return workbook;
        }
        #endregion XLSX
        #endregion Excel

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
        }

        /// <summary>
        /// 更換Buttom的class
        /// </summary>
        /// <param name="type"></param>
        private void NavButtom(string type)
        {
            TempData["ButtomIndex"]= "btn btn-outline-success btn-lg";
            TempData["ButtomBulletin"]= "btn btn-outline-success btn-lg";
            TempData["ButtomInfo"]= "btn btn-outline-success btn-lg";
            TempData["ButtomAbnormal"] = "btn btn-outline-success btn-lg";
            TempData["Buttomhistory"]= "btn btn-outline-success btn-lg";
            TempData["ButtomMaintain"]= "btn btn-outline-success btn-lg";
            type = type.Trim();
            switch (type)
            {
                case "Index":
                    TempData["ButtomIndex"] = "btn btn-success btn-lg";
                    break;
                case "Bulletin":
                    TempData["ButtomBulletin"]= "btn btn-success btn-lg";
                    break;
                case "Info":
                    TempData["ButtomInfo"]= "btn btn-success btn-lg";
                    break;
                case "Abnormal":
                    TempData["ButtomAbnormal"]= "btn btn-success btn-lg";
                    break;
                case "History":
                    TempData["Buttomhistory"]= "btn btn-success btn-lg";
                    break;
                case "Maintain":
                    TempData["ButtomMaintain"] = "btn btn-success btn-lg";
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
            stations.ForEach(x=> {
                if (x.StationCode!=1)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = x.StationName,
                        Value = x.Id.ToString()
                    });
                }
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
            AT.ForEach(x=> {
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