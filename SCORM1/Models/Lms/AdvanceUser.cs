using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class AdvanceUser
    {
        [Key]
        public int AdUs_Id { get; set; }
        [Display(Name = "Puntaje Hasta el Momento")]
        public double AdUs_ScoreObtained { get; set; }
        [Display(Name = "Fecha Presentación")]
        public DateTime AdUs_PresentDate { get; set; }


        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("TopicsCourse")]
        public int ToCo_id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }

        public virtual ICollection<AdvanceCourse> AdvanceCourse { get; set; }
    }
}