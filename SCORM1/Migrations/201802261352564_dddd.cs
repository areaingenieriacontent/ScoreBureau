namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dddd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Job", "Job_Visible", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Job", "Job_Visible");
        }
    }
}
