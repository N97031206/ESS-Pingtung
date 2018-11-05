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


    }
}
