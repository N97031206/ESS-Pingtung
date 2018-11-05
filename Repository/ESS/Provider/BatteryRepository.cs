using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class BatteryRepository : GenericRepository<Battery>
    {
        public BatteryRepository() : base(new ESSContext()) { }

        public override void Create(Battery battery)
        {
            if (battery == null)
            {
                throw new ArgumentNullException();
            }

            base.Create(battery);
        }
    }
}
