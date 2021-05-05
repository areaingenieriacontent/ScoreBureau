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
    public class TrueFalseSurveyQuestion
    {
        [Key]
        public int tfsq_id { get; set; }
        [ForeignKey("SurveyQuestionBank")]
        public int bank_id { get; set; }
        public string title { get; set; }
        [AllowHtml]
        public string content { get; set; }
        public virtual SurveyQuestionBank SurveyQuestionBank { get; set; }
    }
}