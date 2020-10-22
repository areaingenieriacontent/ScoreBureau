using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Enum;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCORM1.Models.SCORM1;

namespace SCORM1.Models.MeasuringSystem
{
    public class ContentQuestionViewModels : BaseViewModel
    {
        public List<Question> Questions { get; set; }
        public IEnumerable<SelectListItem> Proficiencies { get; set; }
        public int QuestionId { get; set; }
        [Required(ErrorMessage = "La pregunta debe tener una Descripcion")]
        [Display(Name = "Descripcion de la Pregunta")]
        public string QuestionDescription { get; set; }
        [Required(ErrorMessage = "La pregunta debe tener una Competencia")]
        [Display(Name = "Competencia")]
        public int QuestionProficiencyId { get; set; }
        public QUESTION_TYPE QuestionType { get; set; }
        [Display(Name = "Buscar Pregunta")]
        public string SearchQuestion { get; set; }
    }

    public class ContentProficiencyViewModel : BaseViewModel
    {
        public List<Proficiency> Proficiencies { get; set; }
        [Required(ErrorMessage = "La Competencia debe tener un nombre.")]
        [Display(Name = "Nombre de la Competencia")]
        public string ProficiencyDescription { get; set; }
        [Display(Name = "Buscar Competencia")]
        public string SearchProficiency { get; set; }
        public int ProficiencyId { get; set; }
    }

    public class AssignedQuestionData
    {
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public bool Assigned { get; set; }
    }

    public class ContentTestViewModel : BaseViewModel
    {
        public List<Test> Tests { get; set; }
        public List<Question> Questions { get; set; }
        public int TestId { get; set; }
        [Required(ErrorMessage = "La Prueba debe tener una Descripción.")]
        [Display(Name = "Descripcion de la Prueba")]
        public string TestDescription { get; set; }
        [Required(ErrorMessage = "La prueba debe tener un tipo.")]
        [Display(Name = "Tipo de la Prueba")]
        public EVALUATE_TO EvaluateTo { get; set; }
        public List<Question> TestQuestions { get; set; }
        [Display(Name = "Buscar Prueba")]
        public string SearchTest { get; set; }
    }

    public class ContentPlansViewModel : BaseViewModel
    {
        public List<Plan> Plans { get; set; }
        public int PlanId { get; set; }
        [Required(ErrorMessage = "El Plan debe tener una Descripción.")]
        [Display(Name = "Descripción del Plan")]
        public string PlanDescription { get; set; }
        [Required(ErrorMessage = "El Plan debe tener un puntaje minimo.")]
        [Display(Name = "Puntaje Minimo del Plan")]
        public int PlanMinScore { get; set; }
        [Required(ErrorMessage = "El Plan debe tener una Puntaje Maximo.")]
        [Display(Name = "Puntaje Maximo del Plan")]
        public int PlanMaxScore { get; set; }
        [Required(ErrorMessage = "El Plan debe tener un objetivo.")]
        [Display(Name = "Objetivo del Plan")]
        public PLAN_TO PlanTo { get; set; }
        public int PlanResourceId;
        public Resource PlanResource { get; set; }
        [Display(Name = "Buscar Plan")]
        public string SearchPlan { get; set; }
        public IEnumerable<SelectListItem> Proficiencies { get; set; }
        public int PlanProficiencyId { get; set; }
    }

    public class ContentUserViewModel : BaseViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        [Display(Name = "Buscar Usuario")]
        public string SearchUser { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "El Usuario debe tener un Nombre de Usuario.")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }
        [Required(ErrorMessage = "El Usuario debe tener una Contraseña.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "El Usuario debe tener una Rol.")]
        [Display(Name = "Rol")]
        public ROLES Role { get; set; }
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Nombres")]
        public string FirstName { get; set; }
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }
        [Display(Name = "Documento")]
        public string Document { get; set; }
        public int? CompanyId { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; }
    }

    public class ContentCompaniesViewModel : BaseViewModel
    {
        public List<Company> Companies { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Nit de la Compañia")]
        [Required(ErrorMessage = "La Compañia Debe tener un NIT")]
        public string CompanyNit { get; set; }
        [Display(Name = "Tipo de Compañia")]
        [Required(ErrorMessage = "La Compañia Debe Pertenecer a un Tipo")]
        public COMPANY_TYPE CompanyType { get; set; }
        [Display(Name = "Sector de la Compañia")]
        [Required(ErrorMessage = "La Compañia Debe Pertenecer a un Sector")]
        public COMPANY_SECTOR CompanySector { get; set; }
        [Required(ErrorMessage = "La Compañia debe tener un nombre.")]
        [Display(Name = "Nombre de la Compañia")]
        public string CompanyName { get; set; }
        [Display(Name = "Buscar Compañia")]
        public string SearchCompany { get; set; }
    }
}