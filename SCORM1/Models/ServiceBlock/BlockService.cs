using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ServiceBlock
{
    public class BlockService
    {
        [Key]
        public int BlSe_Id { get; set; }
        [Display(Name = "Fecha de Creación")]
        public DateTime BlSe_Date { get; set; }


        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("TypeServiceBlock")]
        public int TySe_Id { get; set; }
        public virtual TypeServiceBlock TypeServiceBlock { get; set; }
    }
}