using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class ErrorCodes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid AlartTypeID { get; set; }
        [ForeignKey("AlartTypeID")]
        public virtual AlartType AlartType { get; set; }

        public string AlartCode { get; set; }

        public string AlartContext { get; set; }

        public DateTime CreateTimet { get; set; }

        public DateTime UpdateTimet { get; set; }
    }
}
