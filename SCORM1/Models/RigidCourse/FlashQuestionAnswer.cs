using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class FlashQuestionAnswer
    {
        [Key]
        public int FlashQuestionAnswerId { get; set; }
        [ForeignKey("FlashQuestion")]
        public int FlashQuestionId { get; set; }
        public string Content { get; set; }
        public int CorrectAnswer { get; set; }

        public virtual FlashQuestion FlashQuestion { get; set; }
    }
}