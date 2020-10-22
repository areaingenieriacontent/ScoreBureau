using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class Point
    {
        [Key]
        public int Poin_Id { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Poin_Date { get; set; }
        public DateTime Poin_End_Date { get; set; }
        public int Quantity_Points { get; set; }
     

        [ForeignKey("TypePoint")]
        public int TyPo_Id { get; set; }
        public virtual TypePoint TypePoint { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Exchange> Exchange { get; set; }
    }
}