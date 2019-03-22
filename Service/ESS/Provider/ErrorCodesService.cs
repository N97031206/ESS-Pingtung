using AutoMapper;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using System.Collections.Generic;
using System.Linq;

namespace Service.ESS.Provider
{
    public class ErrorCodesService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RoleMapper>();
                cfg.AddProfile<AccountMapper>();
            });

        private readonly IMapper mapper = null;
        private ErrorCodesRepository errorCodesRepository  = new ErrorCodesRepository();

        public ErrorCodesService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.ErrorCodes> ReadAll()
        {
            return mapper.Map<List<Model.ErrorCodes>>(errorCodesRepository.ReadAll().ToList());
        }

        public string ReadContext(string codes)
        {
            return errorCodesRepository.ReadBy(x => x.AlartCode == codes.Trim()).AlartContext.ToString();
        }

    }
}
