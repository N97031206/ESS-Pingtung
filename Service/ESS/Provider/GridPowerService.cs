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

        private IMapper mapper = null;

        private GridPowerRepository gridPowerRepository = new GridPowerRepository();

        public GridPowerService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
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
            Domain.GridPower account = gridPowerRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.GridPower>(account);
        }


        public Model.GridPower ReadByIDIndex0(Guid ID)
        {
            Domain.GridPower account = gridPowerRepository.ReadBy(x => x.Id == ID && x.index==0);
            return this.mapper.Map<Model.GridPower>(account);
        }


        public List<Model.GridPower> ReadByListID(List<ESSObject> ESSList)
        {
            List<Domain.GridPower> gridPowersList = new List<Domain.GridPower>();
            string[] IDs=null;
            ESSList.ForEach(x => { IDs = x.GridPowerIDs.Trim().Split('|'); });
            foreach (var gp in IDs)
            {
                Guid gpID = Guid.Parse(gp);
                gridPowersList.AddRange(gridPowerRepository.ReadListBy(x => x.Id == gpID).ToList());
            }
            return this.mapper.Map < List<Model.GridPower>>(gridPowersList);
        }

        public Model.GridPower ReadNow()
        {
            Domain.GridPower gridPower = gridPowerRepository.ReadAll().OrderByDescending(x=>x.date_time).FirstOrDefault();
            return this.mapper.Map<Model.GridPower>(gridPower);
        }

        public List<Model.GridPower> ReadByInfoList(DateTime StartTime,DateTime endTime)
        {
            List<Domain.GridPower> gridPowersList= gridPowerRepository.ReadListBy(x => x.date_time >= StartTime && x.date_time < endTime) .ToList();
            return this.mapper.Map<List<Model.GridPower>>(gridPowersList);
        }

        public int Count(DateTime StartTime, DateTime endTime)
        {
            return gridPowerRepository.ReadListBy(x => x.date_time >= StartTime && x.date_time < endTime).Count(); 
        }


        public float minuskHWt(DateTime dateTime, float kWht, int index)
        {
            DateTime utc8 = dateTime.AddHours(8);
            DateTime basetime= new DateTime(utc8.Year, utc8.Month, utc8.Day).AddHours(-8);
            var basekWht = gridPowerRepository.ReadListBy(x => x.date_time < basetime && x.index==index).OrderByDescending(x => x.date_time).FirstOrDefault().kWHt;
            return kWht- basekWht;
        }


    }
}
