using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.GuanTsai
{
    public class DailyReportData
    {
        public string Time { get; set; }
        public string Generator { get; set; }
        public string HomeOnePower { get; set; }
        public string HomeTwoPower { get; set; }
        public string HomeThreePower { get; set; }
        public string HomeFourPower { get; set; }
        public string P7TimelyPower { get; set; }
        public string P7Timely110VPower { get; set; }
        public string P7Timely220VPower { get; set; }
        public string P9TimelyPower { get; set; }
        public string P9Timely110VPower { get; set; }
        public string TotalPVTimelyPower { get; set; }
        public string TotalLoad { get; set; }
        public string TotalTPC { get; set; }
    }
}