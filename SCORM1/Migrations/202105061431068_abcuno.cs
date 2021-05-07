namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abcuno : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Advance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Modulo_Id = c.Int(nullable: false),
                        Score = c.Single(nullable: false),
                        FechaActualizacion = c.DateTime(nullable: false),
                        Usuario_Id = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Cate_Id = c.Int(nullable: false, identity: true),
                        Cate_Name = c.String(),
                        Cate_Desc = c.String(),
                        ToCo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Cate_Id)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_Id, cascadeDelete: true)
                .Index(t => t.ToCo_Id);
            
            CreateTable(
                "dbo.CategoryQuestionBank",
                c => new
                    {
                        Cate_Id = c.Int(nullable: false),
                        Modu_Id = c.Int(nullable: false),
                        EvaluatedQuestionQuantity = c.Int(nullable: false),
                        AprovedCategoryPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Cate_Id, t.Modu_Id })
                .ForeignKey("dbo.Category", t => t.Cate_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProtectedFailureTest", t => t.Modu_Id, cascadeDelete: true)
                .Index(t => t.Cate_Id)
                .Index(t => t.Modu_Id);
            
            CreateTable(
                "dbo.ProtectedFailureTest",
                c => new
                    {
                        Modu_Id = c.Int(nullable: false),
                        PF_Name = c.String(),
                        PF_Description = c.String(),
                        GeneralAproveScore = c.Single(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        testAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Modu_Id)
                .ForeignKey("dbo.Module", t => t.Modu_Id)
                .Index(t => t.Modu_Id);
            
            CreateTable(
                "dbo.FlashQuestion",
                c => new
                    {
                        FlashQuestionId = c.Int(nullable: false, identity: true),
                        FlashTestId = c.Int(nullable: false),
                        Enunciado = c.String(),
                    })
                .PrimaryKey(t => t.FlashQuestionId)
                .ForeignKey("dbo.FlashTest", t => t.FlashTestId, cascadeDelete: true)
                .Index(t => t.FlashTestId);
            
            CreateTable(
                "dbo.FlashQuestionAnswer",
                c => new
                    {
                        FlashQuestionAnswerId = c.Int(nullable: false, identity: true),
                        FlashQuestionId = c.Int(nullable: false),
                        Content = c.String(),
                        CorrectAnswer = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FlashQuestionAnswerId)
                .ForeignKey("dbo.FlashQuestion", t => t.FlashQuestionId, cascadeDelete: true)
                .Index(t => t.FlashQuestionId);
            
            CreateTable(
                "dbo.FlashTest",
                c => new
                    {
                        FlashTestId = c.Int(nullable: false, identity: true),
                        ToCo_Id = c.Int(nullable: false),
                        FlashTestName = c.String(),
                        AprovedPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.FlashTestId)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_Id, cascadeDelete: true)
                .Index(t => t.ToCo_Id);
            
            CreateTable(
                "dbo.MultipleOptionsSurveyAnswer",
                c => new
                    {
                        mosa_id = c.Int(nullable: false, identity: true),
                        mosq_id = c.Int(nullable: false),
                        content = c.String(),
                        correct_answer = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.mosa_id)
                .ForeignKey("dbo.MultipleOptionsSurveyQuestion", t => t.mosq_id, cascadeDelete: true)
                .Index(t => t.mosq_id);
            
            CreateTable(
                "dbo.MultipleOptionsSurveyQuestion",
                c => new
                    {
                        mosq_id = c.Int(nullable: false, identity: true),
                        bank_id = c.Int(nullable: false),
                        title = c.String(),
                        content = c.String(),
                    })
                .PrimaryKey(t => t.mosq_id)
                .ForeignKey("dbo.SurveyQuestionBank", t => t.bank_id, cascadeDelete: true)
                .Index(t => t.bank_id);
            
            CreateTable(
                "dbo.SurveyQuestionBank",
                c => new
                    {
                        bank_id = c.Int(nullable: false, identity: true),
                        survey_id = c.Int(nullable: false),
                        questionsToEvaluate = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.bank_id)
                .ForeignKey("dbo.SurveyModule", t => t.survey_id, cascadeDelete: false)
                .Index(t => t.survey_id);
            
            CreateTable(
                "dbo.SurveyModule",
                c => new
                    {
                        survey_id = c.Int(nullable: false, identity: true),
                        survey_name = c.String(),
                        treshold = c.Single(nullable: false),
                        secondTreshold = c.Single(nullable: false),
                        modu_id = c.Int(nullable: false),
                        survey_description = c.String(),
                        survey_instructions = c.String(),
                        survey_time_minutes = c.Int(nullable: false),
                        date_available = c.DateTime(nullable: false),
                        date_closed = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.survey_id)
                .ForeignKey("dbo.Module", t => t.modu_id, cascadeDelete: false)
                .Index(t => t.modu_id);
            
            CreateTable(
                "dbo.MultipleOptionsSurveyUser",
                c => new
                    {
                        usr_id = c.Int(nullable: false),
                        mosa_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.usr_id, t.mosa_id })
                .ForeignKey("dbo.MultipleOptionsSurveyAnswer", t => t.mosa_id, cascadeDelete: false)
                .ForeignKey("dbo.UserSurveyResponse", t => t.usr_id, cascadeDelete: false)
                .Index(t => t.usr_id)
                .Index(t => t.mosa_id);
            
            CreateTable(
                "dbo.UserSurveyResponse",
                c => new
                    {
                        us_id = c.Int(nullable: false, identity: true),
                        calification = c.Single(nullable: false),
                        enro_id = c.Int(nullable: false),
                        survey_initial_time = c.DateTime(nullable: false),
                        survey_finish_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.us_id)
                .ForeignKey("dbo.Enrollment", t => t.enro_id, cascadeDelete: false)
                .Index(t => t.enro_id);
            
            CreateTable(
                "dbo.ProtectedFailureAnswer",
                c => new
                    {
                        answerId = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        AnswerContent = c.String(),
                        isCorrectQuestion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.answerId)
                .ForeignKey("dbo.ProtectedFailureMultiChoice", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.ProtectedFailureMultiChoice",
                c => new
                    {
                        Category_Id = c.Int(nullable: false),
                        Modu_Id = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.CategoryQuestionBank", t => new { t.Category_Id, t.Modu_Id }, cascadeDelete: true)
                .Index(t => new { t.Category_Id, t.Modu_Id });
            
            CreateTable(
                "dbo.ProtectedFailureMultiChoiceAnswer",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        AnswerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.AnswerId })
                .ForeignKey("dbo.ProtectedFailureAnswer", t => t.AnswerId, cascadeDelete: true)
                .Index(t => t.AnswerId);
            
            CreateTable(
                "dbo.ProtectedFailureResults",
                c => new
                    {
                        Enro_id = c.Int(nullable: false),
                        Cate_Id = c.Int(nullable: false),
                        correctAnswersQuantity = c.Int(nullable: false),
                        Score = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enro_id, t.Cate_Id })
                .ForeignKey("dbo.Category", t => t.Cate_Id, cascadeDelete: true)
                .ForeignKey("dbo.Enrollment", t => t.Enro_id, cascadeDelete: true)
                .Index(t => t.Enro_id)
                .Index(t => t.Cate_Id);
            
            CreateTable(
                "dbo.TrueFalseSurveyQuestion",
                c => new
                    {
                        tfsq_id = c.Int(nullable: false, identity: true),
                        bank_id = c.Int(nullable: false),
                        title = c.String(),
                        content = c.String(),
                    })
                .PrimaryKey(t => t.tfsq_id)
                .ForeignKey("dbo.SurveyQuestionBank", t => t.bank_id, cascadeDelete: true)
                .Index(t => t.bank_id);
            
            CreateTable(
                "dbo.TrueFalseSurveyUser",
                c => new
                    {
                        usr_id = c.Int(nullable: false),
                        tfsq_id = c.Int(nullable: false),
                        user_answer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.usr_id, t.tfsq_id })
                .ForeignKey("dbo.TrueFalseSurveyQuestion", t => t.tfsq_id, cascadeDelete: true)
                .ForeignKey("dbo.UserSurveyResponse", t => t.usr_id, cascadeDelete: true)
                .Index(t => t.usr_id)
                .Index(t => t.tfsq_id);
            
            CreateTable(
                "dbo.UserModuleAdvance",
                c => new
                    {
                        Enro_id = c.Int(nullable: false),
                        ToCo_id = c.Int(nullable: false),
                        Completed = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enro_id, t.ToCo_id })
                .ForeignKey("dbo.Enrollment", t => t.Enro_id, cascadeDelete: true)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_id, cascadeDelete: true)
                .Index(t => t.Enro_id)
                .Index(t => t.ToCo_id);
            
            CreateTable(
                "dbo.VsdrEnrollment",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        vsdr_enro_init_date = c.DateTime(nullable: false),
                        vsdr_enro_finish_date = c.DateTime(nullable: false),
                        qualification = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.user_id, t.vsdr_id })
                .ForeignKey("dbo.Users", t => t.user_id, cascadeDelete: true)
                .ForeignKey("dbo.VsdrSession", t => t.vsdr_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.vsdr_id);
            
            CreateTable(
                "dbo.VsdrSession",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        case_content = c.String(),
                        start_date = c.DateTime(nullable: false),
                        end_date = c.DateTime(nullable: false),
                        resource_url = c.String(),
                        available = c.Boolean(nullable: false),
                        open = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.VsdrTeacherComment",
                c => new
                    {
                        user_id = c.String(maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        comment_id = c.Int(nullable: false, identity: true),
                        teacher_id = c.String(),
                        content = c.String(),
                        commentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.comment_id)
                .ForeignKey("dbo.VsdrEnrollment", t => new { t.user_id, t.vsdr_id })
                .Index(t => new { t.user_id, t.vsdr_id });
            
            CreateTable(
                "dbo.VsdrUserFile",
                c => new
                    {
                        user_id = c.String(maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        vsdr_file_id = c.Int(nullable: false, identity: true),
                        register_name = c.String(),
                        file_description = c.String(),
                        file_name = c.String(),
                        file_extention = c.String(),
                        registered_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.vsdr_file_id)
                .ForeignKey("dbo.VsdrEnrollment", t => new { t.user_id, t.vsdr_id })
                .Index(t => new { t.user_id, t.vsdr_id });
            
            AddColumn("dbo.TopicsCourse", "First_Topic", c => c.Int(nullable: false));
            AddColumn("dbo.TopicsCourse", "ConditionedTopic", c => c.Int(nullable: false));
            AddColumn("dbo.TopicsCourse", "Topic_Available", c => c.Boolean(nullable: false));
            AddColumn("dbo.Module", "hasProtectedFailure", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrEnrollment", "vsdr_id", "dbo.VsdrSession");
            DropForeignKey("dbo.VsdrEnrollment", "user_id", "dbo.Users");
            DropForeignKey("dbo.UserModuleAdvance", "ToCo_id", "dbo.TopicsCourse");
            DropForeignKey("dbo.UserModuleAdvance", "Enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.TrueFalseSurveyUser", "usr_id", "dbo.UserSurveyResponse");
            DropForeignKey("dbo.TrueFalseSurveyUser", "tfsq_id", "dbo.TrueFalseSurveyQuestion");
            DropForeignKey("dbo.TrueFalseSurveyQuestion", "bank_id", "dbo.SurveyQuestionBank");
            DropForeignKey("dbo.ProtectedFailureResults", "Enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.ProtectedFailureResults", "Cate_Id", "dbo.Category");
            DropForeignKey("dbo.ProtectedFailureMultiChoiceAnswer", "AnswerId", "dbo.ProtectedFailureAnswer");
            DropForeignKey("dbo.ProtectedFailureAnswer", "QuestionId", "dbo.ProtectedFailureMultiChoice");
            DropForeignKey("dbo.ProtectedFailureMultiChoice", new[] { "Category_Id", "Modu_Id" }, "dbo.CategoryQuestionBank");
            DropForeignKey("dbo.MultipleOptionsSurveyUser", "usr_id", "dbo.UserSurveyResponse");
            DropForeignKey("dbo.UserSurveyResponse", "enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.MultipleOptionsSurveyUser", "mosa_id", "dbo.MultipleOptionsSurveyAnswer");
            DropForeignKey("dbo.MultipleOptionsSurveyAnswer", "mosq_id", "dbo.MultipleOptionsSurveyQuestion");
            DropForeignKey("dbo.MultipleOptionsSurveyQuestion", "bank_id", "dbo.SurveyQuestionBank");
            DropForeignKey("dbo.SurveyQuestionBank", "survey_id", "dbo.SurveyModule");
            DropForeignKey("dbo.SurveyModule", "modu_id", "dbo.Module");
            DropForeignKey("dbo.FlashQuestion", "FlashTestId", "dbo.FlashTest");
            DropForeignKey("dbo.FlashTest", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.FlashQuestionAnswer", "FlashQuestionId", "dbo.FlashQuestion");
            DropForeignKey("dbo.CategoryQuestionBank", "Modu_Id", "dbo.ProtectedFailureTest");
            DropForeignKey("dbo.ProtectedFailureTest", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.CategoryQuestionBank", "Cate_Id", "dbo.Category");
            DropForeignKey("dbo.Category", "ToCo_Id", "dbo.TopicsCourse");
            DropIndex("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "user_id" });
            DropIndex("dbo.UserModuleAdvance", new[] { "ToCo_id" });
            DropIndex("dbo.UserModuleAdvance", new[] { "Enro_id" });
            DropIndex("dbo.TrueFalseSurveyUser", new[] { "tfsq_id" });
            DropIndex("dbo.TrueFalseSurveyUser", new[] { "usr_id" });
            DropIndex("dbo.TrueFalseSurveyQuestion", new[] { "bank_id" });
            DropIndex("dbo.ProtectedFailureResults", new[] { "Cate_Id" });
            DropIndex("dbo.ProtectedFailureResults", new[] { "Enro_id" });
            DropIndex("dbo.ProtectedFailureMultiChoiceAnswer", new[] { "AnswerId" });
            DropIndex("dbo.ProtectedFailureMultiChoice", new[] { "Category_Id", "Modu_Id" });
            DropIndex("dbo.ProtectedFailureAnswer", new[] { "QuestionId" });
            DropIndex("dbo.UserSurveyResponse", new[] { "enro_id" });
            DropIndex("dbo.MultipleOptionsSurveyUser", new[] { "mosa_id" });
            DropIndex("dbo.MultipleOptionsSurveyUser", new[] { "usr_id" });
            DropIndex("dbo.SurveyModule", new[] { "modu_id" });
            DropIndex("dbo.SurveyQuestionBank", new[] { "survey_id" });
            DropIndex("dbo.MultipleOptionsSurveyQuestion", new[] { "bank_id" });
            DropIndex("dbo.MultipleOptionsSurveyAnswer", new[] { "mosq_id" });
            DropIndex("dbo.FlashTest", new[] { "ToCo_Id" });
            DropIndex("dbo.FlashQuestionAnswer", new[] { "FlashQuestionId" });
            DropIndex("dbo.FlashQuestion", new[] { "FlashTestId" });
            DropIndex("dbo.ProtectedFailureTest", new[] { "Modu_Id" });
            DropIndex("dbo.CategoryQuestionBank", new[] { "Modu_Id" });
            DropIndex("dbo.CategoryQuestionBank", new[] { "Cate_Id" });
            DropIndex("dbo.Category", new[] { "ToCo_Id" });
            DropColumn("dbo.Module", "hasProtectedFailure");
            DropColumn("dbo.TopicsCourse", "Topic_Available");
            DropColumn("dbo.TopicsCourse", "ConditionedTopic");
            DropColumn("dbo.TopicsCourse", "First_Topic");
            DropTable("dbo.VsdrUserFile");
            DropTable("dbo.VsdrTeacherComment");
            DropTable("dbo.VsdrSession");
            DropTable("dbo.VsdrEnrollment");
            DropTable("dbo.UserModuleAdvance");
            DropTable("dbo.TrueFalseSurveyUser");
            DropTable("dbo.TrueFalseSurveyQuestion");
            DropTable("dbo.ProtectedFailureResults");
            DropTable("dbo.ProtectedFailureMultiChoiceAnswer");
            DropTable("dbo.ProtectedFailureMultiChoice");
            DropTable("dbo.ProtectedFailureAnswer");
            DropTable("dbo.UserSurveyResponse");
            DropTable("dbo.MultipleOptionsSurveyUser");
            DropTable("dbo.SurveyModule");
            DropTable("dbo.SurveyQuestionBank");
            DropTable("dbo.MultipleOptionsSurveyQuestion");
            DropTable("dbo.MultipleOptionsSurveyAnswer");
            DropTable("dbo.FlashTest");
            DropTable("dbo.FlashQuestionAnswer");
            DropTable("dbo.FlashQuestion");
            DropTable("dbo.ProtectedFailureTest");
            DropTable("dbo.CategoryQuestionBank");
            DropTable("dbo.Category");
            DropTable("dbo.Advance");
        }
    }
}
