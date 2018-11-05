using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class Orgin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrginCode { get; set; }

        [Required]
        public String OrginName { get; set; }

    }
}

