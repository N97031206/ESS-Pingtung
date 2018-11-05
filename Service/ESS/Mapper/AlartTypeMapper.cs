using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class AlartTypeMapper : Profile
    {
        public AlartTypeMapper()
        {
            base.CreateMap<Domain.AlartType, Model.AlartType>();
            base.CreateMap<Model.AlartType, Domain.AlartType>();
        }
    }
}
