using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class StationMapper : Profile
    {
        public StationMapper()
        {
            base.CreateMap<Domain.Station, Model.Station>();
            base.CreateMap<Model.Station, Domain.Station>();
        }
    }
}
