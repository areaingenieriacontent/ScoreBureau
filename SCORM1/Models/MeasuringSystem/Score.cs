using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Score
    {
        [Key]
        public int ScoreId { get; set; }
        [Display(Name = "Valor")]
        public int Value { get; set; }
        [Display(Name = "Competencia")]
        public int ProficiencyId { get; set; }
        [Display(Name = "Resultado")]
        public int ResultId { get; set; }

        [ForeignKey("ProficiencyId")]
        public virtual Proficiency Proficiency { get; set; }
        [ForeignKey("ResultId")]
        public virtual Result Results { get; set; }
    }
}