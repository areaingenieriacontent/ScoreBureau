namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fechatest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnswerOptionMultipleStudent", "Date_Present_test", c => c.DateTime(nullable: false));
            AddColumn("dbo.AnserPairingStudent", "Date_Present_test", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrueOrFalseStudent", "Date_Present_test", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrueOrFalseStudent", "Date_Present_test");
            DropColumn("dbo.AnserPairingStudent", "Date_Present_test");
            DropColumn("dbo.AnswerOptionMultipleStudent", "Date_Present_test");
        }
    }
}
