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

namespace SCORM1.Models.ViewModel
{
    public class AdminInformationViewModel : BaseViewModel
    {
        public Banner Banner { get; set; }
        public Changeinterface Changeinterface { get; set; }
        public List<Module> Module { get; set; }
        public Edition Edition { get; set; }
        public ApplicationUser User { get; set; }


    }

    public class AdminInformationEditionViewModel : BaseViewModel
    {
        public List<Edition> Editions { get; set; }

        [Display(Name = "Nombre Edición")]
        public string name { get; set; }

        [Display(Name = "Puntaje")]
        public int ValuePoints { get; set; }

        [Display(Name = "Estado edición")]
        public EDITIONSTATE state { get; set; }

        [Display(Name = "Descripción ")]
        public string descriptions { get; set; }

        [Display(Name = "Imagen")]
        public byte[] image { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.Date)]
        public DateTime InintDate { get; set; }

        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }

        public int editionId { get; set; }

        public Edition editionsForUpdate { get; set; }

    }

    public class AdminInformationProfileViewModel : BaseViewModel
    {
        [Display(Name = "Autorizo el tratamiento de datos y acepto los términos y condiciones de la plataforma.")]
        public bool termsandconditions { get; set; }
        public bool videos { get; set; }
        public bool colorsBacgraundTitles { get; set; }
        public Edition EditionCurrentToActive { get; set; }
        public List<Article> ListArticles { get; set; }
        public List<ApplicationUser> listuser { get; set; }
        public ApplicationUser user { get; set; }
        public List<Edition> ListNewspaper { get; set; }
    }
    public class AminInformationLMSViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
    }

    public class AdminInformationEngagementViewModel : BaseViewModel
    {
        public List<Prize> Prizes { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<Point> Point { get; set; }
    }

    public class AdminInformationManagementUser : BaseViewModel
    {
        public ApplicationUser User { get; set; }
    }

    public class AdminInformationReportsViewModel : BaseViewModel
    {
        public List<Edition> Editions { get; set; }
    }
    public class FileManager : BaseViewModel
    {
        public string nameFile { get; set; }
        public string Author { get; set; }
    }

    public class AdminInformationArticles : BaseViewModel
    {
        public string baseUrl { get; set; }
        public Edition editions { get; set; }
        public Article ViewArticle { get; set; }
        public Article ArticleForUpdate { get; set; }
        public List<Section> sectionsList { get; set; }
        public List<Article> articles { get; set; }
        public List<Comments> comments { get; set; }

        [Display(Name = "Nombre del artículo")]
        public string arti_Name { get; set; }

        [Display(Name = "Descripción")]
        public string arti_Description { get; set; }

        [Display(Name = "Contenido")]
        [AllowHtml]
        public string arti_Content { get; set; }

        [Display(Name = "Estado del artículo")]
        public ARTICLESTATE arti_State { get; set; }
        public int arti_Id { get; set; }

        [Display(Name = "Aceptar Comentario")]
        public ARTICLEWITHCOMMENT ArticleWithComment { get; set; }

        //Section de section
        [Display(Name = "Nombre de la sección")]
        public string Sect_Name { get; set; }
        public int SectionId { get; set; }

        //Section of comments
        [Display(Name = "Titulo")]
        public string comm_Title { get; set; }

        [Display(Name = "Descripción")]
        [AllowHtml]
        public string comm_Description { get; set; }

        public Comments commentToUpdate { get; set; }
        [Display(Name = "Realiza tu comentario")]
        [AllowHtml]
        public string commentOfAdmin { get; set; }
    }
}