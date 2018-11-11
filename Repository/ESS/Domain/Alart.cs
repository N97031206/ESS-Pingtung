using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class Alart
    {
        /// <summary>
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid StationID { get; set; }
        [ForeignKey("StationID")]
        [Column(Order = 1)]
        public virtual Station Station { get; set; }

        [Required]
        public Guid AlartTypeID { get; set; }
        [ForeignKey("AlartTypeID")]
        [Column(Order = 2)]
        public virtual AlartType AlartType { get; set; }

        public string AlartContext { get; set; }

        public DateTime StartTimet { get; set; }

        public DateTime? EndTimet { get; set; }

        public bool Disabled { get; set; }

    }
}
