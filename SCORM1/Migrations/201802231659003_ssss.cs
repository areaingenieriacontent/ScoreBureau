namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ssss : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TopicsCourse", "ToCo_Visible", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TopicsCourse", "ToCo_Visible");
        }
    }
}
