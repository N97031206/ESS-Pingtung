using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class forget
    {
        public string UserMail { get; set; }
        public string UserGuest { get; set; }
        public string UserPassword { get; set; }
    }
}