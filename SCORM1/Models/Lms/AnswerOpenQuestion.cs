using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class AnswerOpenQuestion
    {
        [Key]
        public int AnOQ_Id { get; set; }
        [Display(Name = "Respuesta")]
        public string AnOQ_Answer { get; set; }
       
        [ForeignKey("OpenQuestion")]
        public int OpQu_Id { get; set; }
        public virtual OpenQuestion OpenQuestion { get; set; }
    }
}