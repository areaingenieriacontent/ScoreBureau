using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class FlashQuestion
    {
        [Key]
        public int FlashQuestionId { get; set; }
        [ForeignKey("FlashTest")]
        public int FlashTestId { get; set; }
        public string Enunciado { get; set; }

        public virtual FlashTest FlashTest { get; set; }

        public virtual List<FlashQuestionAnswer> AnswerList { get; set; }
    }
}