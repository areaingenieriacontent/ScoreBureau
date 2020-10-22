using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class Prize
    {
        [Key]
        public int Priz_Id { get; set; }
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Premio")]
        [Display(Name = "Nombre Premio")]
        public string Priz_Name { get; set; }
        [Display(Name = "Descripción")]
        public string Priz_Description { get; set; }
        [Display(Name = "Puntos Requeridos")]
        public int Priz_RequiredPoints { get; set; }
        [Display(Name = "Cantidad ")]
        public int Priz_Quantity { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Priz_Date { get; set; }
        [Display(Name = "Estado Banner")]
        public PRIZESTATE Priz_Stateprize { get; set; }



        [ForeignKey("CategoryPrize")]
        public int Capr_Id { get; set; }
        public virtual CategoryPrize CategoryPrize { get; set; }
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Exchange> Exchange { get; set; }
        public ICONPRIZE Prize_Icon { get; set; }
    }


}
    
