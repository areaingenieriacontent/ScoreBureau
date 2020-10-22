namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateAdvanceUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdvanceUser", "AdUs_PresentDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdvanceUser", "AdUs_PresentDate");
        }
    }
}
