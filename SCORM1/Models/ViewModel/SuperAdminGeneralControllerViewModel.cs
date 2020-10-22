using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Enum;
using SCORM1.Models.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.SCORM1;
using PagedList;

namespace SCORM1.Models.ViewModel
{
    public class SuperAdminGeneralControllerViewModel : BaseViewModel
    {
        public List<Company> ListCompanies { get; set; }

        public Company CompanyToUpdate { get; set; }

        public int CompanyId { get; set; }

        [Display(Name = "Tipo")]
        public COMPANY_TYPE CompanyType { get; set; }

        [Display(Name = "Sector")]
        public COMPANY_SECTOR CompanySector { get; set; }

        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }

        [Display(Name = "Nit")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(12)]
        [Index(IsUnique = true)]
        public string CompanyNit { get; set; }

        [Display(Name = "Juego")]
        public GAME CompanyGame { get; set; }
    }

    public class SuperAdminGeneralManagementUsersAdminOfTheCompanies : BaseViewModel
    {

        public IPagedList<ApplicationUser> UserOfCompany { get; set; }

        //Create User
        public string UserId { get; set; }

        public int companyId { get; set; }

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

        [Display(Name = "Activo")]
        public ACTIVEUSERTOENTER enable { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }

}