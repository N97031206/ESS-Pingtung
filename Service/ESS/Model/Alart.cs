using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Alart
    {
        /// <summary>
        /// 帳戶編號
        /// </summary>
        [Display(Name = "異常編號")]
        public Guid Id { get; set; }

        [Display(Name = "站點編號")]
        public Guid StationID { get; set; }
        public virtual Station Station { get; set; }

        [Display(Name = "異常分類")]
        public Guid AlartTypeID { get; set; }
        public virtual AlartType AlartType { get; set; }

        [Display(Name = "異常描述")]
        public string AlartContext { get; set; }

        [Display(Name = "異常起始時間")]
        public DateTime StartTimet { get; set; }

        [Display(Name = "異常結束時間")]
        public DateTime EndTimet { get; set; }

        [Display(Name = "開啟是否")]
        public bool Disabled { get; set; }

        public Guid EquipmentID { get; set; }

    }
}
