using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;

namespace Service.ESS.Provider
{
   public class HistoryService
    {
        private MapperConfiguration mapperConfiguration =
           new MapperConfiguration(cfg =>
           {
               cfg.AddProfile<AlartMapper>();
               cfg.AddProfile<ESSObjectMapper>();
           });

        private IMapper mapper = null;

        private AlartRepository alartRepository = new AlartRepository();
        private AlartTypeRepository alarttypeRepository = new AlartTypeRepository();
        private StationRepository stationRepository = new StationRepository();
        private ESSObjectRepository objectRepository = new ESSObjectRepository();

        public HistoryService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.ESSObject> ReadAll()
        {
            List<Domain.ESSObject> domainalart = objectRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.ESSObject>>(domainalart);
        }

        public List<Model.ESSObject> ReadListBy(DateTime SD, DateTime ED, Guid alarttypeID, Guid stationID)
        {
            string AlartTypeAll = "所有異常";//所有異常代碼
            string StationAll = "所有站別";//所有站別代碼
            bool alartAll = alarttypeRepository.ReadBy(x => x.Id == alarttypeID).AlartTypeCode.Equals(AlartTypeAll);
            bool sationAll = stationRepository.ReadBy(x => x.Id == stationID).StationCode.Equals(StationAll);

            List<Domain.ESSObject> domainalart = new List<Domain.ESSObject>();


            return this.mapper.Map<List<Model.ESSObject>>(domainalart);
        }



    }
}
