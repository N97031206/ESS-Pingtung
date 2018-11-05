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
    public class InverterService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InverterMapper>();
            });

        private IMapper mapper = null;

        private InverterRepository inverterRepository = new InverterRepository();
      
        public InverterService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Inverter ReadByID(Guid ID)
        {
            Domain.Inverter inverter = inverterRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.Inverter>(inverter);
        }

        public Guid Create(Model.Inverter model)
        {
            Domain.Inverter domain = this.mapper.Map<Domain.Inverter>(model);
            inverterRepository.Create(domain);
            inverterRepository.SaveChanges();
            return domain.Id;
        }

        public Model.Inverter ReadNow()
        {
            Domain.Inverter inverter = inverterRepository.ReadAll().OrderByDescending(x => x.UpdateTime).FirstOrDefault();
            return this.mapper.Map<Model.Inverter>(inverter);
        }

        public List<Model.Inverter> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.Inverter> inverterList =
                inverterRepository.ReadListBy(x => x.UpdateTime >= StartTime && x.UpdateTime < endTime).ToList();
            return this.mapper.Map<List<Model.Inverter>>(inverterList);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return inverterRepository.ReadListBy(x => x.UpdateTime >= StartTime && x.UpdateTime < endTime).Count();
        }
    }
}
