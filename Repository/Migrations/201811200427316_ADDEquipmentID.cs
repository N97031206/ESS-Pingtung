namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADDEquipmentID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alarts", "EquipmentID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Alarts", "EquipmentID");
        }
    }
}
