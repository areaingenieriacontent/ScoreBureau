namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agregardescripcionpregunta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankQuestion", "BaQu_Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankQuestion", "BaQu_Description");
        }
    }
}
