using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class Bulletin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string title { get; set; }

        public string context { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 是否關閉
        /// </summary>
        public bool Disabled { get; set; }

        [Required]
        public Guid OrginID { get; set; }
        [ForeignKey("OrginID")]
        public virtual Orgin Orgin { get; set; }

        public Guid AccountID { get; set; }

    }
}
