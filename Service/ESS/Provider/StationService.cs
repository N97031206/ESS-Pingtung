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

        public Guid Create(Model.Station station)
        {
            Domain.Station domainStation = this.mapper.Map<Domain.Station>(station);
            stationRepository.Create(domainStation);
            stationRepository.SaveChanges();
            return domainStation.Id;
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



        public List<Model.Station> ReadAll()
        {
            return mapper.Map<List<Model.Station>>(stationRepository.ReadAll().ToList());
        }

        public Model.Station ReadName(string StationName)
        {
            return mapper.Map<Model.Station>(stationRepository.ReadBy(x => x.StationName == StationName));
        }

        public Model.Station ReadID(Guid StationID)
        {
            return mapper.Map<Model.Station>(stationRepository.ReadBy(x => x.Id == StationID));
        }

        public Model.Station ReadUUID(Guid ID)
        {
            return mapper.Map<Model.Station>(stationRepository.ReadBy(x => x.UUID == ID));
        }

        public Guid UUID(int code)
        {
            return stationRepository.ReadBy(x => x.StationCode == code).UUID;
        }

        public Guid StationID(int code)
        {
            return stationRepository.ReadBy(x => x.StationCode == code).Id;
        }
    }
}
