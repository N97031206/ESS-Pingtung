using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Web.Models.Json
{
    public class schemaJson
    {
        public DateTime updateTime { get; set; }
        public string stationUUID { get; set; }
        public string stationName { get; set; }
        public IList<GridPowers> GridPowers { get; set; }
        public IList<LoadPowers> LoadPowers { get; set; }
        public IList<Generators> Generators { get; set; }
        public IList<Inverters> Inverters { get; set; }
        public IList<Battery> Battery { get; set; }
    }
}