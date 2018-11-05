using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class OrginRepository : GenericRepository<Orgin>
    {
        public OrginRepository() : base(new ESSContext()) { }
        public override void Create(Orgin create)
        {
            if (create == null)
            {
                throw new ArgumentNullException();
            }
            base.Create(create);
        }
    }
}
