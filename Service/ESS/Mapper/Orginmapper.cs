using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class OrginMapper:Profile
    {
        public OrginMapper()
        {
            base.CreateMap<Domain.Orgin, Model.Orgin>();
            base.CreateMap<Model.Orgin, Domain.Orgin>();
        }
    }
}
