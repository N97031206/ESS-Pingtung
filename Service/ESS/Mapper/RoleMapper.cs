using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            base.CreateMap<Domain.Role, Model.Role>();
            base.CreateMap<Model.Role, Domain.Role>();
        }
    }
}
