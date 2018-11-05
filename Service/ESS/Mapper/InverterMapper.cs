using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class InverterMapper : Profile
    {
        public InverterMapper()
        {
            base.CreateMap<Domain.Inverter, Model.Inverter>();
            base.CreateMap<Model.Inverter, Domain.Inverter>();
        }
    }
}
