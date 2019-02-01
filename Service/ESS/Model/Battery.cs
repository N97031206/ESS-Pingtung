using System;
using System.ComponentModel.DataAnnotations;



namespace Service.ESS.Model
{
    public class Battery
    {
        [Display(Name = "電池編號")]
        public Guid Id { get; set; }
        public Guid uuid { get; set; } //1070118 站點ID
        public float version { get; set; }
        public float index { get; set; }
        public string modelSerial { get; set; }
        public string serialNO { get; set; }
        public string name { get; set; }
        public bool connected { get; set; }
        public DateTime updateTime { get; set; }
        public float voltage { get; set; }// 總電壓 V
        public float charging_current { get; set; }// 充電電流 A
        public float discharging_current { get; set; }//放電電流 A
        public float charging_watt { get; set; }// 充電功率 W
        public float discharging_watt { get; set; }// 放電功率 W
        public float SOC { get; set; } // 電池容量 %
        public float Cycle { get; set; }// 充電次數 次
        public float charge_direction { get; set; }// 充電方向 0 不充電 1 充電 2 放電
        public float temperature { get; set; }// 溫度 度C
        //cells start
        public string cells_index { get; set; } // 核心陣列索引
        public string cells_voltage { get; set; } // 核心陣列電壓 V
        //Cells end
        //AlarmState start
        public bool OV_DIS { get; set; }// Over Voltage
        public bool UV_DIS { get; set; }// Under Voltage
        public bool OC_DIS { get; set; }// Over Current
        public bool SC_DIS { get; set; }// Short Current
        public bool OT_DIS { get; set; }// Over Temperature
        public bool UT_DIS { get; set; }// Under Temperature
        public bool RV_DIS { get; set; }// Reversal Voltage
        public bool OC0_DIS { get; set; }// Discharge Over Current 0 (OC0_DIS)
        //AlarmState end
    }
}
