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
    public class OrginService
    {
        private MapperConfiguration mapperConfiguration =
    new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<RoleMapper>();
        cfg.AddProfile<AccountMapper>();
        cfg.AddProfile<OrginMapper>();
    });

        private readonly IMapper mapper = null;

        private OrginRepository orginRepository = new OrginRepository();

        public OrginService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Orgin ReadID(Guid ID)
        {
            Domain.Orgin orginData = orginRepository.ReadBy(x => x.Id==ID );
            return this.mapper.Map<Model.Orgin>(orginData);
        }

        public List<Model.Orgin> ReadAll()
        {
            List<Domain.Orgin> domainOrgins = orginRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.Orgin>>(domainOrgins);
        }

        public Guid Create(Model.Orgin station)
        {
            Domain.Orgin domainOrgins = this.mapper.Map<Domain.Orgin>(station);

            orginRepository.Create(domainOrgins);
            orginRepository.SaveChanges();

            return domainOrgins.Id;
        }
    }
}
