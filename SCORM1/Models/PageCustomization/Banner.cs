using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.PageCustomization
{
    public class Banner
    {
        [Key]
        public int Bann_Id { get; set; }
        [Display(Name = "Titulo")]
        public string Bann_Name { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Bann_Date { get; set; }
        [Display(Name = "Descripción")]
        public string Bann_Description { get; set; }
        [Display(Name = "Link")]
        public string Bann_Link { get; set; }
        [Display(Name = "imagen")]
        [AllowHtml]
        public string Bann_Image { get; set; }
        public int? companyId { get; set; }

    }
}