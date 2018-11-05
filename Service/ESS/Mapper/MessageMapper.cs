using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            base.CreateMap<Domain.Message, Model.Message>();
            base.CreateMap<Model.Message, Domain.Message>();
        }
    }
}
