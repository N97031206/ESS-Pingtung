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
            Domain.Inverter inverter = inverterRepository.ReadAll().OrderByDescending(x => x.CreateTime).FirstOrDefault();
            return this.mapper.Map<Model.Inverter>(inverter);
        }

        public List<Model.Inverter> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.Inverter> inverterList =
                inverterRepository.ReadListBy(x => x.CreateTime>= StartTime && x.CreateTime < endTime).ToList();
            return this.mapper.Map<List<Model.Inverter>>(inverterList);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return inverterRepository.ReadListBy(x => x.CreateTime >= StartTime && x.CreateTime < endTime).Count();
        }

        public List<double> historySPM90ActivePower(DateTime baseTime, DateTime nowTime)
        {
            List<double> data = new List<double>();
            var nowt = inverterRepository.ReadListBy(x => x.CreateTime < nowTime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            var baset = inverterRepository.ReadListBy(x => x.CreateTime < baseTime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            for (int i = 0; i < nowt.Count-1; i++)
            {
                data.Add(Convert.ToDouble(nowt[i]) - Convert.ToDouble(baset[i]));
            }
            return data;
        }


        public float minusEnergy1(DateTime dateTime, float ActiveEnergy, int id)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime = new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            var baseActiveEnergy = inverterRepository.ReadListBy(x => x.CreateTime < basetime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            float baseEnergy = 0;
            baseEnergy = Convert.ToSingle(baseActiveEnergy[0]);
            return ActiveEnergy - baseEnergy;
        }

        public float minusEnergy2(DateTime dateTime, float ActiveEnergy, int id)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime = new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            var baseActiveEnergy = inverterRepository.ReadListBy(x => x.CreateTime < basetime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            float baseEnergy = 0;
            baseEnergy = Convert.ToSingle(baseActiveEnergy[1]);
            return ActiveEnergy - baseEnergy;
        }
    }
}
