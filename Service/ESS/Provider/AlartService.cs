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
    public class AlartService
    {
        public AlartService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        private MapperConfiguration mapperConfiguration =
    new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<AlartMapper>();
    });

        private IMapper mapper = null;

        private AlartRepository alartRepository = new AlartRepository();
        private AlartTypeRepository alarttypeRepository = new AlartTypeRepository();
        private StationRepository stationRepository = new StationRepository();


        public List<Model.Alart> ReadAll()
        {
            List<Domain.Alart> domainalart = alartRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.Alart>>(domainalart);
        }

        public List<Model.Alart> ReadListBy(DateTime SD, DateTime ED,Guid alarttypeID, Guid stationID)
        {
            string AlartTypeAll = "所有異常";//所有異常
            string StationAll = "所有站別";//所有站別
            bool alartAll = alarttypeRepository.ReadBy(x => x.Id == alarttypeID).AlartTypeName.Equals(AlartTypeAll);
            bool sationAll = stationRepository.ReadBy(x => x.Id == stationID).StationName.Equals(StationAll);

            List<Domain.Alart> domainalart = new List<Domain.Alart>();

            if (sationAll)
            {
                if (alartAll)
                {
                    if (SD == ED)
                    {
                       domainalart = alartRepository.ReadAll().ToList();
                    }
                    else
                    {
                        domainalart = alartRepository.ReadListBy(x => x.StartTimet >= SD && x.StartTimet < ED ).ToList();
                    }
                }
                else
                {
                    if (SD == ED)
                    {
                        domainalart = alartRepository.ReadListBy(x => x.AlartTypeID == alarttypeID).ToList();
                    }
                    else
                    {
                        domainalart = alartRepository.ReadListBy(x => x.StartTimet >= SD && x.StartTimet <= ED && x.AlartTypeID == alarttypeID).ToList();
                    }
                }
            }
            else
            {
                if (alartAll)
                {
                    if (SD == ED)
                    {
                        domainalart = alartRepository.ReadListBy(x => x.StationID== stationID).ToList();
                    }
                    else
                    {
                        domainalart = alartRepository.ReadListBy(x => x.StartTimet >= SD && x.StartTimet <= ED &&  x.StationID == stationID).ToList();
                    }
                }
                else
                {
                    if (SD == ED)
                    {
                        domainalart = alartRepository.ReadListBy(x => x.AlartTypeID == alarttypeID && x.StationID == stationID).ToList();
                    }
                    else
                    {
                         domainalart = alartRepository.ReadListBy(x => x.StartTimet >= SD && x.StartTimet <= ED && x.AlartTypeID== alarttypeID && x.StationID == stationID).ToList();
                    }
                }
            }

            return this.mapper.Map<List<Model.Alart>>(domainalart);
        }


        public Guid Create(Model.Alart alart)
        {
            Domain.Alart domain = this.mapper.Map<Domain.Alart>(alart);

            alartRepository.Create(domain);
            alartRepository.SaveChanges();

            return domain.Id;
        }

    }
}
