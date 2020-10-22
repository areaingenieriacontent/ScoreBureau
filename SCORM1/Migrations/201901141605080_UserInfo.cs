namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInfo",
                c => new
                    {
                        Nombreprueba = c.Int(nullable: false, identity: true),
                        Jajaja = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Nombreprueba)
                .ForeignKey("dbo.Users", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfo", "ApplicationUser_Id", "dbo.Users");
            DropIndex("dbo.UserInfo", new[] { "ApplicationUser_Id" });
            DropTable("dbo.UserInfo");
        }
    }
}
