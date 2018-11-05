using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Json.Batterys
{
    public class AlarmState
    {
        public bool OV_DIS { get; set; }
        public bool UV_DIS { get; set; }
        public bool OC_DIS { get; set; }
        public bool SC_DIS { get; set; }
        public bool OT_DIS { get; set; }
        public bool UT_DIS { get; set; }
        public bool RV_DIS { get; set; }
        public bool OC0_DIS { get; set; }

    }
}