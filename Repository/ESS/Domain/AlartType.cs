using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class AlartType
    {
        /// <summary>
        /// 權限編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// 權限類型
        /// </summary>
        [ DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlartTypeCode { get; set; }

        [Required]
        public string AlartTypeName { get; set; }

    }
}
