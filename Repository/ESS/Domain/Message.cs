using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    /// <summary>
    /// 訊息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 訊息編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required, StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        [StringLength(200), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [Required, Column(TypeName = "nvarchar(MAX)")]
        public string Content { get; set; }

        /// <summary>
        /// 是否已經處理
        /// </summary>
        [Required]
        public bool IsHandled { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }
    }
}
