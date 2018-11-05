using System;
using System.ComponentModel.DataAnnotations;

namespace Service.ESS.Model
{
    public class Orgin
    {
        public Guid Id { get; set; }
        public String OrginCode { get; set; }
        public String OrginName { get; set; }

    }
}
