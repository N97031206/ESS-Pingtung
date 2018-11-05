using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Repository.ESS.Domain
{
    public class Battery
    {
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //start
        public float version { get; set; }
        public float index { get; set; }
        public string modelSerial { get; set; }
        public string serialNO { get; set; }
        public string name { get; set; }
        public bool connected { get; set; }
       public DateTime updateTime { get; set; }
        public float voltage { get; set; }
        public float charging_current { get; set; }
        public float discharging_current { get; set; }
        public float charging_watt { get; set; }
        public float discharging_watt { get; set; }
        public float SOC { get; set; }
        public float Cycle { get; set; }
        public float charge_direction { get; set; }
        public float temperature { get; set; }
        //cells start
        public string cells_index { get; set; }
        public string cells_voltage { get; set; }
        //Cells end
        //AlarmState start
        public bool OV_DIS { get; set; }
        public bool UV_DIS { get; set; }
        public bool OC_DIS { get; set; }
        public bool SC_DIS { get; set; }
        public bool OT_DIS { get; set; }
        public bool UT_DIS { get; set; }
        public bool RV_DIS { get; set; }
        public bool OC0_DIS { get; set; }
        //AlarmState end
        //end
    }
}
