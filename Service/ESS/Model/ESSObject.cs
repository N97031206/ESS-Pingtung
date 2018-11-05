using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class ESSObject
    {
        [Display(Name = "帳戶編號")]
        public Guid Id { get; set; }

        [Display(Name = " UTC更新時間")]
        public DateTime UpdateDate { get; set; }

        [Display(Name = "場所名稱")]
        public string stationUUID { get; set; }

        [Display(Name = "場所識別碼")]
        public string stationName { get; set; }

        /// <summary>
        /// EMS資料
        /// </summary>
        [Display(Name = "市電陣列")]
        public string GridPowerIDs { get; set; }

        [Display(Name = "負載陣列")]
        public string LoadPowerIDs { get; set; }

        [Display(Name = "發電機陣列")]
        public string GeneratorIDs { get; set; }

        [Display(Name = " 逆變器陣列")]
        public string InvertersIDs { get; set; }

        [Display(Name = "電池組陣列")]
        public string BatteryIDs { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "接收時間")]
        public DateTime CreateTime { get; set; }

    }
}
