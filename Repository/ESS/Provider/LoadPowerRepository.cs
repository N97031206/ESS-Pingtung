using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class LoadPowerRepository : GenericRepository<LoadPower>
    {
        public LoadPowerRepository() : base(new ESSContext()) { }

        public override void Create(LoadPower load)
        {
            if (load == null)
            {
                throw new ArgumentNullException();
            }

            base.Create(load);
        }
    }
}
