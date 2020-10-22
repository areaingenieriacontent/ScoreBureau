using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ratings
{
    public class ResourceJobs
    {
        [Key]
        public int ReJo_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReJo_Name { get; set; }
        [Display(Name = "Descripción")]
        public string ReJo_Description { get; set; }       
        [Display(Name = "Fecha de creación")]
        public DateTime ReJo_InitDate { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ReJo_Content { get; set; }
        [Display(Name = "Recurso")]
        public string ReJo_Resource { get; set; }
        public string ReJo_Ext { get; set; }

        [ForeignKey("Job")]
        public int Job_Id { get; set; }
        public virtual Job Job { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<BookRatings> BookRatings { get; set; }
    }
}