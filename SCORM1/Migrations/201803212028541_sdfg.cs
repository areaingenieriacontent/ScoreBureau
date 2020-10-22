namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sdfg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookRatings", "BoRa_Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookRatings", "BoRa_Description");
        }
    }
}
