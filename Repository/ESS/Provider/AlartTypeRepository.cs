using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class AlartTypeRepository : GenericRepository<AlartType>
    {
        public AlartTypeRepository() : base(new ESSContext()) { }

        public override void Create(AlartType alartType)
        {
            if (alartType == null)
            {
                throw new ArgumentNullException();
            }

            base.Create(alartType);
        }

    }
}
