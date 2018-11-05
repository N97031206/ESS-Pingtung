using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository() : base(new ESSContext()) { }

        public override void Create(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }

            message.IsHandled = false;
            message.CreateDate = DateTime.Now;

            base.Create(message);
        }
    }
}
