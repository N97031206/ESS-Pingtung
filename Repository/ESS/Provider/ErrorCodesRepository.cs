using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class ErrorCodesRepository : GenericRepository<ErrorCodes>
    {
        public ErrorCodesRepository() : base(new ESSContext()) { }
        public override void Create(ErrorCodes errorCodes)
        {
            if (errorCodes == null)
            {
                throw new ArgumentNullException();
            }

            errorCodes.CreateTimet = DateTime.Now;
            errorCodes.UpdateTimet = DateTime.Now;
            base.Create(errorCodes);
        }

        public override void Update(ErrorCodes errorCodes)
        {
            if (errorCodes == null)
            {
                throw new ArgumentNullException();
            }

            errorCodes.UpdateTimet = DateTime.Now;

            base.Update(errorCodes);
        }

    }
}
