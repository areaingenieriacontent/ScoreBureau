using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Games
{
    public class LockGame
    {
        [Key]
        public int LoGa_Id { get; set; }
        public DateTime LoGa_InitDate { get; set; }
        public DateTime LoGa_FinishDate { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("Games")]
        public int Game_Id { get; set; }
        public virtual Game Games { get; set; }
        [ForeignKey("TypeBaneo")]
        public int TyBa_Id { get; set; }
        public virtual TypeBaneo TypeBaneo { get; set; }


    }
}