using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class Station
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StationCode { get; set; }

        [Required]
        public string StationName { get; set; }

        public Guid UUID { get; set; }
    }
}
