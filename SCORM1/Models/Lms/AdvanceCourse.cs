using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class AdvanceCourse
    {
        [Key]
        public int AdCo_Id { get; set; }
        [Display(Name = "Puntaje Hasta el Momento")]
        public double AdCo_ScoreObtanied { get; set; }


        [ForeignKey("Enrollment")]
        public int Enro_Id { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("AdvanceUser")]
        public int AdUs_Id { get; set; }
        public virtual AdvanceUser AdvanceUser { get; set; }

       

    }
}