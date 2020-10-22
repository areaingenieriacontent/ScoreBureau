namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablaoptionmultipluserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnserPairingStudent", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.TrueOrFalseStudent", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AnswerOptionMultipleStudent", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AnswerOptionMultipleStudent", "User_Id");
            CreateIndex("dbo.AnserPairingStudent", "User_Id");
            CreateIndex("dbo.TrueOrFalseStudent", "User_Id");
            AddForeignKey("dbo.AnswerOptionMultipleStudent", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.AnserPairingStudent", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.TrueOrFalseStudent", "User_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrueOrFalseStudent", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AnserPairingStudent", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AnswerOptionMultipleStudent", "User_Id", "dbo.Users");
            DropIndex("dbo.TrueOrFalseStudent", new[] { "User_Id" });
            DropIndex("dbo.AnserPairingStudent", new[] { "User_Id" });
            DropIndex("dbo.AnswerOptionMultipleStudent", new[] { "User_Id" });
            DropColumn("dbo.AnswerOptionMultipleStudent", "User_Id");
            DropColumn("dbo.TrueOrFalseStudent", "User_Id");
            DropColumn("dbo.AnserPairingStudent", "User_Id");
        }
    }
}
