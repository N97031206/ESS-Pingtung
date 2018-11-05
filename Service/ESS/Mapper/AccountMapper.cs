using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            base.CreateMap<Domain.Account, Model.Account>();
            base.CreateMap<Model.Account, Domain.Account>();
        }
    }
}
