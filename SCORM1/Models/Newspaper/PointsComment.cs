using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Newspaper
{
    public class PointsComment
    {
        [Key]
        public int PoCo_Id { get; set; }
        [Display(Name = "Fecha de Creación")]
        public DateTime PoCo_Date { get; set; }


        [ForeignKey("Comments")]
        public int Comm_Id { get; set; }
        public virtual Comments Comments { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}