namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tertyk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollment", "Enro_RoleEnrollment", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollment", "Enro_RoleEnrollment");
        }
    }
}
