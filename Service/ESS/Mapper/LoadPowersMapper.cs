using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class LoadPowersMapper : Profile
    {
        public LoadPowersMapper()
        {
            base.CreateMap<Domain.LoadPower, Model.LoadPower>();
            base.CreateMap<Model.LoadPower, Domain.LoadPower>();
        }
    }
}
