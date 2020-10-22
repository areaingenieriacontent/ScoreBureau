using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class AnserPairingStudent
    {
        [Key]
        public int AnPa_Id { get; set; }
        [Display(Name = "Preguntas")]
        public string AnPa_OptionsQuestion { get; set; }
        [Display(Name = "Respuestas")]
        public string AnPa_OptionAnswer { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Pairing")]
        public int Pair_Id { get; set; }
        public virtual Pairing Pairing { get; set; }
        public DateTime Date_Present_test { get; set; }
    }
}