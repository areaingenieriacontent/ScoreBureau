using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Measure
    {
        [Key]
        public int MeasureId { get; set; }
        [Display(Name = "Fecha Inicial")]
        public DateTime MeasureInitDate { get; set; }
        [Display(Name = "Fecha Final")]
        public DateTime MeasureFinishDate { get; set; }
        [Display(Name = "Compañia")]
        public int CompanyId { get; set; }
        [Display(Name = "Test")]
        public int TestId { get; set; }

        public virtual ICollection<MeasureUser> CompletedUsers { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}