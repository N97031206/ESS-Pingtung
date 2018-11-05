using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class Account
    {
        /// <summary>
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Required, StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required, StringLength(50)]
        public string Password { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        [StringLength(100), DataType(DataType.PhoneNumber)]
        public string Tel { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        [StringLength(250), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
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

        /// <summary>
        /// 是否核准
        /// </summary>

        public bool IsApproved { get; set; }

        /// <summary>
        /// 是否封鎖
        /// </summary>

        public bool IsLocked { get; set; }

        /// <summary>
        /// 最後登入日期
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 最後登出日期
        /// </summary>
        public DateTime? LastLogoutDate { get; set; }

        /// <summary>
        /// 密碼失敗次數
        /// (超過3次就封鎖，登入成功就歸零)
        /// </summary>
        public int PasswordFailureCount { get; set; }

        /// <summary>
        /// 最後密碼失敗日期
        /// </summary>
        public DateTime? LastPasswordFailureDate { get; set; }

        /// <summary>
        /// 最後封鎖日期
        /// </summary>
        public DateTime? LastLockedDate { get; set; }

        /// <summary>
        /// 最後更新密碼日期
        /// </summary>
        public DateTime? LastPasswordChangedDate { get; set; }

        /// <summary>
        /// 角色編號
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public Guid OrginId { get; set; }
        [ForeignKey("OrginId")]
        public virtual Orgin Orgin { get; set; }
    }
}
