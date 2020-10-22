using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ratings
{
    public class ResourceForum
    {
        [Key]
        public int ReFo_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReFo_Name { get; set; }
        [Display(Name = "Descripción")]
        public string ReFo_Description { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime ReFo_InitDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReFo_Content { get; set; }
        [Display(Name = "Recurso")]
        public string ReFo_Resource { get; set; }

        [ForeignKey("Job")]
        public int Job_Id { get; set; }
        public virtual Job Job { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<BookRatings> BookRatings { get; set; }
        public virtual ICollection<AnswersForum> AnswersForum { get; set; }
    }
}