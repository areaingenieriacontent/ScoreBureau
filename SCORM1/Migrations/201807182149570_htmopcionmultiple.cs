namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class htmopcionmultiple : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptionMultiple", "OpMult_Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptionMultiple", "OpMult_Content");
        }
    }
}
