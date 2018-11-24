using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;

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

        private IMapper mapper = null;

        private RoleService roleService = new RoleService();
        private ErrorCodesRepository errorCodesRepository  = new ErrorCodesRepository();

        public ErrorCodesService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.ErrorCodes> ReadAll()
        {
            List<Domain.ErrorCodes> domainstations = errorCodesRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.ErrorCodes>>(domainstations);
        }

        public string ReadContext(string codes)
        {
            string context = errorCodesRepository.ReadBy(x => x.AlartCode == codes.Trim()).AlartContext.ToString();
            return context;
        }

    }
}
