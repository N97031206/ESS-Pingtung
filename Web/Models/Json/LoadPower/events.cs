using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.LoadPower
{
    public class events
    {
        public bool IsCurrent { get; set; }
        public string ErrorMessage { get; set; }
        public int event_info { get; set; }
        public DateTime event_date_time { get; set; }
        public info info { get; set; }
    }
}