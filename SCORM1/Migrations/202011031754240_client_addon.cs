namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class client_addon : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clasificacion",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Cliente",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        firstName = c.String(),
                        lastName = c.String(),
                        identification = c.String(),
                        enterpriseName = c.String(),
                        idClasificacion = c.Int(nullable: false),
                        userId = c.String(maxLength: 128),
                        idDia = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Clasificacion", t => t.idClasificacion, cascadeDelete: true)
                .ForeignKey("dbo.Dia", t => t.idDia, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.userId)
                .Index(t => t.idClasificacion)
                .Index(t => t.userId)
                .Index(t => t.idDia);
            
            CreateTable(
                "dbo.Dia",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cliente", "userId", "dbo.Users");
            DropForeignKey("dbo.Cliente", "idDia", "dbo.Dia");
            DropForeignKey("dbo.Cliente", "idClasificacion", "dbo.Clasificacion");
            DropIndex("dbo.Cliente", new[] { "idDia" });
            DropIndex("dbo.Cliente", new[] { "userId" });
            DropIndex("dbo.Cliente", new[] { "idClasificacion" });
            DropColumn("dbo.Users", "hasClientProfile");
            DropTable("dbo.Dia");
            DropTable("dbo.Cliente");
            DropTable("dbo.Clasificacion");
        }
    }
}
