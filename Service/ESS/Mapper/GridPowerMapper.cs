using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class GridPowerMapper : Profile
    {
        public GridPowerMapper()
        {
            base.CreateMap<Domain.GridPower, Model.GridPower>();
            base.CreateMap<Model.GridPower, Domain.GridPower>();
        }
    }
}
