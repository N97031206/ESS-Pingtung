using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;
using NLog;

namespace Service.ESS.Provider
{
    public class ESSObjecterService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ESSObjectMapper>();
            });

        private IMapper mapper = null;

        private AlartRepository alartRepository = new AlartRepository();
        private AlartTypeRepository alarttypeRepository = new AlartTypeRepository();
        private StationRepository stationRepository = new StationRepository();
        private ESSObjectRepository objectRepository = new ESSObjectRepository();
        private BatteryRepository batteryRepository = new BatteryRepository();
        private InverterRepository frequencyRepository = new InverterRepository();
        private GeneratorRepository generatorRepository = new GeneratorRepository();
        private GridPowerRepository gridPowerRepository = new GridPowerRepository();
        private LoadPowerRepository loadRepository = new LoadPowerRepository();


        private GridPowerService gridPowerService = new GridPowerService();
        private LoadPowerService LoadPowerService = new LoadPowerService();
        private InverterService InverterService = new InverterService();


        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();//Log檔

        public ESSObjecterService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Guid Create(Model.ESSObject eSSObject)
        {
            Domain.ESSObject domain= this.mapper.Map<Domain.ESSObject>(eSSObject);

            objectRepository.Create(domain);
            objectRepository.SaveChanges();

            return domain.Id;
        }

        public List<Model.ESSObject> ReadAll()
        {
            List<Domain.ESSObject> domainEss = objectRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.ESSObject>>(domainEss);
        }



        public List<Model.ESSObject> ReadTimeInterval(DateTime Start, DateTime End)
        {
            List<Domain.ESSObject> domainEss = 
                objectRepository.ReadListBy(x => x.UpdateDate >= Start && x.UpdateDate <= End).
                OrderByDescending(x => x.UpdateDate).ToList();
            return this.mapper.Map<List<Model.ESSObject>>(domainEss);
        }



        public List<Model.ESSObject> ReadListBy(DateTime SD, DateTime ED,  Guid stationID)
        {
            string StationAll = "S000";//所有站別代碼
            bool sationAll = stationRepository.ReadBy(x => x.Id == stationID).StationCode.Equals(StationAll);

            List<Domain.ESSObject> domainEss = new List<Domain.ESSObject>();        

            return this.mapper.Map<List<Model.ESSObject>>(domainEss);
        }

        public Model.ESSObject ReadNow()
        {
            Domain.ESSObject ReadNow = objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            return this.mapper.Map<Model.ESSObject>(ReadNow);
        }
        


        public List<float> ReadTodayGridPowerkWHT()
        {
            List<float> ZerokWHt = new List<float>();
            List<float> totalkWHt = new List<float>();
            Domain.ESSObject Readnow = objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            Domain.ESSObject Readzero = objectRepository.ReadAll().Where(x => x.CreateTime > DateTime.Today ).OrderBy(x => x.CreateTime).FirstOrDefault(); 
            var ZerogridID = Readzero.GridPowerIDs.Split('|');
            var NowgridID = Readnow.GridPowerIDs.Split('|');
            foreach (var x in ZerogridID)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    Guid id = Guid.Parse(x.Trim());
                    ZerokWHt.Add (gridPowerService.ReadByID(id).kWHt);
                }
            }
            int i = 0;
            foreach (var y in NowgridID)
            {
                if (!string.IsNullOrEmpty(y))
                {
                    Guid id = Guid.Parse(y.Trim());
                    totalkWHt.Add(gridPowerService.ReadByID(id).kWHt- ZerokWHt[i]);
                    i++;
                }
            }
            return totalkWHt;
        }


        public List<float> ReadTodayLoadkWHt()
        {
            List<float> ZerokWHt = new List<float>();
            List<float> totalkWHt = new List<float>();
            Domain.ESSObject Readnow = objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            Domain.ESSObject Readzero = objectRepository.ReadAll().Where(x => x.CreateTime > DateTime.Today).OrderBy(x => x.CreateTime).FirstOrDefault();
            var ZerogridID = Readzero.LoadPowerIDs.Split('|');
            var NowgridID = Readnow.LoadPowerIDs.Split('|');
            foreach (var x in ZerogridID)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    Guid id = Guid.Parse(x.Trim());
                    ZerokWHt.Add(LoadPowerService.ReadByID(id).kWHt);
                }
            }
            int i = 0;
            foreach (var y in NowgridID)
            {
                if (!string.IsNullOrEmpty(y))
                {
                    Guid id = Guid.Parse(y.Trim());
                    totalkWHt.Add(LoadPowerService.ReadByID(id).kWHt - ZerokWHt[i]);
                    i++;
                }
            }
            return totalkWHt;
        }


        public List<double> ReadTodaySolarkPower()
        {
            List<double> ZerokWHt = new List<double>();
            List<double> totalkWHt = new List<double>();
            Domain.ESSObject Readnow = objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            Domain.ESSObject Readzero = objectRepository.ReadAll().Where(x => x.CreateTime >= DateTime.Today).OrderBy(x => x.CreateTime).FirstOrDefault();
            var ZerogridID = Readzero.InvertersIDs .Split('|');
            var NowgridID = Readnow.InvertersIDs.Split('|');
            foreach (var x in ZerogridID)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    Guid id = Guid.Parse(x.Trim());
                    var Energy = InverterService.ReadByID(id).SPM90ActiveEnergy.Split('|');
                    foreach (var e in Energy)
                    {
                        if (!string.IsNullOrEmpty(e))
                        {
                            ZerokWHt.Add(Convert.ToDouble(e));
                        }
                    }                                     
                }
            }
            int i = 0;
            foreach (var y in NowgridID)
            {
                if (!string.IsNullOrEmpty(y))
                {
                    Guid id = Guid.Parse(y.Trim());
                    var Energy = InverterService.ReadByID(id).SPM90ActiveEnergy.Split('|');
                    foreach (var e in Energy)
                    {
                        if (!string.IsNullOrEmpty(e))
                        {
                            totalkWHt.Add(Convert.ToDouble(e)- ZerokWHt[i]);
                            i++;
                        }
                    }
                }
            }
            return totalkWHt;
        }


    }  
}
