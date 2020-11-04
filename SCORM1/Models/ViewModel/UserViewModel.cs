using SCORM1.Enum;
using SCORM1.Models.Personalizations;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Newspaper;
using SCORM1.Models.PageCustomization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class UsersViewModel
    {
        public Banner Banner { get; set; }
        public Changeinterface Changeinterface { get; set; }
        public Module Module { get; set; }
        public Edition Edition { get; set; }
        public ApplicationUser User { get; set; }

    }

    public class UsersEditionViewModel : BaseViewModel
    {
        public Edition Editions { get; set; }
    }

    public class UsersLMSViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
    }

    public class UsersEngagementViewModel : BaseViewModel
    {
        public List<Prize> Prizes { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<Point> Point { get; set; }
    }

    public class UsersManagementUser : BaseViewModel
    {
        public ApplicationUser User { get; set; }
    }

    public class UserEnrollmentViewmodel : BaseViewModel
    {
        public List<Enrollment> enrollment { get; set; }
        public List<QuienSabeMasPuntaje> quienSabeMasPuntajes { get; set; }

        public string SearchModuleVirtual { get; set; }
        public string SearchModuleEvaluative { get; set; }
    }
    public class UserPendientes : BaseViewModel
    {
        public List<TopicsCourse> CursosFaltantes { get; set; }
    }
    public class UserViewEvaluaciones : BaseViewModel
    {
        //    public List<Enrollment> Enrollment { get; set; }
        //public List<TopicsCourse> listenrollment { get; set; }
        public List<BankQuestion> ListBankQuestion { get; set; }
        //    public ApplicationUser user { get; set; }
    }

    public class UserArticleViewModel : BaseViewModel
    {
        public List<Article> articles { get; set; }
        public string SearchArticle { get; set; }
    }
    public class UserProfileViewModel : BaseViewModel
    {
        public FormViewModel Form { get; set; }
        public List<Certification> listcert { get; set; }
        [Display(Name = "Autorizo el tratamiento de datos y acepto los términos y condiciones de la plataforma.")]
        public bool termsandconditions { get; set; }
        public bool videos { get; set; }
        public bool Opción1 { get; set; }
        public bool Opción2 { get; set; }
        public double progress { get; set; }
        public GAME juego { get; set; }
        public int Idgame { get; set; }
        public bool game { get; set; }
        public DateTime fechajuego { get; set; }
        public List<Enrollment> Listmodulevirtual { get; set; }
        public List<Enrollment> Listmoduleevaluative { get; set; }
        public List<Enrollment> Listmoduleapplicative { get; set; }
        public Edition EditionCurrentToActive { get; set; }
        public List<Article> ListArticles { get; set; }
        public List<ApplicationUser> listuser { get; set; }
        public ApplicationUser user { get; set; }
        public List<Module> Listmodule { get; set; }
        public List<Edition> ListNewspaper { get; set; }
        [Display(Name = "Contraseña")]
        [StringLength(255, ErrorMessage = "La Contraseña debe ser mayor a 5 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public Int32? ComunidadActiva { get; set; }
        public bool hasClientProfile { get; set; }

    }

    public class UserGeneralViewModel1 : BaseViewModel
    {
        public List<TopicsCourse> listenrollment { get; set; }
        public List<BankQuestion> ListBankQuestion { get; set; }
        public string baseUrl { get; set; }
        public List<AdvanceUser> listadvanceuser { get; set; }
        public List<Attempts> listattempts { get; set; }
        public IEnumerable<SelectListItem> CategoryModule { get; set; }
        [Display(Name = "Categoria Modulo")]
        public int Modu_CategoryModuleId { get; set; }
        public Attempts AttemptsUser { get; set; }
        public AdvanceUser AdvanceUser { get; set; }
        public Module Modules { get; set; }
        public List<Enrollment> ListaEnrol { get; set; }
        public ApplicationUser user { get; set; }
        public List<Module> ListModules { get; set; }
        public List<TopicsCourse> ListTopics { get; set; }
        public TopicsCourse topics { get; set; }
        public List<ApplicationUser> ListUsers { get; set; }
        public List<CategoryModule> ListCategoryModules { get; set; }
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

        //topics
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
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_ContentVirtual { get; set; }
        [Display(Name = "Portada")]
        public string Toco_Image { get; set; }
        [Display(Name = "Total Preguntas")]
        public int ToCo_TotalQuestion { get; set; }
        [Display(Name = "Requerido para Evaluación")]
        public REQUIREDEVALUATION ToCo_RequiredEvaluation { get; set; }
        public string SearchTopic { get; set; }

        [Display(Name = "Documento de Apoyo")]
        public TYPESUPPORTDOCUMENT ToCo_TypeDocument { get; set; }

        //BetterPractice

        public BetterPractice BetterPractice { get; set; }
        public int BePr_Id { get; set; }
        [Display(Name = "Puntos por Buena Practica")]
        public int BePr_Points { get; set; }

        [Display(Name = "Resultado Obtenido")]
        [AllowHtml]
        public string BePr_Comment { get; set; }

        [Display(Name = "¿Qué se hizo?")]
        public string BePr_TiTle { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string BePr_Name { get; set; }
        [Display(Name = "Recurso")]
        public string BePr_Resource { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime? BePr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime? BePr_FinishDate { get; set; }
        //Improvement

        public Improvement Improvement { get; set; }
        public int Impr_Id { get; set; }
        [Display(Name = "Puntos por Mejora")]
        public int Impr_Points { get; set; }

        [Display(Name = "Propuesta")]
        [AllowHtml]
        public string Impr_Comment { get; set; }

        [Display(Name = "Resultado Esperado")]
        [AllowHtml]
        public string Impr_Comment2 { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string Impr_Name { get; set; }
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

    }

    public class UserGeneralViewModel : BaseViewModel
    {
        public List<TopicsCourse> listenrollment { get; set; }
        public List<BankQuestion> ListBankQuestion { get; set; }
        public string baseUrl { get; set; }
        public List<AdvanceUser> listadvanceuser { get; set; }
        public List<Attempts> listattempts { get; set; }
        public IEnumerable<SelectListItem> CategoryModule { get; set; }
        [Display(Name = "Categoria Modulo")]
        public int Modu_CategoryModuleId { get; set; }
        public Attempts AttemptsUser { get; set; }
        public AdvanceUser AdvanceUser { get; set; }
        public Module Modules { get; set; }
        public Enrollment Enrollment { get; set; }
        public ApplicationUser user { get; set; }
        public List<Module> ListModules { get; set; }
        public List<TopicsCourse> ListTopics { get; set; }
        public TopicsCourse topics { get; set; }
        public List<ApplicationUser> ListUsers { get; set; }
        public List<CategoryModule> ListCategoryModules { get; set; }
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

        //topics
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
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_ContentVirtual { get; set; }
        [Display(Name = "Portada")]
        public string Toco_Image { get; set; }
        [Display(Name = "Total Preguntas")]
        public int ToCo_TotalQuestion { get; set; }
        [Display(Name = "Requerido para Evaluación")]
        public REQUIREDEVALUATION ToCo_RequiredEvaluation { get; set; }
        public string SearchTopic { get; set; }

        [Display(Name = "Documento de Apoyo")]
        public TYPESUPPORTDOCUMENT ToCo_TypeDocument { get; set; }

        //BetterPractice

        public BetterPractice BetterPractice { get; set; }
        public int BePr_Id { get; set; }
        [Display(Name = "Puntos por Buena Practica")]
        public int BePr_Points { get; set; }

        [Display(Name = "Resultado Obtenido")]
        [AllowHtml]
        public string BePr_Comment { get; set; }

        [Display(Name = "¿Qué se hizo?")]
        public string BePr_TiTle { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string BePr_Name { get; set; }
        [Display(Name = "Recurso")]
        public string BePr_Resource { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime? BePr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime? BePr_FinishDate { get; set; }
        //Improvement

        public Improvement Improvement { get; set; }
        public int Impr_Id { get; set; }
        [Display(Name = "Puntos por Mejora")]
        public int Impr_Points { get; set; }

        [Display(Name = "Propuesta")]
        [AllowHtml]
        public string Impr_Comment { get; set; }

        [Display(Name = "Resultado Esperado")]
        [AllowHtml]
        public string Impr_Comment2 { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string Impr_Name { get; set; }
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

    }
    public class UserExchangeViewModel : BaseViewModel
    {
        public List<Exchange> ListExchange { get; set; }
        public List<Prize> ListPrize { get; set; }
        public List<Point> ListPoint { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public int Exch_Id { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Exch_date { get; set; }
        [Display(Name = "Fecha de Aprobación")]
        public DateTime? Exch_Finishdate { get; set; }


        //point of user
        public int TotalPointUser { get; set; }
        public int TotalPointComent { get; set; }


    }

    public class UserTestViewModel : BaseViewModel
    {
        public bool acces { get; set; }
        public string UserLog { get; set; }
        public int pointsObtained { get; set; }

        public int Atte_id { get; set; }
        public IEnumerable<SelectListItem> AnswerPairing { get; set; }
        //topic
        public TopicsCourse topic { get; set; }
        public Module module { get; set; }
        public List<TopicsCourse> ListTopic { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public List<Module> ListModule { get; set; }
        public int ToCo_Id { get; set; }
        // list questions
        public List<answerpairing> Listrequestpairing { get; set; }
        public List<GeneralQuestions> Listgeneralquestion { get; set; }
        public List<openquestion> lismopenquestion { get; set; }
        public List<optionmultiple> listmodeloptionmultiple { get; set; }
        public List<pairing> listmodelpairing { get; set; }
        public List<trueorfalse> listmodeltrueorfalse { get; set; }
        //bank question
        public List<BankQuestion> ListBankQuestion { get; set; }
        public BankQuestion BankQuestion { get; set; }
        public int BaQu_Id { get; set; }
        [Display(Name = "Nombre Prueba")]
        public string BaQu_Name { get; set; }
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime BaQu_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime BaQu_FinishDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string BaQu_Content { get; set; }
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
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        //Answer Option Multiple
        public List<AnswerOptionMultiple> ListAnswerOptionMultiple { get; set; }
        public AnswerOptionMultiple AnswerOptionMultiple { get; set; }
        public int AnOp_Id { get; set; }
        [Display(Name = "Respuesta")]
        public string AnOp_OptionAnswer { get; set; }
        [Display(Name = "Tipo")]
        public OPTIONANSWER AnOp_TrueAnswer { get; set; }
        //Pairing
        public List<Pairing> ListPairing { get; set; }
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

    }

    public class GeneralQuestions : BaseViewModel
    {
        public TYPEQUESTIONS TypeQuestion { get; set; }
        //Question Option Multiple
        public int BaQu_Id { get; set; }
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int AnOp_Id { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        [AllowHtml]
        public string OpMult_Content { get; set; }

        public List<AnswerOptionMultiple> listanswerOM { get; set; }
        //Question Pairing
        public int Pair_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string Pair_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int Pair_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string Pair_Description { get; set; }
        public int AnPa_Id { get; set; }
        public List<AnswerPairing> listanswerpairing { get; set; }
        public IEnumerable<SelectListItem> AnswerPairing { get; set; }
        //Question True or False
        public int TrFa_Id { get; set; }
        [Display(Name = " Pregunta")]
        public string TrFa_Question { get; set; }
        [Display(Name = "Descripción")]
        public string TrFa_Description { get; set; }
        [AllowHtml]
        public string TrFa_Content { get; set; }

        [Display(Name = "Respuesta Falsa")]
        public OPTIONANSWER TrFa_Answer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public string TrFa_TrueAnswer { get; set; }
        [Display(Name = "Puntaje")]
        public string TrFa_Score { get; set; }
        [Display(Name = "Estado Pregunta")]
        public OPTIONANSWER TrFa_State { get; set; }

    }

    public class answerpairing : BaseViewModel
    {
        public int Pair_Id { get; set; }
        public int AnPa_IdOption { get; set; }
        public int AnPa_IdRequest { get; set; }
        [Display(Name = "Preguntas")]
        public string AnPa_OptionsQuestion { get; set; }
        [Display(Name = "Respuestas")]
        public string AnPa_OptionAnswer { get; set; }

    }
    public class openquestion : BaseViewModel
    {
        public int OpQu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpQu_Question { get; set; }
        [Display(Name = "Respuesta")]
        public string OpQu_Answer { get; set; }
        [Display(Name = "Puntaje")]
        public int OpQu_Score { get; set; }
        public int BaQu_Id { get; set; }
        public int AnOQ_Id { get; set; }
        [Display(Name = "Respuesta")]
        public string AnOQ_Answer { get; set; }

    }



    public class optionmultiple : BaseViewModel
    {
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int AnOp_Id { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        [AllowHtml]
        public string OpMult_Content { get; set; }
        //Answer Option Multiple
        public int BaQu_Id { get; set; }
        public List<AnswerOptionMultiple> listanswer { get; set; }


    }

    public class pairing : BaseViewModel
    {
        public int Pair_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string Pair_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int Pair_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string Pair_Description { get; set; }
        public int BaQu_Id { get; set; }
        public int AnPa_Id { get; set; }
        public List<AnswerPairing> listanswerpairing { get; set; }
        public IEnumerable<SelectListItem> AnswerPairing { get; set; }


    }
    public class trueorfalse : BaseViewModel
    {

        public int BaQu_Id { get; set; }
        public int TrFa_Id { get; set; }
        [Display(Name = " Pregunta")]
        public string TrFa_Question { get; set; }
        [Display(Name = "Descripción")]
        [AllowHtml]
        public string TrFa_Content { get; set; }
        public string TrFa_Description { get; set; }
        [Display(Name = "Respuesta Falsa")]
        public OPTIONANSWER TrFa_Answer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public string TrFa_TrueAnswer { get; set; }
        [Display(Name = "Puntaje")]
        public string TrFa_Score { get; set; }
        [Display(Name = "Estado Pregunta")]
        public OPTIONANSWER TrFa_State { get; set; }

    }


    // section de Articles and Edtions 
    public class UserInformationArticles : BaseViewModel
    {
        public string baseUrl { get; set; }
        public string myUser { get; set; }
        public Article ViewArticle { get; set; }
        public List<Comments> comments { get; set; }
        public int arti_Id { get; set; }

        //Section of comments
        [Display(Name = "Titulo")]
        public string comm_Title { get; set; }

        [Display(Name = "Descripción")]
        [AllowHtml]
        public string comm_Description { get; set; }

        public Comments commentToUpdate { get; set; }

    }

    public class UserCertificado : BaseViewModel
    {

        public ApplicationUser user { get; set; }
        public Certification certificado { get; set; }

    }
}