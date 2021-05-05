namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abc : DbMigration
    {
        public override void Up()
        {
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
           })
           .PrimaryKey(t => t.id);

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
            AddColumn("dbo.VsdrSession", "open", c => c.Boolean(nullable: false));

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
                .ForeignKey("dbo.MultipleOptionsSurveyQuestion", t => t.mosq_id, cascadeDelete: false)
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
                .ForeignKey("dbo.SurveyQuestionBank", t => t.bank_id, cascadeDelete: false)
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
                    presented = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.us_id)
                .ForeignKey("dbo.Enrollment", t => t.enro_id, cascadeDelete: false)
                .Index(t => t.enro_id);

            CreateTable(
                "dbo.TrueFalseSurveyQuestion",
                c => new
                {
                    tfsq_id = c.Int(nullable: false, identity: true),
                    bank_id = c.Int(nullable: false),
                    title = c.String(),
                    content = c.String(),
                    correct = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.tfsq_id)
                .ForeignKey("dbo.SurveyQuestionBank", t => t.bank_id, cascadeDelete: false)
                .Index(t => t.bank_id);

            CreateTable(
                "dbo.TrueFalseSurveyUser",
                c => new
                {
                    usr_id = c.Int(nullable: false),
                    tfsq_id = c.Int(nullable: false),
                    user_answer = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.usr_id, t.tfsq_id })
                .ForeignKey("dbo.TrueFalseSurveyQuestion", t => t.tfsq_id, cascadeDelete: false)
                .ForeignKey("dbo.UserSurveyResponse", t => t.usr_id, cascadeDelete: false)
                .Index(t => t.usr_id)
                .Index(t => t.tfsq_id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.TrueFalseSurveyUser", "usr_id", "dbo.UserSurveyResponse");
            DropForeignKey("dbo.TrueFalseSurveyUser", "tfsq_id", "dbo.TrueFalseSurveyQuestion");
            DropForeignKey("dbo.TrueFalseSurveyQuestion", "bank_id", "dbo.SurveyQuestionBank");
            DropForeignKey("dbo.MultipleOptionsSurveyUser", "usr_id", "dbo.UserSurveyResponse");
            DropForeignKey("dbo.UserSurveyResponse", "enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.MultipleOptionsSurveyUser", "mosa_id", "dbo.MultipleOptionsSurveyAnswer");
            DropForeignKey("dbo.MultipleOptionsSurveyAnswer", "mosq_id", "dbo.MultipleOptionsSurveyQuestion");
            DropForeignKey("dbo.MultipleOptionsSurveyQuestion", "bank_id", "dbo.SurveyQuestionBank");
            DropForeignKey("dbo.SurveyQuestionBank", "survey_id", "dbo.SurveyModule");
            DropForeignKey("dbo.SurveyModule", "modu_id", "dbo.Module");
            DropIndex("dbo.TrueFalseSurveyUser", new[] { "tfsq_id" });
            DropIndex("dbo.TrueFalseSurveyUser", new[] { "usr_id" });
            DropIndex("dbo.TrueFalseSurveyQuestion", new[] { "bank_id" });
            DropIndex("dbo.UserSurveyResponse", new[] { "enro_id" });
            DropIndex("dbo.MultipleOptionsSurveyUser", new[] { "mosa_id" });
            DropIndex("dbo.MultipleOptionsSurveyUser", new[] { "usr_id" });
            DropIndex("dbo.SurveyModule", new[] { "modu_id" });
            DropIndex("dbo.SurveyQuestionBank", new[] { "survey_id" });
            DropIndex("dbo.MultipleOptionsSurveyQuestion", new[] { "bank_id" });
            DropIndex("dbo.MultipleOptionsSurveyAnswer", new[] { "mosq_id" });
            DropTable("dbo.TrueFalseSurveyUser");
            DropTable("dbo.TrueFalseSurveyQuestion");
            DropTable("dbo.UserSurveyResponse");
            DropTable("dbo.MultipleOptionsSurveyUser");
            DropTable("dbo.SurveyModule");
            DropTable("dbo.SurveyQuestionBank");
            DropTable("dbo.MultipleOptionsSurveyQuestion");
            DropTable("dbo.MultipleOptionsSurveyAnswer");
            DropTable("dbo.Advance");
            DropForeignKey("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrEnrollment", "vsdr_id", "dbo.VsdrSession");
            DropForeignKey("dbo.VsdrEnrollment", "user_id", "dbo.Users");
            DropIndex("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "user_id" });
            DropTable("dbo.VsdrUserFile");
            DropTable("dbo.VsdrTeacherComment");
            DropTable("dbo.VsdrEnrollment");
            DropTable("dbo.VsdrSession");
        }
    }
}
