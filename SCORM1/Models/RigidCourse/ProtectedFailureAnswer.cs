using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.RigidCourse
{
    public class ProtectedFailureAnswer
    {
        [Key]
        public int answerId { get; set; }
        [ForeignKey("ProtectedFailureMultiChoice")]
        public int QuestionId { get; set; }
        [AllowHtml]
        public string AnswerContent { get; set; }
        public bool isCorrectQuestion { get; set; }
        public virtual ProtectedFailureMultiChoice ProtectedFailureMultiChoice { get; set; }
    }
}