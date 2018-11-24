namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddErrorCodes1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorCodes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AlartTypeID = c.Guid(nullable: false),
                        AlartCode = c.String(),
                        AlartContext = c.String(),
                        CreateTimet = c.DateTime(nullable: false),
                        UpdateTimet = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AlartTypes", t => t.AlartTypeID, cascadeDelete: true)
                .Index(t => t.AlartTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ErrorCodes", "AlartTypeID", "dbo.AlartTypes");
            DropIndex("dbo.ErrorCodes", new[] { "AlartTypeID" });
            DropTable("dbo.ErrorCodes");
        }
    }
}
