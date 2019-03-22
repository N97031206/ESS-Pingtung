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
    public class AlartTypeService
    {
        public AlartTypeService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        private MapperConfiguration mapperConfiguration =
    new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<RoleMapper>();
        cfg.AddProfile<AccountMapper>();
        cfg.AddProfile<AlartMapper>();
        cfg.AddProfile<AlartTypeMapper>();
        cfg.AddProfile<StationMapper>();
    });

        private readonly IMapper mapper = null;

        private AlartTypeRepository alarttypeRepository = new AlartTypeRepository();

        public List<Model.AlartType> ReadAll()
        {
            return mapper.Map<List<Model.AlartType>>(alarttypeRepository.ReadAll().ToList());
        }

        public Model.AlartType ReadID(Guid ID)
        {
            return mapper.Map<Model.AlartType>(alarttypeRepository.ReadBy(x => x.Id == ID));
        }

        public Model.AlartType ID(int i)
        {
            return mapper.Map<Model.AlartType>(alarttypeRepository.ReadBy(x => x.AlartTypeCode == i));
        }

    }
}
