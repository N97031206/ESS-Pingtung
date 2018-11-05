using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class AlartRepository : GenericRepository<Alart>
    {
        public AlartRepository() : base(new ESSContext()) { }

        public override void Create(Alart alart)
        {
            if (alart == null)
            {
                throw new ArgumentNullException();
            }

            alart.StartTimet = DateTime.Now;
            alart.EndTimet = DateTime.Now;
            alart.Disabled = false;

            base.Create(alart);
        }


        public override void Update(Alart alart)
        {
            if (alart == null)
            {
                throw new ArgumentNullException();
            }
            alart.EndTimet = DateTime.Now;

            base.Update(alart);
        }
    }
}
