namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class htmoanswerpcionmultiple : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnswerOptionMultiple", "Answer_OpMult_Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AnswerOptionMultiple", "Answer_OpMult_Content");
        }
    }
}
