using NLog;
using OfficeOpenXml;
using PagedList;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace Web.Controllers
{
    [HandleError]
    public class TabController : Controller
    {
        #region private
        //Tab
        private readonly AccountService accountService = new AccountService();
        private StationService stationService = new StationService();
        private readonly AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private OrginService orginService = new OrginService();
        //EMS
        private ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();
        private InfoChartsService InfoChartsService = new InfoChartsService();
        private InfoDatasService InfoDatasService = new InfoDatasService();
        //分頁
        private int PageSizes() { if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out int s)) { s = 10; } return s; }
        //Log檔
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region Index首頁
        /// <summary>
        /// 大武
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [OutputCache(CacheProfile = "Cache5Min")]
        public ActionResult Index()
        {
            NavButtom("Index");
            return View();
        }

        /// <summary>
        /// 佳興
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [OutputCache(CacheProfile = "Cache5Min")]
        public ActionResult Jiasing()
        {
            NavButtom("Index");
            return View();
        }
        #endregion

        #region Info 及時資訊
        /// <summary>
        /// 及時資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [OutputCache(CacheProfile = "Cache1Min")]
        public ActionResult Info(string tabType, int StationNum=2)
        {
            NavButtom("Info");
            tabType = string.IsNullOrEmpty(tabType) ? "Load" : tabType;//預設
            StationList(stationService.StationID(StationNum));
            InfoNav(tabType);
            Guid StationUUID = stationService.UUID(StationNum);

            InfoData(tabType, StationUUID);
            ChartData(tabType, StationUUID);
            SOCLoadWatt(StationUUID);

            ViewBag.StationNum = StationNum;
            ViewBag.viewstation = stationService.ReadAll().Where(x => x.StationCode == StationNum).First().StationName;
            return View();
        }

        private void InfoData(string tabType, Guid StationUUID)
        {
            InfoDatas InfoDatas = InfoDatasService.ReadUUID(StationUUID);
            ViewBag.EssTime = InfoDatas.UpdateTime.ToString();

            switch (tabType.Trim())
            {
                case "GridPower":
                    #region GridPower
                    string gridInfo1 = "市電迴路(目前離線中)";
                    double gridInfo2 = 0, gridInfo3 = 0, gridInfo4 = 0, gridInfo5 = 0, gridInfo6 = 0, gridInfo7 = 0, gridInfo8 = 0, gridInfo9 = 0;

                    if (!string.IsNullOrEmpty( InfoDatas.GridPower))
                    {
                        var GridData = InfoDatas.GridPower.Trim().Split('|');
                        gridInfo1 = string.IsNullOrEmpty(GridData[0].Trim()) ? "市電迴路(目前離線中)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(GridData[0]).Ticks) ? "市電迴路(目前離線中)" : "市電迴路";
                        gridInfo2 = Math.Round(Convert.ToDouble(GridData[1]), 2);
                        gridInfo3 = Math.Round(Convert.ToDouble(GridData[2]), 2);
                        gridInfo4 = Math.Round(Convert.ToDouble(GridData[3]), 2);
                        gridInfo5 = Math.Round(Convert.ToDouble(GridData[4]), 2);
                        gridInfo6 = Math.Round(Convert.ToDouble(GridData[5]), 2);
                        gridInfo7 = Math.Round(Convert.ToDouble(GridData[6]), 2);
                        gridInfo8 = Math.Round(Convert.ToDouble(GridData[7]), 2);
                        gridInfo9 = Math.Round(Convert.ToDouble(GridData[8]), 2);
                    }

                    ViewBag.gridInfo1 = gridInfo1;
                    ViewBag.gridInfo2 = gridInfo2;
                    ViewBag.gridInfo3 = gridInfo3;
                    ViewBag.gridInfo4 = gridInfo4;
                    ViewBag.gridInfo5 = gridInfo5;
                    ViewBag.gridInfo6 = gridInfo6;
                    ViewBag.gridInfo7 = gridInfo7;
                    ViewBag.gridInfo8 = gridInfo8;
                    ViewBag.gridInfo9 = gridInfo9;
                    #endregion
                    break;
                case "Inverters":
                    #region Inverter
                    string InvInfo1 = "逆變器(目前離線中)", InvInfo2 = "故障模式", InvInfo3 = "0", InvInfo4 = "0", InvInfo5 = "0", InvInfo6 = "0", InvInfo7 = "0", InvInfo8 = "0", InvInfo9 = "0", InvInfo10 = "0", InvInfo11 = "0";

                    if (!string.IsNullOrEmpty(InfoDatas.Inverters))
                    {
                        var InvData = InfoDatas.Inverters.Trim().Split('|');
                        InvInfo1 = string.IsNullOrEmpty(InvData[0].Trim()) ? "逆變器(目前離線中)":(DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(InvData[0]).Ticks) ? "逆變器(目前離線中)" : "逆變器" ;
                        InvInfo2 = InvData[1].Trim();
                        InvInfo3 = InvData[2].Trim();
                        InvInfo4 = InvData[3].Trim();
                        InvInfo5 = InvData[4].Trim();
                        InvInfo6 = InvData[5].Trim();
                        InvInfo7 = InvData[6].Trim();
                        InvInfo8 = InvData[7].Trim();
                        InvInfo9 = InvData[8].Trim();
                        InvInfo10 = InvData[9].Trim();
                        InvInfo11 = InvData[10].Trim();
                    }
                   
                    ViewBag.InvInfo1 = InvInfo1;
                    ViewBag.InvInfo2 = InvInfo2;
                    ViewBag.InvInfo3 = InvInfo3;
                    ViewBag.InvInfo4 = InvInfo4;
                    ViewBag.InvInfo5 = InvInfo5;
                    ViewBag.InvInfo6 = InvInfo6;
                    ViewBag.InvInfo7 = InvInfo7;
                    ViewBag.InvInfo8 = InvInfo8;
                    ViewBag.InvInfo9 = InvInfo9;
                    ViewBag.InvInfo10 = InvInfo10;
                    ViewBag.InvInfo11 = InvInfo11;
                    #endregion
                    break;
                case "Solar":
                    #region Solar
                    string solarInfo11 = "太陽能(目前離線中)", solarInfo21 = "太陽能2(目前離線中)";
                    double solarInfo12 = 0, solarInfo13 = 0, solarInfo14 = 0, solarInfo15 = 0;
                    double solarInfo22 = 0, solarInfo23 = 0, solarInfo24 = 0, solarInfo25 = 0;

                    if (!string.IsNullOrEmpty(InfoDatas.Solar))
                    {
                        var SolarData = InfoDatas.Solar.Trim().Split('$');
                        var Solar1 = SolarData[0].Trim().Split('|');
                        var Solar2 = SolarData[1].Trim().Split('|');

                        solarInfo11 = string.IsNullOrEmpty(Solar1[0].Trim()) ? "太陽能(目前離線中)":(DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(Solar1[0]).Ticks) ? "太陽能(目前離線中)" : "太陽能" ;
                        solarInfo12 = Math.Round(Convert.ToDouble(Solar1[1]), 2);
                        solarInfo13 = Math.Round(Convert.ToDouble(Solar1[2]), 2);
                        solarInfo14 = Math.Round(Convert.ToDouble(Solar1[3]), 2);
                        solarInfo15 = Math.Round(Convert.ToDouble(Solar1[4]), 2);

                        solarInfo21 = string.IsNullOrEmpty(Solar2[0].Trim()) ? "太陽能2 (目前離線中)":(DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(Solar2[0]).Ticks) ? "太陽能2(目前離線中)" : "太陽能2";
                        solarInfo22 = Math.Round(Convert.ToDouble(Solar2[1]), 2);
                        solarInfo23 = Math.Round(Convert.ToDouble(Solar2[2]), 2);
                        solarInfo24 = Math.Round(Convert.ToDouble(Solar2[3]), 2);
                        solarInfo25 = Math.Round(Convert.ToDouble(Solar2[4]), 2);
                    }

                    ViewBag.solarInfo11 = solarInfo11;
                    ViewBag.solarInfo12 = solarInfo12;
                    ViewBag.solarInfo13 = solarInfo13;
                    ViewBag.solarInfo14 = solarInfo14;
                    ViewBag.solarInfo15 = solarInfo15;

                    ViewBag.solarInfo21 = solarInfo21;
                    ViewBag.solarInfo22 = solarInfo22;
                    ViewBag.solarInfo23 = solarInfo23;
                    ViewBag.solarInfo24 = solarInfo24;
                    ViewBag.solarInfo25 = solarInfo25;
                    #endregion
                    break;
                case "Battery":
                    #region Battery
                    string BatteryInfo11 = "電池組1(目前離線中)", BatteryInfo12 = "0", BatteryInfo13 = "0", BatteryInfo14 = "0", BatteryInfo15 = "0", BatteryInfo16 = "0", BatteryInfo17 = "離線";
                    string BatteryInfo21 = "電池組2(目前離線中)", BatteryInfo22 = "0", BatteryInfo23 = "0", BatteryInfo24 = "0", BatteryInfo25 = "0", BatteryInfo26 = "0", BatteryInfo27 = "離線";
                    string BatteryInfo31 = "電池組3(目前離線中)", BatteryInfo32 = "0", BatteryInfo33 = "0", BatteryInfo34 = "0", BatteryInfo35 = "0", BatteryInfo36 = "0", BatteryInfo37 = "離線";
                    string BatteryInfo41 = "電池組4(目前離線中)", BatteryInfo42 = "0", BatteryInfo43 = "0", BatteryInfo44 = "0", BatteryInfo45 = "0", BatteryInfo46 = "0", BatteryInfo47 = "離線";

                    if (!string.IsNullOrEmpty(InfoDatas.Battery))
                    {
                        var BatteryData = InfoDatas.Battery.Trim().Split('$');
                        var Battery1 = BatteryData[0].Trim().Split('|');
                        var Battery2 = BatteryData[1].Trim().Split('|');
                        var Battery3 = BatteryData[2].Trim().Split('|');
                        var Battery4 = BatteryData[3].Trim().Split('|');

                        BatteryInfo11 = string.IsNullOrEmpty(Battery1[0].Trim()) ? "電池組1(目前離線中)":(DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(Battery1[0]).Ticks) ? "電池組1(目前離線中)" : "電池組1 " ;
                        BatteryInfo12 = Battery1[1];
                        BatteryInfo13 = Battery1[2];
                        BatteryInfo14 = Battery1[3];
                        BatteryInfo15 = Battery1[4];
                        BatteryInfo16 = Battery1[5];
                        BatteryInfo17 = Battery1[6];

                        BatteryInfo21 = string.IsNullOrEmpty(Battery2[0].Trim()) ? "電池組2(目前離線中)" : (DateTime.Now.AddMinutes(-5) > DateTime.Parse(Battery2[0])) ? "電池組2(目前離線中)" : "電池組2";
                        BatteryInfo22 = Battery2[1];
                        BatteryInfo23 = Battery2[2];
                        BatteryInfo24 = Battery2[3];
                        BatteryInfo25 = Battery2[4];
                        BatteryInfo26 = Battery2[5];
                        BatteryInfo27 = Battery2[6];

                        BatteryInfo31 = string.IsNullOrEmpty(Battery3[0].Trim()) ? "電池組3(目前離線中)" : (DateTime.Now.AddMinutes(-5) > DateTime.Parse(Battery3[0])) ? "電池組3(目前離線中)" : "電池組3";
                        BatteryInfo32 = Battery3[1];
                        BatteryInfo33 = Battery3[2];
                        BatteryInfo34 = Battery3[3];
                        BatteryInfo35 = Battery3[4];
                        BatteryInfo36 = Battery3[5];
                        BatteryInfo37 = Battery3[6];

                        BatteryInfo41 = string.IsNullOrEmpty(Battery4[0].Trim()) ? "電池組4(目前離線中)" : (DateTime.Now.AddMinutes(-5) > DateTime.Parse(Battery4[0])) ? "電池組4(目前離線中)" : "電池組4";
                        BatteryInfo42 = Battery4[1];
                        BatteryInfo43 = Battery4[2];
                        BatteryInfo44 = Battery4[3];
                        BatteryInfo45 = Battery4[4];
                        BatteryInfo46 = Battery4[5];
                        BatteryInfo47 = Battery4[6];

                    }
                    ViewBag.BatteryInfo11 = BatteryInfo11;
                    ViewBag.BatteryInfo12 = BatteryInfo12;
                    ViewBag.BatteryInfo13 = BatteryInfo13;
                    ViewBag.BatteryInfo14 = BatteryInfo14;
                    ViewBag.BatteryInfo15 = BatteryInfo15;
                    ViewBag.BatteryInfo16 = BatteryInfo16;
                    ViewBag.BatteryInfo17 = BatteryInfo17;

                    ViewBag.BatteryInfo21 = BatteryInfo21;
                    ViewBag.BatteryInfo22 = BatteryInfo22;
                    ViewBag.BatteryInfo23 = BatteryInfo23;
                    ViewBag.BatteryInfo24 = BatteryInfo24;
                    ViewBag.BatteryInfo25 = BatteryInfo25;
                    ViewBag.BatteryInfo26 = BatteryInfo26;
                    ViewBag.BatteryInfo27 = BatteryInfo27;

                    ViewBag.BatteryInfo31 = BatteryInfo31;
                    ViewBag.BatteryInfo32 = BatteryInfo32;
                    ViewBag.BatteryInfo33 = BatteryInfo33;
                    ViewBag.BatteryInfo34 = BatteryInfo34;
                    ViewBag.BatteryInfo35 = BatteryInfo35;
                    ViewBag.BatteryInfo36 = BatteryInfo36;
                    ViewBag.BatteryInfo37 = BatteryInfo37;

                    ViewBag.BatteryInfo41 = BatteryInfo41;
                    ViewBag.BatteryInfo42 = BatteryInfo42;
                    ViewBag.BatteryInfo43 = BatteryInfo43;
                    ViewBag.BatteryInfo44 = BatteryInfo44;
                    ViewBag.BatteryInfo45 = BatteryInfo45;
                    ViewBag.BatteryInfo46 = BatteryInfo46;
                    ViewBag.BatteryInfo47 = BatteryInfo47;
                    #endregion
                    break;
                case "Load":
                    #region Load
                    string LoadInfo1 = "負載(目前離線中)";
                    double LoadInfo2 = 0, LoadInfo3 = 0, LoadInfo4 = 0, LoadInfo5 = 0, LoadInfo6 = 0, LoadInfo7 = 0, LoadInfo8 = 0, LoadInfo9 = 0;
                    if (!string.IsNullOrEmpty(InfoDatas.LoadPower))
                    {
                        var LoadData = InfoDatas.LoadPower.Trim().Split('|');
                        LoadInfo1 = string.IsNullOrEmpty(LoadData[0].Trim()) ?"負載(目前離線中)":(DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(LoadData[0]).Ticks) ? "負載(目前離線中)" : "負載" ;
                        LoadInfo2 = Math.Round(Convert.ToDouble(LoadData[1]), 2);
                        LoadInfo3 = Math.Round(Convert.ToDouble(LoadData[2]), 2);
                        LoadInfo4 = Math.Round(Convert.ToDouble(LoadData[3]), 2);
                        LoadInfo5 = Math.Round(Convert.ToDouble(LoadData[4]), 2);
                        LoadInfo6 = Math.Round(Convert.ToDouble(LoadData[5]), 2);
                        LoadInfo7 = Math.Round(Convert.ToDouble(LoadData[6]), 2);
                        LoadInfo8 = Math.Round(Convert.ToDouble(LoadData[7]), 2);
                        LoadInfo9 = Math.Round(Convert.ToDouble(LoadData[8]), 2);
                    }

                    ViewBag.LoadInfo1 = LoadInfo1;
                    ViewBag.LoadInfo2 = LoadInfo2;
                    ViewBag.LoadInfo3 = LoadInfo3;
                    ViewBag.LoadInfo4 = LoadInfo4;
                    ViewBag.LoadInfo5 = LoadInfo5;
                    ViewBag.LoadInfo6 = LoadInfo6;
                    ViewBag.LoadInfo7 = LoadInfo7;
                    ViewBag.LoadInfo8 = LoadInfo8;
                    ViewBag.LoadInfo9 = LoadInfo9;
                    #endregion
                    break;
                case "Generator":
                    #region GeneratorChart
                    string GenInfo1 = "發電機(目前離線中)", GenInfo13 = "離線", GenInfo2 = "0", GenInfo3 = "0", GenInfo4 = "0", GenInfo5 = "0", GenInfo6 = "0", GenInfo7 = "0", GenInfo8 = "0", GenInfo9 = "0", GenInfo10 = "0", GenInfo11 = "0", GenInfo12 = "0", GenInfo14 = "0";

                    if (!string.IsNullOrEmpty(InfoDatas.Generator))
                    {
                        var GenData = InfoDatas.Generator.Trim().Split('|');
                        GenInfo1 = string.IsNullOrEmpty(GenData[0].Trim()) ? "發電機(目前離線中)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(GenData[0]).Ticks) ? "發電機(目前離線中)" : "發電機";
                        GenInfo2 = GenData[1];
                        GenInfo3 = GenData[2];
                        GenInfo4 = GenData[3];
                        GenInfo5 = GenData[4];
                        GenInfo6 = GenData[5];
                        GenInfo7 = GenData[6];
                        GenInfo8 = GenData[7];
                        GenInfo9 = GenData[8];
                        GenInfo10 = GenData[9];
                        GenInfo11 = GenData[10];
                        GenInfo12 = GenData[11];
                        GenInfo13 = GenData[12];
                        GenInfo14 = GenData[13];
                    }

                    ViewBag.GenInfo1 = GenInfo1;
                    ViewBag.GenInfo2 = GenInfo2;
                    ViewBag.GenInfo3 = GenInfo3;
                    ViewBag.GenInfo4 = GenInfo4;
                    ViewBag.GenInfo5 = GenInfo5;
                    ViewBag.GenInfo6 = GenInfo6;
                    ViewBag.GenInfo7 = GenInfo7;
                    ViewBag.GenInfo8 = GenInfo8;
                    ViewBag.GenInfo9 = GenInfo9;
                    ViewBag.GenInfo10 = GenInfo10;
                    ViewBag.GenInfo11 = GenInfo11;
                    ViewBag.GenInfo12 = GenInfo12;
                    ViewBag.GenInfo13 = GenInfo13;
                    ViewBag.GenInfo14 = GenInfo14;
                    #endregion
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// EChart資料
        /// </summary>
        /// <param name="tabType"></param>
        private void ChartData(string tabType, Guid StationUUID)
        {
            switch (tabType.Trim())
            {
                case "GridPower":
                    #region GridPowerChart                 
                    TempData["Grid1"] = InfoChartsService.Readby(StationUUID, "GridPower").QuarterList.Trim(); 
                    #endregion GridPowerChart
                    break;
                case "Inverters":
                    #region InvertersChart        
                    TempData["InverterData"] = InfoChartsService.Readby(StationUUID, "Inverters").QuarterList.Trim();
                    #endregion InvertersChart
                    break;
                case "Solar":
                    #region SolarChart
                    var Sun = InfoChartsService.Readby(StationUUID, "Solar").QuarterList.Trim().Split('|');
                    TempData["Sun0"] = Sun[0];
                    TempData["Sun1"] = Sun[1];
                    #endregion  SolarChart
                    break;
                case "Battery":
                    #region BatteryChart
                    TempData["BatteryData"] = InfoChartsService.Readby(StationUUID, "Battery").QuarterList.Trim();
                    #endregion
                    break;
                case "Load":
                    #region LoadChart            
                    TempData["Load1"] = InfoChartsService.Readby(StationUUID, "LoadPower").QuarterList.Trim();
                    #endregion LoadChart
                    break;
                case "Generator":
                    #region GeneratorChart            
                    TempData["Generator1"] = InfoChartsService.Readby(StationUUID, "Generator").QuarterList.Trim();
                    #endregion GeneratorChart
                    break;
                default:
                    break;
            }
        }
           
        //Right View
        private void SOCLoadWatt(Guid StationUUID)
        {

            InfoDatas InfoDatas = InfoDatasService.ReadUUID(StationUUID);
            double soc = Math.Round(InfoDatas.Demand, 2);
            ViewBag.Demand = Math.Round(soc, 2).ToString() + "kWh";
            ViewBag.RemainTime = InfoDatas.RemainTime <= 0 ? "無負載" : InfoDatas.RemainTime.ToString() + "小時";
        }
        /// <summary>
        /// 更換Nav class
        /// </summary>
        /// <param name="tabType"></param>
        private void InfoNav(string tabType = "Load")
        {
            string MainTab = "tab-pane active";
            string MuteTab = "tab-pane";
            string MainNav = "nav-link active";
            string MuteNav = "nav-link";

            TempData["tabGridPower"] = tabType.Trim().Contains("GridPower") ? MainTab : MuteTab;
            TempData["tabLoad"] = tabType.Trim().Contains("Load") ? MainTab : MuteTab;
            TempData["tabSolar"] = tabType.Trim().Contains("Solar") ? MainTab : MuteTab;
            TempData["tabGenerator"] = tabType.Trim().Contains("Generator") ? MainTab : MuteTab;
            TempData["tabInverters"] = tabType.Trim().Contains("Inverters") ? MainTab : MuteTab;
            TempData["tabBattery"] = tabType.Trim().Contains("Battery") ? MainTab : MuteTab;

            TempData["navGridPower"] = tabType.Trim().Contains("GridPower") ? MainNav : MuteNav;
            TempData["navLoad"] = tabType.Trim().Contains("Load") ? MainNav : MuteNav;
            TempData["navSolar"] = tabType.Trim().Contains("Solar") ? MainNav : MuteNav;
            TempData["navGenerator"] = tabType.Trim().Contains("Generator") ? MainNav : MuteNav;
            TempData["navInverters"] = tabType.Trim().Contains("Inverters") ? MainNav : MuteNav;
            TempData["navBattery"] = tabType.Trim().Contains("Battery") ? MainNav : MuteNav;

            TempData["nav"] = tabType;
        }

        #endregion
      
        #region History歷史資訊
        /// <summary>
        /// 歷史資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult History(string connStr,string stationID,  int StationNum=2 ,int page=1 )
        {
            string tabType = "Load";
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

            NavButtom("History");
            StationList(stationService.StationID(StationNum));
            AlartTypeList();
            TabAction(tabType);

            ViewBag.RangeStart = DateTime.Now.AddDays(-1);
            ViewBag.RangeEnd = DateTime.Now;
            ViewBag.StationNum = StationNum;
            ViewBag.stationID = stationService.StationID(StationNum).ToString();

            List<ESSObject> List = new List<ESSObject>();

            if (sDay == null || eDay == null)
            {
                ViewBag.startDay = DateTime.Now.AddDays(-1);
                ViewBag.endDay = DateTime.Now;
                //取得此時間點後1日的資料
                List = ESSObjecterService.ReadTimeIntervalStation(DateTime.Now.AddDays(-1).AddHours(-8), DateTime.Now.AddHours(-8),stationService.UUID(StationNum).ToString());
            }
            else
            {
                ViewBag.startDay = DateTime.Parse(sDay);
                ViewBag.endDay = DateTime.Parse(eDay);
                //取得資料
                List = ESSObjecterService.ReadTimeIntervalStation(DateTime.Parse(sDay).AddHours(-8), DateTime.Parse(eDay).AddHours(-8), stationService.UUID(StationNum).ToString());
            }

            ViewBag.Count = List.Count();
            ViewBag.StationName = "所有站別";
            int currentPage = page < 1 ? 1 : page;
            var result = List.ToPagedList(currentPage, 10);
            return View(result);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult History(FormCollection From, int? page)
        {
            string date = Request.Form["datetimes"];
            string tabType = Request.Form["tabType"];
            Guid StationID = Guid.Parse(Request.Form["Statons"]);
            int StationNum = stationService.ReadID(StationID).StationCode;

            string[] data = date.ToString().Trim().Split('-');
            DateTime startday = Convert.ToDateTime(data[0]);
            DateTime endday = Convert.ToDateTime(data[1]);

            ViewBag.RangeStart = startday;
            ViewBag.RangeEnd = endday;

            ViewBag.startDay = startday;
            ViewBag.endDay = endday;

            NavButtom("History");
            StationList(StationID);
            AlartTypeList();
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
            ViewBag.StationNum = StationNum;

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
            var radio = Request.Form["inlineRadio"];
            string tabType = Request.Form["tabType"].Trim();
            DateTime startDay = Convert.ToDateTime(Request.Form["startDay"] + " " + Request.Form["startTime"]);
            DateTime endDay = Convert.ToDateTime(Request.Form["endDay"] + " " + Request.Form["endTime"]);
            string connStr = tabType + "+" + startDay + "+" + endDay;
            int StationNum = Convert.ToInt32(Request.Form["StationNum"]);
            string StationID = stationService.StationID(StationNum).ToString();

            try
            {
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
        
                string reportName = tabType + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";

                List<ESSObject> data = ESSObjecterService.ReadTimeIntervalStation(startDay, endDay, stationService.UUID(StationNum).ToString());
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
                                                                T1 = gridPowers.Vavg;
                                                                T2 = gridPowers.Isum;
                                                                T3 = gridPowers.Watt_t / 1000.0;
                                                                T4 = gridPowers.Var_t / 1000.00;
                                                                T5 = gridPowers.VA_t / 1000.00;
                                                                T6 = gridPowers.PF_t;
                                                                T7 = gridPowers.Frequency;
                                                                T8 = gridPowers.MinuskWHt;                                                          
                                                        }
                                                    }
                                                }
                                                sheet.Cells[rowIdx, 1].Value = Timer.ToString();
                                                sheet.Cells[rowIdx, 2].Value = Math.Round(T1 ,2);
                                                sheet.Cells[rowIdx, 3].Value = Math.Round(T2, 2);
                                                sheet.Cells[rowIdx, 4].Value = Math.Round(T3, 2);
                                                sheet.Cells[rowIdx, 5].Value = Math.Round(T4, 2);
                                                sheet.Cells[rowIdx, 6].Value = Math.Round(T5, 2);
                                                sheet.Cells[rowIdx, 7].Value = Math.Round(T6, 2);
                                                sheet.Cells[rowIdx, 8].Value = Math.Round(T7, 2);
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
                                                            if (mod == "P") { mod = "電源模式"; }
                                                            else if (mod == "S") { mod = "待機模式"; }
                                                            else if (mod == "L") { mod = "市電模式"; }
                                                            else if (mod == "B") { mod = "電池模式"; }
                                                            else if (mod == "F") { mod = " 故障模式"; }
                                                            else if (mod == "H") { mod = "省電模式"; }
                                                            else { mod = "離線"; }
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
                return RedirectToAction("History", "Tab", new { connStr, StationNum,StationID });
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                return RedirectToAction("History", "Tab", new { connStr, StationNum, StationID });
            }
        }
        #endregion Excel

        private void TabAction(string tabType= "GridPower")
        {
            string MainTab = "tab-pane active";
            string MuteTab = "tab-pane";
            ViewBag.tabGridPower = tabType.Trim().Contains("GridPower") ? MainTab:MuteTab;
            ViewBag.tabLoad = tabType.Trim().Contains("Load") ? MainTab:MuteTab;
            ViewBag.tabSolar = tabType.Trim().Contains("Solar") ? MainTab:MuteTab;
            ViewBag.tabGenerator = tabType.Trim().Contains("Generator") ? MainTab:MuteTab;
            ViewBag.tabInverters = tabType.Trim().Contains("Inverters") ? MainTab:MuteTab;
            ViewBag.tabBattery = tabType.Trim().Contains("Battery") ? MainTab:MuteTab;

            string MainNav = "nav-link active";
            string MuteNav = "nav-link";
            ViewBag.navGridPower = tabType.Trim().Contains("GridPower") ? MainNav : MuteNav;
            ViewBag.navLoad = tabType.Trim().Contains("Load") ? MainNav: MuteNav;
            ViewBag.navSolar = tabType.Trim().Contains("Solar") ? MainNav: MuteNav;
            ViewBag.navGenerator = tabType.Trim().Contains("Generator") ? MainNav: MuteNav;
            ViewBag.navInverters = tabType.Trim().Contains("Inverters") ? MainNav: MuteNav;
            ViewBag.navBattery = tabType.Trim().Contains("Battery") ? MainNav: MuteNav;

            ViewBag.onTab = tabType;       
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
        /// 更換Buttom的class
        /// </summary>
        /// <param name="type"></param>
        private void NavButtom(string type= "Index")
        {
            TagTitle();

            string MainBtn = "btn btn-success btn-lg";
            string MuteBtn = "btn btn-outline-success btn-lg";
            TempData["ButtomIndex"] = type.Trim().Contains("Index") ?MainBtn :MuteBtn;
            TempData["ButtomBulletin"] = type.Trim().Contains("Bulletin") ?MainBtn :MuteBtn;
            TempData["ButtomInfo"] = type.Trim().Contains("Info") ?MainBtn :MuteBtn;
            TempData["ButtomAbnormal"] = type.Trim().Contains("Abnormal") ?MainBtn :MuteBtn;
            TempData["Buttomhistory"] = type.Trim().Contains("History") ?MainBtn :MuteBtn;
            TempData["ButtomMaintain"] = type.Trim().Contains("Maintain") ?MainBtn :MuteBtn;
            TempData["ButtomQRCode"] = type.Trim().Contains("QRCode") ?MainBtn :MuteBtn;

            ViewBag.Index = Resources.Resource.Index;
            ViewBag.GridPower = Resources.Resource.GridPower;
            ViewBag.Battery = Resources.Resource.Battery;
            ViewBag.Solar = Resources.Resource.Solar;
            ViewBag.Load = Resources.Resource.Load;
            ViewBag.Generator = Resources.Resource.Generator;
            ViewBag.Inverters = Resources.Resource.Inverters;
            ViewBag.QRCode = Resources.Resource.QRCode;
        }

        /// <summary>
        /// Station下拉式選單
        /// </summary>
        private void StationList(Guid SID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<Station> stations = stationService.ReadAll().OrderBy(x => x.StationCode).ToList();
            stations.ForEach(x => {
                items.Add(new SelectListItem()
                {
                    Text = x.StationName,
                    Value = x.Id.ToString(),
                    Selected=x.Id.Equals(SID)
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

        private void AlartTypeList(Guid ATID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<AlartType> AT = alarttypeService.ReadAll().OrderBy(x => x.AlartTypeCode).ToList();
            AT.ForEach(x => {
                items.Add(new SelectListItem()
                {
                    Text = x.AlartTypeName,
                    Value = x.Id.ToString(),
                    Selected=x.Id.Equals(ATID)
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
        }
        #endregion
    }
}