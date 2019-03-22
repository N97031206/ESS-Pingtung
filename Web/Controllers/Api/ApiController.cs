using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.ESS.Model;
using Service.ESS.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.WebPages;
using Support.Authorize;
using Battery = Service.ESS.Model.Battery;


namespace Web.Controllers.Api
{
    public class ApiController : System.Web.Http.ApiController
    {
        #region private
        //Account
        private AccountService accountService = new AccountService();
        private RoleService roleService = new RoleService();
        //Tab
        private BulletinService bulletinService = new BulletinService();
        private readonly StationService stationService = new StationService();
        private AlartService alartService = new AlartService();
        private AlartTypeService alarttypeService = new AlartTypeService();
        private OrginService orginService = new OrginService();
        //EMS
        private readonly ESSObjecterService essObjecterService = new ESSObjecterService();
        private BatteryService BatteryService = new BatteryService();
        private GridPowerService GridPowerService = new GridPowerService();
        private GeneratorService GeneratorService = new GeneratorService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService inverterService = new InverterService();
        private readonly ErrorCodesService errorCodesService = new ErrorCodesService();
        #endregion

        [Route("api/BatteryJson")]
        [HttpGet]
        public IHttpActionResult BatteryJson()
        {
            var BatteryJson = BatteryService.ReadByInfoList(DateTime.Today, DateTime.Now);
            return Json(BatteryJson);
        }


        [Route("api/APPLogin")]
        [HttpPost]
        public IHttpActionResult APPLogin(string UserName, string Password)
        {
            Account account = accountService.ReadBy(UserName, Password);
            chkLogin chklogin = new chkLogin();
            chklogin.Check = (account != null)?true:false;
            chklogin.Level = (account != null)?(int)Enum.Parse(typeof(RoleType), roleService.ReadByID(account.RoleId).Type.ToString()):999;
            return Ok(chklogin);
        }



        [Route("api/Index")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            Guid uuid = stationService.UUID(2);
            double soc = BatteryService.TotalSOC(uuid) * 20.0;
            double LoadWatt = LoadPowerService.ReadNow(uuid).Watt_t;
            string Data = "[";
            Data += "{";
            Data += "'Location':" + "'大武社區'" + ",";
            Data += "'soc':" + Math.Round(soc, 1) + ",";
            Data += "'hour':" + Math.Round((soc / LoadWatt), 1);
            Data += "}";
            //Data += ",";
            //Data += "{";
            //Data += "'Location':" + "'林邊鄉'" + ",";
            //Data += "'soc':" + Math.Round(soc, 1) + ",";
            //Data += "'hour':" + Math.Round((soc / LoadWatt), 1);
            //Data += "}";
            //Data += ",";
            //Data += "{";
            //Data += "'Location':" + "'林邊鄉'" + ",";
            //Data += "'soc':" + Math.Round(soc, 1) + ",";
            //Data += "'hour':" + Math.Round((soc / LoadWatt), 1);
            //Data += "}";
            Data += "]";
            return Ok(Data);
        }

        /// <summary>
        /// 佈告欄
        /// </summary>
        /// <returns></returns>
        [Route("api/Tab/Bulletin")]
        [HttpPost]
        public IHttpActionResult Bulletin([FromBody]BullPost date)
        {
            var Bulletun = bulletinService.ReadAll().Where(x => x.Disabled == false).ToList();
            int i = 0;
            string Data = "[";
            Bulletun.ForEach(x =>
            {
                Data += "{";
                Data += "'year':" + x.CreateDate.Year + ",";
                Data += "'month':" + x.CreateDate.Month + ",";
                Data += "'day':" + x.CreateDate.Day + ",";
                Data += "'hour':" + x.CreateDate.Hour + ",";
                Data += "'minute':" + x.CreateDate.Minute + ",";
                Data += "'title':'" + x.title.Trim() + "'" + ",";
                Data += "'orgin':'" + orginService.ReadID(x.OrginID).OrginName.ToString().Trim() + "',";
                Data += "'content':'" + x.context.Trim() + "'";
                i++;
                Data += i < Bulletun.Count() ? "}," : "}";
            });
            Data += "]";
            return Ok(Data);
        }

        //收Android  Post過來的資料
        public class BullPost
        {
            public string energy { get; set; }
        }

        [Route("api/Tab/BullGet")]
        [HttpGet]
        public IHttpActionResult BullGet()
        {
            var Bulletun = bulletinService.ReadAll().Where(x => x.Disabled == false).ToList();
            int i = 0;
            string Data = "[";
            Bulletun.ForEach(x =>
            {
                Data += "{";
                Data += "'year':" + x.CreateDate.Year + ",";
                Data += "'month':" + x.CreateDate.Month + ",";
                Data += "'day':" + x.CreateDate.Day + ",";
                Data += "'hour':" + x.CreateDate.Hour + ",";
                Data += "'minute':" + x.CreateDate.Minute + ",";
                Data += "'title':'" + x.title.Trim() + "'" + ",";
                Data += "'orgin':'" + orginService.ReadID(x.OrginID).OrginName.ToString().Trim() + "',";
                Data += "'content':'" + x.context.Trim() + "'";
                i++;
                Data += i < Bulletun.Count() ? "}," : "}";
            });
            Data += "]";
            return Ok(Data);
        }

        //統計個別異常數量
        [Route("api/Tab/AbnormalCount")]
        [HttpPost]
        public IHttpActionResult AbnormalCount()
        {
            var abnormal = alartService.ReadListTime(DateTime.Today.AddHours(-8), DateTime.Now.AddHours(-8));
            var type = alarttypeService.ReadAll();
            string Data = "[";
            Data += "{'type':'GridPower','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "市電").FirstOrDefault().Id) + "'},";
            Data += "{'type':'LoadPower','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "負載").FirstOrDefault().Id) + "'},";
            Data += "{'type':'Battery','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "電池").FirstOrDefault().Id) + "'},";
            Data += "{'type':'Inverter','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "逆變器").FirstOrDefault().Id) + "'},";
            Data += "{'type':'Solar','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "太陽能").FirstOrDefault().Id) + "'},";
            Data += "{'type':'Generator','count':'" + abnormal.Count(x => x.AlartType.Id == type.Where(y => y.AlartTypeName == "發電機").FirstOrDefault().Id) + "'}";
            Data += "]";
            return Ok(Data);
        }

        [Route("api/BatteryStr")]
        [HttpPost]
        public IHttpActionResult BatteryStr()
        {
            DateTime starttime = DateTime.Today;
            List<double> sum = new List<double>();
            #region Battery
            //資料       
            string BatteryStr = null;
            BatteryStr = "[";
            for (int i = 0; i < 24; i++)
            {
                var count = BatteryService.ReadByInfoList(starttime.AddHours(-8), starttime.AddHours(-7));
                List<double> batteryVolt = new List<double>();
                List<double> batteryTotalVolt = new List<double>();
                if (count.Count() > 0)
                {
                    int c = 0;
                    foreach (var B in count)
                    {
                        if (c < 4)
                        {
                            batteryVolt.Add(B.voltage);
                            c++;
                            if (c == 4)
                            {
                                batteryTotalVolt.Add((batteryVolt.Average() - 42) / (58 - 42));
                                c = 0;
                            }
                        }
                    }
                }
                else
                {
                    batteryTotalVolt.Add(0);
                }

                double data = batteryTotalVolt.Average() == 0 ? 0 : Math.Round(batteryTotalVolt.Average() * 100.00, 2);
                BatteryStr += (i > 22) ? "{'hour':" + starttime.Hour + ",'data':" + data + "}" : "{'hour':" + starttime.Hour + ",'data':" + data + " },";
                starttime = starttime.AddHours(1);
            }
            BatteryStr += "]";
            #endregion
            return Ok(BatteryStr);
        }



        [Route("api/GridPowerStr")]
        [HttpPost]
        public IHttpActionResult GridPowerStr()
        {
            DateTime starttime = DateTime.Today;
            List<double> sum = new List<double>();
            #region GridPower
            //資料       
            string GridPowerStr="[";
            sum.Clear();
            for (int i = 0; i <= 24; i++)
            {
                if (i > 0)
                {
                    var count = GridPowerService.ReadByInfoList(starttime.AddHours(-9), starttime.AddHours(-8));
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -8 - j;
                        j++;
                        int k2 = -8 - j;
                        count.Clear();
                        count = GridPowerService.ReadByInfoList(starttime.AddHours(k2), starttime.AddHours(k1));
                    }
                    double num0 = count.Count(x => x.index == 0) == 0 ? 0 : count.Where(x => x.index == 0).Average(x => x.kWHt);
                    double num1 = count.Count(x => x.index == 1) == 0 ? 0 : count.Where(x => x.index == 1).Average(x => x.kWHt);
                    double num = num0 + num1;
                    double miner = num - sum.Last();
                    sum.Add(num);
                    double data = (miner < 0) ? 0 : Math.Round(miner, 2);
                    GridPowerStr += (i > 23) ? "{'hour':" + starttime.AddHours(-1).Hour + ",'data':" + data + "}" : "{'hour':" + starttime.AddHours(-1).Hour + ",'data':" + data + " },";
                }
                else
                {
                    var count = GridPowerService.ReadByInfoList(starttime.AddHours(-9), starttime.AddHours(-8));
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -8 - j;
                        j++;
                        int k2 = -8 - j;
                        count.Clear();
                        count = GridPowerService.ReadByInfoList(starttime.AddHours(k2), starttime.AddHours(k1));
                    }
                    double num0 = count.Count(x => x.index == 0) == 0 ? 0 : count.Where(x => x.index == 0).Average(x => x.kWHt);
                    double num1 = count.Count(x => x.index == 1) == 0 ? 0 : count.Where(x => x.index == 1).Average(x => x.kWHt);
                    double num = num0 + num1;
                    sum.Add(num);
                }           
                starttime = starttime.AddHours(1);
            }
            GridPowerStr += "]";
            #endregion
            return Ok(GridPowerStr);
        }

        [Route("api/GeneratorStr")]
        [HttpPost]
        public IHttpActionResult GeneratorStr()
        {
            DateTime starttime = DateTime.Today;
            #region Generator
            //資料       
            string GeneratorStr = "[";
            for (int i = 0; i < 24; i++)
            {
                var count = GeneratorService.ReadByInfoList(starttime.AddHours(-8), starttime.AddHours(-7));
                double data  =(count.Count == 0) ? 0 : Math.Round(count.Average(x => x.positiveKWhours) / 1000.00, 2);
                double Oil= (count.Count == 0) ? 0 : Math.Round(count.Average(x => x.FuleLevel) , 2);
                GeneratorStr += (i > 22) ? "{'hour':" + starttime.Hour + ",'data':" + data + ",'Oil':"+Oil+"}" : "{'hour':" + starttime.Hour + ",'data':" + data + ",'Oil':" + Oil + "},";
                starttime = starttime.AddHours(1);
            }
            GeneratorStr += "]";
            #endregion
            return Ok(GeneratorStr);
        }

        [Route("api/SolarStr")]
        [HttpPost]
        public IHttpActionResult SolarStr()
        {
            DateTime starttime = DateTime.Today;
            #region Solar
            //資料       
            string SolarStr = "[";
            for (int i = 0; i < 24; i++)
            {
                var count = inverterService.ReadByInfoList(starttime.AddHours(-8), starttime.AddHours(-7));
                double data = Math.Round((count.Count == 0) ? 0 : (count.Average(x => x.SPM90ActivePower.Split('|').ToList().Sum(y => y.IsEmpty() ? 0 : Convert.ToDouble(y) / 1000.00))), 2);
                SolarStr += (i > 22) ? "{'hour':" + starttime.Hour + ",'data':" + data + "}" : "{'hour':" + starttime.Hour + ",'data':" + data + " },";
                starttime = starttime.AddHours(1);
            }
            SolarStr += "]";
            #endregion
            return Ok(SolarStr);
        }

        [Route("api/LoadPowerStr")]
        [HttpPost]
        public IHttpActionResult LoadPowerStr()
        {
            DateTime starttime = DateTime.Today;
            List<double> sum = new List<double>();
            #region LoadPower
            //資料       
            string LoadPowerStr = "[";
            sum.Clear();
            for (int i = 0; i <= 24; i++)
            {           
                if (i > 0)
                {
                    var count = LoadPowerService.ReadByInfoList(starttime.AddHours(-9), starttime.AddHours(-8));
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -8 - j;
                        j++;
                        int k2 = -8 - j;
                        count.Clear();
                        count = LoadPowerService.ReadByInfoList(starttime.AddHours(k2), starttime.AddHours(k1));
                    }
                    double num0 = count.Count(x => x.index == 2) == 0 ? 0 : count.Where(x => x.index == 2).Average(x => x.kWHt);
                    double num1 = count.Count(x => x.index == 3) == 0 ? 0 : count.Where(x => x.index == 3).Average(x => x.kWHt);
                    double num = num0 + num1;
                    double miner = num - sum.Last();
                    sum.Add(num);

                    double  data = (miner < 0) ? 0 : Math.Round(miner, 2);
                    LoadPowerStr += (i > 23) ? "{'hour':" + starttime.AddHours(-1).Hour + ",'data':" + data + "}" : "{'hour':" + starttime.AddHours(-1).Hour + ",'data':" + data + "},";
                }
                else
                {
                    var count = LoadPowerService.ReadByInfoList(starttime.AddHours(-9), starttime.AddHours(-8));
                    int j = 0;
                    while (count.Count < 1)
                    {
                        int k1 = -8 - j;
                        j++;
                        int k2 = -8 - j;
                        count.Clear();
                        count = LoadPowerService.ReadByInfoList(starttime.AddHours(k2), starttime.AddHours(k1));
                    }
                    double num0 = count.Count(x => x.index == 2) == 0 ? 0 : count.Where(x => x.index == 2).Average(x => x.kWHt);
                    double num1 = count.Count(x => x.index == 3) == 0 ? 0 : count.Where(x => x.index == 3).Average(x => x.kWHt);
                    double num = num0 + num1;
                    sum.Add(num);
                }
                starttime = starttime.AddHours(1);
            }
            LoadPowerStr += "]";
            #endregion

            return Ok(LoadPowerStr);
        }

        private class Chart
        {
            public int hour { get; set; }
            public double data { get; set; }
        }

        private class genChart
        {
            public int hour { get; set; }
            public double data { get; set; }
            public double Oil { get; set; }
        }


        private class chkLogin
        {
            public bool Check { get; set; }
            public int Level { get; set; }
        }
    }
}
