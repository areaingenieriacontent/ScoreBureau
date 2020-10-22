namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablaoptionmultiplestudent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerOptionMultipleStudent",
                c => new
                    {
                        AnOp_Id = c.Int(nullable: false, identity: true),
                        AnOp_OptionAnswer = c.String(),
                        AnOp_TrueAnswer = c.Int(nullable: false),
                        Answer_OpMult_Content = c.String(),
                        OpMu_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnOp_Id)
                .ForeignKey("dbo.OptionMultiple", t => t.OpMu_Id, cascadeDelete: true)
                .Index(t => t.OpMu_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnswerOptionMultipleStudent", "OpMu_Id", "dbo.OptionMultiple");
            DropIndex("dbo.AnswerOptionMultipleStudent", new[] { "OpMu_Id" });
            DropTable("dbo.AnswerOptionMultipleStudent");
        }
    }
}
