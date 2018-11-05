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
    public class MessageService 
    {
        private MapperConfiguration mapperConfiguration =
           new MapperConfiguration(cfg =>
           {
               cfg.AddProfile<MessageMapper>();
           });

        private IMapper mapper = null;
        private MessageRepository messageRepository = new MessageRepository();
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();//Log檔

        public MessageService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public Guid Create(Model.Message message)
        {
            Domain.Message domainMessage = this.mapper.Map<Domain.Message>(message);
            try
            {
                messageRepository.Create(domainMessage);
                messageRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
            }
            return domainMessage.Id;
        }


    }
}
