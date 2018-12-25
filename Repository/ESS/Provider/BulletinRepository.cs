using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class BulletinRepository : GenericRepository<Bulletin>
    {
        public BulletinRepository() : base(new ESSContext()) { }
        public override void Create(Bulletin bulletin)
        {
            if (bulletin == null)
            {
                throw new ArgumentNullException();
            }

            bulletin.CreateDate = DateTime.Now;

            base.Create(bulletin);
        }

        public override void Update(Bulletin bulletin)
        {
            if (bulletin == null)
            {
                throw new ArgumentNullException();
            }

            bulletin.UpdateDate = DateTime.Now;

            base.Update(bulletin);
        }


        public override void Delete(Bulletin bulletin)
        {
            if (bulletin == null)
            {
                throw new ArgumentNullException();
            }

            base.Delete(bulletin);
        }


    }
}
