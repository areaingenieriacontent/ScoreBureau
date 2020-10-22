using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Display(Name = "Fecha")]
        public DateTime ResultDate { get; set; }

        [Display(Name = "Total")]
        public int ResultTotalScore { get; set; }

        [Display(Name = "Resultados de")]
        public ApplicationUser ResultOwner { get; set; }

        public ApplicationUser QualifiedUser { get; set; }

        [Display(Name = "Medicion")]
        public int MeasureId { get; set; }

        [ForeignKey("MeasureId")]
        public virtual Measure Measure { get; set; }
        public virtual ICollection<Score> Scores { get; set; }
    }
}