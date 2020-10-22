namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuienSabeMas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuienSabeMasPuntaje",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.String(),
                        User_Id_QSM = c.String(),
                        Mudole_Id = c.Int(nullable: false),
                        Mudole_Id_QSM = c.Int(nullable: false),
                        FechaPresentacion = c.DateTime(nullable: false),
                        Puntaje = c.Single(nullable: false),
                        PorcentajeAprobacion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Module", "QSMActive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Module", "QSMActive");
            DropTable("dbo.QuienSabeMasPuntaje");
        }
    }
}
