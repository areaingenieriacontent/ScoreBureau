namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class htmoanswerTrFa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrueOrFalse", "TrFa_Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrueOrFalse", "TrFa_Content");
        }
    }
}
