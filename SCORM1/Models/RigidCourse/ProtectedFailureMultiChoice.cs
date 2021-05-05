using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class ProtectedFailureMultiChoice
    {
        [Key]
        public int QuestionId { get; set; }

        [ForeignKey("CategoryQuestionBank")]
        [Column(Order = 0)]
        public int Category_Id { get; set; }

        [ForeignKey("CategoryQuestionBank")]
        [Column(Order = 1)]
        public int Modu_Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public virtual CategoryQuestionBank CategoryQuestionBank { get; set; }
    }
}