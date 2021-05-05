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
    public class SurveyQuestionBank
    {
        [Key]
        public int bank_id { get; set; }
        [ForeignKey("SurveyModule")]
        public int survey_id { get; set; }
        public int questionsToEvaluate { get; set; }
        public virtual SurveyModule SurveyModule { get; set; }
    }
}