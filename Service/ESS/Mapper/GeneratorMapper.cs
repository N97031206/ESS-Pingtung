using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class GeneratorMapper : Profile
    {
        public GeneratorMapper()
        {
            base.CreateMap<Domain.Generator, Model.Generator>();
            base.CreateMap<Model.Generator, Domain.Generator>();
        }
    }
}
