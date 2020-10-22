using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class AdvanceLoseUser
    {
        [Key]
        public int AdLoUs_Id { get; set; }
        [Display(Name = "Puntaje Hasta el Momento")]
        public double AdLoUs_ScoreObtained { get; set; }
        [Display(Name = "Fecha Presentación")]
        public DateTime AdLoUs_PresentDate { get; set; }


        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("TopicsCourse")]
        public int ToCo_id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }

        //public virtual ICollection<AdvanceCourse> AdvanceCourse { get; set; }
    }
}