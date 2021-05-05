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
    public class SurveyModule
    {
        [Key]
        public int survey_id { get; set; }
        public string survey_name { get; set; }
        public float treshold { get; set; }
        public float secondTreshold { get; set; }
        [ForeignKey("Module")]
        public int modu_id { get; set; }
        [AllowHtml]
        public string survey_description { get; set; }
        [AllowHtml]
        public string survey_instructions { get; set; }
        public int survey_time_minutes { get; set; }
        public DateTime date_available { get; set; }
        public DateTime date_closed { get; set; }
        public virtual Module Module { get; set; }
    }
}