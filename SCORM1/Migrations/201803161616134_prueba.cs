namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prueba : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Foto_perfil", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Foto_perfil");
        }
    }
}
