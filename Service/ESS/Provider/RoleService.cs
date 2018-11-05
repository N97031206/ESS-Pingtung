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
    public class RoleService
    {
        private MapperConfiguration mapperConfiguration =
           new MapperConfiguration(cfg =>
           {
               cfg.AddProfile<RoleMapper>();
           });

        private IMapper mapper = null;
        private RoleRepository roleRepository = new RoleRepository();

        public RoleService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Role ReadBy(RoleType roleType)
        {
            Domain.Role domainRole = roleRepository.ReadBy(x => x.Type == (int)roleType);
            return this.mapper.Map<Model.Role>(domainRole);
        }

        public Model.Role ReadByID(Guid RoldID)
        {
            Domain.Role domainRole = roleRepository.ReadBy(x => x.Id == RoldID);
            return this.mapper.Map<Model.Role>(domainRole);
        }



        public List<Model.Role> ReadAll()
        {
            List<Domain.Role> domainstations = roleRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.Role>>(domainstations);
        }

        public Guid Create(Model.Role station)
        {
            Domain.Role domainRole = this.mapper.Map<Domain.Role>(station);

            roleRepository.Create(domainRole);
            roleRepository.SaveChanges();

            return domainRole.Id;
        }

    }
}
