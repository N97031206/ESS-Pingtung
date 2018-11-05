using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository() : base(new ESSContext()) { }

        public override void Create(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException();
            }

            account.CreateDate = DateTime.Now;
            account.UpdateDate = DateTime.Now;
            account.Disabled = false;
            account.IsApproved = true;
            account.IsLocked = false;
            account.PasswordFailureCount = 0;

            base.Create(account);
        }


        public override void Update(Account account )
        {
            if (account == null)
            {
                throw new ArgumentNullException();
            }

            account.UpdateDate = DateTime.Now;

            base.Update(account);
        }

    }
}
