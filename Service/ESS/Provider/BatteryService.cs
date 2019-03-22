using AutoMapper;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain = Repository.ESS.Domain;

namespace Service.ESS.Provider
{
    public class BatteryService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BatteryMapper>();
            });

        private readonly IMapper mapper = null;
      
        private BatteryRepository batteryRepository = new BatteryRepository();
      
        public BatteryService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Battery ReadByID(Guid ID)
        {
            return mapper.Map<Model.Battery>(batteryRepository.ReadBy(x => x.Id == ID));
        }

        public Guid Create(Model.Battery model)
        { 
            Domain.Battery domain = this.mapper.Map<Domain.Battery>(model);
            batteryRepository.Create(domain);
            batteryRepository.SaveChanges();
            return domain.Id;
        }

        public List<Domain.Battery> ReadNow(Guid uid)
        {
            return mapper.Map<List<Domain.Battery>>(batteryRepository.ReadListBy(x => x.uuid == uid && x.connected == true).OrderByDescending(x => x.updateTime).Take(4).ToList());
        }

        public List<Model.Battery> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<Model.Battery>>(batteryRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).ToList());
        }

        public List<Model.Battery> ReadByInfoListForUpdata(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<Model.Battery>>(batteryRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).ToList());
        }

        public double TotalSOC(Guid id)
        {
            float AverageVoltage = batteryRepository.ReadAll().Count(x => x.uuid == id) == 0 ?
                0 : (batteryRepository.ReadListBy(x => x.uuid == id && x.connected==true).OrderByDescending(x => x.updateTime).Take(4).Average(x => x.voltage));
            return AverageVoltage<=48.0?0: AverageVoltage>=53?100:((AverageVoltage - 48) / (53 - 48) * 100.00);
        }

        public double EachSOC(float voltage)
        {       
            return voltage<=48.0?0:Math.Round(voltage,2)<48.0?0: voltage>=53?100:((voltage - 48) / (53 - 48) * 100.00);
        }

    }
}
