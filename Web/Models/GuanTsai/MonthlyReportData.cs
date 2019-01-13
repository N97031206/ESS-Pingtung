using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.GuanTsai
{
    public class MonthlyReportData
    {
        public string Day { get; set; }
        public string TotalGenerator { get; set; }
        public string TotalPV { get; set; }
        public string TotalLoad { get; set; }
        public string TotalTPC { get; set; }
    }
}