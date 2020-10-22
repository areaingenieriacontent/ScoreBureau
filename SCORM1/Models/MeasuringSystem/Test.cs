using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Test
    {
        public Test()
        {
            this.Questions = new HashSet<Question>();
        }
        [Key]
        public int TestId { get; set; }
        [Display(Name = "Descripción")]
        public string TestDescription { get; set; }
        [Display(Name = "Tipo de evaluación")]
        public EVALUATE_TO EvaluateTo { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}