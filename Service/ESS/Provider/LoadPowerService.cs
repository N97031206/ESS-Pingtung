using System;
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

        public Model.LoadPower ReadNow(Guid uid)
        {
            Domain.LoadPower loadPower = loadRepository.ReadAll().Where(x=>x.index==2 && x.uuid==uid).OrderByDescending(x => x.date_Time).FirstOrDefault();
            return this.mapper.Map<Model.LoadPower>(loadPower);
        }

        public List<Model.LoadPower> ReadByInfoList(DateTime StartTime, DateTime endTime)
        {
            List<Domain.LoadPower> loadPower =
                loadRepository.ReadListBy(x => x.date_Time >= StartTime && x.date_Time < endTime).ToList();
            return this.mapper.Map<List<Model.LoadPower>>(loadPower);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return loadRepository.ReadListBy(x => x.date_Time >= StartTime && x.date_Time < endTime).Count();
        }


        public float minuskHWt(DateTime dateTime, float kWht, int index)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime = new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            var basekWht = loadRepository.ReadListBy(x => x.date_Time < basetime && x.index==index).OrderByDescending(x => x.date_Time).FirstOrDefault().kWHt;
            return kWht - basekWht;
        }

    }
}
