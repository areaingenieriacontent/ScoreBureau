using PagedList;
using SCORM1.Enum;
using SCORM1.Models.Engagement;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class AdminPointManagerCategory : BaseViewModel
    {
         //Lista de gestión de puntos
        public List<PointManagerCategory> PointManagerCategory { get; set; }
        //Variable utilizada para asignar una instancia de una gestión de puntos
        public PointManagerCategory PointComments { get; set; }
        //Variable utilizada para asignar el id de una gestión de puntos
        public int PoMaCa_Id { get; set; }
        //Variable utilizada para asignar los puntos de comentarios del periódico
        [Display(Name = "Comentarios del Periodico")]
        //[Required(ErrorMessage = " Debe ingresar cantidad puntos ")]
        public int PoMaCa_Periodical { get; set; }
        //Variable utilizada para asignar los puntos de las buenas prácticas
        [Display(Name = "Evidencias ")]
        //[Required(ErrorMessage = " Debe ingresar cantidad puntos ")]
        public int PoMaCa_course { get; set; }
        //Variable utilizada para asignar los puntos de las mejoras
        [Display(Name ="Foro")]
        //[Required(ErrorMessage = " Debe ingresar cantidad puntos ")]
        public int PoMaCa_Improvements { get; set; }
        //Variable utilizada para asignar una gestión de puntos por su nombre
        public string searchPointManagerCategory { get; set; }
        //Lista de premios
        public List<Prize> ListPrize { get; internal set; }
        //Variable utilizada para asignar el nombre del premio
        public string Priz_Name { get; internal set; }
        //Variable utilizada para asignar la descripción del premio
        public string Priz_Description { get; internal set; }
        //Variable utilizada para asignar los puntos de un premio
        public int Priz_RequiredPoints { get; internal set; }
        //Variable utilizada para asignar la cantidad de premios
        public int Priz_Quantity { get; internal set; }
        //Variable utilizada para asignar el estado del premio
        public PRIZESTATE Priz_Stateprize { get; internal set; }
        //Variable utilizada para asignar la fecha de registro del premio
        public DateTime Priz_Date { get; internal set; }
        //Variable utilizada para asignar la instancia de una compañia
        public Company Company { get; internal set; }
    }
    public class pointuser : BaseViewModel
    {
        //Listado de usuarios de una compañia
        public IPagedList<ApplicationUser> UserEnrolllment { get; set; }
        //Listado de usuarios de una compañia
        public List<ApplicationUser> listuser { get; set; }
        //Variable utilizada para asignar la busqueda de un usuario por su nombre
        public string SearchUser { get; set; }
        //Variable utilizada para asignar la descripción de la asignación de los puntos
        [Required(ErrorMessage = "Debe asignar una descripción")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        //Variable utilizada para asignar la instancia de un usuario
        public ApplicationUser user { get; set; }
        //Variable utilizada para asignar el id de un usuario
        public string User_Id { get; set; }
        //Variable utilizada para asignar los puntos que se le van a asignar a un usuario
        [Required(ErrorMessage = "Debe asignar puntos")]
        [Display(Name = "Puntos")]
        public int puntos { get; set; }
        //Lista desplegable de las categorias de puntos por compañia
        public IEnumerable<SelectListItem> Categorias { get; set; }
        //Variable utilizada para asignar el id de la categoría de puntos 
        [Display(Name = "Categoría")]
        public int Cate_Id { get; set; }
    }
}


