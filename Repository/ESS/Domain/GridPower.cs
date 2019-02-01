using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Repository.ESS.Domain
{
    public class GridPower
    {
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //start
        public Guid uuid { get; set; } //1070118 站點ID

        public int version { get; set; }    //	model版本
        public int index { get; set; }  //	陣列編號
        public string modelSerial { get; set; } //	型號
        public string serialNO { get; set; }    //	序號
        public string name { get; set; }    //	名稱
       //public DateTime updateTime { get; set; }
        public DateTime date_time { get; set; }
        public float VA { get; set; }   //	A相電壓
        public float VB { get; set; }   //	B相電壓
        public float VC { get; set; }   //	C相電壓
        public float Vavg { get; set; } //	平均電壓
        public float Ia { get; set; }   //	A相電流
        public float Ib { get; set; }   //	B相電流
        public float Ic { get; set; }   //	C相電流
        public float In { get; set; }   //	N相電流
        public float Isum { get; set; } //	總電流
        public float Watt_a { get; set; }   //	A相實功率
        public float Watt_b { get; set; }   //	B相實功率
        public float Watt_c { get; set; }   //	C相實功率
        public float Watt_t { get; set; }   //	總實功率
        public float Var_a { get; set; }    //	A相虛功率
        public float Var_b { get; set; }    //	B相虛功率
        public float Var_c { get; set; }    //	C相虛功率
        public float Var_t { get; set; }    //	總虛功率
        public float VA_a { get; set; } //	A相視在功率
        public float VA_b { get; set; } //	B相視在功率
        public float VA_c { get; set; } //	C相視在功率
        public float VA_t { get; set; } //	總視在功率
        public float PF_a { get; set; } //	A相功率因數
        public float PF_b { get; set; } //	B相功率因數
        public float PF_c { get; set; } //	C相功率因數
        public float PF_t { get; set; } //	總功率因數
        public float Angle_Va { get; set; } //	A相電壓角度
        public float Angle_Vb { get; set; } //	B相電壓角度
        public float Angle_Vc { get; set; } //	C相電壓角度
        public float Angle_Ia { get; set; } //	A相電流角度
        public float Angle_Ib { get; set; } //	B相電流角度
        public float Angle_Ic { get; set; } //	C相電流角度
        public float Frequency { get; set; }    //	頻率
        public float Vab { get; set; }  //	AB線間電壓
        public float Vbc { get; set; }  //	BC線間電壓
        public float Vca { get; set; }  //	CA線間電壓
        public float VIIavg { get; set; }   //	線間平均電壓
        public float kWHt { get; set; } //	總實功電能

        //20181203新增
        public float MinuskWHt { get; set; } //相差總實功電能(與前一筆)

        public float kWHa { get; set; } //	A相實功電能
        public float kWHb { get; set; } //	B相實功電能
        public float kWHc { get; set; } //	C相實功電能
        public float kVarHt { get; set; }   //	總虛功電能
        public float kVarHa { get; set; }   //	A相虛功電能
        public float kVarHb { get; set; }   //	B相虛功電能
        public float kVarHc { get; set; }   //	C相虛功電能
        public float kVAHt { get; set; }    //	總視在功電能
        public float kVAHa { get; set; }    //	A相視在功電能
        public float kVAHb { get; set; }    //	B相視在功電能
        public float kVAHc { get; set; }    //	C相虛功電能
        public float Demand { get; set; }   //	需量
        public float Prev_Demand { get; set; }   //	上一筆需量
        public float Prev_Demand2 { get; set; }  //	上二筆需量
        public float Prev_Demand3 { get; set; }  //	上三筆需量
        public float Max_Demand_CurrnetMonth { get; set; }  //	當月最大需量
        public float Max_Demand_LastMonth { get; set; } //	上個月最大需量
        public int Remain_Time { get; set; } //	更新剩餘時間
        //event
        public bool IsCurrent { get; set; }
        public string ErrorMessage { get; set; }
        public int event_info { get; set; }
        public DateTime event_date_time { get; set; }
        //info
        public int Alarm { get; set; }
        public int ELeve { get; set; }
        public int EType { get; set; }
        public int ELoop { get; set; }

    }
}
