using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Message
    {
        [Display(Name = "訊息編號")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "電子信箱")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "內容")]
        public string Content { get; set; }
    }
}
