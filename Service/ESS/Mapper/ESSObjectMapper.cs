using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class ESSObjectMapper : Profile
    {
        public ESSObjectMapper()
        {
            base.CreateMap<Domain.ESSObject, Model.ESSObject>();
            base.CreateMap<Model.ESSObject, Domain.ESSObject>();
        }
    }
}
