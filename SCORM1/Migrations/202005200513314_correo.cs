namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CorreoModel",
                c => new
                    {
                        IdMensaje = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(),
                        Categoria = c.String(),
                        Documento = c.String(),
                        Empresa = c.String(),
                        Mensaje = c.String(),
                    })
                .PrimaryKey(t => t.IdMensaje);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CorreoModel");
        }
    }
}
