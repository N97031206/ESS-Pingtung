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
using System.Runtime.Caching;

namespace Web.Areas.APP.Controllers
{
    public class APPController : Controller
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

        // GET: APP/APP
        public ActionResult Index()
        {
            double soc = BatteryService.totalSOC() * 0.2;
            double LoadWatt = LoadPowerService.ReadNow().Watt_t;
            //可用總電量(度) = SOC(%) *20kWh(額定容量)
            ViewBag.Demand =Math.Round(soc, 2).ToString()+"kWh";
            //可用電時數(H) = 可用總電量(度) / 負載實功率(kW)
            ViewBag.RemainTime = LoadWatt <= 0 ? "無負載" : Math.Round(soc / LoadWatt, 1).ToString() + "小時";

            return View();
        }

        public ActionResult Info()
        {
            ChartData();

            //取最新時間
            var ReadNow = ESSObjecterService.ReadNow();
            string EssTime = ReadNow.CreateTime.ToString();
            ViewBag.EssTime = EssTime;

            #region Load
            List<LoadPower> loadPowers = new List<LoadPower>();
            var LoadReadNow= ReadNow.LoadPowerIDs;
            if (LoadReadNow != null)
            {
                var LoadID = LoadReadNow.Split('|');
                foreach (var x in LoadID)
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        Guid id = Guid.Parse(x.Trim());
                        loadPowers.Add(LoadPowerService.ReadByID(id));
                    }
                }
            }
            LoadPower lo = loadPowers.Where(x => x.index == 2).FirstOrDefault();
            ViewBag.LoadInfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "負載" : "負載(目前離線中)";
            ViewBag.LoadInfo2 = Math.Round(lo.Vavg, 2);
            ViewBag.LoadInfo3 = Math.Round(lo.Isum, 2);
            ViewBag.LoadInfo4 = Math.Round(lo.Watt_t / 1000.0, 2);
            ViewBag.LoadInfo5 = Math.Round(lo.Var_t / 1000.00, 2);
            ViewBag.LoadInfo6 = Math.Round(lo.VA_t / 1000.0, 2);
            ViewBag.LoadInfo7 = Math.Round(lo.PF_t, 2);
            ViewBag.LoadInfo8 = Math.Round(lo.Frequency, 2);
            ViewBag.LoadInfo9 = Math.Round(lo.MinuskWHt, 2);
            #endregion

            #region Solar
            //取相關資料
            List<Inverter> inverters = new List<Inverter>();
            var SolarReadNow = ReadNow.InvertersIDs;

            if (SolarReadNow != null)
            {
                var spmID = SolarReadNow.Split('|');
                foreach (var x in spmID)
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        Guid id = Guid.Parse(x.Trim());
                        inverters.Add(InverterService.ReadByID(id));
                    }
                }
            }

            ViewBag.SolarInfo = inverters;
            #endregion

            #region GridPower
            List<GridPower> gridPowers = new List<GridPower>();
            var GridReadNow = ReadNow.GridPowerIDs;
            if (GridReadNow != null)
            {
                var gridID = GridReadNow.Split('|');
                foreach (var x in gridID)
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        Guid id = Guid.Parse(x.Trim());
                        gridPowers.Add(GridPowerService.ReadByID(id));
                    }
                }
            }
            GridPower gp = gridPowers.Where(x => x.index == 0).FirstOrDefault();
            ViewBag.gridInfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "市電迴路" : "市電迴路(目前離線中)";
            ViewBag.gridInfo2 = string.Format("{0:#,0.00}", gp.Vavg);
            ViewBag.gridInfo3 = string.Format("{0:#,0.00}", gp.Isum);
            ViewBag.gridInfo4 = string.Format("{0:#,0.00}", gp.Watt_t / 1000.0);
            ViewBag.gridInfo5 = string.Format("{0:#,0.00}", gp.Var_t / 1000.00);
            ViewBag.gridInfo6 = string.Format("{0:#,0.00}", gp.VA_t / 1000.0);
            ViewBag.gridInfo7 = string.Format("{0:#,0.00}", gp.PF_t);
            ViewBag.gridInfo8 = string.Format("{0:#,0.00}", gp.Frequency);
            ViewBag.gridInfo9 = string.Format("{0:#,0.00}", gp.MinuskWHt);
            #endregion

            #region GeneratorChart
            string Info1 = "發電機(目前離線中)",Info13 = "離線", Info2 = "0", Info3 = "0", Info4 = "0", Info5 = "0", Info6 = "0", Info7 = "0", Info8 = "0", Info9 = "0", Info10 = "0", Info11 = "0", Info12 = "0", Info14 = "0", Info15 = "0";

            List<Generator> generators = new List<Generator>();
            var GenReadNow = ReadNow.GeneratorIDs;
            if (GenReadNow != null)
            {
                var generID = GenReadNow.Split('|');
                foreach (var x in generID)
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        Guid id = Guid.Parse(x.Trim());
                        generators.Add(GeneratorService.ReadByID(id));
                    }
                }

                foreach (var gen in generators)
                {
                    Info1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "發電機" + gen.index.ToString() : "發電機" + gen.index.ToString() + "(目前離線中)";
                    Info2 = string.Format("{0:#,0}", gen.FuleLevel);
                    Info3 = string.Format("{0:#,0.00}", gen.L1Nvoltage);
                    Info4 = string.Format("{0:#,0.00}", gen.L2Nvoltage);
                    Info5 = string.Format("{0:#,0.00}", gen.L3Nvoltage);
                    Info6 = string.Format("{0:#,0.00}", gen.L1current);
                    Info7 = string.Format("{0:#,0.00}", gen.L2current);
                    Info8 = string.Format("{0:#,0.00}", gen.L3current);
                    Info9 = string.Format("{0:#,0.00}", gen.totalwatts / 1000);
                    Info10 = string.Format("{0:#,0.00}", gen.averagepowerfactor);
                    Info11 = string.Format("{0:#,0.00}", gen.positiveKWhours);
                    Info12 = string.Format("{0:#,0.00}", gen.negativeKWhours);
                    Info13 = gen.ControlStatus.Equals("true") ? "啟動中" : "已關閉";
                    Info14 = string.Format("{0:#,0.00}", gen.AvailabilityEnergy);
                    Info15 = string.Format("{0:#,0.00}", gen.AvailabilityHour);
                }
            }

            ViewBag.GenInfo1 = Info1;
            ViewBag.GenInfo2 = Info2 ;
            ViewBag.GenInfo3 = Info3;
            ViewBag.GenInfo4 = Info4;
            ViewBag.GenInfo5 = Info5;
            ViewBag.GenInfo6 = Info6;
            ViewBag.GenInfo7 = Info7;
            ViewBag.GenInfo8 = Info8;
            ViewBag.GenInfo9 = Info9;
            ViewBag.GenInfo10 = Info10;
            ViewBag.GenInfo11 = Info11;
            ViewBag.GenInfo12 = Info12;
            ViewBag.GenInfo13 = Info13;
            ViewBag.GenInfo14 = Info14;
            ViewBag.GenInfo15 = Info15;
            #endregion

            #region Inverter
            List<Inverter> inverInfo = new List<Inverter>();
            var InverReadNow = ReadNow.InvertersIDs;
            var invID = InverReadNow.Split('|');
            if (InverReadNow != null)
            {
                foreach (var x in invID)
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        Guid id = Guid.Parse(x.Trim());
                        inverInfo.Add(InverterService.ReadByID(id));
                    }
                }
            }

            foreach(var inv in inverInfo)
            {
                ViewBag.Invnfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "逆變器" : "逆變器(目前離線中)";
                ViewBag.InvInfo2 = inv.DeviceMode.Trim();
                if (Info2 == "P") { Info2 = "Power On Mode"; }
                else if (Info2 == "S") { Info2 = "Standby Mode"; }
                else if (Info2 == "L") { Info2 = "Line Mode"; }
                else if (Info2 == "B") { Info2 = "Battery Mode"; }
                else if (Info2 == "F") { Info2 = "Fault Mode"; }
                else if (Info2 == "H") { Info2 = "Power Saving Mode"; }
                else { Info2 = "Unknown Mode"; }
                ViewBag.InvInfo3 = string.Format("{0:#,0.00}", inv.GridVoltage);
                ViewBag.InvInfo4 = string.Format("{0:#,0.00}", inv.GridFrequency);
                ViewBag.InvInfo5 = string.Format("{0:#,0.00}", inv.AC_OutputVoltage);
                ViewBag.InvInfo6 = string.Format("{0:#,0.00}", inv.AC_OutputFrequency);
                ViewBag.InvInfo7 = string.Format("{0:#,0.00}", inv.ParallelInformation_TotalOutputActivePower.Split('|').ToList().Sum(x => x.IsEmpty() ? 0 : Convert.ToDouble(x)) / 1000.0);
                ViewBag.InvInfo8 = string.Format("{0:#,0.00}", inv.BatteryVoltage);
                ViewBag.InvInfo9 = string.Format("{0:#,0.00}", inv.BatteryCapacity);
                ViewBag.InvInfo10 = string.Format("{0:#,0.00}", inv.PV_InputVoltage);
                ViewBag.InvInfo11 = string.Format("{0:#,0.00}", inv.ParallelInformation_TotalChargingCurrent.Split('|').ToList().Sum(x => x.IsEmpty() ? 0 : Convert.ToDouble(x)) / 1000.0);
            }

            #endregion

            #region Battery

            //取相關資料
            var batteryID = ReadNow.BatteryIDs.Split('|').ToList();
            List<Battery> batteries = new List<Battery>();
            batteryID.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x))
                {
                    batteries.Add(BatteryService.ReadByID(Guid.Parse(x.Trim())));
                }
            });
            ViewBag.BatteryInfo = batteries;

            #endregion

            return View();
        }

        private void ChartData()
        {
            DateTime starttime = DateTime.Today.AddHours(-8);
            string Data = null;
            List<double> sum = new List<double>();
            List<double> sum1 = new List<double>();
            List<double> sum2 = new List<double>();

            #region LoadChart
            //資料       
            Data = null;
            string Load1 = null;
            sum1.Clear();
            sum2.Clear();
            starttime = starttime.AddHours(-1);
            var LoadDate = LoadPowerService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.index == 2).ToList();     //負載迴路一
            for (int i = 0; i <= 24; i++)
            {
                var count = LoadDate.Where(x => x.date_Time >= starttime && x.date_Time < starttime.AddHours(1)).ToList();
                if (i > 0)
                {
                    //負載迴路一
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -1 * j;
                        j++;
                        int k2 = -1 * j;
                        count = LoadDate.Where(x => x.date_Time >= starttime.AddMinutes(k2) && x.date_Time < starttime.AddMinutes(k1)).ToList();
                    }
                    double g1 = count.Average(x => x.kWHt);
                    double gs1 = g1 - sum1.Last();
                    Load1 += Convert.ToString(Math.Round(gs1 < 0 ? 0 : gs1, 2));
                    Load1 += (i < 24) ? "," : "";
                    sum1.Add(g1);
                }
                else
                {
                    //負載迴路一
                    int j = 1;
                    while (count.Count < 1)
                    {
                        int k1 = -1 * j;
                        j++;
                        int k2 = -1 * j;
                        count = LoadDate.Where(x => x.date_Time >= starttime.AddMinutes(k2) && x.date_Time < starttime.AddMinutes(k1)).ToList();
                    }
                    double g1 = count.Average(x => x.kWHt);
                    sum1.Add(g1);
                }
                starttime = starttime.AddHours(1);
            }
            LoadDate.Clear();
            //組圖表資料
            ViewBag.LoadAPP = Load1;
            #endregion LoadChart

            #region SolarChart
            //資料
            Data = null;
            string sun0 = null, sun1 = null;
            sum.Clear();
            var SolarDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
            for (int i = 0; i < 24; i++)
            {
                var count = SolarDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddHours(1)).ToList();
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
                starttime = starttime.AddHours(1);
            }
            SolarDate.Clear();
            //組圖表資料
            ViewBag.Sun0 = sun0;
            ViewBag.Sun1 = sun1;
            #endregion  SolarChart

            #region GridPowerChart          
            //資料       
            starttime = DateTime.Today.AddHours(-8);
            Data = null;
            string Grid = null;
            sum1.Clear();
            sum2.Clear();
            starttime = starttime.AddHours(-1);
            var GridPowerDate = GridPowerService.ReadByInfoList(starttime, starttime.AddDays(1)).Where(x => x.index == 0).ToList();
            for (int i = 0; i <= 24; i++)
            {
                var count = GridPowerDate.Where(x => x.date_time >= starttime && x.date_time < starttime.AddHours(1)).ToList();
                if (i > 0)
                {
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -1 * j;
                        j++;
                        int k2 = -1 * j;
                        count = GridPowerDate.Where(x => x.date_time >= starttime.AddHours(k2) && x.date_time < starttime.AddHours(k1)).ToList();
                    }
                    double g1 = count.Average(x => x.kWHt);
                    double gs1 = g1 - sum1.Last();
                    Grid += Math.Round(gs1 < 0 ? 0 : gs1, 2) + ",";
                    sum1.Add(g1);
                }
                else//第一筆資料
                {
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -1 * j;
                        j++;
                        int k2 = -1 * j;
                        count = GridPowerDate.Where(x => x.date_time >= starttime.AddHours(k2) && x.date_time < starttime.AddHours(k1)).ToList();
                    }
                    double g1 = count.Average(x => x.kWHt);
                    sum1.Add(g1);
                }
                starttime = starttime.AddHours(1);
            }
            GridPowerDate.Clear();
            //組圖表資料
            ViewBag.Grid = Grid;
            #endregion GridPowerChart

            #region GeneratorChart
            //資料
            starttime = DateTime.Today.AddHours(-8);
            Data = null;
            string Generqtor1 = null;
            sum1.Clear();
            var GeneratorDate = GeneratorService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
            for (int i = 0; i <= 24; i++)
            {
                var count = GeneratorDate.Where(x => x.UpdateTime >= starttime && x.UpdateTime < starttime.AddHours(1) && x.index == 0).ToList();
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
                starttime = starttime.AddHours(1);
            }
            GeneratorDate.Clear();
            //組圖表資料
            ViewBag.Generator = Generqtor1;
            #endregion GeneratorChart


            #region BatteryChart
            //資料
            Data = null;
            starttime = DateTime.Today.AddHours(-8);
            var BatteryDate = BatteryService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
            for (int i = 0; i < 24; i++)
            {
                var count = BatteryDate.Where(x => x.updateTime >= starttime && x.updateTime < starttime.AddHours(1)).ToList();
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
                starttime = starttime.AddHours(1);
            }
            BatteryDate.Clear();
            //組圖表資料
            ViewBag.BatteryData = Data;
            #endregion BatteryChart





            #region InvertersChart
            //資料
            starttime = DateTime.Today.AddHours(-8);
            Data = null;
            sum.Clear();
            var InvertersDate = InverterService.ReadByInfoList(starttime, starttime.AddDays(1)).ToList();
            for (int i = 0; i < 24; i++)
            {
                var count = InvertersDate.Where(x => x.CreateTime >= starttime && x.CreateTime < starttime.AddHours(1)).ToList();
                Data += string.Format("{0:N2},",
                    (count.Count == 0) ? 0 :
                    (count.Average(x => x.ParallelInformation_TotalOutputActivePower
                    .Split('|').ToList()
                    .Sum(y => y.IsEmpty() ? 0 : Convert.ToDouble(y) / 1000.0)))).Trim();
                starttime = starttime.AddHours(1);
            }
            InvertersDate.Clear();
            //組圖表資料
            ViewBag.InverterData = Data;
            #endregion Chart


        }


        public ActionResult History()
        {
            return View();
        }

        public ActionResult GuanTsai()
        {
            return View();
        }

        #region Abnormal
        public ActionResult Abnormal()
        {

            List<Alart> alartList = alartService.ReadAll().Where(x => x.StartTimet >= DateTime.Today.AddHours(-8)).ToList();
            int GridnAlartCount = alartList.Count(x => x.AlartType.AlartTypeCode == 2);
            int GridnBatteryCount = alartList.Count(x => x.AlartType.AlartTypeCode == 3);
            int GridnSolartCount = alartList.Count(x => x.AlartType.AlartTypeCode == 4);
            int GridnLoadCount = alartList.Count(x => x.AlartType.AlartTypeCode == 5);
            int GridnGenCount = alartList.Count(x => x.AlartType.AlartTypeCode == 6);
            int GridnInvCount = alartList.Count(x => x.AlartType.AlartTypeCode == 7);

            ViewBag.AbGrid = GridnAlartCount;
            ViewBag.AbBattery = GridnBatteryCount;
            ViewBag.AbSolar = GridnSolartCount;
            ViewBag.AbLoad = GridnLoadCount;
            ViewBag.AbGen = GridnGenCount;
            ViewBag.AbInv = GridnInvCount;

            ViewBag.AbGridClass = GridnAlartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbBatteryClass = GridnBatteryCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbSolarClass = GridnSolartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbLoadClass = GridnLoadCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbGenClass = GridnGenCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbInvClass = GridnInvCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";

            ViewBag.RangeStart = DateTime.Today.ToShortDateString();
            ViewBag.RangeEnd = DateTime.Today.ToShortDateString() ;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Abnormal(FormCollection From)
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

            DateTime S = Start;
            DateTime E= End.AddDays(1);

            List<Alart> alartList = alartService.ReadAll().Where(y=> y.StartTimet >= S &&  y.StartTimet<E).ToList();

            int GridnAlartCount = alartList.Count(y => y.AlartType.AlartTypeCode == 2);
            int GridnBatteryCount = alartList.Count(y => y.AlartType.AlartTypeCode == 3);
            int GridnSolartCount = alartList.Count(y => y.AlartType.AlartTypeCode == 4);
            int GridnLoadCount = alartList.Count(y => y.AlartType.AlartTypeCode == 5);
            int GridnGenCount = alartList.Count(y => y.AlartType.AlartTypeCode == 6);
            int GridnInvCount = alartList.Count(y => y.AlartType.AlartTypeCode == 7);

            ViewBag.AbGrid = GridnAlartCount;
            ViewBag.AbBattery = GridnBatteryCount;
            ViewBag.AbSolar = GridnSolartCount;
            ViewBag.AbLoad = GridnLoadCount;
            ViewBag.AbGen = GridnGenCount;
            ViewBag.AbInv = GridnInvCount;

            ViewBag.AbGridClass = GridnAlartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbBatteryClass = GridnBatteryCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbSolarClass = GridnSolartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbLoadClass = GridnLoadCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbGenClass = GridnGenCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbInvClass = GridnInvCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";

            return View();
        }

        public ActionResult AbList(string type,string sday,string eday, int page = 1)
        {
            int types =1;
            string equip = "未知";
            DateTime Start = Convert.ToDateTime(sday);
            DateTime End = Convert.ToDateTime(eday);   
            switch (type)
            {
                case "Load": types = 5; equip = "負載"; break;
                case "Solar":types = 4; equip = "太陽能"; break;
                case "Grid": types = 2; equip = "市電"; break;
                case "Gen": types = 6; equip = "發電機"; break;
                case "Battery": types = 3; equip = "電池"; break;
                case "Inv": types = 7; equip = "逆變器"; break;
                default: break;
            }
            ViewBag.equip = equip;
            ViewBag.RangeStart = Start.ToShortDateString();
            ViewBag.RangeEnd = End.ToShortDateString();
            ViewBag.type = type;


            DateTime S = Start;
            DateTime E = End.AddDays(1);
            List<Alart> alartList = new List<Alart>();

           alartList = alartService.ReadAll().Where(y => y.StartTimet >= S && y.StartTimet < E && y.AlartType.AlartTypeCode == types).OrderByDescending(y => y.StartTimet).ToList();
                   
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = alartList.ToPagedList(currentPage, PageSizes());

            return View(result);
        }

        #endregion

        #region Bulletin

        public ActionResult Bulletin(int page = 1)
        {
            //一開始抓取全部資料
            List<Bulletin> bulletins = bulletinService.ReadAllView().ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());

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
            //寫入表單
            List<Bulletin> bulletins = (Start == End) ? bulletinService.ReadAll().ToList() : bulletinService.ReadListBy(Start, End).ToList();
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());

            return View(result);
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

      
        #endregion

    }
}