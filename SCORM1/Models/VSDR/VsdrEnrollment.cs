using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCORM1.Models.VSDR
{
    public class VsdrEnrollment
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("User")]
        public string user_id { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("vsdr")]
        public int vsdr_id { get; set; }
        [Display(Name ="Fecha de matrícula")]
        public DateTime vsdr_enro_init_date { get; set; }
        [Display(Name = "Fecha de finalización")]
        public DateTime vsdr_enro_finish_date { get; set; }
        [Display(Name = "Calificación")]
        public float qualification { get; set; }
        public virtual VsdrSession vsdr { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}