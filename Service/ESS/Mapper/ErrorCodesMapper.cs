using AutoMapper;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Mapper
{
    public class ErrorCodeMapper : Profile
    {
        public ErrorCodeMapper()
        {
            base.CreateMap<Domain.ErrorCodes, Model.ErrorCodes>();
            base.CreateMap<Model.ErrorCodes, Domain.ErrorCodes>();
        }
    }
}
