namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinuskWHt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GridPowers", "MinuskWHt", c => c.Single(nullable: false));
            AddColumn("dbo.Inverters", "SPM90ActiveEnergyMinus", c => c.String());
            AddColumn("dbo.LoadPowers", "MinuskWHt", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadPowers", "MinuskWHt");
            DropColumn("dbo.Inverters", "SPM90ActiveEnergyMinus");
            DropColumn("dbo.GridPowers", "MinuskWHt");
        }
    }
}
