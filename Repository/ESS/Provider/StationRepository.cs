using System;
using Repository.ESS.Domain;
using Support.EntityFramework;


namespace Repository.ESS.Provider
{
    public class StationRepository : GenericRepository<Station>
    {
        public StationRepository() : base(new ESSContext()) { }
        public override void Create(Station create)
        {
            if (create == null)
            {
                throw new ArgumentNullException();
            }
            base.Create(create);
        }
    }
}
