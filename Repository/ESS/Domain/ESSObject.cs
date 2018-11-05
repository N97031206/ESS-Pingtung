using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.ESS.Domain
{
    public class ESSObject
    {
        /// <summary>
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime UpdateDate { get; set; }
        public string stationUUID { get; set; }
        public string stationName { get; set; }
        /// <summary>
        /// 市電
        /// </summary>
        public string GridPowerIDs { get; set; }
        //[ForeignKey("GridPowerID")]
        //[Column(Order = 1)]
        //public virtual GridPower GridPower { get; set; }
      
        /// <summary>
        /// 負載
        /// </summary>
        public string LoadPowerIDs { get; set; }
        //[ForeignKey("LoadPowerID")]
        //public virtual LoadPower LoadPower{ get; set; }
      
            /// 發電機
        /// </summary>
        public string GeneratorIDs { get; set; }
        //[ForeignKey("GeneratorIDs")]
        //public virtual Generator Generator { get; set; }

        /// <summary>
        /// 逆變
        /// </summary>
        public string InvertersIDs { get; set; }
        //[ForeignKey("InvertersI")]
        //public virtual Inverter Inverter{ get; set; }

        /// <summary>
        /// 電池
        /// </summary>
        public string BatteryIDs { get; set; }
        //[ForeignKey("BatteryID")]
        //public virtual Battery Battery { get; set; }
        /// <summary>

        public DateTime CreateTime { get; set; }

    }
}
