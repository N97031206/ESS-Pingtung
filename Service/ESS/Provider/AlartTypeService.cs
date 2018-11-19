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

        private IMapper mapper = null;

        private AlartTypeRepository alarttypeRepository = new AlartTypeRepository();

        public List<Model.AlartType> ReadAll()
        {
            List<Domain.AlartType> RA = alarttypeRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.AlartType>>(RA);
        }

        public Model.AlartType ReadID(Guid ID)
        {
            Domain.AlartType RI = alarttypeRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.AlartType>(RI);
        }

        public Model.AlartType ID(int i)
        {
            Domain.AlartType RI = alarttypeRepository.ReadBy(x => x.AlartTypeCode == i);
            return this.mapper.Map<Model.AlartType>(RI);
        }

    }
}
