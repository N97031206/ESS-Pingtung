using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Repository.ESS.Domain
{
    public class LoadPower
    {
        /// 帳戶編號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int version { get; set; }    //	model版本	
        public int index { get; set; }  //	陣列編號	
        public string modelSerial { get; set; } //	型號	
        public string serialNO { get; set; }    //	序號	
        public string name { get; set; }    //	名稱	
        public bool connected { get; set; } //	連線通訊狀態
        public DateTime date_Time { get; set; }
        //start
        public float VA { get; set; }   //	A相電壓	V
        public float VB { get; set; }   //	B相電壓	V
        public float VC { get; set; }   //	C相電壓	V
        public float Vavg { get; set; } //	平均電壓	V
        public float Ia { get; set; }   //	A相電流	A
        public float Ib { get; set; }   //	B相電流	A
        public float Ic { get; set; }   //	C相電流	A
        public float In { get; set; }   //	N相電流	A
        public float Isum { get; set; } //	總電流	A
        public float Watt_a { get; set; }   //	A相實功率	W
        public float Watt_b { get; set; }   //	B相實功率	W
        public float Watt_c { get; set; }   //	C相實功率	W
        public float Watt_t { get; set; }   //	總實功率	W
        public float Var_a { get; set; }    //	A相虛功率	VAR
        public float Var_b { get; set; }    //	B相虛功率	VAR
        public float Var_c { get; set; }    //	C相虛功率	VAR
        public float Var_t { get; set; }    //	總虛功率	VAR
        public float VA_a { get; set; } //	A相視在功率	VA
        public float VA_b { get; set; } //	B相視在功率	VA
        public float VA_c { get; set; } //	C相視在功率	VA
        public float VA_t { get; set; } //	總視在功率	VA
        public float PF_a { get; set; } //	A相功率因數	NA
        public float PF_b { get; set; } //	B相功率因數	NA
        public float PF_c { get; set; } //	C相功率因數	NA
        public float PF_t { get; set; } //	總功率因數	NA
        public float Angle_Va { get; set; } //	A相電壓角度	degree
        public float Angle_Vb { get; set; } //	B相電壓角度	degree
        public float Angle_Vc { get; set; } //	C相電壓角度	degree
        public float Angle_Ia { get; set; } //	A相電流角度	degree
        public float Angle_Ib { get; set; } //	B相電流角度	degree
        public float Angle_Ic { get; set; } //	C相電流角度	degree
        public float Frequency { get; set; }    //	頻率	Hz
        public float Vab { get; set; }  //	AB線間電壓	V
        public float Vbc { get; set; }  //	BC線間電壓	V
        public float Vca { get; set; }  //	CA線間電壓	V
        public float VIIavg { get; set; }   //	線間平均電壓	V
        public float kWHt { get; set; } //	總實功電能	kWh

        //20181203新增
        public float MinuskWHt { get; set; } //相差總實功電能(與前一筆)

        public float kWHa { get; set; } //	A相實功電能	kWh
        public float kWHb { get; set; } //	B相實功電能	kWh
        public float kWHc { get; set; } //	C相實功電能	kWh
        public float kVarHt { get; set; }   //	總虛功電能	kVARh
        public float kVarHa { get; set; }   //	A相虛功電能	kVARh
        public float kVarHb { get; set; }   //	B相虛功電能	kVARh
        public float kVarHc { get; set; }   //	C相虛功電能	kVARh
        public float kVAHt { get; set; }    //	總視在電能	kVAh
        public float kVAHa { get; set; }    //	A相視在電能	kVAh
        public float kVAHb { get; set; }    //	B相視在電能	kVAh
        public float kVAHc { get; set; }    //	C相視在電能	kVAh
        public float Demand { get; set; }   //	需量	kW
        public float prev_demand { get; set; }	//	需量	kW
        public float prev_demand2 { get; set; }	//	需量	kW
        public float prev_demand3 { get; set; }	//	需量	kW
        public float maxdemand_currnetmonth { get; set; }	//	需量	kW
        public float maxdemand_lastmonth { get; set; }	//	需量	kW
        public float remain_time { get; set; }  //	需量	kW

        //event
        public string IsCurrent { get; set; }
        public string ErrorMessage { get; set; }
        public string event_info { get; set; }
        public string event_date_time { get; set; }
        //info
        public string Alarm { get; set; }
        public string ELeve { get; set; }
        public string EType { get; set; }
        public string ELoop { get; set; }
        //end
    }
}
