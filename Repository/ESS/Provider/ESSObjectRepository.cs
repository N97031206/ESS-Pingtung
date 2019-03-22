using System;
using System.Data.Entity;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class ESSObjectRepository : GenericRepository<ESSObject>
    {
        public ESSObjectRepository() : base(new ESSContext()) {}


        public override void Create(ESSObject essObjects)
        {
            if (essObjects == null)
            {
                throw new ArgumentNullException();
            }

            essObjects.CreateTime = DateTime.Now;

            base.Create(essObjects);
        }
    }
}
