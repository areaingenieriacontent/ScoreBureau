using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Newspaper
{
    public class ResourceNw
    {
        [Key]
        public int ReNw_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReNw_Name { get; set; }

        [Display(Name = "Tipo de Recurso")]
        public string ReNw_ResourceType { get; set; }
        [Display(Name = "Contenido")]
        public byte[] ReNw_Content { get; set; }


        [ForeignKey("Article")]
        public int Arti_Id { get; set; }
        public virtual Article Article { get; set; }
    }
}