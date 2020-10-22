namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removevirtualadvacecourse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", "dbo.AdvanceLoseUser");
            DropIndex("dbo.AdvanceCourse", new[] { "AdvanceLoseUser_AdLoUs_Id" });
            DropColumn("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", c => c.Int());
            CreateIndex("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id");
            AddForeignKey("dbo.AdvanceCourse", "AdvanceLoseUser_AdLoUs_Id", "dbo.AdvanceLoseUser", "AdLoUs_Id");
        }
    }
}
