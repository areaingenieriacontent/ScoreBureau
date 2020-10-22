using SCORM1.Models.PageCustomization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class AdminPageCustomizationViewModel:BaseViewModel
    {
        public List<Banner> Banners { get; set; }
        public string baseUrl { get; set; }

    }
    public class AdminPageLogoViewModel:BaseViewModel
    {
        [AllowHtml]
        [Display(Name = "Logo")]
        [Required(ErrorMessage = "Se debe agregar un logo")]
        public string UrlLogo { get; set; }

        public int? companyId { get; set; }


        [Display(Name = "Color de la Barra de navegación")]
        public string navBarColor { get; set; }

        [Display(Name = "Imagen  1")]
        [AllowHtml]
        [Required(ErrorMessage = "Debe seleccionar una imagen")]
        public string UrlImage1 { get; set; }

        [Display(Name = "Imagen  2")]
        [Required(ErrorMessage = "Debe seleccionar una imagen")]
        [AllowHtml]
        public string UrlImage2 { get; set; }

        [Display(Name = "Imagen  3")]
        [Required(ErrorMessage = "Debe seleccionar una imagen")]
        [AllowHtml]
        public string UrlImage3 { get; set; }

        [Display(Name = "Imagen  4")]
        [Required(ErrorMessage = "Debe seleccionar una imagen ")]
        [AllowHtml]
        public string UrlImage4 { get; set; }

        [Display(Name = "Titulo 1")]
        [Required(ErrorMessage = "Se debe contar con un titulo")]
        public string Title1 { get; set; }

        [Display(Name = "Titulo 2")]
        [Required(ErrorMessage = "Se debe contar con un titulo")]
        public string Title2 { get; set; }

        [Display(Name = "Titulo 3")]
        [Required(ErrorMessage = "Se debe contar con un titulo")]
        public string Title3 { get; set; }

        public string baseUrl { get; set; }
    }

    public class AdminPageTermsandConditionsViewModel : BaseViewModel
    {
        [AllowHtml]
        [Display(Name = "Logo")]
        public string UrlLogo { get; set; }
        public int? companyId { get; set; }
        [Display(Name = "Archivo")]
        public string FileTerms { get; set; }
        public string baseUrl { get; set; }
    }
    public class adminPageTitles
    {
        public int? companyId { get; set; }

        [Display(Name = "Titulo 1")]
        [Required(ErrorMessage = "Falta título en 1")]
        public string Title1 { get; set; }
        [Display(Name = "Titulo 2")]
        [Required(ErrorMessage = "Falta título en 2")]
        public string Title2 { get; set; }
        [Display(Name = "Titulo 3")]
        [Required(ErrorMessage = "Falta título en 3")]
        public string Title3 { get; set; }
        [Display(Name = "Color de los Títulos")]
        [Required(ErrorMessage = "Agrega el color de tu preferencia para los títulos")]
        public string colorsTittle { get; set; }

        [Display(Name = "Titulo ")]
        [Required(ErrorMessage = "Agrega el color de tu preferencia al fondo")]
        public string colorsBacgraundTitles { get; set; }
        public string baseUrl { get; set; }

    }
}