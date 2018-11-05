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
    public class BulletinService
    {
        private MapperConfiguration mapperConfiguration =
    new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<RoleMapper>();
        cfg.AddProfile<AccountMapper>();
        cfg.AddProfile<BulletinMapper>();
        cfg.AddProfile<StationMapper>();
    });

        private IMapper mapper = null;
        private BulletinRepository bulletinRepository = new BulletinRepository();
        private StationRepository stationRepository = new StationRepository();

        public BulletinService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.Bulletin> ReadAll()
        {
            List<Domain.Bulletin> domainbulletins = bulletinRepository.ReadAll().OrderByDescending(x => x.CreateDate).ToList();
            return this.mapper.Map<List<Model.Bulletin>>(domainbulletins);
        }

        public List<Model.Bulletin> ReadListBy(DateTime SD, DateTime ED)
        {
            List<Domain.Bulletin> domainbulletins = bulletinRepository.ReadListBy(x => x.CreateDate >= SD && x.CreateDate <=ED).OrderByDescending(x => x.CreateDate).ToList();
            return this.mapper.Map<List<Model.Bulletin>>(domainbulletins);
        }

        public Guid Create(Model.Bulletin bulletin)
        {
            Domain.Bulletin domain = this.mapper.Map<Domain.Bulletin>(bulletin);

            bulletinRepository.Create(domain);
            bulletinRepository.SaveChanges();

            return domain.Id;
        }


    }
}
