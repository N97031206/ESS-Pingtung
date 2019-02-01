﻿using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;

namespace Service.ESS.Provider
{
    public class BatteryService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BatteryMapper>();
            });

        private IMapper mapper = null;
      
        private BatteryRepository batteryRepository = new BatteryRepository();
      
        public BatteryService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.Battery ReadByID(Guid ID)
        {
            Domain.Battery account = batteryRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.Battery>(account);
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
            List<Domain.Battery> batteryPower = batteryRepository.ReadAll().Where(x=>x.uuid==uid).OrderByDescending(x => x.updateTime).Take(4).ToList();
            return this.mapper.Map<List<Domain.Battery>>(batteryPower);
        }


        public List<Model.Battery> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.Battery> gridPowersList = batteryRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).ToList();
            return this.mapper.Map<List<Model.Battery>>(gridPowersList);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return batteryRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).Count(); 
        }

        public double totalSOC(Guid id)
        {

            return (batteryRepository.ReadAll().Count(x => x.uuid == id) == 0 ? 0 :(batteryRepository.ReadAll().Where(x=>x.uuid==id) .OrderByDescending(x => x.updateTime).Take(4).ToList().Average(x => x.voltage) - 42) / (58 - 42) * 100.00);
        }

        public double EachSOC(float voltage)
        {
            return ((voltage - 42) / (58 - 42)) * 100.00;
        }

    }
}
