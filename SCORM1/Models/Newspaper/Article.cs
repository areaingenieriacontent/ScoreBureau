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
    public class Article
    {
        [Key]
        public int Arti_Id { get; set; }

        [Display(Name = "Nombre Articulo")]
        public string Arti_Name { get; set; }

        public string Arti_imagen { get; set; }

        [Display(Name = "Descripción")]
        public string Arti_Description { get; set; }

        [Display(Name = "Contenido")]
        [AllowHtml]
        public string Arti_Content { get; set; }

        [Display(Name = "Cantidad ")]
        public string Arti_Author { get; set; }

        [Display(Name = "Estado Articulo")]
        public ARTICLESTATE Arti_StateArticle { get; set; }

        [Display (Name ="Aceptar Comentario")]
        public ARTICLEWITHCOMMENT ArticleWithComment { get; set; }

        [ForeignKey("Section")]
        public int sect_Id { get; set; }
        public virtual Section Section { get; set; }

        public virtual ICollection<ResourceNw> ResourceNw { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}