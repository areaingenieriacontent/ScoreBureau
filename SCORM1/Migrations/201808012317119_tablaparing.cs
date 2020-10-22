namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablaparing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnserPairingStudent",
                c => new
                    {
                        AnPa_Id = c.Int(nullable: false, identity: true),
                        AnPa_OptionsQuestion = c.String(),
                        AnPa_OptionAnswer = c.String(),
                        Pair_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnPa_Id)
                .ForeignKey("dbo.Pairing", t => t.Pair_Id, cascadeDelete: true)
                .Index(t => t.Pair_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnserPairingStudent", "Pair_Id", "dbo.Pairing");
            DropIndex("dbo.AnserPairingStudent", new[] { "Pair_Id" });
            DropTable("dbo.AnserPairingStudent");
        }
    }
}
