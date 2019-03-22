using AutoMapper;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Provider
{
    public class ESSObjecterService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ESSObjectMapper>();
            });

        private readonly IMapper mapper = null;
        private ESSObjectRepository objectRepository = new ESSObjectRepository();



        public ESSObjecterService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public Guid Create(Model.ESSObject eSSObject)
        {
            Domain.ESSObject domain= mapper.Map<Domain.ESSObject>(eSSObject);
            objectRepository.Create(domain);
            objectRepository.SaveChanges();
            return domain.Id;
        }

        public List<Model.ESSObject> ReadAll()
        {
            return mapper.Map<List<Model.ESSObject>>(objectRepository.ReadAll()).ToList();
        }

        public List<Model.ESSObject> ReadTimeInterval(DateTime Start, DateTime End)
        {
            return mapper.Map<List<Model.ESSObject>>(objectRepository.ReadListBy(x => x.UpdateDate >= Start && x.UpdateDate <= End).OrderByDescending(x => x.UpdateDate)).ToList();
        }

        public List<Model.ESSObject> ReadTimeIntervalStation(DateTime Start, DateTime End,string StationUUID)
        { 
            return mapper.Map<List<Model.ESSObject>>(objectRepository.ReadListBy(x => x.UpdateDate >= Start && x.UpdateDate <= End && x.stationUUID == StationUUID).OrderByDescending(x => x.UpdateDate)).ToList();
        }

        public Model.ESSObject ReadNow()
        {
            return mapper.Map<Model.ESSObject>(objectRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault());
        }

        public Model.ESSObject ReadNowUid(Guid uid)
        {
            Domain.ESSObject ReadNowUid = objectRepository.ReadAll().Where(x=>x.stationUUID==uid.ToString()).OrderByDescending(x => x.CreateTime).FirstOrDefault();
            return ReadNowUid == null?null: this.mapper.Map<Model.ESSObject>(ReadNowUid);
        }

    }  
}
