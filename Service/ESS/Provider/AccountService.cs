using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Domain = Repository.ESS.Domain;
using Repository.ESS.Provider;
using Service.ESS.Mapper;
using Support.Authorize;

namespace Service.ESS.Provider
{
    public class AccountService
    {
        private MapperConfiguration mapperConfiguration =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RoleMapper>();
                cfg.AddProfile<AccountMapper>();
            });

        private IMapper mapper = null;

        private RoleService roleService = new RoleService();
        private AccountRepository accountRepository = new AccountRepository();

        public AccountService()
        {
            this.mapper = mapperConfiguration.CreateMapper();
        }

        public List<Model.Account> ReadAll()
        {
            List<Domain.Account> domainstations = accountRepository.ReadAll().ToList();
            return this.mapper.Map<List<Model.Account>>(domainstations);
        }



        public List<Model.Account> ReadByType(string Type,string user)
        {
            List<Domain.Account> domainstations = new List<Domain.Account>();
            
            if (RoleType.系統管理員.ToString().Equals(Type))
            {
                domainstations = accountRepository.ReadAll().ToList();
            }
            else if (RoleType.一般使用者.ToString().Equals(Type))
            {
                domainstations = accountRepository.ReadAll().
                    Where(x => x.UserName== user).
                    ToList();

                //domainstations = accountRepository.ReadAll().
                //    Where(x => x.Role.Type == (int)RoleType.一般使用者 ||  x.Role.Type == (int)RoleType.參觀帳號).
                //    ToList();
            }
            else
            {
                domainstations = accountRepository.ReadAll().
                    Where(x => x.Role.Type == (int)RoleType.參觀帳號).
                    ToList();
            }

            return this.mapper.Map<List<Model.Account>>(domainstations);
        }


            public Model.Account ReadBy(string username, string password)
        {
            Domain.Account account = accountRepository.ReadBy(x => x.UserName.ToUpper() == username.ToUpper() && x.Password.ToUpper() == password.ToUpper());
            return this.mapper.Map<Model.Account>(account);
        }

        public Model.Account ReadByID(Guid ID)
        {
            Domain.Account account = accountRepository.ReadBy(x => x.Id == ID);
            return this.mapper.Map<Model.Account>(account);
        }


        public Model.Account ReadByName(string username)
        {
            Domain.Account accountName = accountRepository.ReadBy(x => x.UserName.ToUpper() == username.ToUpper());
            return this.mapper.Map<Model.Account>(accountName);
        }

        public Model.Account ReadByPassword(string password)
        {
            Domain.Account accountPassword = accountRepository.ReadBy(x => x.Password.ToUpper() == password.ToUpper());
            return this.mapper.Map<Model.Account>(accountPassword);
        }

        public List<Domain.Account> ReadByMail(string Mail)
        {
            List<Domain.Account> domainstations = accountRepository.ReadAll().Where(x => x.Email == Mail).ToList();
            return this.mapper.Map<List<Domain.Account>>(domainstations);
        }

        public Guid Create(Model.Account account)
        {
            account.UserName = account.UserName.Trim();
            account.Password = account.Password.Trim();
            account.Email = account.Email.Trim();
            account.Tel = account.Tel.Trim();

            Domain.Account domainAccount = this.mapper.Map<Domain.Account>(account);

            accountRepository.Create(domainAccount);
            accountRepository.SaveChanges();

            return domainAccount.Id;
        }


        public Guid Update(Model.Account account)
        {
            Domain.Account domainAccount = accountRepository.ReadBy(x => x.Id == account.Id);

            if ( !string.IsNullOrEmpty(account.UserName))
            {
                domainAccount.UserName = account.UserName.Trim();
                domainAccount.Password = account.Password.Trim();
                domainAccount.Email = account.Email.Trim();
                domainAccount.Tel = account.Tel.Trim();
            }
            else
            {
                domainAccount.Disabled = account.Disabled;
            }

            accountRepository.Update(domainAccount);
            accountRepository.SaveChanges();

            return domainAccount.Id;
        }

       

    }
}
