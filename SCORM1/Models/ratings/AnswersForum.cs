using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ratings
{
    public class AnswersForum
    {
        [Key]
        public int AnFo_Id { get; set; }
        [Display(Name = "Nombre")]
        public string AnFo_Name { get; set; }
        [Display(Name = "Descripción")]
        public string AnFo_Description { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime AnFo_InitDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string AnFo_Content { get; set; }
        [Display(Name = "Recurso")]
        public string AnFo_Resource { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("ResourceForum")]
        public int ReFo_Id { get; set; }
        public virtual ResourceForum ResourceForum { get; set; }

    }
}