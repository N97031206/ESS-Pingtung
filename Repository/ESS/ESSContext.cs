using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Repository.ESS.Domain;

namespace Repository.ESS
{
    public class ESSContext : DbContext
    {
        //權限
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }

        //網頁
        public DbSet<AlartType> AlartTypes { get; set; }
        public DbSet<Orgin> Orgins { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Alart> Alarts { get; set; }
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<Message> Messages { get; set; }

        /// <summary>
        /// EMS資料
        /// </summary>
        public DbSet<ESSObject> ESSObjects { get; set; }

        public DbSet<GridPower> GridPowers { get; set; }

        public DbSet<LoadPower> LoadPowers { get; set; }

        public DbSet<Generator> Generators { get; set; }

        public DbSet<Battery> Batteries { get; set; }

        public DbSet<Inverter> Inverters { get; set; }

    }


}
