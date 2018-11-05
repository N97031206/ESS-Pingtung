using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Station
    {
        [Display(Name = "站點編號")]
        public Guid Id { get; set; }

        [Display(Name = "站點序號")]
        public int StationCode { get; set; }

        [Display(Name = "站點名稱")]
        public string StationName { get; set; }

        [Display(Name = "場所識別碼")]
        public Guid UUID { get; set; }

    }
}
