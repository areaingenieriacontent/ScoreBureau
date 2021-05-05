namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TopicsCourse", "hasProtectedFailure", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TopicsCourse", "hasProtectedFailure");
        }
    }
}
