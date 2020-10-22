namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogsComunidad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogsComunidad",
                c => new
                    {
                        IdUsuario = c.String(nullable: false, maxLength: 128),
                        ContOBL = c.Int(nullable: false),
                        ContSoftKills = c.Int(nullable: false),
                        ContBiblioteca = c.Int(nullable: false),
                        ContPeriodico = c.Int(nullable: false),
                        ContJuegos = c.Int(nullable: false),
                        ContVideoteca = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdUsuario);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogsComunidad");
        }
    }
}
