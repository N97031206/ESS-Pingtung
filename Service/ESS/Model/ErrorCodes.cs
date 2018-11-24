using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.ESS.Model
{
    public class ErrorCodes
    {
        [Display(Name = "異常編號")]
        public Guid Id { get; set; }


        public Guid AlartTypeID { get; set; }
        [Display(Name = "異常單位")]
        public virtual AlartType AlartType { get; set; }

        public string AlartCode { get; set; }

        public string AlartContext { get; set; }

        public DateTime CreateTimet { get; set; }

        public DateTime UpdateTimet { get; set; }
    }
}
