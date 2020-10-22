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
    public class AdminPointViewModel
    {
    }
    public class AdminProfileViewModel : BaseViewModel
    {
        //Variable utilizada para asignar los terminos y condiciones del usuario
        [Display(Name = "Autorizo el tratamiento de datos y acepto los términos y condiciones de la plataforma.")]
        public bool termsandconditions { get; set; }
        //Variable utilizada para asignar la autorización de los videos
        public bool videos { get; set; }
        //Variable utilizada para asignar la instancia de un usuario
        public ApplicationUser user { get; set; }
      
    }
    public class AdminPointCategory : BaseViewModel
    {
        //Lista de tipos de puntos creados por compañia
        public List<TypePoint> listcategory { get; set; }
        //Variable utiliza para buscar una categoria de puntos por nombre
        public string SearchCate { get; set; }
        //Variable utilizada para asignar el id de un tipo de categoría
        public int TyPo_Id { get; set; }
        //Variable utilizada para asignar la descripción de una categoría
        [Display(Name = "Descripción")]
        public string TyPo_Description { get; set; }
        //Variable utilizada para asignar el indicador de una categoría 
        [Display(Name = "Indicador")]
        public TYPEPOINTS Poin_TypePoints { get; set; }
    }
    public class AdminPointMassive : BaseViewModel
    {
        //Variable utilizada para asignar los puntos adicionales
        [Required(ErrorMessage = "Debe asignar puntos")]
        [Display(Name = "Puntos")]
        public int puntos { get; set; }
        //Lista desplegable de las categorías de puntos por compañias
        public IEnumerable<SelectListItem> Categorias { get; set; }
        //Variable utilizada para asignar el id de una categoría 
        [Display(Name = "Categoría")]
        public int Cate_Id { get; set; }
        //Lista desplegable de las áreas disponibles
        public IEnumerable<SelectListItem> Areas { get; set; }
        //Variable utilizada para asignar el id de un área
        [Display(Name = "Área")]
        public int Area_Id { get; set; }
        //Lista desplegable de los cargos
        public IEnumerable<SelectListItem> Cargos { get; set; }
        //Variable utilizada para asignar el id de un cargo
        [Display(Name = "Cargo")]
        public int Posi_Id { get; set; }
        //Lista desplegable de las ciudades
        public IEnumerable<SelectListItem> Ciudades { get; set; }
        //Variable utilizada para asignar el id de la ciudad
        [Display(Name = "Ciudad")]
        public int City_Id { get; set; }
        //Lista desplegable de las ubicaciones
        public IEnumerable<SelectListItem> Ubicación { get; set; }
        //Variable utilizada para asignar el id de la ubicación
        [Display(Name = "Ubicación")]
        public int Loca_Id { get; set; }
        //Variable utilizada para asignar una descripción
        [Display(Name = "Descripción")]
        public string description { get; set;}
    }

}