using SCORM1.Enum;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Newspaper;
using SCORM1.Models.Personalizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Models.SCORM1;
using PagedList;
using SCORM1.Models.ratings;
using SCORM1.Models.RigidCourse;

namespace SCORM1.Models.ViewModel
{
    public class AdminTrainingContentViewModel : BaseViewModel
    {
        public Banner Banner { get; set; }
        public Changeinterface Changeinterface { get; set; }
        public List<Module> Module { get; set; }
        public Edition Edition { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class AdminTrainingEnrollmentViewModel : BaseViewModel
    {
        public IPagedList<ApplicationUser> UserEnrolllment { get; set; }
        public List<Module> listmodule { get; set; }
        public List<ApplicationUser> listuser { get; set; }
        public List<Enrollment> listenrollmentvirtual { get; set; }
        public List<Enrollment> listenrollmentevaluative { get; set; }
        public List<Enrollment> listenrollmentapplicative { get; set; }
        public Enrollment Enrollment { get; set; }
        public ApplicationUser User { get; set; }
        public string SearchUser { get; set; }
        public List<enrollment> listinfoenrollment { get; set; }
    }
    public class enrollment : BaseViewModel
    {
        public int Modu_Id { get; set; }
        public string User_id { get; set; }
        [Display(Name = "Nombre Modulo")]
        public string Modu_Name { get; set; }
        [Display(Name = "Descripción")]
        public string Modu_Description { get; set; }
        [Display(Name = "Estado Modulo")]
        public MODULESTATE Modu_Statemodule { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Modu_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        public DateTime Modu_FinishDate { get; set; }
        [Display(Name = " Imagen")]
        public string Modu_ImageName { get; set; }

        [Display(Name = "Tipo de Recurso")]
        public string Modu_Image { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string Modu_Content { get; set; }
        [Display(Name = "Puntos Modulo")]
        public int Modu_Points { get; set; }
        [Display(Name = "Tipo de Curso")]
        public CURSO Modu_TypeOfModule { get; set; }
        [Display(Name = "Foro")]
        public FORO Modu_Forum { get; set; }
        [Display(Name = "Buena Practica")]
        public FORO Modu_BetterPractice { get; set; }
        [Display(Name = "Mejora")]
        public FORO Modu_Improvement { get; set; }
        [Display(Name = "Evaluación")]
        public FORO Modu_Test { get; set; }
    }
    public class AdminTrainingModuleViewModel : BaseViewModel
    {

        public IEnumerable<SelectListItem> CategoryModule { get; set; }
        [Display(Name = "Categoria Modulo")]
        public int Modu_CategoryModuleId { get; set; }
        public Module Modules { get; set; }
        public List<Module> ListModules { get; set; }
        public List<TopicsCourse> ListTopics { get; set; }
        public TopicsCourse topics { get; set; }
        public List<ApplicationUser> ListUsers { get; set; }
        public List<CategoryModule> ListCategoryModules { get; set; }
        public int Modu_Id { get; set; }
        [Display(Name = "Nombre Curso")]
        public string Modu_Name { get; set; }
        [Display(Name = "Descripción")]
        [StringLength(333, ErrorMessage = "El tamaño máximo es de 333 caracteres")]
        public string Modu_Description { get; set; }
        [Display(Name = "Estado Curso")]
        public MODULESTATE Modu_Statemodule { get; set; }
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime Modu_InitDate { get; set; }
        [Display(Name = "Vigencia")]
        public int Modu_Validity { get; set; }
        [Display(Name = "Periodo")]
        public VIGENCIA Modu_Period { get; set; }
        [Display(Name = "Nombre Imagen")]
        public string Modu_ImageName { get; set; }
        [Display(Name = "Tipo Recurso")]
        public string Modu_ImageType { get; set; }
        [Display(Name = "Portada")]
        public byte[] Modu_Image { get; set; }
        [Display(Name = "Puntos Curso")]
        public int Modu_Points { get; set; }
        public int Modu_CompanyId { get; set; }
        public string Modu_UserId { get; set; }
        public string SearchModules { get; set; }
        [Display(Name = "Tipo Curso")]
        public CURSO Modu_TypeOfModule { get; set; }
        [Display(Name = "Foro")]
        public FORO Modu_Forum { get; set; }
        [Display(Name = "Buena Practica")]
        public FORO Modu_BetterPractice { get; set; }
        [Display(Name = "Mejora")]
        public FORO Modu_Improvement { get; set; }
        [Display(Name = "Evaluación")]
        public FORO Modu_Test { get; set; }
        [Display(Name = "Quien Sabe Mas")]
        public FORO QSMActive { get; set; }
        [Display(Name = "Falla Protegida")]
        public FORO HasProtectedFailure { get; set; }
    }

    #region Protected Failure Viewmodels

    //For creating a protected failure test
    public class AdminProtectedFailure : BaseViewModel
    {
        public int modu_id { get; set; }
        public ProtectedFailureTest protectedFailureTest { get; set; }
        public List<Category> categoryList { get; set; }
        public List<bool> bankToCreateList { get; set; }
        public List<int> questionQuantityList { get; set; }
        public List<float> approvedPercentageList { get; set; }
    }

    //for creating protected failuire questions and answers
    public class ProtectedFailureConfiguration : BaseViewModel
    {
        public List<CategoryQuestionBank> listOfQuestionBanks { get; set; }
        public List<ProtectedFailureMultiChoice> questionsList { get; set; }
        public List<ProtectedFailureMultiChoiceAnswer> answersList { get; set; }
    }

    //To-Do for reading user protected failure results
    public class ProtectedFailureUserResultsViewModel : BaseViewModel
    {

    }

    //To-Do Protected failure test user
    public class ProtectedFailureTestViewModel : BaseViewModel
    {
        public Module actualModule { get; set; }
        public Enrollment enrollment { get; set; }
        public ProtectedFailureTest protectedFailureTest { get; set; }
        public List<Category> listOfCategories { get; set; }
        public List<CategoryQuestionBank> questionBanks { get; set; }
        public List<ProtectedFailureMultiChoice> questionsList { get; set; }
        public List<ProtectedFailureAnswer> answersList { get; set; }
        public List<ProtectedFailureResults> resultList { get; set; }
        public List<int> selectedAnswers { get; set; }
        public bool isRecoverySession { get; set; }
    }

    public class UserSelectedAnswers
    {
        public int answerId { get; set; }
        public int isCorrect { get; set; }
        public int questionId { get; set; }
    }

    #endregion
    public class AdminTrainingCategoryModuleViewModel : BaseViewModel
    {
        public List<CategoryModule> ListCategoryModule { get; set; }
        public CategoryModule CategoryModules { get; set; }
        public int CaMo_Id { get; set; }
        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Categoria")]
        public string CaMo_Category { get; set; }
        public string SearchCategoryModule { get; set; }
    }

    public class AdminTrainingTopicViewModel : BaseViewModel
    {
        public IEnumerable<SelectListItem> modules { get; set; }
        [Display(Name = "Cursos Disponibles")]
        public int ToCo_ModuleId { get; set; }
        public List<TopicsCourse> ListTopic { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public List<Module> ListModule { get; set; }
        public int ToCo_Id { get; set; }
        [Display(Name = "Nombre Tema")]
        public string ToCo_Name { get; set; }
        [Display(Name = "Descripción")]
        public string ToCo_Description { get; set; }
        [Display(Name = "Intentos Permitidos")]
        public int ToCo_Attempt { get; set; }
        [Display(Name = "Puntaje Esperado")]
        public int ToCo_ExpectedScore { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime ToCo_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime ToCo_FinishDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_Content { get; set; }
        [Display(Name = "Portada")]
        public string Toco_Image { get; set; }
        [Display(Name = "Total Preguntas")]
        public int ToCo_TotalQuestion { get; set; }
        [Display(Name = "Requerido para Evaluación")]
        public REQUIREDEVALUATION ToCo_RequiredEvaluation { get; set; }
        public string SearchTopic { get; set; }

    }

    public class AdminTrainingEditionViewModel : BaseViewModel
    {
        public Edition Editions { get; set; }
    }

    public class AminTrainngLMSViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
    }

    public class AdminTrainingEngagementViewModel : BaseViewModel
    {
        public List<Prize> Prizes { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<Point> Point { get; set; }
    }

    public class AdminTrainingManagementUser : BaseViewModel
    {
        public List<ApplicationUser> Users { get; set; }
    }

    public class AdminTrainingReportsViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
    }

    //ViewTEST 
    public class AdminTrainingViewTest : BaseViewModel
    {
        public IPagedList<AdvanceUser> ListTest { get; set; }
        public double cali { get; set; }
    }

    public class AdminTrainingProfileViewModel : BaseViewModel
    {
        [Display(Name = "Autorizo el tratamiento de datos y acepto los términos y condiciones de la plataforma.")]
        public bool termsandconditions { get; set; }
        public bool videos { get; set; }
        public bool colorsBacgraundTitles { get; set; }
        public List<ApplicationUser> listuser { get; set; }
        public ApplicationUser user { get; set; }
        public List<Enrollment> Listmodulevirtual { get; set; }
        public List<Enrollment> Listmoduleevaluative { get; set; }
        public List<Edition> ListNewspaper { get; set; }
    }

    public class AdminTrainingGeneralViewModel : BaseViewModel
    {
        public List<BankQuestion> listtest { get; set; }
        [AllowHtml]
        public string BePr_CommentAdmin { get; set; }
        [AllowHtml]
        public string Impr_CommentAdmin { get; set; }
        public string baseUrl { get; set; }
        public Point Userpoint { get; set; }
        public IEnumerable<SelectListItem> CategoryModule { get; set; }
        [Display(Name = "Categoria Modulo")]
        public int Modu_CategoryModuleId { get; set; }
        public Module Modules { get; set; }
        public List<Module> ListModules { get; set; }
        public List<TopicsCourse> ListTopics { get; set; }
        public TopicsCourse topics { get; set; }
        public List<ApplicationUser> ListUsers { get; set; }
        public List<CategoryModule> ListCategoryModules { get; set; }
        public List<Job> ListJobs { get; set; }
        public int Modu_Id { get; set; }
        [Display(Name = "Nombre Modulo")]
        public string Modu_Name { get; set; }
        [Display(Name = "Descripción")]
        public string Modu_Description { get; set; }
        [Display(Name = "Estado Modulo")]
        public MODULESTATE Modu_Statemodule { get; set; }
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime Modu_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime Modu_FinishDate { get; set; }
        [Display(Name = "Nombre Imagen")]
        public string Modu_ImageName { get; set; }

        [Display(Name = "Tipo de Recurso")]
        public string Modu_ImageType { get; set; }
        [Display(Name = "Portada")]
        public byte[] Modu_Image { get; set; }
        [Display(Name = "Puntos Modulo")]
        public int Modu_Points { get; set; }
        public int Modu_CompanyId { get; set; }
        public string Modu_UserId { get; set; }
        public string SearchModules { get; set; }

        //Jobs
        //Variable utilizada para almacenar el id del recurso
        public int Job_Id { get; set; }
        //Variable utilizada para almacenar el nombre del recurso
        [Display(Name = "Nombre")]
        public string Job_Name { get; set; }
        //Variable utilizada para almacenar la descripción del recurso
        [Display(Name = "Descripción")]
        public string Job_Description { get; set; }
        //Variable utilizada para almacenar el tipo de recurso
        [Display(Name = "Tipo Trabajo")]
        public TYPEJOB Job_TypeJob { get; set; }
        //Variable utilizada para almacenar la visibilidad del curso
        [Display(Name = "Visible")]
        public FORO Job_Visible { get; set; }
        //Variable utilizada para almacenar el estado del recurso
        [Display(Name = "Estado")]
        public MODULESTATE Job_StateJob { get; set; }
        //Variable utilizada para almacenar la fecha de inicio del recurso
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime Job_InitDate { get; set; }
        //Variable utilizada para almacenar la fecha de finalización del recurso
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime Job_FinishDate { get; set; }
        //Variable utilizada para almacenar el contenido del recurso
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string Job_Content { get; set; }
        //Variable utilizada para almacenar los puntos del recurso
        [Display(Name = "Puntos")]
        public int Job_Points { get; set; }

        public List<TopicsCourse> ListTopic { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public List<Module> ListModule { get; set; }
        public int ToCo_Id { get; set; }
        [Display(Name = "Nombre Tema")]
        public string ToCo_Name { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string ReMo_Name { get; set; }
        [Display(Name = "Descripción")]
        [StringLength(333, ErrorMessage = "El tamaño máximo es de 333 caracteres")]
        public string ToCo_Description { get; set; }
        [Display(Name = "Intentos Permitidos")]
        public int ToCo_Attempt { get; set; }
        [Display(Name = "Puntaje Esperado")]
        public int ToCo_ExpectedScore { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime ToCo_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime ToCo_FinishDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_Content { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_ContentVirtual { get; set; }
        [Display(Name = "Portada")]
        public string Toco_Image { get; set; }
        [Display(Name = "Total Preguntas")]
        public int ToCo_TotalQuestion { get; set; }
        [Display(Name = "Evaluación")]
        public REQUIREDEVALUATION ToCo_RequiredEvaluation { get; set; }
        public string SearchTopic { get; set; }

        [Display(Name = "Documento de Apoyo")]
        public TYPESUPPORTDOCUMENT ToCo_TypeDocument { get; set; }

        [Display(Name = "Tipo")]
        public TYPE ToCo_Type { get; set; }
        [Display(Name = "Visible")]
        public FORO ToCo_Visible { get; set; }

        //BetterPractice
        public List<BetterPractice> ListBetterPractice { get; set; }
        public BetterPractice BetterPractice { get; set; }
        public int BePr_Id { get; set; }
        [Display(Name = "Puntos por Buena Practica")]
        public int BePr_Points { get; set; }
        [Display(Name = "Resultado Obtenido")]
        public string BePr_Comment { get; set; }
        [Display(Name = "¿Qué se Hizo?")]
        public string BePr_TiTle { get; set; }
        [Display(Name = "Recurso")]
        public string BePr_Resource { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime? BePr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime? BePr_FinishDate { get; set; }
        [Display(Name = "Estado Aporte")]
        public BETTERPRACTICESTATE BePr_StateBetterpractice { get; set; }
        //Improvement 
        public List<Improvement> ListImprovement { get; set; }
        public Improvement Improvement { get; set; }
        public int Impr_Id { get; set; }
        [Display(Name = "Puntos por Mejora")]
        public int Impr_Points { get; set; }
        [Display(Name = "Propuesta")]
        public string Impr_Comment { get; set; }
        [Display(Name = "Resultado Esperado")]
        public string Impr_Comment2 { get; set; }
        [Display(Name = "¿Qué pasaba?")]
        public string Impr_Title { get; set; }
        [Display(Name = "Recurso")]
        public string Impr_Resource { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime? Impr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime? Impr_FinishDate { get; set; }
        [Display(Name = "Estado Aporte")]
        public IMPROVEMENTSTATE Impr_StateImprovement { get; set; }

    }

    public class AdminTrainingTestViewModel : BaseViewModel
    {
        public IEnumerable<SelectListItem> AnswerPairing { get; set; }
        //topic
        public TopicsCourse topic { get; set; }
        public Module module { get; set; }
        public List<TopicsCourse> ListTopic { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public List<Module> ListModule { get; set; }
        public int ToCo_Id { get; set; }
        //bank question
        public List<BankQuestion> ListBankQuestion { get; set; }
        public BankQuestion BankQuestion { get; set; }
        public int BaQu_Id { get; set; }
        [Display(Name = "Nombre Prueba")]
        public string BaQu_Name { get; set; }
        [Display(Name = "Porcentaje certificación")]
        public int BaQu_Porcentaje { get; set; }
        [Display(Name = "Porcentaje de Evaluación")]
        public int BaQu_Porcentaje2 { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime BaQu_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime BaQu_FinishDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string BaQu_Content { get; set; }
        [Display(Name = "Total Preguntas")]
        public int BaQu_TotalQuestion { get; set; }
        [Display(Name = "Preguntas certificación")]
        public int BaQu_QuestionUser { get; set; }
        public int TotalQuestion { get; set; }
        [Display(Name = "Seleccionar Preguntas")]
        public FORO BaQu_SelectQuestion { get; set; }

        //open question
        public List<OpenQuestion> ListOpenQuestion { get; set; }
        public OpenQuestion OpenQuestion { get; set; }
        public int OpQu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpQu_Question { get; set; }
        [Display(Name = "Respuesta")]
        public string OpQu_Answer { get; set; }
        [Display(Name = "Puntaje")]
        public int OpQu_Score { get; set; }
        //Answer Open Question
        public int AnOQ_Id { get; set; }
        [Display(Name = "Respuesta")]
        public string AnOQ_Answer { get; set; }
        //Option Multiple
        public List<OptionMultiple> ListOptionMultiple { get; set; }
        public OptionMultiple OptionMultiple { get; set; }
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]

        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int OpMu_Score { get; set; }
        [Display(Name = "Descripción pregunta")]
        public string OpMu_Description { get; set; }

        [AllowHtml]
        public string OpMult_Content { get; set; }


        //Answer Option Multiple
        public List<AnswerOptionMultiple> ListAnswerOptionMultiple { get; set; }
        public AnswerOptionMultiple AnswerOptionMultiple { get; set; }
        public int AnOp_Id { get; set; }
        [Display(Name = "Respuesta")]
        public string AnOp_OptionAnswer { get; set; }
        [Display(Name = "Tipo")]
        public OPTIONANSWER AnOp_TrueAnswer { get; set; }
        public string baseUrl { get; set; }
        [AllowHtml]
        public string Answer_OpMult_Content { get; set; }
        //Pairing
        public List<pairing> ListPairing { get; set; }
        public List<Pairing> ListPairings { get; set; }
        public Pairing Pairing { get; set; }
        public int Pair_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string Pair_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int Pair_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string Pair_Description { get; set; }
        //AnswerPairing
        public List<AnswerPairing> ListAnswerpairing { get; set; }
        public AnswerPairing AnswerParing { get; set; }
        public int AnPa_Id { get; set; }
        [Display(Name = "Preguntas")]
        public string AnPa_OptionsQuestion { get; set; }
        [Display(Name = "Respuestas")]
        public string AnPa_OptionAnswer { get; set; }
        //true or false
        public List<TrueOrFalse> ListTrueOrFalse { get; set; }
        public TrueOrFalse TrueOrFalse { get; set; }
        public int TrFa_Id { get; set; }
        [Display(Name = " Pregunta")]
        public string TrFa_Question { get; set; }
        [Display(Name = "Descripción")]
        public string TrFa_Description { get; set; }
        [Display(Name = "Respuesta Falsa")]
        public string TrFa_FalseAnswer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public string TrFa_TrueAnswer { get; set; }
        [Display(Name = "Puntaje")]
        public string TrFa_Score { get; set; }
        [Display(Name = "Estado Pregunta")]
        public OPTIONANSWER TrFa_State { get; set; }
        [AllowHtml]
        public string TrFa_Content { get; set; }

    }

    public class AdminTrainingBankQuestionViewModel : BaseViewModel
    {
        public BankQuestion bankquestion { get; set; }
        public List<BankQuestion> Listbank { get; set; }
        public int TotalQuestion { get; set; }
    }

    // copia del banco de pregunta
    public class AdminManagementBankQuestions : BaseViewModel
    {
        public int bankReceiving { get; set; }
        public List<BankQuestion> bankquestion { get; set; }
        public BankQuestion bank { get; set; }
        public List<ListTrueorfalse> TrueOrFalse { get; set; }
        public List<ListPairing> Pairing { get; set; }
        public List<ListOptionmultiple> OptionMultiple { get; set; }

    }

    public class ListOptionmultiple : BaseViewModel
    {
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int OpMu_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }

        public bool Check { get; set; }
        //Answer Option Multiple
        public int BaQu_Id { get; set; }
        public ICollection<AnswerOptionMultiple> listanswer { get; set; }


    }

    public class ListPairing : BaseViewModel
    {
        public int Pair_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string Pair_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int Pair_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string Pair_Description { get; set; }
        public bool Check { get; set; }
        public int BaQu_Id { get; set; }
        public IEnumerable<AnswerPairing> AnswerPairing { get; set; }

    }
    public class ListTrueorfalse : BaseViewModel
    {
        public bool Check { get; set; }
        public int BaQu_Id { get; set; }
        public int TrFa_Id { get; set; }
        [Display(Name = " Pregunta")]
        public string TrFa_Question { get; set; }
        [Display(Name = "Descripción")]
        public string TrFa_Description { get; set; }
        [Display(Name = "Respuesta Falsa")]
        public string TrFa_FalseAnswer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public string TrFa_TrueAnswer { get; set; }
        [Display(Name = "Puntaje")]
        public string TrFa_Score { get; set; }
        [Display(Name = "Estado Pregunta")]
        public OPTIONANSWER TrFa_State { get; set; }
        [AllowHtml]
        public string TrFa_Content { get; set; }

    }

    public class AdminTrainingNewAttemptsViewMode : BaseViewModel
    {
        public List<NewAttempts> NewAttempts { get; set; }
    }

    public class AdminModuleViewModel : BaseViewModel
    {
        //en esta lista se van a almacenar los cursos que tenga matriculado el administrador de formación
        public Enrollment Enrollment { get; set; }

        public List<Enrollment> listenrrolment { get; set; }
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }
        [Display(Name = "Mensaje")]
        [AllowHtml]
        public string Mensaje { get; set; }
        public int idEnroll { get; set; }
        public string baseUrl { get; set; }
    }
    public class AdminTrainingMassiveEnrollmentModuleViewMode : BaseViewModel
    {
        public List<Module> ListModuleEnrollment { get; set; }
        public string SearchModuleEnrollment { get; set; }
    }

    public class AdminTrainingMassiveEnrollmentViewMode : BaseViewModel
    {
        public List<Module> ListModuleEnrollment { get; set; }
        public Module Module { get; set; }
        [Display(Name = "Areas")]
        public int Area_Id { get; set; }
        [Display(Name = "Cargos")]
        public int Posi_Id { get; set; }
        [Display(Name = "Ciudades")]
        public int Cyty_Id { get; set; }
        [Display(Name = "Ubicaciones")]
        public int Loca_Id { get; set; }
        public List<ApplicationUser> listuser { get; set; }
        public List<AllUser> ListAllUser { get; set; }
        public List<Areas> ListAreas { get; set; }
        public List<Positions> ListPositions { get; set; }
        public List<Cities> ListCitices { get; set; }
        public List<Locations> ListLocations { get; set; }
        public List<user> ListUserCancel { get; set; }
        public List<user1> ListUserCancel1 { get; set; }
        public List<user2> ListUserCancel2 { get; set; }
    }

    public class user : BaseViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class user1 : BaseViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class user2 : BaseViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class AllUser : BaseViewModel
    {
        public int Modu_Id { get; set; }
        public string User_Id { get; set; }
    }

    public class Areas : BaseViewModel
    {
        public int Area_Id { get; set; }
        public int Modu_Id { get; set; }
        public List<Area> Listareas { get; set; }
    }

    public class Positions : BaseViewModel
    {
        public int Posi_Id { get; set; }
        public int Modu_Id { get; set; }
        public List<Position> Listpositions { get; set; }
    }

    public class Cities : BaseViewModel
    {
        public int City_Id { get; set; }
        public int Modu_Id { get; set; }
        public List<City> Listcities { get; set; }
    }

    public class Locations : BaseViewModel
    {
        public int Loca_Id { get; set; }
        public int Modu_Id { get; set; }
        public List<Location> Listlocations { get; set; }
    }

}