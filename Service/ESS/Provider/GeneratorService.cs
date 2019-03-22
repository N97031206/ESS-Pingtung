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

        private readonly IMapper mapper = null;

        private GeneratorRepository generatorRepository = new GeneratorRepository();

        public GeneratorService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Generator ReadByID(Guid ID)
        {
            return mapper.Map<Model.Generator>(generatorRepository.ReadBy(x => x.Id == ID));
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
            return mapper.Map<Model.Generator>(generatorRepository.ReadListBy(x => x.uuid == uid).OrderByDescending(x => x.UpdateTime).FirstOrDefault());
        }

        public List<Model.Generator> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<Model.Generator>>(generatorRepository.ReadListBy(x => x.UpdateTime >= StartTime && x.UpdateTime < endTime).ToList());
        }

 
    }
}
