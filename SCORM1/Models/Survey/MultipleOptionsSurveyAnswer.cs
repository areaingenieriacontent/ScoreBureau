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
    public class MultipleOptionsSurveyAnswer
    {
        [Key]
        public int mosa_id { get; set; }
        [ForeignKey("MultipleOptionsSurveyQuestion")]
        public int mosq_id { get; set; }
        [AllowHtml]
        public string content { get; set; }
        public int correct_answer { get; set; }
        public virtual MultipleOptionsSurveyQuestion MultipleOptionsSurveyQuestion { get; set; }
    }
}