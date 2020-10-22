using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Newspaper
{
    public class Comments
    {
        [Key]
        public int Comm_Id { get; set; }

        [Display(Name = "Titulo")]
        public string comm_Title { get; set; }

        [Display(Name = "Descripción")]
        [AllowHtml]
        public string comm_Description { get; set; }

        [Display(Name = "Valor")]
        public int Comm_value { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime Comm_Date { get; set; }

        [Display(Name = "Estado Comentario")]
        public COMMENTSTATE Comm_StateComment { get; set; }

        [Display(Name = "autor")]
        public virtual ApplicationUser comm_Author { get; set; }

        [ForeignKey("Article")]
        public int Arti_Id { get; set; }
        public virtual Article Article { get; set; }

        public virtual ICollection<PointsComment> PointsComment { get; set; }
    }
}