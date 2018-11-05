using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class AlartType
    {
        [Display(Name = "異常分類編號")]
        public Guid Id { get; set; }

        [Display(Name = "異常分類")]
        public int AlartTypeCode { get; set; }

        [Display(Name = "異常分類描述")]
        public string AlartTypeName { get; set; }
    }
}
