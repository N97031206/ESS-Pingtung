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
            mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Orgin ReadID(Guid ID)
        {
            return mapper.Map<Model.Orgin>(orginRepository.ReadBy(x => x.Id == ID));
        }

        public List<Model.Orgin> ReadAll()
        {
            return mapper.Map<List<Model.Orgin>>(orginRepository.ReadAll().ToList());
        }

        public Guid Create(Model.Orgin orgin)
        {
            Domain.Orgin domainOrgins = this.mapper.Map<Domain.Orgin>(orgin);

            orginRepository.Create(domainOrgins);
            orginRepository.SaveChanges();

            return domainOrgins.Id;
        }


        public Boolean Delete(Guid ID)
        {
            Domain.Orgin domainOrgin = orginRepository.ReadBy(x => x.Id == ID);

            if (domainOrgin.OrginName != null)
            {
                orginRepository.Delete(domainOrgin);
                orginRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Guid Update(Model.Orgin orgin)
        {
            Domain.Orgin domainOrgin = orginRepository.ReadBy(x => x.Id == orgin.Id);
            if (domainOrgin.OrginName != null)
            {
                domainOrgin.OrginName = orgin.OrginName;
                orginRepository.Update(domainOrgin);
                orginRepository.SaveChanges();
                return orgin.Id;
            }
            else
            {
                return Guid.Empty;
            }
        }


    }
}
