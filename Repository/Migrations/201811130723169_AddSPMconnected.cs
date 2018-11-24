namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSPMconnected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inverters", "SPMconnected", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inverters", "SPMconnected");
        }
    }
}
