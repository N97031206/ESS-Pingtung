using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Account
    {
        [Display(Name = "帳戶編號")]
        public Guid Id { get; set; }

        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }

        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "電話")]
        public string Tel { get; set; }

        [Display(Name = "電子信箱")]
        public string Email { get; set; }

        [Display(Name = "建立日期")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "更新日期")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "是否關閉")]
        public bool Disabled { get; set; }

        [Display(Name = "是否核准")]
        public bool IsApproved { get; set; }

        [Display(Name = "是否封鎖")]
        public bool IsLocked { get; set; }

        [Display(Name = "最後登入日期")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "最後登出日期")]
        public DateTime? LastLogoutDate { get; set; }

        [Display(Name = "密碼失敗次數")]
        [Required]
        public int PasswordFailureCount { get; set; }

        [Display(Name = "最後密碼失敗日期")]
        public DateTime? LastPasswordFailureDate { get; set; }

        [Display(Name = "最後封鎖日期")]
        public DateTime? LastLockedDate { get; set; }

        [Display(Name = "最後更新密碼日期")]
        public DateTime? LastPasswordChangedDate { get; set; }

        public Guid RoleId { get; set; }
        [Display(Name = "權限")]
        public Role Role { get; set; }

        public Guid OrginId { get; set; }
        [Display(Name = "單位部門")]
        public Orgin Orgin { get; set; }
    }
}
