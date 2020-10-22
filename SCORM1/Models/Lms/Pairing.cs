using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Pairing
    {
        [Key]
        public int Pair_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string Pair_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int Pair_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string Pair_Description { get; set; }


        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }

        public virtual ICollection<AnswerPairing> AnswerPairing { get; set; }
        public virtual ICollection<AnserPairingStudent> AnserPairingStudent { get; set; }

    }
}