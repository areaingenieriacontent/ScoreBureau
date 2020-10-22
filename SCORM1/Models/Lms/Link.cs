using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Link
    {
        [Key]
        public int Link_Id { get; set; }
        [Display(Name = "Descripción")]
        public string Link_Description { get; set; }
        [Display(Name = "Requerido para Evaluación")]
        public REQUIREDEVALUATION Link_RequiredEvaluation { get; set; }


        [ForeignKey("TopicsCourse")]
        public int ToCo_id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }
    }
}