using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class GridPowerRepository : GenericRepository<GridPower>
    {
        public GridPowerRepository() : base(new ESSContext()) { }

        public override void Create(GridPower gridPower)
        {
            if (gridPower == null)
            {
                throw new ArgumentNullException();
            }
            base.Create(gridPower);
        }
    }
}
