using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class InverterRepository : GenericRepository<Inverter>
    {
        public InverterRepository() : base(new ESSContext()) { }

        public override void Create(Inverter inverter)
        {
            if (inverter == null)
            {
                throw new ArgumentNullException();
            }

            base.Create(inverter);
        }
    }
}
