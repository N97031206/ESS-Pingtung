using System;
using Repository.ESS.Domain;
using Support.EntityFramework;


namespace Repository.ESS.Provider
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository() : base(new ESSContext()) { }
        public override void Create(Role create)
        {
            if (create == null)
            {
                throw new ArgumentNullException();
            }
            base.Create(create);
        }
    }
}
