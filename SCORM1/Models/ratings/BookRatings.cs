using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ratings
{
    public class BookRatings
    {
        [Key]
        public int BoRa_Id { get; set; }
        [Display(Name = "Fecha")]
        public DateTime? BoRa_InitDate { get; set; }
        [Display(Name = "Estado")]
        public STATESCORE BoRa_StateScore { get; set; }
        [Display(Name = "Calificación")]
        public double BoRa_Score { get; set; }
        [Display(Name = "Puntos")]
        public int BoRa_Point { get; set; }
        [Display(Name = "Retroalimentación")]
        public string BoRa_Description { get; set; }

        [ForeignKey("ResourceForum")]
        public int? ReFo_Id { get; set; }
        public virtual ResourceForum ResourceForum { get; set; }
        [ForeignKey("ResourceJobs")]
        public int? ReJo_Id { get; set; }
        public virtual ResourceJobs ResourceJobs { get; set; }

    }
}