using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class AlartMapper : Profile
    {
        public AlartMapper()
        {
            base.CreateMap<Domain.Alart, Model.Alart>();
            base.CreateMap<Model.Alart, Domain.Alart>();
        }
    }
}
