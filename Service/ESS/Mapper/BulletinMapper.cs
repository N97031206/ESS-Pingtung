using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class BulletinMapper : Profile
    {
        public BulletinMapper()
        {
            base.CreateMap<Domain.Bulletin, Model.Bulletin>();
            base.CreateMap<Model.Bulletin, Domain.Bulletin>();
        }
    }
}
