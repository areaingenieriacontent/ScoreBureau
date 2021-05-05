namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Module", "hasProtectedFailure", c => c.Int(nullable: false));
            DropColumn("dbo.TopicsCourse", "hasProtectedFailure");
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Module", "hasProtectedFailure");
        }
    }
}
