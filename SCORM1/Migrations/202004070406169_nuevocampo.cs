namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevocampo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ComunidadActiva", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ComunidadActiva");
        }
    }
}
