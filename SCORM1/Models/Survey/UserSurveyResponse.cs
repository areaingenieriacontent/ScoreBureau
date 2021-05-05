using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using SCORM1.Models.Lms;

namespace SCORM1.Models.Survey
{
    public class UserSurveyResponse
    {
        [Key]
        public int us_id { get; set; }
        public float calification { get; set; }
        [ForeignKey("Enrollment")]
        public int enro_id { get; set; }
        public DateTime survey_initial_time { get; set; }
        public DateTime survey_finish_time { get; set; }
        public virtual Enrollment Enrollment { get; set; }
    }
}