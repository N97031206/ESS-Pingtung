using AutoMapper;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain = Repository.ESS.Domain;

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

        private readonly IMapper mapper = null;
        private BulletinRepository bulletinRepository = new BulletinRepository();
        private readonly StationRepository stationRepository = new StationRepository();

        public BulletinService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.Bulletin> ReadAll()
        {
            return mapper.Map<List<Model.Bulletin>>(bulletinRepository.ReadAll().OrderByDescending(x => x.CreateDate).ToList());
        }

        public List<Model.Bulletin> ReadAllView()
        {
            return mapper.Map<List<Model.Bulletin>>(bulletinRepository.ReadAll().Where(x=>x.Disabled==false).OrderByDescending(x => x.CreateDate).ToList());
        }


        public List<Model.Bulletin> ReadListBy(DateTime SD, DateTime ED)
        {
            return this.mapper.Map<List<Model.Bulletin>>(bulletinRepository.ReadListBy(x => x.CreateDate >= SD && x.CreateDate <= ED).OrderByDescending(x => x.CreateDate)).ToList();
        }

        public Model.Bulletin ReadByID(Guid ID)
        {
            return this.mapper.Map<Model.Bulletin>(bulletinRepository.ReadBy(x => x.Id == ID));
        }

        public Guid Create(Model.Bulletin bulletin)
        {
            Domain.Bulletin domain = this.mapper.Map<Domain.Bulletin>(bulletin);
            bulletinRepository.Create(domain);
            bulletinRepository.SaveChanges();
            return domain.Id;
        }

        public Guid UpdateDisable(Model.Bulletin bulletin)
        {
            Domain.Bulletin domainbulletins = bulletinRepository.ReadBy(x => x.Id ==bulletin.Id);
            domainbulletins.Disabled = bulletin.Disabled;
            domainbulletins.UpdateDate = bulletin.UpdateDate;
            bulletinRepository.Update(domainbulletins);
            bulletinRepository.SaveChanges();
            return domainbulletins.Id;
        }

        public Guid Update(Model.Bulletin bulletin)
        {
            Domain.Bulletin domainbulletins = bulletinRepository.ReadBy(x => x.Id == bulletin.Id);

            if (domainbulletins.title != null)
            {
                domainbulletins.title = bulletin.title;
                domainbulletins.context = bulletin.context;
                domainbulletins.Disabled = bulletin.Disabled;
                domainbulletins.UpdateDate = bulletin.UpdateDate;
                bulletinRepository.Update(domainbulletins);
                bulletinRepository.SaveChanges();
                return domainbulletins.Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Boolean Delete(Guid ID)
        {
            Domain.Bulletin domainbulletins = bulletinRepository.ReadBy(x => x.Id == ID);

            if (domainbulletins.title != null)
            {
                bulletinRepository.Delete(domainbulletins);
                bulletinRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
