using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using SCORM1.Models.MeasuringSystem;
using SCORM1.Models.Lms;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.Engagement;
using SCORM1.Models.Newspaper;
using SCORM1.Models.SCORM1;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SCORM1.Enum;
using SCORM1.Models.Logs;
using SCORM1.Models.Personalizations;
using SCORM1.Models.ServiceBlock;
using System.Data.Entity.ModelConfiguration.Conventions;
using SCORM1.Models.Games;
using System;
using SCORM1.Models.MainGame;
using SCORM1.Models.ratings;
using SCORM1.Models.RigidCourse;
using SCORM1.Models.VSDR;
using SCORM1.Models.Survey;

namespace SCORM1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombres")]
        public string FirstName { get; set; }
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }
        [Display(Name = "Rol")]
        public ROLES Role { get; set; }
        [Display(Name = "Estado Usuario")]
        public STATEUSER StateUser { get; set; }
        [Display(Name = "Country")]
        public COUNTRY Country { get; set; }
        [Display(Name = "Documento")]
        public string Document { get; set; }
        [Display(Name = "Compañia")]
        public int? CompanyId { get; set; }
        [Display(Name = "Cargo")]
        public int? PositionId { get; set; }
        [Display(Name = "Area")]
        public int? AreaId { get; set; }
        [Display(Name = "Ciudad")]
        public int? CityId { get; set; }
        [Display(Name = "Ubicación")]
        public int? LocationId { get; set; }
        [Display(Name = "Activo")]
        public ACTIVEUSERTOENTER Enable { get; set; }

        [DataType(DataType.Date)]
        public DateTime? lastAccess { get; set; }

        [DataType(DataType.Date)]
        public DateTime? firstAccess { get; set; }
        [Display(Name = "Terminos y Condiciones")]
        public Terms_and_Conditions TermsandConditions { get; set; }
        [Display(Name = "Terminos y Condiciones")]
        public VIDEOS Videos { get; set; }
        [Display(Name = "Sesión Usuario")]
        public SESION SesionUser { get; set; }
        public Terms_and_Conditions TermsJuego { get; set; }
        public String Foto_perfil { get; set; }
        public Int32? ComunidadActiva { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        public virtual ICollection<MeasureUser> Measures { get; set; }
        public virtual ICollection<Module> Module { get; set; }
        public virtual ICollection<Result> Result { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
        public virtual ICollection<UserInfo> UserInfo { get; set; }
        public virtual ICollection<BetterPractice> BetterPractice { get; set; }
        public virtual ICollection<Improvement> Improvement { get; set; }
        public virtual ICollection<Certification> Certification { get; set; }
        public virtual ICollection<Desertify> Desertify { get; set; }
        public virtual ICollection<AdvanceCourse> AdvanceCourse { get; set; }
        public virtual ICollection<Point> Point { get; set; }
        public virtual ICollection<AdvanceUser> AdvanceUser { get; set; }
        public virtual ICollection<PointsComment> PointsComment { get; set; }
        public virtual ICollection<ApplicationUser> SuperiorUsers { get; set; }
        public virtual ICollection<ApplicationUser> EqualUsers { get; set; }
        public virtual ICollection<ApplicationUser> ClientsUsers { get; set; }
        public virtual ICollection<ApplicationUser> MyOfficeUsers { get; set; }
        public virtual ICollection<LockGame> LockGame { get; set; }
        public virtual ICollection<Attempts> Attempts { get; set; }
        public virtual ICollection<NewAttempts> NewAttempts { get; set; }
        public virtual ICollection<MG_Point> MG_Point { get; set; }
        public virtual ICollection<MG_AnswerUser> MG_AnswerUser { get; set; }
        public virtual ICollection<MG_BlockGameUser> MG_BlockGameUser { get; set; }
        public virtual ICollection<Job> Job { get; set; }
        public virtual ICollection<ResourceForum> ResourceForum { get; set; }
        public virtual ICollection<ResourceJobs> ResourceJobs { get; set; }
        public virtual ICollection<AnswersForum> AnswersForum { get; set; }


        //Here are the Added Needed Values to IdentityUser to Score Application.

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("UserId", userIdentity.GetUserId()));
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("BureauScore", throwIfV1Schema: false)
        {
        }

        //Creating table Of SCORM1 
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<LogsComunidad> LogsComunidad { get; set; }
        public virtual DbSet<CorreoModel> Correos { get; set; }

        //Creating table Of Measuring System
        public DbSet<MeasureUser> MeasureUser { get; set; }
        public DbSet<LogsUserInPlans> LogsUserInPlans { get; set; }
        public virtual DbSet<Proficiency> Proficiencies { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Measure> Measures { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        //Creating table Of PageCustomization
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<StylesLogos> StylesLogos { get; set; }

        //Creating table Of Engagement
        public virtual DbSet<PointManagerCategory> PointManagerCategory { get; set; }
        public virtual DbSet<CategoryPrize> CategoryPrizes { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<TypePoint> TypePoints { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<Exchange> Exchanges { get; set; }
        //Creating table Of Lms
        public virtual DbSet<CategoryModule> CategoryModules { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Improvement> Improvements { get; set; }
        public virtual DbSet<ResourceBetterPractice> ResourceBetterPractices { get; set; }
        public virtual DbSet<BetterPractice> BetterPractices { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<TopicsCourse> TopicsCourses { get; set; }
        public virtual DbSet<ResourceTopic> ResourceTopics { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<BankQuestion> BankQuestions { get; set; }
        public virtual DbSet<AdvanceUser> AdvanceUsers { get; set; }
        public virtual DbSet<AdvanceLoseUser> AdvanceLoseUser { get; set; }

        public virtual DbSet<AdvanceCourse> AdvanceCourses { get; set; }
        public virtual DbSet<Certification> Certifications { get; set; }
        public virtual DbSet<Desertify> Desertifys { get; set; }
        public virtual DbSet<OpenQuestion> OpenQuestions { get; set; }
        public virtual DbSet<TrueOrFalse> TrueOrFalses { get; set; }
        public virtual DbSet<TrueOrFalseStudent> TrueOrFalseStudent { get; set; }

        public virtual DbSet<Pairing> Pairings { get; set; }
        public virtual DbSet<OptionMultiple> OptionMultiples { get; set; }
        public virtual DbSet<AnswerPairing> AnswerPairings { get; set; }
        public virtual DbSet<AnserPairingStudent> AnserPairingStudent { get; set; }
        public virtual DbSet<AnswerOptionMultiple> AnswerOptionMultiples { get; set; }
        public virtual DbSet<AnswerOptionMultipleStudent> AnswerOptionMultipleStudent { get; set; }

        public virtual DbSet<Advance> Advances {get;set;}

        //Creating table Of Logs
        public virtual DbSet<CodeLogs> CodeLogs { get; set; }
        public virtual DbSet<TableChange> TableChanges { get; set; }
        public virtual DbSet<IdChange> IdChanges { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Attempts> Attempts { get; set; }
        public virtual DbSet<ResourceTopics> ResourceTopicss { get; set; }
        public virtual DbSet<NewAttempts> NewAttempts { get; set; }

        //Creating table Of Newspaper
        public virtual DbSet<Edition> Editions { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ResourceNw> ResourceNws { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<PointsComment> PointsComments { get; set; }
        public virtual DbSet<Section> Section { get; set; }

        //Creating table Of Personalizations
        public virtual DbSet<Changeinterface> Changeinterfaces { get; set; }

        //Creating table Of Service Block
        public virtual DbSet<TypeServiceBlock> TypeServiceBlocks { get; set; }
        public virtual DbSet<BlockService> BlockServices { get; set; }

        //Creating table Of LockGame
        public virtual DbSet<TypeBaneo> TypeBaneos { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<LockGame> LockGame { get; set; }
        public virtual DbSet<PointsObtainedForUser> PointsObtainedForUser { get; set; }
        public virtual DbSet<ImageUpload> ImageUpload { get; set; }

        //Creating table of Main Game
        public virtual DbSet<MG_AnswerMultipleChoice> MG_AnswerMultipleChoice { get; set; }
        public virtual DbSet<MG_AnswerOrder> MG_AnswerOrder { get; set; }
        public virtual DbSet<MG_AnswerPairing> MG_AnswerPairing { get; set; }
        public virtual DbSet<MG_MultipleChoice> MG_MultipleChoice { get; set; }
        public virtual DbSet<MG_Order> MG_Order { get; set; }
        public virtual DbSet<MG_Pairing> MG_Pairing { get; set; }
        public virtual DbSet<MG_Point> MG_Point { get; set; }
        public virtual DbSet<MG_SettingMp> MG_SettingMp { get; set; }
        public virtual DbSet<MG_Template> MG_Template { get; set; }
        public virtual DbSet<MG_AnswerUser> MG_AnswerUser { get; set; }
        public virtual DbSet<MG_BlockGameUser> MG_BlockGameUser { get; set; }

        //Creating table Of Ratings
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<ResourceJobs> ResourceJobs { get; set; }
        public virtual DbSet<ResourceForum> ResourceForum { get; set; }
        public virtual DbSet<AnswersForum> AnswersForum { get; set; }
        public virtual DbSet<BookRatings> BookRatings { get; set; }
        public virtual DbSet<QuienSabeMasPuntaje> QuienSabeMasPuntajes { get; set; }

        //Rigid Course 
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ProtectedFailureTest> ProtectedFailureTests { get; set; }
        public virtual DbSet<CategoryQuestionBank> CategoryQuestionsBanks { get; set; }
        public virtual DbSet<ProtectedFailureMultiChoice> ProtectedFailureMultiChoices { get; set; }
        public virtual DbSet<ProtectedFailureResults> ProtectedFailureResults { get; set; }
        public virtual DbSet<ProtectedFailureAnswer> ProtectedFailureAnswer { get; set; }
        public virtual DbSet<FlashTest> FlashTest { get; set; }
        public virtual DbSet<FlashQuestion> FlashQuestion { get; set; }
        public virtual DbSet<FlashQuestionAnswer> FlashQuestionAnswer { get; set; }
        public virtual DbSet<UserModuleAdvance> UserModuleAdvances { get; set; }
        public virtual DbSet<ProtectedFailureMultiChoiceAnswer> ProtectedFailureMultiChoiceAnswers { get; set; }

        //Virtual Syncronic Debate Room
        public virtual DbSet<VsdrSession> VsdrSessions { get; set; }
        public virtual DbSet<VsdrEnrollment> VsdrEnrollments { get; set; }
        public virtual DbSet<VsdrTeacherComment> VsdrTeacherComments { get; set; }
        public virtual DbSet<VsdrUserFile> VsdrUserFiles { get; set; }

        //Survey
        public virtual DbSet<SurveyModule> Surveys { get; set; }
        public virtual DbSet<SurveyQuestionBank> SurveyQuestionBanks { get; set; }
        public virtual DbSet<UserSurveyResponse> UserSurveyResponses { get; set; }
        public virtual DbSet<MultipleOptionsSurveyQuestion> MultipleOptionsSurveyQuestions { get; set; }
        public virtual DbSet<MultipleOptionsSurveyAnswer> MultipleOptionsSurveyAnswers { get; set; }
        public virtual DbSet<MultipleOptionsSurveyUser> MultipleOptionsSurveyUsers { get; set; }
        public virtual DbSet<TrueFalseSurveyQuestion> TrueFalseSurveyQuestions { get; set; }
        public virtual DbSet<TrueFalseSurveyUser> TrueFalseSurveyUsers { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<Advance>().ToTable("Advance");
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(a => a.Area)
                .WithMany()
                .HasForeignKey(a => a.AreaId);

            modelBuilder.Entity<Area>()
                .HasOptional(b => b.ApplicationUser)
                .WithMany()
                .HasForeignKey(b => b.UserId);


            //Create the SuperiorUser Table
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.SuperiorUsers)
                .WithMany()
                .Map(ma =>
                {
                    ma.ToTable("UserSuperiors");
                });

            //Create the SuperiorUser Table
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.EqualUsers)
                .WithMany()
                .Map(ma =>
                {
                    ma.ToTable("UserEquals");
                });

            //Create the SuperiorUser Table
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.ClientsUsers)
                .WithMany()
                .Map(ma =>
                {
                    ma.ToTable("UserClients");
                });

            //Create the SuperiorUser Table
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.MyOfficeUsers)
                .WithMany()
                .Map(ma =>
                {
                    ma.ToTable("MyOfficeUser");
                });


            modelBuilder.Entity<VsdrSession>().ToTable("VsdrSession");
            modelBuilder.Entity<VsdrEnrollment>().ToTable("VsdrEnrollment");
            modelBuilder.Entity<VsdrTeacherComment>().ToTable("VsdrTeacherComment");
            modelBuilder.Entity<VsdrUserFile>().ToTable("VsdrUserFile");

            modelBuilder.Entity<SurveyModule>().ToTable("SurveyModule");
            modelBuilder.Entity<SurveyQuestionBank>().ToTable("SurveyQuestionBank");
            modelBuilder.Entity<UserSurveyResponse>().ToTable("UserSurveyResponse");
            modelBuilder.Entity<MultipleOptionsSurveyQuestion>().ToTable("MultipleOptionsSurveyQuestion");
            modelBuilder.Entity<MultipleOptionsSurveyAnswer>().ToTable("MultipleOptionsSurveyAnswer");
            modelBuilder.Entity<MultipleOptionsSurveyUser>().ToTable("MultipleOptionsSurveyUser");
            modelBuilder.Entity<TrueFalseSurveyQuestion>().ToTable("TrueFalseSurveyQuestion");
            modelBuilder.Entity<TrueFalseSurveyUser>().ToTable("TrueFalseSurveyUser");

            modelBuilder.Entity<Question>()
                .HasMany(question => question.Tests)
                .WithMany(test => test.Questions)
                .Map(ma =>
                {
                    ma.MapLeftKey("QuestionId");
                    ma.MapRightKey("TestId");
                    ma.ToTable("QuestionTest");
                });


            modelBuilder.Entity<TopicsCourse>()
            .HasRequired(t => t.Module)
            .WithMany(m => m.TopicsCourse)
            .HasForeignKey(d => d.Modu_Id)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<CategoryModule>()
            .HasRequired(t => t.Company)
            .WithMany(m => m.CategoryModule)
            .HasForeignKey(d => d.CompanyId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<CategoryPrize>()
            .HasRequired(t => t.Company)
            .WithMany(m => m.CategoryPrize)
            .HasForeignKey(d => d.CompanyId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<ResourceTopic>()
          .HasRequired(t => t.Company)
          .WithMany(m => m.ResourceTopic)
          .HasForeignKey(d => d.CompanyId)
          .WillCascadeOnDelete(false);

            modelBuilder.Entity<Enrollment>()
            .HasRequired(t => t.Company)
            .WithMany(m => m.Enrollment)
            .HasForeignKey(d => d.CompanyId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Module>()
           .HasRequired(t => t.Company)
           .WithMany(m => m.Module)
           .HasForeignKey(d => d.CompanyId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<MG_Template>()
         .HasRequired(t => t.Company)
         .WithMany(m => m.MG_Template)
         .HasForeignKey(d => d.Company_Id)
         .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}