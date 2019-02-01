using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.GuanTsai;
using NLog;
using Service.ESS.Provider;

namespace Web.Controllers
{
    [HandleError]
    public class GuanTsaiController : Controller
    {
        #region private
        private readonly BulletinService bulletinService = new BulletinService();
        private readonly AlartService alartService = new AlartService();
        private readonly AlartTypeService alarttypeService = new AlartTypeService();
        private readonly StationService stationService = new StationService();
        //EMS
        private readonly ESSObjecterService ESSObjecterService = new ESSObjecterService();
        private readonly BatteryService BatteryService = new BatteryService();
        private readonly GridPowerService GridPowerService = new GridPowerService();
        private readonly GeneratorService GeneratorService = new GeneratorService();
        private readonly LoadPowerService LoadPowerService = new LoadPowerService();
        private readonly InverterService InverterService = new InverterService();
             //Log檔
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region 參考程式
        /// <summary>
        /// 更換Buttom的class
        /// </summary>
        /// <param name="type"></param>
        private void NavButtom(string type)
        {
            TagTitle();
            ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
            TempData["ButtomMonthlyReport"] = "btn btn-outline-success btn-lg";
            TempData["ButtomDailyReport"] = "btn btn-outline-success btn-lg";
            TempData["ButtomFileUpload"] = "btn btn-outline-success btn-lg";
            type = type.Trim();
            switch (type)
            {
                case "MonthlyReport":
                    TempData["ButtomMonthlyReport"] = "btn btn-success btn-lg";
                    break;
                case "DailyReport":
                    TempData["ButtomDailyReport"] = "btn btn-success btn-lg";
                    break;
                case "FileUpload":
                    TempData["ButtomFileUpload"] = "btn btn-success btn-lg";
                    break;
                default:
                    break;
            }
        }

        private void DataPicker(int i)
        {
            HashSet<string> datepicker = new HashSet<string>();
            //1 為月 2為日
            string path = (i == 1) ? "~/Content/GuanTsai/MonthlyReport/" : "~/Content/GuanTsai/DailyReport/";

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath(path), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                datepicker.Add(FN[0].Substring(0, (i == 1) ? 6 : 8));
            }

            if (i == 1)
            {
                ViewBag.maxData = datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) + "-01";
                ViewBag.minData = datepicker.Min().Substring(0, 4) + "-" + datepicker.Min().Substring(4, 2) + "-01";
                ViewBag.searchData = datepicker.Max().Substring(0, 4).ToString() + datepicker.Max().Substring(4, 2).ToString();
            }
            else if (i == 2)
            {
                ViewBag.maxData = datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) + "-" + datepicker.Max().Substring(6, 2);
                ViewBag.minData = datepicker.Min().Substring(0, 4) + "-" + datepicker.Min().Substring(4, 2) + "-" + datepicker.Min().Substring(6, 2);
                ViewBag.search = datepicker.Max().Substring(0, 4) + "-" + datepicker.Max().Substring(4, 2) + "-" + datepicker.Max().Substring(6, 2);
                ViewBag.searchData = datepicker.Max().Substring(0, 4) + datepicker.Max().Substring(4, 2) + datepicker.Max().Substring(6, 2);
            }
        }

        [Authorize]
        // GET: GuanTsai
        private void DailyItem()
        {
            List<string> FileName = new List<string>();
            List<SelectListItem> item = new List<SelectListItem>();
            foreach (string fname in System.IO.Directory.GetFileSystemEntries(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');

                item.Add(new SelectListItem
                {
                    Text = FN[0],
                    Value = fname
                });
            }
            ViewBag.filename = item;
        }

        #endregion

        #region 報表上傳
        [Authorize]
        public ActionResult FileUpload()
        {
            NavButtom("FileUpload");
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult FileUpload( HttpPostedFileBase file)
        {
                NavButtom("FileUpload");
                string MF = "MONTHLYREPORT";
                string DF = "DailyReport";

            if (file != null && file.ContentLength > 0)
            {
                var fileName = file.FileName;
                var MIME = MimeMapping.GetMimeMapping(fileName);
                if (MIME.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    var FN = fileName.Split('_').ToList();

                    if (FN[0].Equals(MF))
                    {
                        fileName = Path.GetFileName(file.FileName);
                        var path = Server.MapPath("~/Content/GuanTsai/MonthlyReport/");
                        //若該資料夾不存在，則新增一個
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = Path.Combine(path, fileName);
                        file.SaveAs(path);
                        ViewBag.fileContext = fileName + "上傳成功";
                    }
                    else if (FN[0].Equals(DF))
                    {
                        fileName = Path.GetFileName(file.FileName);
                        var path = Server.MapPath("~/Content/GuanTsai/DailyReport/");
                        //若該資料夾不存在，則新增一個
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = Path.Combine(path, fileName);
                        file.SaveAs(path);
                        ViewBag.fileContext = fileName + "上傳成功";
                    }
                }
                else
                {
                    ViewBag.fileError = "格式錯誤";
                }
            }
            else
            {
                ViewBag.fileError = "沒有選擇上傳報表";
            }
            return View();
        }


        #endregion

        #region 月報表
        [Authorize]
        public ActionResult Index()
        {
            NavButtom("MonthlyReport");
            DataPicker(1);
            ViewBag.ChkData = false;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection From)
        {

            NavButtom("MonthlyReport");
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
                        for (int i = RowId; i <= 35; i++)
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

                        ViewBag.lastDay = Days.Last().ToString();
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
        #endregion

        #region 日報表
        [Authorize]
        // GET: GuanTsai
        public ActionResult DailyReport()
        {
            NavButtom("DailyReport");
            DataPicker(2);
            ViewBag.ChkData = false;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DailyReport(FormCollection From)
        {
            string daily = Request.Form["Day"];
            DataPicker(2);
            ViewBag.ChkData = true;
            ViewBag.searchData = daily;
            ViewBag.search = daily.Substring(0, 4) + "-" + daily.Substring(4, 2) + "-" + daily.Substring(6, 2);
            NavButtom("DailyReport");

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

                string PVTotal = null, PV1 = null, PV2 = null, PV3 = null, PV4 = null;
                string P7 = null, P7110 = null, P7220 = null, P9 = null, P9110 = null;
                string Gen = null, Load = null, TPC = null;
                int i = 1;
                int count = RowData.Count;
                foreach (var data in RowData.ToList())
                {
                    if (i < count)
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
        #endregion

        protected internal void TagTitle()
        {
            ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
            if (Session["UserName"] != null)
            {
                string un = Session["UserName"].ToString();
                ViewBag.User = string.IsNullOrEmpty(un) ? null : un;
            }
        }


    }
}