using System.Collections.Generic;
using SCORM1.Enum;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCORM1.Models.SCORM1;
using PagedList;
using System.Data;

namespace SCORM1.Models.ViewModel
{
    public class UserAndMassiveManagementViewModel : BaseViewModel
    {
        public IPagedList<ApplicationUser> UserOfCompany { get; set; }

        //Create User
        public string User_Id { get; set; }

        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "Se Debe Contar con un Nick de Usuario")]
        public string UserName { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Se Debe Contar con un Apellido")]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Se Debe Contar con un Rol de Usuario")]
        public ADMIN_CREATE_ROLES Role { get; set; }

        [Display(Name = "Estado Usuario")]
        public STATEUSER StateUser { get; set; }

        [Display(Name = "Pais")]
        public COUNTRY Country { get; set; }

        [Display(Name = "Documento")]
        public string Document { get; set; }

        [Display(Name = "Cargo")]
        public int? PositionId { get; set; }

        [Display(Name = "Area")]
        public int? AreaId { get; set; }

        [Display(Name = "Ciudad")]
        public int? CityId { get; set; }

        [Display(Name = "Ubicación")]
        public int? LocationId { get; set; }

        [Display(Name = "Activo")]
        public ACTIVEUSERTOENTER enable { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> AreasOfTheCompany { get; set; }
        public IEnumerable<SelectListItem> PositionTheCompany { get; set; }
        public IEnumerable<SelectListItem> CityOfTheCompany { get; set; }
        public IEnumerable<SelectListItem> LocationOfTheCompany { get; set; }

        //Create the Areas
        [Display(Name = "Area")]
        public string AreaName { get; set; }

        //Create the Locations
        [Display(Name = "Ubicación")]
        public string LocationName { get; set; }

        //Create the Citys
        [Display(Name = "Ciudad")]
        public string CityName { get; set; }

        //Create the Positions
        [Display(Name = "Cargo")]
        public string PositionName { get; set; }
    }

    public class
         comoustdlollame
    {
        public DataTable data { get; set; }
        public string Logo { get; set; }
        public SESION Sesion { get; set; }
    }

    public class LogoUserUpdate
    {
        public SESION Sesion { get; set; }
        public DataTable data { get; set; }
        public string Logo { get; set; }
        public List<UserUpdate> usuarios_actualizados { get; set; }
        public List<UserNew> usuarios_nuevos { get; set; }
    }
    public class UserUpdate
    {
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string document { get; set; }
        public int codigo_area { get; set; }
        public string área { get; set; }
        public int codigo_ubicación { get; set; }
        public string ubicación { get; set; }
        public int codigo_cargo { get; set; }
        public string cargo { get; set; }
        public int codigo_ciudad { get; set; }
        public string ciudad { get; set; }

    }
    public class UserNew
    {
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string document { get; set; }
        public int codigo_area { get; set; }
        public string área { get; set; }
        public int codigo_ubicación { get; set; }
        public string ubicación { get; set; }
        public int codigo_cargo { get; set; }
        public string cargo { get; set; }
        public int codigo_ciudad { get; set; }
        public string ciudad { get; set; }

    }

}