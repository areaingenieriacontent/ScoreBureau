namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactoCorreos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CorreoModel", "Correos", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CorreoModel", "Correos");
        }
    }
}
