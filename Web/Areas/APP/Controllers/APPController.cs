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
using System.Web.Mvc;
using System.Web.WebPages;
using Web.Models.GuanTsai;
using Web.Models.Tabs;

namespace Web.Areas.APP.Controllers
{
    public class APPController : Controller
    {
        #region private
        private BulletinService bulletinService = new BulletinService();
        private AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private StationService stationService = new StationService();
        private OrginService orginService = new OrginService();
        //EMS
        private InverterService InverterService = new InverterService();
        private IndexDataService IndexDataService = new IndexDataService();
        private InfoChartsService InfoChartsService = new InfoChartsService();
        private InfoDatasService InfoDatasService = new InfoDatasService();
        //分頁
        private int PageSizes() { if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out int s)) { s = 10; } return s; }
        //Log檔
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        // GET: APP/APP
        public ActionResult Index()
        {
            Guid StationUUID=Guid.Empty;
            double Solar = 0, GirdPower = 0,Load = 0,BatteySOC = 0,BatteyPower = 0,Generator = 0,GridV = 0,LoadV = 0,solarV = 0,GenV = 0, BattV = 0, GeneratorEngineBatteryVoltage=0;
            bool LoadConnected = false, SPM90MConnected = false,GeneratorConnected = false, GridPowerConnected = false, BatteryConnected = false;
            int Direction = 0;
            string BatteyModel = "離線";
            string Connecting = "col border border-light pz";
            string Closed = "col border border-light bg-secondary  pz";
            string Break = "col border border-light bg-danger pz";

            #region 霧台(WuTai)
            StationUUID = stationService.UUID(2);

            var Data = IndexDataService.ReadUUID(StationUUID);
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan Update = new TimeSpan(Data.UpdateTime.Ticks);
            TimeSpan ts = now.Subtract(Update).Duration();

            if (ts.Minutes <= 5)
            {
                Solar = Data.Solar;
                GirdPower = Data.GirdPower;
                Load = Data.Load;
                BatteySOC = Data.BatteySOC;
                BatteyModel = Data.BatteyModel;
                BatteyPower = Data.BatteyPower;
                Generator = Data.GeneratorPower;

                GridV = Data.GirdPowerVoltage;
                LoadV = Data.LoadVoltage;
                solarV = Data.SolarVoltage;
                GenV = Data.GeneratorVoltage;
                BattV = Data.BatteryVoltage;

                LoadConnected = Data.LoadConnected;
                SPM90MConnected = Data.SPM90MConnected;
                GeneratorConnected = Data.GeneratorConnected;
                GridPowerConnected = Data.GridPowerConnected;
                BatteryConnected = Data.BatteryConnected;
            }
            ViewBag.Demand = Math.Round(BatteySOC * 0.2, 2).ToString() + "kWh";
            ViewBag.RemainTime = Load <= 0 ? "無負載" : Math.Round(BatteySOC * 0.2 / Load, 1).ToString() + "小時";

            #region
            ViewBag.grid = GirdPower;
            ViewBag.Load = Load;
            ViewBag.Solar = Solar;
            ViewBag.Generator = Generator;
            ViewBag.BatterySOC = BatteySOC;
            ViewBag.BatteryPower = BatteyPower;
            ViewBag.BatteryModel = BatteyModel;

            ViewBag.Colgrid = GridV < 50.0 ?Break  : GirdPower <= 0 ? Closed : Connecting;
            ViewBag.ColLoad = LoadConnected==false || LoadV < 50.0 ?Break  : Load <= 0 ? Closed:Connecting;
            ViewBag.ColSolar = SPM90MConnected==false ||  solarV == 0 ?Break  : Solar <= 0 ? Closed:Connecting;
            ViewBag.ColGenerator = GeneratorConnected==false && GeneratorEngineBatteryVoltage < 5 ?Break  : Generator <= 0 ? Closed:Connecting;
            ViewBag.ColBattery = BattV == 0 ?Break  : Direction > 2 ? Closed:Connecting;

            ViewBag.Namegrid = GridV < 50 ? "市電(電壓不足)" : "市電";
            ViewBag.NameLoad = LoadConnected == false || LoadV < 50.0 ? "負載(電壓不足)" : "負載";
            ViewBag.NameSolar = SPM90MConnected == false || solarV == 0 ? "太陽能(離線)" : "太陽能";
            ViewBag.NameGenerator = GeneratorConnected == false && GeneratorEngineBatteryVoltage < 5 ? "發電機(離線)" : "發電機";
            ViewBag.NameBattery = BattV == 0 ? "電池(離線)" : "電池";
            #endregion
            #endregion

            #region 光采
            HashSet<Int32> eachNum = new HashSet<Int32>();

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/MonthlyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].Split('.');
                eachNum.Add(Convert.ToInt32(FN[0].Substring(0, 6)));
            }
            string maxMonthly = eachNum.Max().ToString();
            ViewBag.maxMonthly = maxMonthly.Substring(0,4).Trim() + "/" + maxMonthly.Substring(4, 2).Trim();

            eachNum.Clear();
            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].Split('.');
                eachNum.Add(Convert.ToInt32(FN[0].Substring(0, 8)));
            }
            string maxDaily = eachNum.Max().ToString();
            ViewBag.maxDaily = maxDaily.Substring(0, 4).Trim() + "/" + maxDaily.Substring(4, 2).Trim() + "/" + maxDaily.Substring(6, 2).Trim() ;
            #endregion

            #region 佳興 JiaSing(JS)
            StationUUID = Guid.Empty;
            Solar = 0; GirdPower = 0; Load = 0; BatteySOC = 0; BatteyPower = 0; Generator = 0; GridV = 0; LoadV = 0; solarV = 0; GenV = 0; BattV = 0; GeneratorEngineBatteryVoltage = 0;
            LoadConnected = false; SPM90MConnected = false; GeneratorConnected = false; GridPowerConnected = false; BatteryConnected = false;
            Direction = 0;
            BatteyModel = "離線";

            Data = null;
            StationUUID = stationService.UUID(6);
            Data = IndexDataService.ReadUUID(StationUUID);

            now = new TimeSpan(DateTime.Now.Ticks);
            Update = new TimeSpan(Data.UpdateTime.Ticks);
            ts = now.Subtract(Update).Duration();

            if (ts.Minutes <= 5)
            {
                Solar = Data.Solar;
                GirdPower = Data.GirdPower;
                Load = Data.Load;
                BatteySOC = Data.BatteySOC;
                BatteyModel = Data.BatteyModel;
                BatteyPower = Data.BatteyPower;
                Generator = Data.GeneratorPower;

                GridV = Data.GirdPowerVoltage;
                LoadV = Data.LoadVoltage;
                solarV = Data.SolarVoltage;
                GenV = Data.GeneratorVoltage;
                BattV = Data.BatteryVoltage;

                LoadConnected = Data.LoadConnected;
                SPM90MConnected = Data.SPM90MConnected;
                GeneratorConnected = Data.GeneratorConnected;
                GridPowerConnected = Data.GridPowerConnected;
                BatteryConnected = Data.BatteryConnected;

                GeneratorEngineBatteryVoltage = Data.GeneratorEngineBatteryVoltage;
                Direction = Data.Direction;
            }

            ViewBag.JSDemand = Math.Round(BatteySOC * 0.2, 2).ToString() + "kWh";
            ViewBag.JSRemainTime = Load <= 0 ? "無負載" : Math.Round(BatteySOC * 0.2 / Load, 1).ToString() + "小時";

            #region
            ViewBag.JSgrid = GirdPower;
            ViewBag.JSLoad = Load;
            ViewBag.JSSolar = Solar;
            ViewBag.JSGenerator = Generator;
            ViewBag.JSBatterySOC = BatteySOC;
            ViewBag.JSBatteryPower = BatteyPower;
            ViewBag.JSBatteryModel = BatteyModel;

            ViewBag.JSColgrid = GridV < 50.0 ?Break  : GirdPower <= 0 ? Closed : Connecting;
            ViewBag.JSColLoad = LoadConnected == false || LoadV < 50.0 ?Break  : Load <= 0 ? Closed:Connecting;
            ViewBag.JSColSolar = SPM90MConnected == false || solarV == 0 ?Break  : Solar <= 0 ? Closed:Connecting;
            ViewBag.JSColGenerator = GeneratorConnected == false && GeneratorEngineBatteryVoltage < 5 ?Break  : Generator <= 0 ? Closed:Connecting;
            ViewBag.JSColBattery = BattV == 0 ?Break  : Direction > 2 ? Closed:Connecting;

            ViewBag.JSNamegrid = GridV < 50 ? "市電(電壓不足)" : "市電";
            ViewBag.JSNameLoad = LoadConnected == false || LoadV < 50.0 ? "負載(電壓不足)" : "負載";
            ViewBag.JSNameSolar = SPM90MConnected == false || solarV == 0 ? "太陽能(離線)" : "太陽能";
            ViewBag.JSNameGenerator = GeneratorConnected == false && GeneratorEngineBatteryVoltage < 5 ? "發電機(離線)" : "發電機";
            ViewBag.JSNameBattery = BattV == 0 ? "電池(離線)" : "電池";
            #endregion
            #endregion

            return View();
        }

        public ActionResult Info(int StationCode=2)
        {
            Guid StationUUID = stationService.UUID(StationCode);
            string StationName = StationCode == 2 ? "霧台大武" : "泰武佳興";           
            ViewBag.AppTitle = "即時資訊("+ StationName + ")";
            ViewBag.EssTime = InfoChartsService.Readby(StationUUID, "LoadPower").UpdateTime;
            ChartData(StationUUID);
            InfoData(StationUUID);
            return View();
        }

        private void InfoData(Guid StationUUID)
        {
            InfoDatas InfoDatas = InfoDatasService.ReadUUID(StationUUID);

            #region Load
            string LoadInfo1 = "負載(離線)";
            double LoadInfo2 = 0, LoadInfo3 = 0, LoadInfo4 = 0, LoadInfo5 = 0, LoadInfo6 = 0, LoadInfo7 = 0, LoadInfo8 = 0, LoadInfo9 = 0;

            if (!string.IsNullOrEmpty(InfoDatas.LoadPower))
            {
                var LoadData = InfoDatas.LoadPower.Trim().Split('|');
                LoadInfo1 = string.IsNullOrEmpty(LoadData[0].Trim()) ? "負載(目前離線中)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(LoadData[0]).Ticks) ? "負載(目前離線中)" : "負載";
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

            #region Solar
            string solarInfo11 = "太陽能1", solarInfo21 = "太陽能2",solarInfoLogo = "太陽能 (離線)";
            double solarInfo12 = 0, solarInfo13 = 0, solarInfo14 = 0, solarInfo15 = 0;
            double solarInfo22 = 0, solarInfo23 = 0, solarInfo24 = 0, solarInfo25 = 0;

            if (!string.IsNullOrEmpty(InfoDatas.Solar))
            {
                var SolarData = InfoDatas.Solar.Trim().Split('$');
                var Solar1 = SolarData[0].Trim().Split('|');
                var Solar2 = SolarData[1].Trim().Split('|');

                solarInfoLogo = string.IsNullOrEmpty(Solar1[0].Trim()) ? "太陽能 (離線)" : (DateTime.Now.AddMinutes(-5) < DateTime.Parse(Solar1[0])) ? "太陽能" : "太陽能(離線)";

                solarInfo11 = string.IsNullOrEmpty(Solar1[0].Trim()) ? "太陽能1 (離線)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(Solar1[0]).Ticks) ? "太陽能1(離線)" : "太陽能1";
                solarInfo12 = Math.Round(Convert.ToDouble(Solar1[1]), 2);
                solarInfo13 = Math.Round(Convert.ToDouble(Solar1[2]), 2);
                solarInfo14 = Math.Round(Convert.ToDouble(Solar1[3]), 2);
                solarInfo15 = Math.Round(Convert.ToDouble(Solar1[4]), 2);

                solarInfo21 = string.IsNullOrEmpty(Solar2[0].Trim()) ? "太陽能2 (離線)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(Solar2[0]).Ticks) ? "太陽能2(離線)" : "太陽能2";
                solarInfo22 = Math.Round(Convert.ToDouble(Solar2[1]), 2);
                solarInfo23 = Math.Round(Convert.ToDouble(Solar2[2]), 2);
                solarInfo24 = Math.Round(Convert.ToDouble(Solar2[3]), 2);
                solarInfo25 = Math.Round(Convert.ToDouble(Solar2[4]), 2);

            }
            ViewBag.solarInfoLogo = solarInfoLogo;
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

            #region GridPower
            string gridInfo1 = "市電(離線)";
            double gridInfo2 = 0, gridInfo3 = 0, gridInfo4 = 0, gridInfo5 = 0, gridInfo6 = 0, gridInfo7 = 0, gridInfo8 = 0, gridInfo9 = 0;

            if (!string.IsNullOrEmpty(InfoDatas.GridPower))
            {
                var GridData = InfoDatas.GridPower.Trim().Split('|');
                gridInfo1 = string.IsNullOrEmpty(GridData[0].Trim()) ? "市電迴路(離線)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(GridData[0]).Ticks) ? "市電迴路(離線)" : "市電迴路";
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

            #region Generator
            string GenInfo1 = "發電機(離線)", GenInfo2 = "0", GenInfo3 = "0", GenInfo4 = "0", GenInfo5 = "0", GenInfo6 = "0", GenInfo7 = "0", GenInfo8 = "0", GenInfo9 = "0", GenInfo10 = "0", GenInfo11 = "0", GenInfo12 = "0", GenInfo13 = "離線", GenInfo14 = "0";

            if (!string.IsNullOrEmpty(InfoDatas.Generator))
            {
                var GenData = InfoDatas.Generator.Trim().Split('|');
                GenInfo1 = string.IsNullOrEmpty(GenData[0].Trim()) ? "發電機(離線)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(GenData[0]).Ticks) ? "發電機(離線)" : "發電機";
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

            #region Inverter
            string InvInfo1 = "逆變器(離線)", InvInfo2 = "故障模式", InvInfo3 = "0", InvInfo4 = "0", InvInfo5 = "0", InvInfo6 = "0", InvInfo7 = "0", InvInfo8 = "0", InvInfo9 = "0", InvInfo10 = "0", InvInfo11 = "0";

            if (!string.IsNullOrEmpty(InfoDatas.Generator))
            {
                var InvData = InfoDatas.Inverters.Trim().Split('|');
                InvInfo1 = string.IsNullOrEmpty(InvData[0].Trim()) ? "逆變器(離線)" : (DateTime.Now.AddMinutes(-5).Ticks > DateTime.Parse(InvData[0]).Ticks) ? "逆變器(離線)" : "逆變器";
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

            #region Battery
            string BatteryInfo11 = "1", BatteryInfo12 = "0", BatteryInfo13 = "0", BatteryInfo14 = "0", BatteryInfo15 = "0", BatteryInfo16 = "0", BatteryInfo17 = "離線";
            string BatteryInfo21 = "2", BatteryInfo22 = "0", BatteryInfo23 = "0", BatteryInfo24 = "0", BatteryInfo25 = "0", BatteryInfo26 = "0", BatteryInfo27 = "離線";
            string BatteryInfo31 = "3", BatteryInfo32 = "0", BatteryInfo33 = "0", BatteryInfo34 = "0", BatteryInfo35 = "0", BatteryInfo36 = "0", BatteryInfo37 = "離線";
            string BatteryInfo41 = "4", BatteryInfo42 = "0", BatteryInfo43 = "0", BatteryInfo44 = "0", BatteryInfo45 = "0", BatteryInfo46 = "0", BatteryInfo47 = "離線";
            string BatteryLogo = "電池(離線)";

            if (!string.IsNullOrEmpty(InfoDatas.Battery))
            {
                var BatteryData = InfoDatas.Battery.Trim().Split('$');
                var Battery1 = BatteryData[0].Trim().Split('|');
                var Battery2 = BatteryData[1].Trim().Split('|');
                var Battery3 = BatteryData[2].Trim().Split('|');
                var Battery4 = BatteryData[3].Trim().Split('|');

                BatteryLogo = string.IsNullOrEmpty(Battery1[0].Trim()) ? "電池(離線)" : (DateTime.Now.AddMinutes(-5).Ticks < DateTime.Parse(Battery1[0]).Ticks) ? "電池" : "電池(離線)";

                BatteryInfo12 = Battery1[1];
                BatteryInfo13 = Battery1[2];
                BatteryInfo14 = Battery1[3];
                BatteryInfo15 = Battery1[4];
                BatteryInfo16 = Battery1[5];
                BatteryInfo17 = Battery1[6];
         
                BatteryInfo22 = Battery2[1];
                BatteryInfo23 = Battery2[2];
                BatteryInfo24 = Battery2[3];
                BatteryInfo25 = Battery2[4];
                BatteryInfo26 = Battery2[5];
                BatteryInfo27 = Battery2[6];
            
                BatteryInfo32 = Battery3[1];
                BatteryInfo33 = Battery3[2];
                BatteryInfo34 = Battery3[3];
                BatteryInfo35 = Battery3[4];
                BatteryInfo36 = Battery3[5];
                BatteryInfo37 = Battery3[6];

                BatteryInfo42 = Battery4[1];
                BatteryInfo43 = Battery4[2];
                BatteryInfo44 = Battery4[3];
                BatteryInfo45 = Battery4[4];
                BatteryInfo46 = Battery4[5];
                BatteryInfo47 = Battery4[6];
            }

            ViewBag.BatteryLogo = BatteryLogo;
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
        }

        private void ChartData(Guid StationUUID)
        {
            ViewBag.Hour = InfoChartsService.ReadUUID(StationUUID).Hour-1;

            #region LoadChart       
            ViewBag.LoadAPP = InfoChartsService.Readby(StationUUID, "LoadPower").HourList.Trim();
            ViewBag.LoadHour = InfoChartsService.Readby(StationUUID, "LoadPower").Hour+9;
            #endregion LoadChart

            #region SolarChart        
            var Sun = InfoChartsService.Readby(StationUUID, "Solar").HourList.Trim().Split('|');
            ViewBag.Sun0 = Sun[0];
            ViewBag.Sun1 = Sun[1];
            ViewBag.SolarHour = InfoChartsService.Readby(StationUUID, "Solar").Hour+9;
            #endregion  SolarChart

            #region GridPowerChart          
            ViewBag.Grid = InfoChartsService.Readby(StationUUID, "GridPower").HourList.Trim();
            ViewBag.GridPowerHour = InfoChartsService.Readby(StationUUID, "GridPower").Hour+9;
            #endregion GridPowerChart

            #region GeneratorChart
            ViewBag.Generator = InfoChartsService.Readby(StationUUID, "Generator").HourList.Trim();
            ViewBag.GeneratorHour = InfoChartsService.Readby(StationUUID, "Generator").Hour+9;
            #endregion GeneratorChart

            #region BatteryChart
            ViewBag.BatteryData = InfoChartsService.Readby(StationUUID, "Battery").HourList.Trim();
            ViewBag.BatteryHour = InfoChartsService.Readby(StationUUID, "Battery").Hour+9;
            #endregion BatteryChart

            #region InvertersChart
            ViewBag.InverterData = InfoChartsService.Readby(StationUUID, "Inverters").HourList.Trim();
            ViewBag.InverterHour = InfoChartsService.Readby(StationUUID, "Inverters").Hour+9;
            #endregion Chart
        }

        #region 光采濕地
        public ActionResult GTIndex()
        {
            ViewBag.AppTitle = "即時資訊(林邊光采)";
            List<int> maxItem = new List<int>();

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/MonthlyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                maxItem.Add(Convert.ToInt32(FN[0].Substring(0, 6)));
            }
            HashSet<int> eachMonthly = new HashSet<int>(maxItem);
            ViewBag.maxMonthly = eachMonthly.Max();
            maxItem.Clear();

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                maxItem.Add(Convert.ToInt32(FN[0].Substring(0, 8)));
            }
            HashSet<int> eachDaily = new HashSet<int>(maxItem);
            ViewBag.maxDaily = eachDaily.Max();
            return View();
        }

        public ActionResult GTMonthlyReport()
        {
            DataPicker(1);
            ViewBag.ChkData = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GTMonthlyReport(FormCollection From)
        {
            string startDate = Request.Form["startDate"];
            DataPicker(1);
            ViewBag.ChkData = true;
            ViewBag.searchData = startDate;

            try
            {
                List<MonthlyReportData> RowData = new List<MonthlyReportData>();

                string fname = Path.Combine(Server.MapPath("~/Content/GuanTsai/MonthlyReport/"), "MONTHLYREPORT_SAMPLE_" + startDate + "01.xlsx");
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (ExcelPackage ep = new ExcelPackage(fs))
                    {
                        ExcelWorksheet sheet = ep.Workbook.Worksheets[1];//取得Sheet1
                        int RowId = 4;   // 因為有標題列，所以從第4列開始讀起
                        for (int i=RowId; i<=35; i++)
                        {
                            string cellValue = sheet.Cells[i, 1].Text;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                // 將資料放入UserListRowData中
                                RowData.Add(new MonthlyReportData()
                                {
                                    Day = cellValue,
                                    TotalGenerator = sheet.Cells[i, 2].Text,
                                    TotalPV = sheet.Cells[i, 3].Text,
                                    TotalLoad = sheet.Cells[i, 4].Text,
                                    TotalTPC = sheet.Cells[i, 5].Text
                                });
                            }
                        }

                        string TotalGeneratorChart = "";
                        string TotalPVChart = "";
                        string TotalLoadChart = "";
                        string TotalTPCChart = "";
                        List<int> Days = new List<int>();

                        foreach (var data in RowData.ToList())
                        {
                            if (data.Day.Substring(0, 1).Equals("2"))
                            {
                                Days.Add(DateTime.Parse(data.Day).Day);
                                TotalGeneratorChart += data.TotalGenerator.Trim() + ",";
                                TotalPVChart += data.TotalPV.Trim() + ",";
                                TotalLoadChart += data.TotalLoad.Trim() + ",";
                                TotalTPCChart += data.TotalTPC.Trim() + ",";
                            }
                        }
                        ViewBag.lastDay =Days.Last().ToString();
                        ViewBag.TotalGeneratorChart = TotalGeneratorChart.Substring(0, TotalGeneratorChart.Length - 1);
                        ViewBag.TotalPVChart = TotalPVChart.Substring(0, TotalPVChart.Length - 1);
                        ViewBag.TotalLoadChart = TotalPVChart.Substring(0, TotalPVChart.Length - 1);
                        ViewBag.TotalTPCChart = TotalTPCChart.Substring(0, TotalTPCChart.Length - 1).ToString();                             
                    }
                }//end using
                return View(RowData);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return View();
            }
        }

        public ActionResult GTDailyReport()
        {
            DataPicker(2);
            ViewBag.ChkData = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GTDailyReport(FormCollection From)
        {
            string daily = Request.Form["Day"];
            DataPicker(2);
            ViewBag.ChkData = true;
            ViewBag.searchData = daily;
            ViewBag.search = daily.Substring(0,4)+"-"+ daily.Substring(4, 2) + "-" + daily.Substring(6, 2);

            try
            {
                List<DailyReportData> RowData = new List<DailyReportData>();

                string fname = Path.Combine(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "DailyReport_sample_" + daily + ".xlsx");
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (ExcelPackage ep = new ExcelPackage(fs))
                    {
                        ExcelWorksheet sheet = ep.Workbook.Worksheets[1];//取得Sheet1


                        bool isLastRow = false;
                        int RowId = 4;   // 因為有標題列，所以從第4列開始讀起

                        do  // 讀取資料，直到讀到空白列為止
                        {
                            string cellValue = sheet.Cells[RowId, 1].Text;
                            if (string.IsNullOrEmpty(cellValue))
                            {
                                isLastRow = true;
                            }
                            else
                            {
                                // 將資料放入UserListRowData中
                                RowData.Add(new DailyReportData()
                                {
                                    Time = cellValue,
                                    Generator = sheet.Cells[RowId, 2].Text,
                                    HomeOnePower = sheet.Cells[RowId, 3].Text,
                                    HomeTwoPower = sheet.Cells[RowId, 4].Text,
                                    HomeThreePower = sheet.Cells[RowId, 5].Text,
                                    HomeFourPower = sheet.Cells[RowId, 6].Text,
                                    P7TimelyPower = sheet.Cells[RowId, 7].Text,
                                    P7Timely110VPower = sheet.Cells[RowId, 8].Text,
                                    P7Timely220VPower = sheet.Cells[RowId, 9].Text,
                                    P9TimelyPower = sheet.Cells[RowId, 10].Text,
                                    P9Timely110VPower = sheet.Cells[RowId, 11].Text,
                                    TotalPVTimelyPower = sheet.Cells[RowId, 12].Text,
                                    TotalLoad = sheet.Cells[RowId, 13].Text,
                                    TotalTPC = sheet.Cells[RowId, 14].Text
                                });
                                RowId += 1;
                            }
                        } while (!isLastRow);
                    }
                }//end using 

                string PVTotal=null, PV1 = null, PV2 = null, PV3 = null, PV4 = null;
                string P7 = null, P7110 = null, P7220 = null, P9 = null, P9110 = null;
                string Gen = null, Load = null, TPC = null;
                int i = 1;
                int count = RowData.Count;
                foreach (var data in RowData.ToList())
                {
                    if (i<count)
                    {
                        PVTotal += data.TotalPVTimelyPower == "????" ? "0.00," : data.TotalPVTimelyPower.Trim() + ",";
                        PV1 += data.HomeOnePower == "????" ? "0.00," : data.HomeOnePower.Trim() + ",";
                        PV2 += data.HomeTwoPower == "????" ? "0.00," : data.HomeTwoPower.Trim() + ",";
                        PV3 += data.HomeThreePower == "????" ? "0.00," : data.HomeThreePower.Trim() + ",";
                        PV4 += data.HomeFourPower == "????" ? "0.00," : data.HomeFourPower.Trim() + ",";
                        P7 += data.P7TimelyPower == "????" ? "0.00," : data.P7TimelyPower.Trim() + ",";
                        P7110 += data.P7Timely110VPower == "????" ? "0.00," : data.P7Timely110VPower.Trim() + ",";
                        P7220 += data.P7Timely220VPower == "????" ? "0.00," : data.P7Timely220VPower.Trim() + ",";
                        P9 += data.P9TimelyPower == "????" ? "0.00," : data.P9TimelyPower.Trim() + ",";
                        P9110 += data.P9Timely110VPower == "????" ? "0.00," : data.P9Timely110VPower.Trim() + ",";
                        Gen += data.Generator == "????" ? "0.00," : data.Generator.Trim() + ",";
                        Load += data.TotalLoad == "????" ? "0.00," : data.TotalLoad.Trim() + ",";
                        TPC += data.TotalTPC == "????" ? "0.00," : data.TotalTPC.Trim() + ",";
                    }
                    i++;
                }

                ViewBag.PVTotal = PVTotal.Substring(0, PVTotal.Length - 1);
                ViewBag.PV1 = PV1.Substring(0, PV1.Length - 1);
                ViewBag.PV2 = PV2.Substring(0, PV2.Length - 1);
                ViewBag.PV3 = PV3.Substring(0, PV3.Length - 1);
                ViewBag.PV4 = PV4.Substring(0, PV4.Length - 1);
                ViewBag.P7 = P7.Substring(0, P7.Length - 1);
                ViewBag.P7110 = P7110.Substring(0, P7110.Length - 1);
                ViewBag.P7220 = P7220.Substring(0, P7220.Length - 1);
                ViewBag.P9 = P9.Substring(0, P9.Length - 1);
                ViewBag.P9110 = P9110.Substring(0, P9110.Length - 1);
                ViewBag.Gen = Gen.Substring(0, Gen.Length - 1);
                ViewBag.Load = Load.Substring(0, Load.Length - 1);
                ViewBag.TPC = TPC.Substring(0, TPC.Length - 1);
                return View(RowData);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return View();
            }
        }

        private void DataPicker(int i)
        {
            HashSet<string> datepicker = new HashSet<string>();

            string path =(i==1)?"~/Content/GuanTsai/MonthlyReport/": "~/Content/GuanTsai/DailyReport/";

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath(path), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                datepicker.Add(FN[0].Substring(0, (i == 1) ? 6 : 8));
            }

            if (i == 1)
            {
                ViewBag.maxData = datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) +"-01";
                ViewBag.minData = datepicker.Min().Substring(0, 4) + "-" + datepicker.Min().Substring(4, 2) + "-01";
                ViewBag.searchData = datepicker.Max().Substring(0, 4).ToString() + datepicker.Max().Substring(4, 2) .ToString();
            } else if (i==2)
            {
                ViewBag.maxData =  datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) + "-" + datepicker.Max().Substring(6, 2);
                ViewBag.minData =  datepicker.Min().Substring(0, 4) + "-" + datepicker.Min().Substring(4, 2) + "-" + datepicker.Min().Substring(6, 2);
                ViewBag.search = datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) + "-" + datepicker.Max().Substring(6, 2);
                ViewBag.searchData = datepicker.Max().Substring(0, 4) + datepicker.Max().Substring(4, 2) + datepicker.Max().Substring(6, 2);
            }
        }
        #endregion

        #region 異常Abnormal
        public ActionResult Abnormal(int StationNum=2)
        {
            Guid StationID = stationService.StationID(StationNum);
            StationList(StationID);
            ViewBag.SID = StationID;

            List<Alart> alartList = alartService.ReadTimeList(DateTime.Today, DateTime.Today.AddDays(1), StationID);

            int AlartCount = 0;
            int BatteryCount = 0;
            int SolartCount = 0;
            int LoadCount = 0;
            int GenCount = 0;
            int InvCount = 0;
            var StationData = alartList.GroupBy(x => x.StationID);
            foreach (var Station in StationData)
            {
                var Items = Station.GroupBy(i => i.AlartTypeID);
                foreach (var item in Items)
                {
                    string TypeName = alarttypeService.ReadID(item.Key).AlartTypeName;
                    var Contexts = item.GroupBy(c => c.AlartContext);

                    foreach (var Item in Contexts)
                    {
                        var Lists = Contexts.Where(k => k.Key == Item.Key).SelectMany(group => group).OrderBy(t => t.StartTimet).ToList();

                        DateTime TimeFirst = DateTime.Now;
                        DateTime TimeRecycle = DateTime.Now;
                        DateTime TimeEnd = DateTime.Now;
                        DateTime STime = DateTime.Now;

                        int x = 0;
                        int c = 1;
                        int count = Lists.Count();
                        foreach (var i in Lists)
                        {
                            if (x == 0)
                            {
                                STime = i.StartTimet;
                                TimeFirst = i.StartTimet;
                                TimeRecycle = i.StartTimet;
                            }
                            else if (x == 1)
                            {
                                STime = TimeRecycle;
                                TimeFirst = TimeRecycle;
                            }

                            TimeEnd = i.StartTimet;

                            TimeSpan ts1 = new TimeSpan(TimeFirst.Ticks);
                            TimeSpan ts2 = new TimeSpan(TimeEnd.Ticks);
                            int tsMin = ts2.Subtract(ts1).Duration().Minutes;

                            if (tsMin > 5 || c == count)
                            {
                                if (TypeName.Equals("市電")) AlartCount++;
                                if (TypeName.Equals("電池")) BatteryCount++;
                                if (TypeName.Equals("太陽能")) SolartCount++;
                                if (TypeName.Equals("負載")) LoadCount++;
                                if (TypeName.Equals("發電機")) GenCount++;
                                if (TypeName.Equals("逆變器")) InvCount++;
                                x = 1;
                                TimeRecycle = i.StartTimet;
                            }
                            else
                            {
                                TimeFirst = i.StartTimet;
                                x++;
                            }
                            c++;
                        }
                    }
                }
            }

            ViewBag.AbGrid = AlartCount;
            ViewBag.AbBattery = BatteryCount;
            ViewBag.AbSolar = SolartCount;
            ViewBag.AbLoad = LoadCount;
            ViewBag.AbGen = GenCount;
            ViewBag.AbInv = InvCount;

            string TakeData = "btn btn-danger btn-lg btn-block";
            string NotData = "btn btn-secondary btn-lg btn-block disabled";

            ViewBag.AbGridClass = AlartCount == 0 ?NotData: TakeData;
            ViewBag.AbBatteryClass = BatteryCount == 0 ? NotData: TakeData;
            ViewBag.AbSolarClass = SolartCount == 0 ? NotData: TakeData;
            ViewBag.AbLoadClass = LoadCount == 0 ? NotData: TakeData;
            ViewBag.AbGenClass = GenCount == 0 ? NotData: TakeData;
            ViewBag.AbInvClass = InvCount == 0 ? NotData: TakeData;

            ViewBag.RangeStart = DateTime.Today.ToShortDateString();
            ViewBag.RangeEnd = DateTime.Today.ToShortDateString() ;
            ViewBag.Range = "區間:" + DateTime.Today.ToShortDateString() + "-" + DateTime.Today.ToShortDateString();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Abnormal(FormCollection From)
        {
            string SE = Request.Form["range_date"];            //查詢時間區間
            Guid StationID =Guid.Parse(Request.Form["Statons"]);
            StationList(StationID);
            ViewBag.SID = StationID;
            //分割字串
            char cc = '-';
            string[] Day = SE.Split(cc);
            //區間
            DateTime Start = Convert.ToDateTime(Day[0]);
            DateTime End = Convert.ToDateTime(Day[1]);

            List<Alart> alartList = alartService.ReadTimeList(Start, End.AddDays(1), StationID);
            int AlartCount = 0, BatteryCount = 0,SolartCount = 0, LoadCount = 0, GenCount = 0, InvCount = 0;
            var StationData = alartList.GroupBy(s => s.StationID);
            foreach (var Station in StationData)
            {
                var Items = Station.GroupBy(i => i.AlartTypeID);
                foreach (var item in Items)
                {
                    string TypeName = alarttypeService.ReadID(item.Key).AlartTypeName;
                    var Contexts = item.GroupBy(c => c.AlartContext);

                    foreach (var Item in Contexts)
                    {
                        var Lists = Contexts.Where(k => k.Key == Item.Key).SelectMany(group => group).OrderBy(t => t.StartTimet).ToList();

                        DateTime TimeFirst = DateTime.Now;
                        DateTime TimeRecycle = DateTime.Now;
                        DateTime TimeEnd = DateTime.Now;
                        DateTime STime = DateTime.Now;

                        int x = 0;
                        int c = 1;
                        int count = Lists.Count();
                        foreach (var i in Lists)
                        {
                            if (x == 0)
                            {
                                STime = i.StartTimet;
                                TimeFirst = i.StartTimet;
                                TimeRecycle = i.StartTimet;
                            }
                            else if (x == 1)
                            {
                                STime = TimeRecycle;
                                TimeFirst = TimeRecycle;
                            }

                            TimeEnd = i.StartTimet;

                            TimeSpan ts1 = new TimeSpan(TimeFirst.Ticks);
                            TimeSpan ts2 = new TimeSpan(TimeEnd.Ticks);
                            int tsMin = ts2.Subtract(ts1).Duration().Minutes;

                            if (tsMin > 5 || c == count)
                            {
                                if (TypeName.Equals("市電")) AlartCount++;
                                if (TypeName.Equals("電池")) BatteryCount++;
                                if (TypeName.Equals("太陽能")) SolartCount++;
                                if (TypeName.Equals("負載")) LoadCount++;
                                if (TypeName.Equals("發電機")) GenCount++;
                                if (TypeName.Equals("逆變器")) InvCount++;
                                x = 1;
                                TimeRecycle = i.StartTimet;
                            }
                            else
                            {
                                TimeFirst = i.StartTimet;
                                x++;
                            }
                            c++;
                        }
                    }
                }
            }

            #region ViewBag
            ViewBag.RangeStart = Start.ToShortDateString();
            ViewBag.RangeEnd = End.ToShortDateString();
            ViewBag.Range = "區間:" + Start.ToShortDateString() + "-" + End.ToShortDateString();

            ViewBag.AbGrid = AlartCount;
            ViewBag.AbBattery = BatteryCount;
            ViewBag.AbSolar = SolartCount;
            ViewBag.AbLoad = LoadCount;
            ViewBag.AbGen = GenCount;
            ViewBag.AbInv = InvCount;

            string TakeData = "btn btn-danger btn-lg btn-block";
            string NotData = "btn btn-secondary btn-lg btn-block disabled";

            ViewBag.AbGridClass = AlartCount == 0 ? NotData: TakeData;
            ViewBag.AbBatteryClass = BatteryCount == 0 ? NotData: TakeData;
            ViewBag.AbSolarClass = SolartCount == 0 ? NotData: TakeData;
            ViewBag.AbLoadClass = LoadCount == 0 ? NotData: TakeData;
            ViewBag.AbGenClass = GenCount == 0 ? NotData: TakeData;
            ViewBag.AbInvClass = InvCount == 0 ? NotData: TakeData;

            #endregion

            return View();
        }

        public ActionResult AbList(string name, string sday,string eday,Guid StationID, int page = 1)
        {
            Guid typesID = Guid.Parse( "96DE23DB-34E3-E811-BE29-0C9D925E499C");//所有異常，以資料庫為主
            DateTime Start = Convert.ToDateTime(sday);
            DateTime End = Convert.ToDateTime(eday);   
            string equip = "未知";

            switch (name)
            {
                case "Load": typesID=alarttypeService.ID(5).Id; equip = "負載"; break;
                case "Solar": typesID = alarttypeService.ID(4).Id; equip = "太陽能"; break;
                case "Grid": typesID = alarttypeService.ID(2).Id; equip = "市電"; break;
                case "Gen": typesID = alarttypeService.ID(6).Id ; equip = "發電機"; break;
                case "Battery": typesID = alarttypeService.ID(3).Id ; equip = "電池"; break;
                case "Inv": typesID = alarttypeService.ID(7).Id; equip = "逆變器"; break;
                default: break;
            }
            ViewBag.equip = equip;
            ViewBag.RangeStart = Start.ToShortDateString();
            ViewBag.RangeEnd = End.ToShortDateString();
            ViewBag.name = name;
            ViewBag.SID= StationID;

            List<AlartGroup> grp = new List<AlartGroup>();
            List<Alart> alartList = alartService.ReadListBy(Start, End.AddDays(1), typesID, StationID);

            var Items = alartList.OrderBy(s => s.StartTimet).GroupBy(i => i.AlartContext).ToList();//依異常分群
            foreach (var Item in Items)
            {
                string itemName = Item.Key;
                var Lists = Items.Where(k => k.Key == Item.Key).SelectMany(group => group).OrderBy(t => t.StartTimet).ToList();

                DateTime TimeFirst = DateTime.Now;
                DateTime TimeRecycle = DateTime.Now;
                DateTime TimeEnd = DateTime.Now;
                DateTime STime = DateTime.Now;

                int x = 0;
                int c = 1;
                int count = Lists.Count();
                foreach (var i in Lists)
                {
                    if (x == 0)
                    {
                        STime = i.StartTimet;
                        TimeFirst = i.StartTimet;
                        TimeRecycle = i.StartTimet;
                    }
                    else if (x == 1)
                    {
                        STime = TimeRecycle;
                        TimeFirst = TimeRecycle;
                    }

                    TimeEnd = i.StartTimet;

                    TimeSpan ts1 = new TimeSpan(TimeFirst.Ticks);
                    TimeSpan ts2 = new TimeSpan(TimeEnd.Ticks);
                    int tsMin = ts2.Subtract(ts1).Duration().Minutes;

                    if (tsMin > 5 || c == count)
                    {
                        grp.Add(new AlartGroup()
                        {
                            Name = equip,
                            Item = itemName,
                            Start = STime,
                            End = TimeFirst
                        });
                        x = 1;
                        TimeRecycle = i.StartTimet;
                    }
                    else
                    {
                        TimeFirst = i.StartTimet;
                        x++;
                    }

                    c++;
                }
            }

            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = grp.ToPagedList(currentPage,5);
            return View(result);
        }

        #endregion

        #region Bulletin

        public ActionResult Bulletin(int page = 1)
        {
            ViewBag.RangeStart = DateTime.Today.ToShortDateString();
            ViewBag.RangeEnd = DateTime.Today.ToShortDateString();

            //一開始抓取全部資料
            List<Bulletin> bulletins = bulletinService.ReadAllView().ToList();
            List<Bulletins> bulls = new List<Bulletins>();
            foreach (var bull in bulletins)
            {
                bulls.Add(new Bulletins
                {
                    CreateDate = bull.CreateDate,
                    Title = bull.title,
                    Context = bull.context,
                    Orgin = orginService.ReadID(bull.OrginID).OrginName.ToString().Trim(),
                    Disable = bull.Disabled
                });
            }
            //分頁
            int currentPage = page < 1 ? 1 : page;
            IPagedList<Bulletins> result = bulls.ToPagedList(currentPage, PageSizes());
            return View(result);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bulletin(FormCollection From, int page = 1)
        {
            //查詢時間區間
            var SE = Request.Form["range_date"];
            //分割字串
            char x = '-';
            string[] Day = SE.Split(x);
            //區間
            DateTime Start = Convert.ToDateTime(Day[0]);
            DateTime End = Convert.ToDateTime(Day[1]);

            ViewBag.RangeStart = Start.ToShortDateString();
            ViewBag.RangeEnd = End.ToShortDateString();
            //寫入表單
            List<Bulletin> bulletins = (Start == End) ? bulletinService.ReadAll().ToList() : bulletinService.ReadListBy(Start, End.AddDays(1)).ToList();
            List<Bulletins> bulls = new List<Bulletins>();

            foreach (var bull in bulletins)
            {
                bulls.Add(new Bulletins
                {
                    CreateDate = bull.CreateDate,
                    Title = bull.title,
                    Context = bull.context,
                    Orgin = orginService.ReadID(bull.OrginID).OrginName.ToString().Trim(),
                    Disable = bull.Disabled
                });
            }
            //分頁
            int currentPage = page < 1 ? 1 : page;
            IPagedList<Bulletins> result = bulls.ToPagedList(currentPage, PageSizes());

            if (Start == End && Start==DateTime.Today)
            {
                ViewBag.Range = "全部資料";
            }
            else
            {
                ViewBag.Range = "區間:" + Start.ToShortDateString() + "到" + End.ToShortDateString();
            }

            return View(result);
        }

        #endregion



        private void StationList(Guid SID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<Station> stations = stationService.ReadAll().OrderBy(x => x.StationCode).ToList();
            stations.ForEach(x =>
            {
                if (x.StationCode==2 || x.StationCode==6)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = x.StationName,
                        Value = x.Id.ToString(),
                        Selected = x.Id.Equals(SID)
                    });
                }
            });
            ViewBag.station = items;
        }
    }
}