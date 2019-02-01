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

namespace Web.Areas.APP.Controllers
{
    public class APPController : Controller
    {
        #region private
        private BulletinService bulletinService = new BulletinService();
        private AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private StationService stationService = new StationService();
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
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        // GET: APP/APP
        public ActionResult Index()
        {
            #region 霧台
            Guid StationUUID = stationService.UUID(2);

            double soc = BatteryService.totalSOC(StationUUID) * 0.2;
            double LoadWatt = LoadPowerService.ReadNow(StationUUID).Watt_t;
            //可用總電量(度) = SOC(%) *20kWh(額定容量)
            ViewBag.Demand = Math.Round(soc, 2).ToString() + "kWh";
            //可用電時數(H) = 可用總電量(度) / 負載實功率(kW)
            ViewBag.RemainTime = LoadWatt <= 0 ? "無負載" : Math.Round(soc / LoadWatt, 1).ToString() + "小時";

            var batteryNow = BatteryService.ReadNow(StationUUID);
            var gridNow = GridPowerService.ReadNow(StationUUID);
            var LoadNow = LoadPowerService.ReadNow(StationUUID);
            var invNow = InverterService.ReadNow(StationUUID);
            var genNow = GeneratorService.ReadNow(StationUUID);

            List<int> cdData = new List<int>();
            batteryNow.ForEach(x=> cdData.Add((int)x.charge_direction));
            int cd= cdData.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Max();//找出最多次的
 
            double Solar = Math.Round(invNow.SPM90ActivePower.Split('|').ToList().Sum(x => string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x) / 1000.0), 2);
            double GirdPower = Math.Round(gridNow.Watt_t / 1000.00, 2);
            double Load = Math.Round(LoadNow.Watt_t / 1000.00, 2);
            string BatteyModel = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
            double BatteySOC = BatteryService.EachSOC(batteryNow.Average(x=>x.voltage));
            double BatteyPower = (cd == 1) ? Math.Round(batteryNow.Average(x=>x.charging_watt) / 1000.0, 2) : (cd == 2) ? Math.Round(batteryNow.Average(x=>x.discharging_watt) / 1000.0, 2) : 0;
            double Generator = Math.Round(genNow.totalwatts, 1);
    
            double GridV =Math.Round(gridNow.Vavg,2);
            double LoadV = Math.Round(LoadNow.Vavg,2);
            double solarV = Math.Round(invNow.SPM90Voltage.Split('|').ToList().Sum(x=>(string.IsNullOrEmpty(x)?0:Convert.ToDouble(x))), 2);
            double GenV = Math.Round(genNow.BatteryHighVoltage, 0);
            double BattV = Math.Round(batteryNow.Average(x => x.voltage), 2);
            #region

            ViewBag.grid = GirdPower;
            ViewBag.Load = Load;
            ViewBag.Solar = Solar;
            ViewBag.Generator = Generator;
            ViewBag.BatterySOC = BatteySOC;
            ViewBag.BatteryPower = BatteyPower;
            ViewBag.BatteryModel = BatteyModel;

            ViewBag.Colgrid = GridV < 50.0 ? "col border border-light bg-danger pz" : GirdPower <= 0 ? "col border border-light bg-secondary  pz" : "col border border-light pz";
            ViewBag.ColLoad = LoadNow.connected==false || LoadV < 50.0 ? "col border border-light bg-danger pz" : Load <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            ViewBag.ColSolar = Convert.ToBoolean( invNow.SPMconnected.Split('|').First())==false ||  solarV == 0 ? "col border border-light bg-danger pz" : Solar <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            ViewBag.ColGenerator =genNow.connected==false && genNow.EngineBatteryVoltage<5 ? "col border border-light bg-danger pz" : Generator <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            ViewBag.ColBattery = BattV == 0 ? "col border border-light bg-danger pz" : cd > 2 ? "col border border-light bg-secondary pz" : "col border border-light pz";

            ViewBag.Namegrid = GridV < 50 ? "市電(電壓不足)" : "市電";
            ViewBag.NameLoad = LoadNow.connected == false || LoadV < 50.0 ? "負載(電壓不足)" : "負載";
            ViewBag.NameSolar = Convert.ToBoolean(invNow.SPMconnected.Split('|').First()) == false || solarV == 0 ? "太陽能(離線)" : "太陽能";
            ViewBag.NameGenerator = genNow.connected == false && genNow.EngineBatteryVoltage < 5 ? "發電機(離線)" : "發電機";
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
            // StationUUID = stationService.UUID(6);

            // soc = BatteryService.totalSOC(StationUUID) * 0.2;
            //LoadWatt = LoadPowerService.ReadNow(StationUUID).Watt_t;
            // //可用總電量(度) = SOC(%) *20kWh(額定容量)
            // ViewBag.JSDemand = Math.Round(soc, 2).ToString() + "kWh";
            // //可用電時數(H) = 可用總電量(度) / 負載實功率(kW)
            // ViewBag.JSRemainTime = LoadWatt <= 0 ? "無負載" : Math.Round(soc / LoadWatt, 1).ToString() + "小時";




            //batteryNow = BatteryService.ReadNow(StationUUID);
            // gridNow = GridPowerService.ReadNow(StationUUID);
            //  LoadNow = LoadPowerService.ReadNow(StationUUID);
            //  invNow = InverterService.ReadNow(StationUUID);
            //  genNow = GeneratorService.ReadNow(StationUUID);


            // batteryNow.ForEach(x => cdData.Add((int)x.charge_direction));
            //  cd = cdData.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Max();//找出最多次的

            //  Solar = Math.Round(invNow.SPM90ActivePower.Split('|').ToList().Sum(x => string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x) / 1000.0), 2);
            //  GirdPower = Math.Round(gridNow.Watt_t / 1000.00, 2);
            //  Load = Math.Round(LoadNow.Watt_t / 1000.00, 2);
            //  BatteyModel = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
            //  BatteySOC = BatteryService.EachSOC(batteryNow.Average(x => x.voltage));
            //  BatteyPower = (cd == 1) ? Math.Round(batteryNow.Average(x => x.charging_watt) / 1000.0, 2) : (cd == 2) ? Math.Round(batteryNow.Average(x => x.discharging_watt) / 1000.0, 2) : 0;
            //  Generator = Math.Round(genNow.totalwatts, 1);

            //GridV = Math.Round(gridNow.Vavg, 2);
            //LoadV = Math.Round(LoadNow.Vavg, 2);
            //solarV = Math.Round(invNow.SPM90Voltage.Split('|').ToList().Sum(x => (string.IsNullOrEmpty(x) ? 0 : Convert.ToDouble(x))), 2);
            //GenV = Math.Round(genNow.BatteryHighVoltage, 0);
            //BattV = Math.Round(batteryNow.Average(x => x.voltage), 2);
            #region

            //ViewBag.JSgrid = GirdPower;
            //ViewBag.JSLoad = Load;
            //ViewBag.JSSolar = Solar;
            //ViewBag.JSGenerator = Generator;
            //ViewBag.JSBatterySOC = BatteySOC;
            //ViewBag.JSBatteryPower = BatteyPower;
            //ViewBag.JSBatteryModel = BatteyModel;

            //ViewBag.JSColgrid = GridV < 50.0 ? "col border border-light bg-danger pz" : GirdPower <= 0 ? "col border border-light bg-secondary  pz" : "col border border-light pz";
            //ViewBag.JSColLoad = LoadNow.connected == false || LoadV < 50.0 ? "col border border-light bg-danger pz" : Load <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            //ViewBag.JSColSolar = Convert.ToBoolean(invNow.SPMconnected.Split('|').First()) == false || solarV == 0 ? "col border border-light bg-danger pz" : Solar <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            //ViewBag.JSColGenerator = genNow.connected == false ? "col border border-light bg-danger pz" : Generator <= 0 ? "col border border-light bg-secondary pz" : "col border border-light pz";
            //ViewBag.JSColBattery = BattV == 0 ? "col border border-light bg-danger pz" : cd > 2 ? "col border border-light bg-secondary pz" : "col border border-light pz";

            //ViewBag.JSNamegrid = GridV < 50 ? "市電(電壓不足)" : "市電";
            //ViewBag.JSNameLoad = LoadNow.connected == false || LoadV < 50.0 ? "負載(電壓不足)" : "負載";
            //ViewBag.JSNameSolar = Convert.ToBoolean(invNow.SPMconnected.Split('|').First()) == false || solarV == 0 ? "太陽能(離線)" : "太陽能";
            //ViewBag.JSNameGenerator = genNow.connected == false ? "發電機(離線)" : "發電機";
            //ViewBag.JSNameBattery = BattV == 0 ? "電池(離線)" : "電池";

            ViewBag.JSDemand = " 0 kWh";
            ViewBag.JSRemainTime = "0 小時";

            ViewBag.JSgrid = 0;
            ViewBag.JSLoad = 0;
            ViewBag.JSSolar = 0;
            ViewBag.JSGenerator = 0;
            ViewBag.JSBatterySOC = 0;
            ViewBag.JSBatteryPower = 0;

            ViewBag.JSNamegrid = "市電(離線)";
            ViewBag.JSNameLoad = "負載(離線)";
            ViewBag.JSNameSolar = "太陽能(離線)";
            ViewBag.JSNameGenerator = "發電機(離線)";
            ViewBag.JSNameBattery = "電池(離線)";

            ViewBag.JSColgrid = "col border border-light bg-danger pz";
            ViewBag.JSColLoad = "col border border-light bg-danger pz";
            ViewBag.JSColSolar = "col border border-light bg-danger pz";
            ViewBag.JSColGenerator = "col border border-light bg-danger pz";
            ViewBag.JSColBattery = "col border border-light bg-danger pz";



            #endregion


            #endregion

            return View();
        }

        public ActionResult Info(int StationCode)
        {
            Guid StationUUID = stationService.UUID(StationCode);
            string StationName = StationCode == 2 ? "霧台大武" : "泰武佳興";           
            ViewBag.AppTitle = "即時資訊("+ StationName + ")";


            //取最新時間
            var ReadNow = ESSObjecterService.ReadNowUid(StationUUID);



            if (ReadNow != null)
            {
                ViewBag.nowHour = ReadNow.CreateTime.AddHours(1).Hour; //utc+8
                ChartData(ReadNow.UpdateDate);//utc

                string EssTime = ReadNow.CreateTime.ToString();
                ViewBag.EssTime = EssTime;

                #region Load
                List<LoadPower> loadPowers = new List<LoadPower>();
                var LoadReadNow = ReadNow.LoadPowerIDs;
                string LoadInfo1 = null;
                double LoadInfo2 = 0, LoadInfo3 = 0, LoadInfo4 = 0, LoadInfo5 = 0, LoadInfo6 = 0, LoadInfo7 = 0, LoadInfo8 = 0, LoadInfo9 = 0;
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
                    LoadPower lo = loadPowers.Where(x => x.index == 2).FirstOrDefault();
                    LoadInfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "負載" : "負載(目前離線中)";
                    LoadInfo2 = Math.Round(lo.Vavg, 2);
                    LoadInfo3 = Math.Round(lo.Isum, 2);
                    LoadInfo4 = Math.Round(lo.Watt_t / 1000.0, 2);
                    LoadInfo5 = Math.Round(lo.Var_t / 1000.00, 2);
                    LoadInfo6 = Math.Round(lo.VA_t / 1000.0, 2);
                    LoadInfo7 = Math.Round(lo.PF_t, 2);
                    LoadInfo8 = Math.Round(lo.Frequency, 2);
                    LoadInfo9 = Math.Round(lo.MinuskWHt, 2);
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
                //取相關資料
                var SolarReadNow = ReadNow.InvertersIDs;

                string solarInfo11 = "'", solarInfo21 = "'";
                double solarInfo12 = 0, solarInfo13 = 0, solarInfo14 = 0, solarInfo15 = 0;
                double solarInfo22 = 0, solarInfo23 = 0, solarInfo24 = 0, solarInfo25 = 0;

                if (SolarReadNow != null)
                {
                    List<Inverter> inverters = new List<Inverter>();
                    var spmID = SolarReadNow.Split('|');
                    foreach (var x in spmID)
                    {
                        if (!string.IsNullOrEmpty(x))
                        {
                            Guid id = Guid.Parse(x.Trim());
                            inverters.Add(InverterService.ReadByID(id));
                        }
                    }

                    if (inverters != null)
                    {
                        foreach (var inv in inverters)
                        {
                            var index = inv.SPMid.Split('|');
                            var voltage = inv.SPM90Voltage.Split('|');
                            var current = inv.SPM90Current.Split('|');
                            var energy = inv.SPM90ActiveEnergy.Split('|');
                            var power = inv.SPM90ActivePower.Split('|');
                            for (int i = 0; i < index.Length - 1; i++)
                            {
                                if (i == 0)
                                {
                                    solarInfo11 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "太陽能" + (i + 1).ToString() : "太陽能" + (i + 1).ToString() + "(目前離線中)";
                                    solarInfo12 = Math.Round(Convert.ToDouble(voltage[i]), 2);
                                    solarInfo13 = Math.Round(Convert.ToDouble(current[i]), 2);
                                    solarInfo14 = Math.Round(Convert.ToDouble(power[i]) / 1000.0, 2);
                                    solarInfo15 = Math.Round(Convert.ToDouble(i == 0 ? inv.SPM90ActiveEnergyMinus1 : inv.SPM90ActiveEnergyMinus2), 2);
                                }
                                else if (i == 1)
                                {
                                    solarInfo21 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "太陽能" + (i + 1).ToString() : "太陽能" + (i + 1).ToString() + "(目前離線中)";
                                    solarInfo22 = Math.Round(Convert.ToDouble(voltage[i]), 2);
                                    solarInfo23 = Math.Round(Convert.ToDouble(current[i]), 2);
                                    solarInfo24 = Math.Round(Convert.ToDouble(power[i]) / 1000.0, 2);
                                    solarInfo25 = Math.Round(Convert.ToDouble(i == 0 ? inv.SPM90ActiveEnergyMinus1 : inv.SPM90ActiveEnergyMinus2), 2);
                                }
                            }
                        }
                    }
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

                #region GridPower
                List<GridPower> gridPowers = new List<GridPower>();
                var GridReadNow = ReadNow.GridPowerIDs;
                string gridInfo1 = null;
                double gridInfo2 = 0, gridInfo3 = 0, gridInfo4 = 0, gridInfo5 = 0, gridInfo6 = 0, gridInfo7 = 0, gridInfo8 = 0, gridInfo9 = 0;

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
                    GridPower gp = gridPowers.Where(x => x.index == 0).FirstOrDefault();
                    gridInfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "市電迴路" : "市電迴路(目前離線中)";
                    gridInfo2 = Math.Round(gp.Vavg, 2);
                    gridInfo3 = Math.Round(gp.Isum, 2);
                    gridInfo4 = Math.Round(gp.Watt_t / 1000.0, 2);
                    gridInfo5 = Math.Round(gp.Var_t / 1000.00, 2);
                    gridInfo6 = Math.Round(gp.VA_t / 1000.0, 2);
                    gridInfo7 = Math.Round(gp.PF_t, 2);
                    gridInfo8 = Math.Round(gp.Frequency, 2);
                    gridInfo9 = Math.Round(gp.MinuskWHt, 2);
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

                #region GeneratorChart
                string Info1 = "發電機(目前離線中)", Info13 = "離線", Info2 = "0", Info3 = "0", Info4 = "0", Info5 = "0", Info6 = "0", Info7 = "0", Info8 = "0", Info9 = "0", Info10 = "0", Info11 = "0", Info12 = "0", Info14 = "0", Info15 = "0";
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
                        foreach (var gen in generators)
                        {
                            Info1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "發電機" : "發電機(目前離線中)";
                            Info2 = string.Format("{0:#,0.0}", gen.FuleLevel);
                            Info3 = string.Format("{0:#,0.0}", gen.L1Nvoltage);
                            Info4 = string.Format("{0:#,0.0}", gen.L2Nvoltage);
                            Info5 = string.Format("{0:#,0.0}", gen.L3Nvoltage);
                            Info6 = string.Format("{0:#,0.0}", gen.L1current);
                            Info7 = string.Format("{0:#,0.0}", gen.L2current);
                            Info8 = string.Format("{0:#,0.0}", gen.L3current);
                            Info9 = string.Format("{0:#,0.0}", gen.totalwatts / 1000);
                            Info10 = string.Format("{0:#,0.0}", gen.averagepowerfactor);
                            Info11 = string.Format("{0:#,0.0}", gen.positiveKWhours);
                            Info12 = string.Format("{0:#,0.0}", gen.negativeKWhours);
                            Info13 = gen.ControlStatus.Equals("true") ? "啟動中" : "已關閉";
                            Info14 = string.Format("{0:#,0.0}", gen.AvailabilityEnergy);
                            Info15 = string.Format("{0:#,0}", gen.AvailabilityHour);
                        }
                    }
                }
                ViewBag.GenInfo1 = Info1;
                ViewBag.GenInfo2 = Info2;
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

                string InvInfo1 = "逆變器(目前離線中)", InvInfo2 = "故障模式", InvInfo3 = "0", InvInfo4 = "0", InvInfo5 = "0", InvInfo6 = "0", InvInfo7 = "0", InvInfo8 = "0", InvInfo9 = "0", InvInfo10 = "0", InvInfo11 = "0";

                var InverReadNow = ReadNow.InvertersIDs;

                if (InverReadNow != null)
                {
                    var invID = InverReadNow.Split('|');
                    foreach (var x in invID)
                    {
                        if (!string.IsNullOrEmpty(x))
                        {
                            Guid id = Guid.Parse(x.Trim());
                            inverInfo.Add(InverterService.ReadByID(id));
                        }
                    }
                    foreach (var inv in inverInfo)
                    {
                        InvInfo1 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? "逆變器" : "逆變器(目前離線中)";
                        string model = inv.DeviceMode.Trim();
                        if (model == "P") { model = "電源模式"; }
                        else if (model == "S") { model = "待機模式"; }
                        else if (model == "L") { model = "市電模式"; }
                        else if (model == "B") { model = "電池模式"; }
                        else if (model == "F") { model = "故障模式"; }
                        else if (model == "H") { model = "省電模式"; }
                        else { model = "其他模式"; }
                        InvInfo2 = model;
                        InvInfo3 = string.Format("{0:#,0.0}", inv.GridVoltage);
                        InvInfo4 = string.Format("{0:#,0.0}", inv.GridFrequency);
                        InvInfo5 = string.Format("{0:#,0.0}", inv.AC_OutputVoltage);
                        InvInfo6 = string.Format("{0:#,0.0}", inv.AC_OutputFrequency);
                        InvInfo7 = string.Format("{0:#,0.0}", inv.ParallelInformation_TotalOutputActivePower.Split('|').ToList().Sum(x => x.IsEmpty() ? 0 : Convert.ToDouble(x)) / 1000.0);
                        InvInfo8 = string.Format("{0:#,0.0}", inv.BatteryVoltage);
                        InvInfo9 = string.Format("{0:#,0.0}", inv.BatteryCapacity);
                        InvInfo10 = string.Format("{0:#,0}", inv.PV_InputVoltage);
                        InvInfo11 = string.Format("{0:#,0}", inv.ParallelInformation_TotalChargingCurrent.Split('|').ToList().Sum(x => x.IsEmpty() ? 0 : Convert.ToDouble(x)) / 1000.0);
                    }
                }
                ViewBag.Invnfo1 = InvInfo1;
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
                string BatteryInfo11 = "目前離線中", BatteryInfo12 = "0", BatteryInfo13 = "0", BatteryInfo14 = "0", BatteryInfo15 = "0", BatteryInfo16 = "0", BatteryInfo17 = "離線";
                string BatteryInfo21 = "目前離線中", BatteryInfo22 = "0", BatteryInfo23 = "0", BatteryInfo24 = "0", BatteryInfo25 = "0", BatteryInfo26 = "0", BatteryInfo27 = "離線";
                string BatteryInfo31 = "目前離線中", BatteryInfo32 = "0", BatteryInfo33 = "0", BatteryInfo34 = "0", BatteryInfo35 = "0", BatteryInfo36 = "0", BatteryInfo37 = "離線";
                string BatteryInfo41 = "目前離線中", BatteryInfo42 = "0", BatteryInfo43 = "0", BatteryInfo44 = "0", BatteryInfo45 = "0", BatteryInfo46 = "0", BatteryInfo47 = "離線";

                //取相關資料
                var batteryID = ReadNow.BatteryIDs.Split('|').ToList();

                if (batteryID != null)
                {
                    List<Battery> batteries = new List<Battery>();
                    batteryID.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x))
                        {
                            batteries.Add(BatteryService.ReadByID(Guid.Parse(x.Trim())));
                        }
                    });


                    foreach (var ba in batteries)
                    {
                        if (ba.index == 0)
                        {
                            BatteryInfo11 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? ba.index.ToString() : ba.index.ToString() + "(目前離線中)";
                            BatteryInfo12 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo13 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo14 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo15 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo16 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = Convert.ToInt32(ba.charge_direction);
                            BatteryInfo17 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 1)
                        {
                            BatteryInfo21 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? ba.index.ToString() : ba.index.ToString() + "(目前離線中)";
                            BatteryInfo22 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo23 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo24 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo25 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo26 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = Convert.ToInt32(ba.charge_direction);
                            BatteryInfo27 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 2)
                        {
                            BatteryInfo31 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? ba.index.ToString() : ba.index.ToString() + "(目前離線中)";
                            BatteryInfo32 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo33 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo34 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo35 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo36 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = Convert.ToInt32(ba.charge_direction);
                            BatteryInfo37 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }

                        if (ba.index == 3)
                        {
                            BatteryInfo41 = (DateTime.Now.AddMinutes(-5) < DateTime.Parse(EssTime)) ? ba.index.ToString() : ba.index.ToString() + "(目前離線中)";
                            BatteryInfo42 = string.Format("{0:#,0.00}", ba.voltage);
                            BatteryInfo43 = string.Format("{0:#,0.00}", ba.charging_current);
                            BatteryInfo44 = string.Format("{0:#,0.00}", ba.discharging_current);
                            BatteryInfo45 = string.Format("{0:#,0.00}", BatteryService.EachSOC(ba.voltage));
                            BatteryInfo46 = string.Format("{0:#,0}", ba.Cycle);
                            int cd = Convert.ToInt32(ba.charge_direction);
                            BatteryInfo47 = (cd == 1) ? "充電" : (cd == 2) ? "放電" : "離線";
                        }
                    }
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
                ViewBag.BatteryInfo47 = BatteryInfo37;
                #endregion

                return View();
            }
            else
            {
                return View();
            }


        }

        private void ChartData(DateTime utcDate)
        {
            DateTime starttime = new DateTime();
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
            starttime = utcDate.AddDays(-1);
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
            starttime = utcDate.AddDays(-1);
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
            Data = null;
            string Grid = null;
            sum1.Clear();
            sum2.Clear();
            starttime = starttime.AddDays(-1);
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
            Data = null;
            string Generqtor1 = null;
            sum1.Clear();
            starttime = utcDate.AddDays(-1);
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
            starttime = utcDate.AddDays(-1);
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
            Data = null;
            sum.Clear();
            starttime = utcDate.AddDays(-1);
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
            HashSet<Int32> eachMonthly = new HashSet<Int32>(maxItem);
            ViewBag.maxMonthly = eachMonthly.Max();
            maxItem.Clear();

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                maxItem.Add(Convert.ToInt32(FN[0].Substring(0, 8)));
            }
            HashSet<Int32> eachDaily = new HashSet<Int32>(maxItem);
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

        #region 嘉興
    

        #endregion

        #region Abnormal
        public ActionResult Abnormal()
        {
            List<Alart> alartList = alartService.ReadListTime(DateTime.Today, DateTime.Today.AddDays(1)).OrderByDescending(x=>x.StartTimet).ToList();

            int AlartCount = alartList.Count(x => x.AlartType.AlartTypeCode == 2);
            int BatteryCount = alartList.Count(x => x.AlartType.AlartTypeCode == 3);
            int SolartCount = alartList.Count(x => x.AlartType.AlartTypeCode == 4);
            int LoadCount = alartList.Count(x => x.AlartType.AlartTypeCode == 5);
            int GenCount = alartList.Count(x => x.AlartType.AlartTypeCode == 6);
            int InvCount = alartList.Count(x => x.AlartType.AlartTypeCode == 7);

            ViewBag.AbGrid = AlartCount;
            ViewBag.AbBattery = BatteryCount;
            ViewBag.AbSolar = SolartCount;
            ViewBag.AbLoad = LoadCount;
            ViewBag.AbGen = GenCount;
            ViewBag.AbInv = InvCount;

            ViewBag.AbGridClass = AlartCount == 0 ?"btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbBatteryClass = BatteryCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbSolarClass = SolartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbLoadClass = LoadCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbGenClass = GenCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbInvClass = InvCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";

            ViewBag.RangeStart = DateTime.Today.ToShortDateString();
            ViewBag.RangeEnd = DateTime.Today.ToShortDateString() ;
            ViewBag.Range = "區間:" + DateTime.Today.ToShortDateString() + "-" + DateTime.Today.ToShortDateString();
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

            List<Alart> alartList = alartService.ReadListTime(Start, End.AddDays(1)).ToList();
           
            int AlartCount = alartList.Count(y => y.AlartType.AlartTypeCode == 2);
            int BatteryCount = alartList.Count(y => y.AlartType.AlartTypeCode == 3);
            int SolartCount = alartList.Count(y => y.AlartType.AlartTypeCode == 4);
            int LoadCount = alartList.Count(y => y.AlartType.AlartTypeCode == 5);
            int GenCount = alartList.Count(y => y.AlartType.AlartTypeCode == 6);
            int InvCount = alartList.Count(y => y.AlartType.AlartTypeCode == 7);

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

            ViewBag.AbGridClass = AlartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbBatteryClass = BatteryCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbSolarClass = SolartCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbLoadClass = LoadCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbGenClass = GenCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";
            ViewBag.AbInvClass = InvCount == 0 ? "btn btn-secondary btn-lg btn-block" : "btn btn-danger btn-lg btn-block";

            #endregion

            return View();
        }

        public ActionResult AbList(string name, string sday,string eday, int page = 1)
        {
            Guid typesID = Guid.Parse( "96DE23DB-34E3-E811-BE29-0C9D925E499C");//所有異常
            Guid StationID = stationService.StationID(2);
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

            List<Alart> alartList = alartService.ReadListBy(Start, End.AddDays(1), typesID, StationID);
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
            ViewBag.RangeStart = DateTime.Today.ToShortDateString();
            ViewBag.RangeEnd = DateTime.Today.ToShortDateString();
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
            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = bulletins.ToPagedList(currentPage, PageSizes());

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

    }
}