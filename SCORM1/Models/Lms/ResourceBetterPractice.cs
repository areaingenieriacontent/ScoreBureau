using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class ResourceBetterPractice
    {
        [Key]
        public int ReBe_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReBe_Name { get; set; }

        [Display(Name = "Tipo de Recurso")]
        public string ReBe_ResourcebettType { get; set; }
        [Display(Name = "Contenido")]
        public byte[] ReBe_Content { get; set; }


        [ForeignKey("BetterPractice")]
        public int BePr_Id { get; set; }
        public virtual BetterPractice BetterPractice { get; set; }
    }
}