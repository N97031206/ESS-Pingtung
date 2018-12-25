using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;
using NLog;
using PagedList;
using Service.ESS.Model;

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
        private InverterRepository inverterRepository = new InverterRepository();
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

        public List<Model.ESSObject> ReadTimeIntervalStation(DateTime Start, DateTime End,string StationUUID)
        {
            List<Domain.ESSObject> domainEss =
                objectRepository.ReadListBy(x => x.UpdateDate >= Start && x.UpdateDate <= End && x.stationUUID== StationUUID).
                OrderByDescending(x => x.UpdateDate).ToList();
            return this.mapper.Map<List<Model.ESSObject>>(domainEss);
        }

        public Model.ESSObject ReadNow()
        {
            Domain.ESSObject ReadNow = objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            return this.mapper.Map<Model.ESSObject>(ReadNow);
        }      


    }  
}
