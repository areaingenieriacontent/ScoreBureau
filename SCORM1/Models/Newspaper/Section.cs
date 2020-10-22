using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



namespace SCORM1.Models.Newspaper
{
    public class Section
    {
        [Key]
        public int sect_Id { get; set; }

        [Display(Name = "Cantidad ")]
        public string sect_name { get; set; }

        [ForeignKey("Edition")]
        public int Edit_Id { get; set; }
        public virtual Edition Edition { get; set; }

        public virtual ICollection<Article> Article { get; set; }
    }
}


