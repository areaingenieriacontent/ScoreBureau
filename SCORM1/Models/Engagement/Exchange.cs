using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class Exchange
    {
        [Key]
        public int Exch_Id { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Exch_date { get; set; }
        [Display(Name = "Fecha de Aprobación")]
        public DateTime? Exch_Finishdate { get; set; }

         
        public int Exch_PointUser{ get; set; }
        [ForeignKey("Prize")]
        public int Priz_Id { get; set; }
        public virtual Prize Prize { get; set; }
        [Display(Name = "User")]
        public string ApplicationUser { get; set; }
        public virtual ApplicationUser User { get; set; }
        public STATEEXCHANGE StateExchange { get; set; }
      



    }
}