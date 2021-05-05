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
    public class MultipleOptionsSurveyUser
    {
        [Key]
        [Column(Order=0)]
        [ForeignKey("UserSurveyResponse")]
        public int usr_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("MultipleOptionsSurveyAnswer")]
        public int mosa_id { get; set; }

        public virtual UserSurveyResponse UserSurveyResponse { get; set; }
        public virtual MultipleOptionsSurveyAnswer MultipleOptionsSurveyAnswer { get; set; }
    }
}