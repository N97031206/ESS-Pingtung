namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSPM90ActiveEnergyMinus1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inverters", "SPM90ActiveEnergyMinus1", c => c.Single(nullable: false));
            AddColumn("dbo.Inverters", "SPM90ActiveEnergyMinus2", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inverters", "SPM90ActiveEnergyMinus2");
            DropColumn("dbo.Inverters", "SPM90ActiveEnergyMinus1");
        }
    }
}
