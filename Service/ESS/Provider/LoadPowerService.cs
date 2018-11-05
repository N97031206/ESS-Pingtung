﻿using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;
using NLog;

namespace Service.ESS.Provider
{
    public class LoadPowerService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LoadPowersMapper>();
            });

        private IMapper mapper = null;

        private LoadPowerRepository loadRepository = new LoadPowerRepository();

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();//Log檔

        public LoadPowerService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Model.LoadPower ReadByID(Guid ID)
        {
            Domain.LoadPower loadpower = loadRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.LoadPower>(loadpower);
        }

        public Guid Create(Model.LoadPower model)
        {
            Domain.LoadPower domain = this.mapper.Map<Domain.LoadPower>(model);
            try
            {
                loadRepository.Create(domain);
                loadRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
            }
                return domain.Id;
        }

        public Model.LoadPower ReadNow()
        {
            Domain.LoadPower loadPower = loadRepository.ReadAll().OrderByDescending(x => x.updateTime).FirstOrDefault();
            return this.mapper.Map<Model.LoadPower>(loadPower);
        }

        public List<Model.LoadPower> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.LoadPower> loadPower =
                loadRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).ToList();
            return this.mapper.Map<List<Model.LoadPower>>(loadPower);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return loadRepository.ReadListBy(x => x.updateTime >= StartTime && x.updateTime < endTime).Count();
        }
    }
}