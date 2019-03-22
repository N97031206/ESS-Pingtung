using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;
using Service.ESS.Model;

namespace Service.ESS.Provider
{
    public class GridPowerService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GridPowerMapper>();
            });

        private readonly IMapper mapper = null;

        private GridPowerRepository gridPowerRepository = new GridPowerRepository();

        public GridPowerService()
        {
            mapper = mapperConfiguration.CreateMapper();
        }

        public Guid Create(Model.GridPower gridPower)
        {
            Domain.GridPower domain = this.mapper.Map<Domain.GridPower>(gridPower);

            gridPowerRepository.Create(domain);
            gridPowerRepository.SaveChanges();

            return domain.Id;
        }

        public Model.GridPower ReadByID(Guid ID)
        {
            return mapper.Map<Model.GridPower>(gridPowerRepository.ReadBy(x => x.Id == ID));
        }

        public Model.GridPower ReadNow(Guid uid)
        {
            return mapper.Map<Model.GridPower>(gridPowerRepository.ReadListBy(x => x.index == 0 && x.uuid == uid).OrderByDescending(x => x.date_time).FirstOrDefault());
        }

        public List<GridPower> ReadByInfoList(DateTime StartTime,DateTime endTime)
        {
            return mapper.Map<List<GridPower>>(gridPowerRepository.ReadListBy(x => x.date_time >= StartTime && x.date_time < endTime).ToList());
        }


        public List<GridPower> ReadByInfoListForUpdata(DateTime StartTime, DateTime endTime)
        {
            return mapper.Map<List<GridPower>>(gridPowerRepository.ReadListBy(x => x.date_time >= StartTime && x.date_time < endTime).ToList());
        }

        public float MinuskHWt(DateTime dateTime, float kWht, int index,Guid UID)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime= new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            var basekWht = gridPowerRepository
                .ReadListBy(x => x.date_time < basetime && x.index==index && x.uuid== UID)
                .OrderByDescending(x => x.date_time)
                .FirstOrDefault()
                .kWHt;
            return kWht- basekWht;
        }


    }
}
