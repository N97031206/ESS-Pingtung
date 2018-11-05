using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class BatteryMapper : Profile
    {
        public BatteryMapper()
        {
            base.CreateMap<Domain.Battery, Model.Battery>();
            base.CreateMap<Model.Battery, Domain.Battery>();
        }
    }
}
