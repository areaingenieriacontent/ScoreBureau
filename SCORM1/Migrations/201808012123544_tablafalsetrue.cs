namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablafalsetrue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrueOrFalseStudent",
                c => new
                    {
                        TrFa_Id = c.Int(nullable: false, identity: true),
                        TrFa_Question = c.String(),
                        TrFa_Description = c.String(),
                        TrFa_Content = c.String(),
                        TrFa_FalseAnswer = c.String(),
                        TrFa_TrueAnswer = c.String(),
                        TrFa_Score = c.String(),
                        TrFa_State = c.Int(nullable: false),
                        BaQu_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrFa_Id)
                .ForeignKey("dbo.BankQuestion", t => t.BaQu_Id, cascadeDelete: true)
                .Index(t => t.BaQu_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrueOrFalseStudent", "BaQu_Id", "dbo.BankQuestion");
            DropIndex("dbo.TrueOrFalseStudent", new[] { "BaQu_Id" });
            DropTable("dbo.TrueOrFalseStudent");
        }
    }
}
