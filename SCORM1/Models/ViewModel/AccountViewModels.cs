using SCORM1.Enum;
using SCORM1.Models.ViewModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class MassiveRegisterMain
    {
        public MassiveRegisterViewModel Massive { get; set; }
        public System.Data.DataTable DataTable { get; set; }
    }

    public class indexMain: AdminPageLogoViewModel
    {
       
    public AdminInformationEditionViewModel AdminInfromation { get; set; }
    public AdminInformationViewModel Admin { get; set; }
    public LoginViewModel Login { get; set; }
    public FormViewModel Form { get; set; }
    public HomeViewModels Home { get; set; }
    public HomeViewModels TermsandConditions { get; set; }
    public SESION sesion { get; set; }
    public int ComunidadActiva { get; set; }


    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel:BaseViewModel
    {
        public string UrlLogo { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Nombre de Usuario")]
        [Display(Name = "Usuario")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    public class FormViewModel : BaseViewModel
    {
        public int Modu_Id { get; set; }
        public string UrlLogo { get; set; }
        [Required(ErrorMessage = "Debe ingresar un comentario")]
        [Display(Name = "Comentario")]
        public string Form_Comment { get; set; }
        [Required(ErrorMessage = "Debe ingresar su Nombre")]
        [Display(Name = "Nombre")]
        public string Form_Name { get; set; }
        [Required(ErrorMessage = "Debe ingresar su Email")]
        [Display(Name = "Email")]
        public string Form_Email { get; set; }
        public IEnumerable<SelectListItem> ListModule { get; set; }
    }

    public class RegisterViewModel
    {
        public string UrlLogo { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} contraseña debe ser de al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Nombre de Usuario")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Usuario")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Se Debe Contar con un Rol de Usuario")]
        [Display(Name = "Rol")]
        public ROLES Role { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre")]
        public string FirstName { get; set; }
        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "Se Debe Contar con un Apellido")]
        public string LastName { get; set; }
        [Display(Name = "Documento")]
        public string Document { get; set; }
        [Display(Name = "Pais")]
        [Required(ErrorMessage = "Se Debe Contar con un Pais")]
        public COUNTRY Country { get; set; }
        [Display(Name = "CompanyId")]
        [Required(ErrorMessage = "Se Debe Contar con una Compañia")]
        public int CompanyId { get; set; }
        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "Se Debe Contar con un Cargo")]
        public int PositionId { get; set; }
        [Display(Name = "Area")]
        [Required(ErrorMessage = "Se Debe Contar con una Area")]
        public int AreaId { get; set; }
        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "Se Debe Contar con una Ciudad")]
        public int CityId { get; set; }
        [Display(Name = "Ubicación")]
        [Required(ErrorMessage = "Se Debe Contar con una Ubicación")]
        public int LocationId { get; set; }
        public string SearchUser { get; set; }
    }

    public class ResetPasswordViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} contraseña debe ser de al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel : BaseViewModel
    {

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        //[Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }
        [Display(Name = "Mail")]
        public string UserMail { get; set; }
        public string UrlLogo { get; set; }
    }

    public class ForgotPasswordConfirmationViewModel
    {

        [Required]
        [Display(Name = "nick")]
        public string user { get; set; }


        [Required]
        [Display(Name = "Codigo")]
        public string Code { get; set; }
        
    }

    public class MassiveRegisterViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name ="Selecciona el archivo Excel")]
        public  HttpPostedFileBase excelUpload { get; set; }
    }

}



