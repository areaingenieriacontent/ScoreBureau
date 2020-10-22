namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testyg : DbMigration
    {
        public override void Up()
        {
                      CreateTable(
                "dbo.Measure",
                c => new
                    {
                        MeasureId = c.Int(nullable: false, identity: true),
                        MeasureInitDate = c.DateTime(nullable: false),
                        MeasureFinishDate = c.DateTime(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        TestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MeasureId)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Test", t => t.TestId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.TestId);
            
            CreateTable(
                "dbo.MeasureUser",
                c => new
                    {
                        MausureUserID = c.Int(nullable: false, identity: true),
                        MausureId = c.Int(nullable: false),
                        UsersId = c.String(maxLength: 128),
                        UserEvaluate = c.String(),
                    })
                .PrimaryKey(t => t.MausureUserID)
                .ForeignKey("dbo.Users", t => t.UsersId)
                .ForeignKey("dbo.Measure", t => t.MausureId, cascadeDelete: true)
                .Index(t => t.MausureId)
                .Index(t => t.UsersId);
            
             
            CreateTable(
                "dbo.Result",
                c => new
                    {
                        ResultId = c.Int(nullable: false, identity: true),
                        ResultDate = c.DateTime(nullable: false),
                        ResultTotalScore = c.Int(nullable: false),
                        MeasureId = c.Int(nullable: false),
                        QualifiedUser_Id = c.String(maxLength: 128),
                        ResultOwner_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ResultId)
                .ForeignKey("dbo.Measure", t => t.MeasureId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.QualifiedUser_Id)
                .ForeignKey("dbo.Users", t => t.ResultOwner_Id)
                .ForeignKey("dbo.Users", t => t.ApplicationUser_Id)
                .Index(t => t.MeasureId)
                .Index(t => t.QualifiedUser_Id)
                .Index(t => t.ResultOwner_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Score",
                c => new
                    {
                        ScoreId = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        ProficiencyId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoreId)
                .ForeignKey("dbo.Proficiency", t => t.ProficiencyId, cascadeDelete: true)
                .ForeignKey("dbo.Result", t => t.ResultId, cascadeDelete: true)
                .Index(t => t.ProficiencyId)
                .Index(t => t.ResultId);
            
            
            CreateTable(
                "dbo.Plan",
                c => new
                    {
                        PlanId = c.Int(nullable: false, identity: true),
                        PlanDescription = c.String(),
                        PlanMinScore = c.Int(nullable: false),
                        PlanMaxScore = c.Int(nullable: false),
                        PlanTo = c.Int(nullable: false),
                        ResourceId = c.Int(nullable: false),
                        ProficiencyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlanId)
                .ForeignKey("dbo.Proficiency", t => t.ProficiencyId, cascadeDelete: true)
                .Index(t => t.ProficiencyId);
             
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.LogsUserInPlans", "planid_PlanId", "dbo.Plan");
            DropForeignKey("dbo.Resource", "PlanId", "dbo.Plan");
            DropForeignKey("dbo.Plan", "ProficiencyId", "dbo.Proficiency");
            DropForeignKey("dbo.BlockService", "TySe_Id", "dbo.TypeServiceBlock");
            DropForeignKey("dbo.BlockService", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AdvanceCourse", "Enro_Id", "dbo.Enrollment");
            DropForeignKey("dbo.AdvanceCourse", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AdvanceCourse", "AdUs_Id", "dbo.AdvanceUser");
            DropForeignKey("dbo.AdvanceUser", "ToCo_id", "dbo.TopicsCourse");
            DropForeignKey("dbo.AdvanceUser", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserSuperiors", "ApplicationUser_Id1", "dbo.Users");
            DropForeignKey("dbo.UserSuperiors", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.Result", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.Score", "ResultId", "dbo.Result");
            DropForeignKey("dbo.Score", "ProficiencyId", "dbo.Proficiency");
            DropForeignKey("dbo.Result", "ResultOwner_Id", "dbo.Users");
            DropForeignKey("dbo.Result", "QualifiedUser_Id", "dbo.Users");
            DropForeignKey("dbo.Result", "MeasureId", "dbo.Measure");
            DropForeignKey("dbo.Point", "TyPo_Id", "dbo.TypePoint");
            DropForeignKey("dbo.Exchange", "Point_Poin_Id", "dbo.Point");
            DropForeignKey("dbo.Point", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MyOfficeUser", "ApplicationUser_Id1", "dbo.Users");
            DropForeignKey("dbo.MyOfficeUser", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.MG_BlockGameUser", "User_Id", "dbo.Users");
            DropForeignKey("dbo.LockGame", "TyBa_Id", "dbo.TypeBaneo");
            DropForeignKey("dbo.LockGame", "Game_Id", "dbo.Game");
            DropForeignKey("dbo.LockGame", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "LocationId", "dbo.Location");
            DropForeignKey("dbo.UserEquals", "ApplicationUser_Id1", "dbo.Users");
            DropForeignKey("dbo.UserEquals", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserClients", "ApplicationUser_Id1", "dbo.Users");
            DropForeignKey("dbo.UserClients", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "CityId", "dbo.City");
            DropForeignKey("dbo.Users", "AreaId", "dbo.Area");
            DropForeignKey("dbo.AnswersForum", "ReFo_Id", "dbo.ResourceForum");
            DropForeignKey("dbo.ResourceForum", "Job_Id", "dbo.Job");
            DropForeignKey("dbo.BookRatings", "ReJo_Id", "dbo.ResourceJobs");
            DropForeignKey("dbo.ResourceJobs", "Job_Id", "dbo.Job");
            DropForeignKey("dbo.Job", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.ResourceTopics", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.TopicsCourse", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.Link", "ToCo_id", "dbo.TopicsCourse");
            DropForeignKey("dbo.TrueOrFalse", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.BankQuestion", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.Pairing", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.AnswerPairing", "Pair_Id", "dbo.Pairing");
            DropForeignKey("dbo.OptionMultiple", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.AnswerOptionMultiple", "OpMu_Id", "dbo.OptionMultiple");
            DropForeignKey("dbo.OpenQuestion", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.NewAttempts", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.NewAttempts", "User_Id", "dbo.Users");
            DropForeignKey("dbo.BankQuestion", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.ResourceTopic", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.ResourceTopic", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Position", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Users", "PositionId", "dbo.Position");
            DropForeignKey("dbo.Prize", "PointManagerCategory_PoMaCa_Id", "dbo.PointManagerCategory");
            DropForeignKey("dbo.PointManagerCategory", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.MG_SettingMp", "Plan_Id", "dbo.MG_Template");
            DropForeignKey("dbo.MG_Template", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.MG_Point", "Sett_Id", "dbo.MG_SettingMp");
            DropForeignKey("dbo.MG_Point", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MG_Pairing", "Sett_Id", "dbo.MG_SettingMp");
            DropForeignKey("dbo.MG_AnswerPairing", "Pairi_Id", "dbo.MG_Pairing");
            DropForeignKey("dbo.MG_Order", "Sett_Id", "dbo.MG_SettingMp");
            DropForeignKey("dbo.MG_AnswerOrder", "Order_Id", "dbo.MG_Order");
            DropForeignKey("dbo.MG_MultipleChoice", "Sett_Id", "dbo.MG_SettingMp");
            DropForeignKey("dbo.MG_AnswerMultipleChoice", "MuCh_ID", "dbo.MG_MultipleChoice");
            DropForeignKey("dbo.MG_AnswerUser", "AnMul_ID", "dbo.MG_AnswerMultipleChoice");
            DropForeignKey("dbo.MG_AnswerUser", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MG_SettingMp", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.Measure", "TestId", "dbo.Test");
            DropForeignKey("dbo.QuestionTest", "TestId", "dbo.Test");
            DropForeignKey("dbo.QuestionTest", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.Question", "ProficiencyId", "dbo.Proficiency");
            DropForeignKey("dbo.MeasureUser", "MausureId", "dbo.Measure");
            DropForeignKey("dbo.MeasureUser", "UsersId", "dbo.Users");
            DropForeignKey("dbo.Measure", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Log", "TaCh_Id", "dbo.TableChange");
            DropForeignKey("dbo.Log", "IdCh_Id", "dbo.IdChange");
            DropForeignKey("dbo.Log", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.Log", "CoLo_Id", "dbo.CodeLogs");
            DropForeignKey("dbo.Log", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Location", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.ImageUpload", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Enrollment", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.Improvement", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.Improvement", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Module", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Certification", "Module_Modu_Id", "dbo.Module");
            DropForeignKey("dbo.ResourceBetterPractice", "BePr_Id", "dbo.BetterPractice");
            DropForeignKey("dbo.BetterPractice", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.BetterPractice", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Module", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AdvanceCourse", "Module_Modu_Id", "dbo.Module");
            DropForeignKey("dbo.Desertify", "Enro_Id", "dbo.Enrollment");
            DropForeignKey("dbo.Desertify", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.Desertify", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Enrollment", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Certification", "Enro_Id", "dbo.Enrollment");
            DropForeignKey("dbo.Certification", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Enrollment", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Section", "Edit_Id", "dbo.Edition");
            DropForeignKey("dbo.Article", "sect_Id", "dbo.Section");
            DropForeignKey("dbo.ResourceNw", "Arti_Id", "dbo.Article");
            DropForeignKey("dbo.PointsComment", "Comm_Id", "dbo.Comments");
            DropForeignKey("dbo.PointsComment", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Comments", "comm_Author_Id", "dbo.Users");
            DropForeignKey("dbo.Comments", "Arti_Id", "dbo.Article");
            DropForeignKey("dbo.Edition", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Changeinterface", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Exchange", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Exchange", "Priz_Id", "dbo.Prize");
            DropForeignKey("dbo.Prize", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Prize", "Capr_Id", "dbo.CategoryPrize");
            DropForeignKey("dbo.CategoryPrize", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.CategoryModule", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Area", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Users", "Area_AreaId", "dbo.Area");
            DropForeignKey("dbo.Area", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Attempts", "BaQu_Id", "dbo.BankQuestion");
            DropForeignKey("dbo.Attempts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Job", "User_Id", "dbo.Users");
            DropForeignKey("dbo.ResourceJobs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.BookRatings", "ReFo_Id", "dbo.ResourceForum");
            DropForeignKey("dbo.ResourceForum", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AnswersForum", "User_Id", "dbo.Users");
            DropIndex("dbo.UserSuperiors", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.UserSuperiors", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.MyOfficeUser", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.MyOfficeUser", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserEquals", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.UserEquals", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserClients", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.UserClients", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.QuestionTest", new[] { "TestId" });
            DropIndex("dbo.QuestionTest", new[] { "QuestionId" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.Resource", new[] { "PlanId" });
            DropIndex("dbo.Plan", new[] { "ProficiencyId" });
            DropIndex("dbo.LogsUserInPlans", new[] { "planid_PlanId" });
            DropIndex("dbo.BlockService", new[] { "TySe_Id" });
            DropIndex("dbo.BlockService", new[] { "User_Id" });
            DropIndex("dbo.UserRoles", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.Score", new[] { "ResultId" });
            DropIndex("dbo.Score", new[] { "ProficiencyId" });
            DropIndex("dbo.Result", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Result", new[] { "ResultOwner_Id" });
            DropIndex("dbo.Result", new[] { "QualifiedUser_Id" });
            DropIndex("dbo.Result", new[] { "MeasureId" });
            DropIndex("dbo.Point", new[] { "User_Id" });
            DropIndex("dbo.Point", new[] { "TyPo_Id" });
            DropIndex("dbo.MG_BlockGameUser", new[] { "User_Id" });
            DropIndex("dbo.UserLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.LockGame", new[] { "TyBa_Id" });
            DropIndex("dbo.LockGame", new[] { "Game_Id" });
            DropIndex("dbo.LockGame", new[] { "UserId" });
            DropIndex("dbo.UserClaims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.ResourceTopics", new[] { "ToCo_Id" });
            DropIndex("dbo.Link", new[] { "ToCo_id" });
            DropIndex("dbo.TrueOrFalse", new[] { "BaQu_Id" });
            DropIndex("dbo.AnswerPairing", new[] { "Pair_Id" });
            DropIndex("dbo.Pairing", new[] { "BaQu_Id" });
            DropIndex("dbo.AnswerOptionMultiple", new[] { "OpMu_Id" });
            DropIndex("dbo.OptionMultiple", new[] { "BaQu_Id" });
            DropIndex("dbo.OpenQuestion", new[] { "BaQu_Id" });
            DropIndex("dbo.NewAttempts", new[] { "User_Id" });
            DropIndex("dbo.NewAttempts", new[] { "BaQu_Id" });
            DropIndex("dbo.ResourceTopic", new[] { "ToCo_Id" });
            DropIndex("dbo.ResourceTopic", new[] { "CompanyId" });
            DropIndex("dbo.Position", new[] { "CompanyId" });
            DropIndex("dbo.PointManagerCategory", new[] { "CompanyId" });
            DropIndex("dbo.MG_Template", new[] { "Company_Id" });
            DropIndex("dbo.MG_Point", new[] { "User_Id" });
            DropIndex("dbo.MG_Point", new[] { "Sett_Id" });
            DropIndex("dbo.MG_AnswerPairing", new[] { "Pairi_Id" });
            DropIndex("dbo.MG_Pairing", new[] { "Sett_Id" });
            DropIndex("dbo.MG_AnswerOrder", new[] { "Order_Id" });
            DropIndex("dbo.MG_Order", new[] { "Sett_Id" });
            DropIndex("dbo.MG_AnswerUser", new[] { "AnMul_ID" });
            DropIndex("dbo.MG_AnswerUser", new[] { "User_Id" });
            DropIndex("dbo.MG_AnswerMultipleChoice", new[] { "MuCh_ID" });
            DropIndex("dbo.MG_MultipleChoice", new[] { "Sett_Id" });
            DropIndex("dbo.MG_SettingMp", new[] { "Company_Id" });
            DropIndex("dbo.MG_SettingMp", new[] { "Plan_Id" });
            DropIndex("dbo.Question", new[] { "ProficiencyId" });
            DropIndex("dbo.MeasureUser", new[] { "UsersId" });
            DropIndex("dbo.MeasureUser", new[] { "MausureId" });
            DropIndex("dbo.Measure", new[] { "TestId" });
            DropIndex("dbo.Measure", new[] { "CompanyId" });
            DropIndex("dbo.Log", new[] { "Company_Id" });
            DropIndex("dbo.Log", new[] { "IdCh_Id" });
            DropIndex("dbo.Log", new[] { "TaCh_Id" });
            DropIndex("dbo.Log", new[] { "CoLo_Id" });
            DropIndex("dbo.Log", new[] { "User_Id" });
            DropIndex("dbo.Location", new[] { "CompanyId" });
            DropIndex("dbo.ImageUpload", new[] { "CompanyId" });
            DropIndex("dbo.Improvement", new[] { "User_Id" });
            DropIndex("dbo.Improvement", new[] { "Modu_Id" });
            DropIndex("dbo.ResourceBetterPractice", new[] { "BePr_Id" });
            DropIndex("dbo.BetterPractice", new[] { "User_Id" });
            DropIndex("dbo.BetterPractice", new[] { "Modu_Id" });
            DropIndex("dbo.Module", new[] { "CompanyId" });
            DropIndex("dbo.Module", new[] { "User_Id" });
            DropIndex("dbo.Desertify", new[] { "Enro_Id" });
            DropIndex("dbo.Desertify", new[] { "User_Id" });
            DropIndex("dbo.Desertify", new[] { "BaQu_Id" });
            DropIndex("dbo.Certification", new[] { "Module_Modu_Id" });
            DropIndex("dbo.Certification", new[] { "User_Id" });
            DropIndex("dbo.Certification", new[] { "Enro_Id" });
            DropIndex("dbo.Enrollment", new[] { "CompanyId" });
            DropIndex("dbo.Enrollment", new[] { "User_Id" });
            DropIndex("dbo.Enrollment", new[] { "Modu_Id" });
            DropIndex("dbo.ResourceNw", new[] { "Arti_Id" });
            DropIndex("dbo.PointsComment", new[] { "User_Id" });
            DropIndex("dbo.PointsComment", new[] { "Comm_Id" });
            DropIndex("dbo.Comments", new[] { "comm_Author_Id" });
            DropIndex("dbo.Comments", new[] { "Arti_Id" });
            DropIndex("dbo.Article", new[] { "sect_Id" });
            DropIndex("dbo.Section", new[] { "Edit_Id" });
            DropIndex("dbo.Edition", new[] { "CompanyId" });
            DropIndex("dbo.Changeinterface", new[] { "CompanyId" });
            DropIndex("dbo.Exchange", new[] { "Point_Poin_Id" });
            DropIndex("dbo.Exchange", new[] { "User_Id" });
            DropIndex("dbo.Exchange", new[] { "Priz_Id" });
            DropIndex("dbo.Prize", new[] { "PointManagerCategory_PoMaCa_Id" });
            DropIndex("dbo.Prize", new[] { "CompanyId" });
            DropIndex("dbo.Prize", new[] { "Capr_Id" });
            DropIndex("dbo.CategoryPrize", new[] { "CompanyId" });
            DropIndex("dbo.CategoryModule", new[] { "CompanyId" });
            DropIndex("dbo.Area", new[] { "CompanyId" });
            DropIndex("dbo.Area", new[] { "UserId" });
            DropIndex("dbo.Company", new[] { "CompanyNit" });
            DropIndex("dbo.Attempts", new[] { "UserId" });
            DropIndex("dbo.Attempts", new[] { "BaQu_Id" });
            DropIndex("dbo.BankQuestion", new[] { "CompanyId" });
            DropIndex("dbo.BankQuestion", new[] { "ToCo_Id" });
            DropIndex("dbo.TopicsCourse", new[] { "Modu_Id" });
            DropIndex("dbo.Job", new[] { "User_Id" });
            DropIndex("dbo.Job", new[] { "ToCo_Id" });
            DropIndex("dbo.ResourceJobs", new[] { "User_Id" });
            DropIndex("dbo.ResourceJobs", new[] { "Job_Id" });
            DropIndex("dbo.BookRatings", new[] { "ReJo_Id" });
            DropIndex("dbo.BookRatings", new[] { "ReFo_Id" });
            DropIndex("dbo.ResourceForum", new[] { "User_Id" });
            DropIndex("dbo.ResourceForum", new[] { "Job_Id" });
            DropIndex("dbo.AnswersForum", new[] { "ReFo_Id" });
            DropIndex("dbo.AnswersForum", new[] { "User_Id" });
            DropIndex("dbo.Users", new[] { "Area_AreaId" });
            DropIndex("dbo.Users", new[] { "LocationId" });
            DropIndex("dbo.Users", new[] { "CityId" });
            DropIndex("dbo.Users", new[] { "AreaId" });
            DropIndex("dbo.Users", new[] { "PositionId" });
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.AdvanceUser", new[] { "ToCo_id" });
            DropIndex("dbo.AdvanceUser", new[] { "User_Id" });
            DropIndex("dbo.AdvanceCourse", new[] { "Module_Modu_Id" });
            DropIndex("dbo.AdvanceCourse", new[] { "AdUs_Id" });
            DropIndex("dbo.AdvanceCourse", new[] { "User_Id" });
            DropIndex("dbo.AdvanceCourse", new[] { "Enro_Id" });
            DropTable("dbo.UserSuperiors");
            DropTable("dbo.MyOfficeUser");
            DropTable("dbo.UserEquals");
            DropTable("dbo.UserClients");
            DropTable("dbo.QuestionTest");
            DropTable("dbo.StylesLogos");
            DropTable("dbo.Roles");
            DropTable("dbo.PointsObtainedForUser");
            DropTable("dbo.Resource");
            DropTable("dbo.Plan");
            DropTable("dbo.LogsUserInPlans");
            DropTable("dbo.TypeServiceBlock");
            DropTable("dbo.BlockService");
            DropTable("dbo.Banner");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Score");
            DropTable("dbo.Result");
            DropTable("dbo.TypePoint");
            DropTable("dbo.Point");
            DropTable("dbo.MG_BlockGameUser");
            DropTable("dbo.UserLogins");
            DropTable("dbo.TypeBaneo");
            DropTable("dbo.Game");
            DropTable("dbo.LockGame");
            DropTable("dbo.UserClaims");
            DropTable("dbo.City");
            DropTable("dbo.ResourceTopics");
            DropTable("dbo.Link");
            DropTable("dbo.TrueOrFalse");
            DropTable("dbo.AnswerPairing");
            DropTable("dbo.Pairing");
            DropTable("dbo.AnswerOptionMultiple");
            DropTable("dbo.OptionMultiple");
            DropTable("dbo.OpenQuestion");
            DropTable("dbo.NewAttempts");
            DropTable("dbo.ResourceTopic");
            DropTable("dbo.Position");
            DropTable("dbo.PointManagerCategory");
            DropTable("dbo.MG_Template");
            DropTable("dbo.MG_Point");
            DropTable("dbo.MG_AnswerPairing");
            DropTable("dbo.MG_Pairing");
            DropTable("dbo.MG_AnswerOrder");
            DropTable("dbo.MG_Order");
            DropTable("dbo.MG_AnswerUser");
            DropTable("dbo.MG_AnswerMultipleChoice");
            DropTable("dbo.MG_MultipleChoice");
            DropTable("dbo.MG_SettingMp");
            DropTable("dbo.Proficiency");
            DropTable("dbo.Question");
            DropTable("dbo.Test");
            DropTable("dbo.MeasureUser");
            DropTable("dbo.Measure");
            DropTable("dbo.TableChange");
            DropTable("dbo.IdChange");
            DropTable("dbo.CodeLogs");
            DropTable("dbo.Log");
            DropTable("dbo.Location");
            DropTable("dbo.ImageUpload");
            DropTable("dbo.Improvement");
            DropTable("dbo.ResourceBetterPractice");
            DropTable("dbo.BetterPractice");
            DropTable("dbo.Module");
            DropTable("dbo.Desertify");
            DropTable("dbo.Certification");
            DropTable("dbo.Enrollment");
            DropTable("dbo.ResourceNw");
            DropTable("dbo.PointsComment");
            DropTable("dbo.Comments");
            DropTable("dbo.Article");
            DropTable("dbo.Section");
            DropTable("dbo.Edition");
            DropTable("dbo.Changeinterface");
            DropTable("dbo.Exchange");
            DropTable("dbo.Prize");
            DropTable("dbo.CategoryPrize");
            DropTable("dbo.CategoryModule");
            DropTable("dbo.Area");
            DropTable("dbo.Company");
            DropTable("dbo.Attempts");
            DropTable("dbo.BankQuestion");
            DropTable("dbo.TopicsCourse");
            DropTable("dbo.Job");
            DropTable("dbo.ResourceJobs");
            DropTable("dbo.BookRatings");
            DropTable("dbo.ResourceForum");
            DropTable("dbo.AnswersForum");
            DropTable("dbo.Users");
            DropTable("dbo.AdvanceUser");
            DropTable("dbo.AdvanceCourse");
        }
    }
}
