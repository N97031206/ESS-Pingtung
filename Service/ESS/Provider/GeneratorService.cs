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
    public class GeneratorService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GeneratorMapper>();
            });

        private IMapper mapper = null;

        private GeneratorRepository generatorRepository = new GeneratorRepository();

        public GeneratorService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Generator ReadByID(Guid ID)
        {
            Domain.Generator generator = generatorRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.Generator>(generator);
        }


        public Guid Create(Model.Generator model)
        {
            Domain.Generator domain = this.mapper.Map<Domain.Generator>(model);
            generatorRepository.Create(domain);
            generatorRepository.SaveChanges();
            return domain.Id;
        }

        public Model.Generator ReadNow(Guid uid)
        {
            Domain.Generator generatorPower = generatorRepository.ReadAll().Where(x=>x.uuid==uid).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
            return this.mapper.Map<Model.Generator>(generatorPower);
        }

        public List<Model.Generator> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.Generator> generatorPower =
                generatorRepository.ReadListBy(x => x.UpdateTime >= StartTime && x.UpdateTime < endTime).ToList();
            return this.mapper.Map<List<Model.Generator>>(generatorPower);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return generatorRepository.ReadListBy(x => x.UpdateTime >= StartTime && x.UpdateTime < endTime).Count();
        }


        public double historykWHt(DateTime baseTime, DateTime nowTime, string name)
        {
            var nowt = generatorRepository.ReadListBy(x => x.UpdateTime < nowTime && x.name == name).OrderByDescending(x => x.UpdateTime).FirstOrDefault().totalwatts;
            var baset = generatorRepository.ReadListBy(x => x.UpdateTime < baseTime && x.name == name).OrderByDescending(x => x.UpdateTime).FirstOrDefault().totalwatts;
            return baset - nowt;
        }

    }
}
