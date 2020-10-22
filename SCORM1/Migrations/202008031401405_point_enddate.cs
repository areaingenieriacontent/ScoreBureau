namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class point_enddate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Point", "Poin_End_Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Point", "Poin_End_Date");
        }
    }
}
