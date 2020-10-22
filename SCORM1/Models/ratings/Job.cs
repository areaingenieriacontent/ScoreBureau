using SCORM1.Enum;
using SCORM1.Models.Lms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ratings
{
    public class Job
    {
        [Key]
        public int Job_Id { get; set; }
        [Display(Name = "Nombre")]
        public string Job_Name { get; set; }
        [Display(Name = "Descripción")]
        public string Job_Description { get; set; }
        [Display(Name = "Recurso")]
        public string Job_Resource { get; set; }
        public string Job_Ext { get; set; }
        [Display(Name = "Tipo Trabajo")]
        public TYPEJOB Job_TypeJob { get; set; }
        [Display(Name = "Estado")]
        public MODULESTATE Job_StateJob { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Job_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        public DateTime Job_FinishDate { get; set; } 
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string Job_Content { get; set; }
        [Display(Name = "Puntos")]
        public int Job_Points { get; set; }
        [Display(Name = "Visible")]
        public FORO Job_Visible { get; set; }

        [ForeignKey("TopicsCourse")]
        public int ToCo_Id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ResourceForum> ResourceForum { get; set; }
        public virtual ICollection<ResourceJobs> ResourceJobs { get; set; }

    }
}