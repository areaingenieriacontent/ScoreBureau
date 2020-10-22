using PagedList;
using SCORM1.Enum;
using SCORM1.Models.Lms;
using SCORM1.Models.ratings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class RatingAdminViewModel
    {
    }
    public class RatingAdminViewJobsT : BaseViewModel
    {
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar una instancia de un recurso
        public Job JOBS { get; set; }
        //Variable utilizada para asignar el nombre de una imagen
        public string Image { get; set; }

    }
    public class RatingUserViewJobsT : BaseViewModel
    {
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar una instancia de un recurso
        public Job JOBS { get; set; }
        //Variable utilizada para asignar el nombre de una imagen
        public string Image { get; set; }
        //Variable utilizada para asignar el nombre de una imagen
        public string ImageR { get; set; }
        //Lista de tareas
        public List<ResourceJobs>ListJobsT { get; set; }
        //Variable utilizada para asignar el id de un recurso
        public int ReJo_Id { get; set; }
        //Variable utilizada para asignar el nombre de un recurso
        [Display(Name = "Nombre")]
        public string ReJo_Name { get; set; }
        //Variable utilizada para asignar la descripción de un recurso
        [Display(Name = "Descripción")]
        public string ReJo_Description { get; set; }
        //Variable utilizada para asignar la fecha de creacion de un recurso
        [Display(Name = "Fecha de creación")]
        public DateTime ReJo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de un recurso
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReJo_Content { get; set; }
        //Variable utilizada para asignar el nombre del archivo
        [Display(Name = "Recurso")]
        public string ReJo_Resource { get; set; }
        //Variable utilizada para asignar la extensión
        public string ReJo_Ext { get; set; }

    }
    public class RatingUserViewJobsF : BaseViewModel
    {
        //Variable utilizada para asignar la instancia de un usuario
        public ApplicationUser User { get; set; }
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar de un recurso
        public Job JOBS { get; set; }
        //Variable utilizada para asignar la imagen de un recurso
        public string Image { get; set; }
        //Variable utilizada para asignar la imagen de un recurso
        public string ImageR { get; set; }
        //Lista de foros
        public List<ResourceForum> ListJobsF { get; set; }
        //Variable utilizada para asignar el id de un recurso
        public int ReFo_Id { get; set; }
        //Variable utilizada para asignar el nombre de un recurso
        [Display(Name = "Nombre")]
        public string ReFo_Name { get; set; }
        //Variable utilizada para asignar la descripción de un recurso
        [Display(Name = "Descripción")]
        public string ReFo_Description { get; set; }
        //Variable utilizada para asignar la fecha de creación de un recurso
        [Display(Name = "Fecha de creación")]
        public DateTime ReFo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de un recurso
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReFo_Content { get; set; }
        //Variable utilizada para asignar el archivo
        [Display(Name = "Recurso")]
        public string ReFo_Resource { get; set; }
        //Lista de recursos
        public List<Job>listjobs { get; set; }
        //Lista de libro de calificaciones
        public List<BookRatings> listbook { get; set; }

    }
    public class RatingForum : BaseViewModel
    {
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar una instancia de un recurso
        public Job JOBS { get; set; }
        //Variable utilizada para asignar la imagen de un recurso
        public string Image { get; set; }
        //Variable utilizada para asignar el id de una respuesta de foro
        public int AnFo_Id { get; set; }
        //Variable utilizada para asignar el nombre de una respuesta de foro
        [Display(Name = "Nombre")]
        public string AnFo_Name { get; set; }
        //Variable utilizada para asignar la descripción de una respuesta de foro
        [Display(Name = "Descripción")]
        public string AnFo_Description { get; set; }
        //Variable utilizada para asignar la fecha de una respuesta de foro
        [Display(Name = "Fecha de creación")]
        public DateTime AnFo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de una respuesta de foro
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string AnFo_Content { get; set; }
        //Variable utilizada para asignar el archivo de una respuesta de foro
        [Display(Name = "Recurso")]
        public string AnFo_Resource { get; set; }
        //Variable utilizada para asignar el id de un recurso
        public int ReFo_Id { get; set; }
        //Variable utilizada para asignar el nombre de un recurso
        [Display(Name = "Nombre")]
        public string ReFo_Name { get; set; }
        //Variable utilizada para asignar la descripción de un recurso
        [Display(Name = "Descripción")]
        public string ReFo_Description { get; set; }
        //Variable utilizada para asignar la fecha de creación de un recurso
        [Display(Name = "Fecha de creación")]
        public DateTime ReFo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de un recurso
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReFo_Content { get; set; }
        //Variable utilizada para asignar el archivo de un recurso
        [Display(Name = "Recurso")]
        public string ReFo_Resource { get; set; }
    }
    public class AnswerFormView : BaseViewModel
    {
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar la instancia de un recurso
        public Job JOBS { get; set; }
        //Variable utilizada para asignar la instancia de un foro
        public ResourceForum Refo { get; set; }
        //Variable utilizada para asignar la imagen de un recurso
        public string Image { get; set; }
        //Variable utilizada para asignar el id de una respuesta de foro
        public int AnFo_Id { get; set; }
        //Variable utilizada para asignar el nombre de una respuesta de foro
        [Display(Name = "Nombre")]
        public string AnFo_Name { get; set; }
        //Variable utilizada para asignar la descripción de una respuesta de foro
        [Display(Name = "Descripción")]
        public string AnFo_Description { get; set; }
        //Variable utilizada para asignar la fecha de creación de una respuesta de foro
        [Display(Name = "Fecha de creación")]
        public DateTime AnFo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de una respuesta de foro
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string AnFo_Content { get; set; }
        //Variable utilizada para asignar el nombre de un recurso
        [Display(Name = "Recurso")]
        public string AnFo_Resource { get; set; }
        //Variable utilizada para asignar de un recurso
        public int ReFo_Id { get; set; }
        //Variable utilizada para asignar el nombre de un recurso
        [Display(Name = "Nombre")]
        public string ReFo_Name { get; set; }
        //Variable utilizada para asignar la descripción de un recurso
        [Display(Name = "Descripción")]
        public string ReFo_Description { get; set; }
        //Variable utilizada para asignar la fecha de creación de un recurso
        [Display(Name = "Fecha de creación")]
        public DateTime ReFo_InitDate { get; set; }
        //Variable utilizada para asignar el contenido de un recurso
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReFo_Content { get; set; }
        //Variable utilizada para asignar el nombre del archivo
        [Display(Name = "Recurso")]
        public string ReFo_Resource { get; set; }
        //Variable utilizada para asignar la instancia de un usuario
        public ApplicationUser user { get; set; }
    }
    public class ScoreF:BaseViewModel
    {
        //Variable utilizada para asignar la instancia de una actividad
        public Job JOBS { get; set; }
        //lista de libro de calificaciones
        public IPagedList<BookRatings> ListBookRatings { get; set; }
        //Variable utilizada para asignar para buscar a un usuario por su nombre
        public string SearchUser { get; set; }
        //Variable utilizada para asignar el id de un libro de calificación
        public int BoRa_Id { get; set; }
        //Variable utilizada para asignar la fecha de creación de un libro de calificación
        [Display(Name = "Fecha")]
        public DateTime BoRa_InitDate { get; set; }
        //Variable utilizada para asignar el estado de un libro de calificación
        [Display(Name = "Estado")]
        public STATESCORE BoRa_StateScore { get; set; }
        //Variable utilizada para asignar la calificación
        [Display(Name = "Calificación")]
        [Range(0.0,5.0, ErrorMessage = "Los rangos deben estar entre {1} y {2}")]
        [Required(ErrorMessage = "*")]
        public double BoRa_Score { get; set; }
        //Variable utilizada para asignar los puntos de un libro de calificaciones
        [Display(Name = "Puntos")]
        public int BoRa_Point { get; set; }
        //Variable utilizada para asignar la retroalimentación de un libro de calificaciones
        [Display(Name = "Retroalimentación")]
        public string BoRa_Description { get; set; }
    }
    public class ScoreT : BaseViewModel
    {
        //Variable utilizada para asignar una instancia de una actividad
        public Job JOBS { get; set; }
        //Lista libro de calificaciones
        public IPagedList<BookRatings> ListBookRatings { get; set; }
        //Variable utilizada para asignar un nombre de usuario para buscar
        public string SearchUser { get; set; }
        //Variable utilizada para asignar el id de un libro de calificaciones
        public int BoRa_Id { get; set; }
        //Variable utilizada para asignar la fecha de creación de un libro de calificaciones
        [Display(Name = "Fecha")]
        public DateTime BoRa_InitDate { get; set; }
        //Variable utilizada para asignar el estado de un libro de calificaciones
        [Display(Name = "Estado")]
        public STATESCORE BoRa_StateScore { get; set; }
        //Variable utilizada para asignar la calificación de un libro de calificaciones
        [Display(Name = "Calificación")]
        [Range(0, 5, ErrorMessage = "Los rangos deben estar entre {1} y {2}")]
        [Required(ErrorMessage = "*")]
        public double BoRa_Score { get; set; }
        //Variable utilizada para asignar los puntos de un libro de calificaciones
        [Display(Name = "Puntos")]
        public int BoRa_Point { get; set; }
        //Variable utilizada para asignar la retroalimentación de un libro de calificaciones
        [Display(Name = "Retroalimentación")]
        public string BoRa_Description { get; set; }
    }
    public class RatingUserViewJobs : BaseViewModel
    {
        //Variable utilizada para asignar la ruta de la plataforma
        public string baseUrl { get; set; }
        //Variable utilizada para asignar la instancia de un libro de calificaciones
        public Job JOBS { get; set; }
        //Lista de actividades
        public List<Job> listjobs { get; set; }
        public List<ResourceForum> listResourceForum { get; set; }

        //Lista de libro de calificaciones
        public List<BookRatings> listbook { get; set; }
        //Lista de resultados de usuario
        public List<resultado> listresultado { get; set; }
        //List of test
        public List<resultado> listTestUs { get; set; }
        public List<Enrollment> listenrrolment { get; set; }
        public string UserLog { get; set; }
        public List <ApplicationUser> ApplicationUser { get; set; }


    }
    public class resultado
    {
        //Variable utilizada para asignar el nombre de la actividad
        public string Nombre { get; set; }
        //Variable utilizada para asignar la fecha de creación del libro de calificaciones
        public DateTime? BoRa_InitDate { get; set; }
        //Variable utilizada para asignar el estado del libro de calificaciones
        [Display(Name = "Estado")]
        public STATESCORE BoRa_StateScore { get; set; }
        //Variable utilizada para asignar la calificación del usuario
        [Display(Name = "Calificación")]
        public double BoRa_Score { get; set; }
        //Variable utilizada para asignar la retroalimentación de la calificación
        [Display(Name = "Retroalimentación")]
        public string BoRa_Description { get; set; }

        public int Enro_Id { get; set; }
        
        public Job JOBS { get; set; }
     



    }
}