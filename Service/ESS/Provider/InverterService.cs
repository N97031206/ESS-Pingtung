using AutoMapper;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Provider
{
    public class InverterService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InverterMapper>();
            });

        private readonly IMapper mapper = null;

        private InverterRepository inverterRepository = new InverterRepository();
      
        public InverterService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Inverter ReadByID(Guid ID)
        {
            return mapper.Map<Model.Inverter>(inverterRepository.ReadBy(x => x.Id == ID));
        }

        public Guid Create(Model.Inverter model)
        {
            Domain.Inverter domain = this.mapper.Map<Domain.Inverter>(model);
            inverterRepository.Create(domain);
            inverterRepository.SaveChanges();
            return domain.Id;
        }

        public Model.Inverter ReadNow(Guid uid)
        {
            return mapper.Map<Model.Inverter>(inverterRepository.ReadListBy(x => x.uuid == uid).OrderByDescending(x => x.CreateTime).FirstOrDefault());
        }

        public List<Model.Inverter> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<Model.Inverter>>(inverterRepository.ReadListBy(x => x.CreateTime >= StartTime && x.CreateTime < endTime).ToList());
        }

        public List<Model.Inverter> ReadByInfoListForUpdata(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<Model.Inverter>>(inverterRepository.ReadListBy(x => x.CreateTime >= StartTime && x.CreateTime < endTime).ToList());
        }

        public List<double> HistorySPM90ActivePower(DateTime baseTime, DateTime nowTime)
        {
            List<double> data = new List<double>();
            List<string> nowt = inverterRepository.ReadListBy(x => x.CreateTime < nowTime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            List<string> baset = inverterRepository.ReadListBy(x => x.CreateTime < baseTime).OrderByDescending(x => x.CreateTime).FirstOrDefault().SPM90ActiveEnergy.Split('|').ToList();
            for (int i = 0; i < nowt.Count-1; i++)
            {
                data.Add(Convert.ToDouble(nowt[i]) - Convert.ToDouble(baset[i]));
            }
            return data;
        }


        public float MinusEnergy1(DateTime dateTime, float ActiveEnergy, int id,Guid UID)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime = new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            if (inverterRepository.ReadListBy(x => x.CreateTime < basetime && x.uuid == UID).Count() > 0)
            {
                List<string> baseActiveEnergy = inverterRepository
                    .ReadListBy(x => x.CreateTime < basetime && x.uuid == UID)
                    .OrderByDescending(x => x.CreateTime)
                    .FirstOrDefault()
                    .SPM90ActiveEnergy.Split('|').ToList();
                return ActiveEnergy - Convert.ToSingle(baseActiveEnergy[0]);//第一筆資料
            }
            else
            {
                return ActiveEnergy;
            }
        }

        public float MinusEnergy4(DateTime dateTime, float ActiveEnergy, int id, Guid UID)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime = new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            if (inverterRepository.ReadListBy(x => x.CreateTime < basetime && x.uuid == UID).Count() > 0)
            {
                List<string> baseActiveEnergy = inverterRepository
                    .ReadListBy(x => x.CreateTime < basetime && x.uuid == UID)
                    .OrderByDescending(x => x.CreateTime)
                    .FirstOrDefault()
                    .SPM90ActiveEnergy.Split('|').ToList();
                return ActiveEnergy - Convert.ToSingle(baseActiveEnergy[1]);//第二筆資料
            }
            else
            {
                return ActiveEnergy;
            }
        }
    }
}
