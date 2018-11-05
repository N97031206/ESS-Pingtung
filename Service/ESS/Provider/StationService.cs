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
    public class StationService
    {

        private MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StationMapper>();
        });

        private IMapper mapper = null;

        private StationRepository stationRepository = new StationRepository();

        public StationService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.Station> ReadAll()
        {
            List<Domain.Station> domainstations = stationRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.Station>>(domainstations);
        }

        public Model.Station ReadID(string StationName)
        {
            Domain.Station ds = stationRepository.ReadBy(x => x.StationName == StationName);
            return this.mapper.Map<Model.Station>(ds);
        }


        public Model.Station ReadID(Guid ID)
        {
            Domain.Station RI = stationRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.Station>(RI);
        }

        public Guid Create(Model.Station station)
        {

            Domain.Station domainStation = this.mapper.Map<Domain.Station>(station);

            stationRepository.Create(domainStation);
            stationRepository.SaveChanges();

            return domainStation.Id;
        }

    }
}
