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

        private readonly IMapper mapper = null;

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

        public Model.Station ReadName(string StationName)
        {
            Domain.Station ds = stationRepository.ReadBy(x => x.StationName == StationName);
            return this.mapper.Map<Model.Station>(ds);
        }

        public Model.Station ReadID(Guid StationID)
        {
            Domain.Station ds = stationRepository.ReadBy(x => x.Id == StationID);
            return this.mapper.Map<Model.Station>(ds);
        }


        public Model.Station ReadData(Guid ID)
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

        public Model.Station ReadUUID(Guid ID)
        {
            Domain.Station UI = stationRepository.ReadBy(x => x.UUID == ID);
            return this.mapper.Map<Model.Station>(UI);
        }

        public Boolean Delete(Guid ID)
        {
            Domain.Station domainStation = stationRepository.ReadBy(x => x.Id == ID);

            if (domainStation.StationName != null)
            {
                stationRepository.Delete(domainStation);
                stationRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Guid Update(Model.Station station)
        {

            Domain.Station domainstation = stationRepository.ReadBy(c => c.Id == station.Id);
            if (domainstation.StationName != null)
            {
                domainstation.StationName = station.StationName;
                domainstation.UUID = station.UUID;
                stationRepository.Update(domainstation);
                stationRepository.SaveChanges();
                return domainstation.Id;
            }
            else
            {
                return Guid.Empty;
            }

        }

    }
}
