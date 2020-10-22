using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Enum;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Chart.Mvc.ComplexChart;
using PagedList;
using SCORM1.Models.SCORM1;

namespace SCORM1.Models.MeasuringSystem
{
    public class AdminUsersViewModels : BaseViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public IEnumerable<SelectListItem> Areas { get; set; }
        [Display(Name = "Area")]
        public int AreaId { get; set; }
        public string SearchUser { get; set; }
        public string UserId { get; set; }       
        [Display(Name = "Nombre de Usuario")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Usuario")]
        public string UserName { get; set; }
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Se Debe Contar con un Contraseña para el  Usuario")]
        [StringLength(255, ErrorMessage = "La Contraseña debe ser mayor a 5 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Se Debe Contar con un Rol de Usuario")]
        [Display(Name = "Rol")]
        public ADMIN_CREATE_ROLES Role { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Nombre")]
        public string  FirstName { get; set; }
        [Display(Name = "Apellido")]
        public string LastName { get; set; }
        [Display(Name = "Documento")]
        public string Document { get; set; }
        [Display(Name = "CompanyId")]
        public int CompanyId { get; set; }
    }

    public class AdminResultsViewModel : BaseViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public Result FirstScore { get; set; }
        public List<Area> Areas { get; set; }
        public BarChart ResultChart { get; set; }
        public LineChart ResultCharCompany { get; set; }
        public string SearchUserArea { get; set; }
    }

    public class AdminPlansViewModel : BaseViewModel
    {
        public List<Plan> Plans { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public string SearchUserPlan { get; set; }
    }

    public class AdminAreaViewModel : BaseViewModel
    {
        public List<Area> Areas { get; set; }
        public List<ApplicationUser> Users { get; set; }
        [Required]
        [Display(Name = "Nombre del Area")]
        public string AreaName { get; set; }
        public int AreaId { get; set; }
        public String AreaSearch { get; set; }
        public String Selected { get; set; }
        public List<ApplicationUser> UsersOfArea { get; set; }
        public List<ApplicationUser> UsersOutArea { get; set; }
    }

    public class 
        AdminRelationsViewModel : BaseViewModel
    {
        public IPagedList<ApplicationUser> Users { get; set; }
        public IPagedList<ApplicationUser> EqualUsers { get; set; }
        public IPagedList<ApplicationUser> SuperiorsUsers { get; set; }
        public IPagedList<ApplicationUser> ClientsUsers { get; set; }
        public IPagedList<ApplicationUser> MyOffice { get; set; }
        public IPagedList<ApplicationUser> UserAddRelations { get; set; }
        public string SearchUser { get; set; }
        public ASINGUSER ASINGUSER { get; set; }
    }

    public class AdminMeasuresViewModel : BaseViewModel
    {
        public List<Test> Tests { get; set; }
        public List<Measure> Measures { get; set; }
        public string SearchMeasures { get; set; }
        public int MeasureId { get; set; }
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime MeasureInitDate { get; set; }
        [Display(Name = "Fecha de Culminación")]
        [DataType(DataType.Date)]
        public DateTime MeasureFinishDate { get; set; }
        public int MeasureCompanyId { get; set; }
        public int MeasureTestId { get; set; }
        public int Selected { get; set; }
    }
    public class ModelReportUserMeasuare : BaseViewModel
    {
        public List<ClassReportUserMeasuare> mode { get; set; }
        public int id { get; set; }
    }
    public class ClassReportUserMeasuare
    {
        public ApplicationUser user { get; set; }

        public List<Score> Score { get; set; }
    }
    public class ClassReportUserMeasuares
    {
        public string Nombre_Completo { get; set; }
        public int SERVICIO_AL_CLIENTE { get; set; }
        public int TRABAJO_EN_EQUIPO { get; set; }
        public int TOMA_DE_DECISIONES { get; set; }
        public int TRABAJO_BAJO_PRESIÓN { get; set; }
        public int ATENCIÓN_AL_DETALLE { get; set; }

    }
}