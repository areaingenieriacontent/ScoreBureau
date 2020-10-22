namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelAdvanceLoseUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvanceLoseUser",
                c => new
                    {
                        AdLoUs_Id = c.Int(nullable: false, identity: true),
                        AdLoUs_ScoreObtained = c.Double(nullable: false),
                        AdLoUs_PresentDate = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                        ToCo_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdLoUs_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.ToCo_id);
            
            AddColumn("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", c => c.Int());
            CreateIndex("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id");
            AddForeignKey("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", "dbo.AdvanceLoseUser", "AdLoUs_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvanceLoseUser", "ToCo_id", "dbo.TopicsCourse");
            DropForeignKey("dbo.AdvanceLoseUser", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", "dbo.AdvanceLoseUser");
            DropIndex("dbo.AdvanceLoseUser", new[] { "ToCo_id" });
            DropIndex("dbo.AdvanceLoseUser", new[] { "User_Id" });
            DropIndex("dbo.AdvanceCourse", new[] { "AdvanceLoseUser_AdLoUs_Id" });
            DropColumn("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id");
            DropTable("dbo.AdvanceLoseUser");
        }
    }
}
