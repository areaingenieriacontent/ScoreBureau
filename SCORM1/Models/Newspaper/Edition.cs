using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Newspaper
{
    public class Edition
    {
        [Key]
        public int Edit_Id { get; set; }
        [Display(Name = "Nombre")]
        public string Edit_Name { get; set; }
        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.Date)]
        public DateTime Edit_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime Edit_FinishDate { get; set; }
        [Display(Name = "Estado edición")]
        public EDITIONSTATE Edit_StateEdition { get; set; }
        [Display(Name = "Descripción ")]
        public string Edit_Description { get; set; }
        [Display(Name = "Nombre Imagen")]
        public string Edit_ImageName { get; set; }
        [Display(Name = "Tipo Contenido")]
        public string Edit_ImageType { get; set; }
        [Display(Name = "Contenido")]
        public byte[] Edit_ImageContent { get; set; }
        [Display(Name = "Puntaje")]
        public int Edit_Points { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
    }
}