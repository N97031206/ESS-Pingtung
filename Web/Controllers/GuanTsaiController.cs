using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Web.Models.GuanTsai;

namespace Web.Controllers
{
    [HandleError]
    public class GuanTsaiController : Controller
    {
        #region 參考程式
        /// <summary>
        /// 更換Buttom的class
        /// </summary>
        /// <param name="type"></param>
        private void NavButtom(string type)
        {
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


        #endregion

        [Authorize]
        public ActionResult FileUpload()
        {
            NavButtom("FileUpload");
            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            NavButtom("MonthlyReport");
            MainItem(1);
            SubItem(MaxItem(1),1);
            ViewBag.ChkData = false;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection From)
        {
            string yearly = Request.Form["Yearly"];
            string monthly = Request.Form["monthly"];
            NavButtom("MonthlyReport");
            MainItem(1);
            SubItem(MaxItem(1), 1);

            if (yearly.Equals("Y"))
            {
                List<string> Yearlys = new List<string>();
                string Monthpath = Server.MapPath("~/Content/GuanTsai/MonthlyReport/");
                ViewBag.ChkData = false;
                foreach (string fname in Directory.GetFileSystemEntries(Monthpath, "*.xlsx"))
                {
                    string[] File = fname.Split('_');
                    string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                    Yearlys.Add(FN[0].Substring(0, 4));
                }

                string y = null;

                foreach (var x in Yearlys)
                {
                    if (Request.Form[x] != null)
                    {
                        y = Request.Form[x];
                        SubItem(y,1);
                    }
                }
                ViewBag.showMonthly = yearly;
                return View();
            }
            else
            {
                ViewBag.ChkData = true;

                try
                {
                    List<MonthlyReportData> RowData = new List<MonthlyReportData>();

                    string fname = Path.Combine(Server.MapPath("~/Content/GuanTsai/MonthlyReport/"), "MONTHLYREPORT_SAMPLE_" + yearly + monthly + "01.xlsx");
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
                                    RowData.Add(new MonthlyReportData()
                                    {
                                        Day = cellValue,
                                        TotalGenerator = sheet.Cells[RowId, 2].Text,
                                        TotalPV = sheet.Cells[RowId, 3].Text,
                                        TotalLoad = sheet.Cells[RowId, 4].Text,
                                        TotalTPC = sheet.Cells[RowId, 5].Text
                                    });
                                    RowId += 1;
                                }
                            } while (!isLastRow);
                        }
                    }//end using
                    ViewBag.showMonthly = yearly + monthly;
                    return View(RowData);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    return View();
                }
            }
        }



        private void MainItem(int i)
        {
            List<string> FileName = new List<string>();
            List<string> Yearly = new List<string>();
            int end = 4;


            string path =  Server.MapPath("~/Content/GuanTsai/MonthlyReport/");
            if (i == 2) {
                path = Server.MapPath("~/Content/GuanTsai/DailyReport/");
                end = 6;
            }

            foreach (string fname in Directory.GetFileSystemEntries(path, "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                Yearly.Add(FN[0].Substring(0, end));
            }
            HashSet<string> eachYearly = new HashSet<string>(Yearly);
            ViewBag.MainItemBtn = eachYearly.ToList();
        }

        private string MaxItem(int i)
        {
            List<int> maxItem = new List<int>();
            int end = 4;

            string path = Server.MapPath("~/Content/GuanTsai/MonthlyReport/");
            if (i == 2)
            {
                path = Server.MapPath("~/Content/GuanTsai/DailyReport/");
                end = 6;
            }

            foreach (string fname in Directory.GetFileSystemEntries(path, "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                maxItem.Add( Convert.ToInt32(FN[0].Substring(0, end)));
            }
            HashSet<Int32> eachYearly = new HashSet<Int32>(maxItem);
            return eachYearly.Max().ToString(); ;
        }

        private void SubItem(string yearly,int i)
        {
            List<string> FileName = new List<string>();
            List<string> Yearlys = new List<string>();
            List<string> Monthlys = new List<string>();
            int end = 4;

            string path ="~/Content/GuanTsai/MonthlyReport/";
            if (i == 2)
            {
                path = "~/Content/GuanTsai/DailyReport/";
                end = 6;
            }

            foreach (string fname in Directory.GetFileSystemEntries(Server.MapPath(path), "*.xlsx"))
            {
                string[] File = fname.Split('_');
                string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');

                Yearlys.Add(FN[0].Substring(0, end));

                if (FN[0].Substring(0, end) == yearly.Trim())
                {
                    Monthlys.Add(FN[0].Substring(end, 2));
                }
            }

            ViewBag.SubItembtm = Monthlys;
            ViewBag.hiddenYear = yearly;
        }


        [Authorize]
        // GET: GuanTsai
        public ActionResult MonthlyReport()
        {
            try {
                List<MonthlyReportData> RowData = new List<MonthlyReportData>();
                string xlsx = "MONTHLYREPORT_SAMPLE_20181201.xlsx";
                string fname = Path.Combine(Server.MapPath("~/Content/GuanTsai/MonthlyReport/"), xlsx);

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
                                RowData.Add(new MonthlyReportData()
                                {
  
                                    Day = cellValue,
                                    TotalGenerator = sheet.Cells[RowId, 2].Text,
                                    TotalPV = sheet.Cells[RowId, 3].Text,
                                    TotalLoad = sheet.Cells[RowId, 4].Text,
                                    TotalTPC = sheet.Cells[RowId, 5].Text
                                });
                                RowId += 1;
                            }
                        } while (!isLastRow);
                    }
                }//end using 

                return View(RowData);
            }
            catch (Exception ex)  {

                Console.Write(ex.ToString());
                return null;
            }
        }

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


        [Authorize]
        // GET: GuanTsai
        public ActionResult DailyReport()
        {
            NavButtom("DailyReport");
            MainItem(2);
            SubItem(MaxItem(2), 2);
            ViewBag.ChkData = false;

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DailyReport(FormCollection From)
        {
            string daily = Request.Form["daily"];
            string monthly = Request.Form["monthly"];
            NavButtom("DailyReport");
            MainItem(2);
            SubItem(MaxItem(2), 2);

            if (monthly.Equals("Y"))
            {
                List<string> monthlys = new List<string>();
                string Monthpath = Server.MapPath("~/Content/GuanTsai/DailyReport/");
                ViewBag.ChkData = false;

                foreach (string fname in Directory.GetFileSystemEntries(Monthpath, "*.xlsx"))
                {
                    string[] File = fname.Split('_');
                    string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
                    monthlys.Add(FN[0].Substring(0, 6));
                }

                string y = null;

                foreach (var x in monthlys)
                {
                    if (Request.Form[x] != null)
                    {
                        y = Request.Form[x];
                        SubItem(y, 2);
                    }

                }
                return View();
            }
            else
            {
                ViewBag.ChkData = true;

                try
                {
                    List<DailyReportData> RowData = new List<DailyReportData>(); 

                    string fname = Path.Combine(Server.MapPath("~/Content/GuanTsai/DailyReport/"), "DailyReport_sample_" + monthly +daily+ ".xlsx");
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
                    ViewBag.showDaily = monthly + daily;
                    return View(RowData);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    return View();
                }
            }
        }




        //private void YearlyItem()
        //{
        //    List<string> FileName = new List<string>();
        //    List<string> Yearly = new List<string>();
        //    //List<SelectListItem> itemYearly = new List<SelectListItem>();
        //    //List<SelectListItem> itemMonthly = new List<SelectListItem>();

        //    string path = Server.MapPath("~/Content/GuanTsai/MonthlyReport/");

        //    foreach (string fname in Directory.GetFileSystemEntries(path, "*.xlsx"))
        //    {
        //        string[] File = fname.Split('_');
        //        string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');
        //        Yearly.Add(FN[0].Substring(0, 4));
        //    }

        //    HashSet<string> eachYearly = new HashSet<string>(Yearly);

        //    //eachYearly.ToList().ForEach(x => itemYearly.Add(
        //    //    new SelectListItem
        //    //    {
        //    //        Text = x,
        //    //        Value = x,
        //    //    }));


        //    ViewBag.BtnYear = eachYearly.ToList();
        //    //ViewBag.YearlyReport = itemYearly;
        //    //ViewBag.MonthlyReport = itemMonthly;
        //}


        //private void MonthlyItem(string yearly)
        //{
        //    List<string> FileName = new List<string>();
        //    List<string> Yearlys = new List<string>();
        //    List<string> Monthlys = new List<string>();
        //    //List<SelectListItem> itemYearly = new List<SelectListItem>();
        //    //List<SelectListItem> itemMonthly = new List<SelectListItem>();

        //    string path = Server.MapPath("~/Content/GuanTsai/MonthlyReport/");

        //    foreach (string fname in Directory.GetFileSystemEntries(path, "*.xlsx"))
        //    {
        //        string[] File = fname.Split('_');
        //        string[] FN = File[File.GetUpperBound(0)].ToString().Split('.');

        //        Yearlys.Add(FN[0].Substring(0, 4));

        //        if (FN[0].Substring(0, 4) == yearly.Trim())
        //        {
        //            Monthlys.Add(FN[0].Substring(4, 2));
        //        }


        //        //itemMonthly.Add(new SelectListItem
        //        //{
        //        //    Text = FN[0].Substring(4, 2),
        //        //    Value = fname
        //        //});
        //    }


        //    //HashSet<string> eachYearly = new HashSet<string>(Yearlys);

        //    //eachYearly.ToList().ForEach(x => itemYearly.Add(
        //    //    new SelectListItem
        //    //    {
        //    //        Text = x,
        //    //        Value = x,
        //    //        Selected = x.ToString().Trim().Equals(yearly.Trim())
        //    //    }));


        //    ViewBag.btmMonth = Monthlys;
        //    //ViewBag.MonthlyReport = itemMonthly;
        //    //ViewBag.YearlyReport = itemYearly;
        //}


    }
}