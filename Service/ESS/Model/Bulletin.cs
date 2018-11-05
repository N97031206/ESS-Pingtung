using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Bulletin
    {
        [Display(Name = "消息編號")]
        public Guid Id { get; set; }

        [Display(Name = "消息標題")]
        public string title { get; set; }

        [Display(Name = "消息內文")]
        public string context { get; set; }

        [Display(Name = "建立日期")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "更新日期")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "是否關閉")]
        public bool Disabled { get; set; }

        [Display(Name = "發布單位")]
        public Guid OrginID { get; set; }

        [Display(Name = "發佈人員")]
        public Guid AccountID { get; set; }
    }
}
