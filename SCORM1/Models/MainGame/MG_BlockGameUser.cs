using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_BlockGameUser
    {
        [Key]
        public int BlGa_Id { get; set; }
        public DateTime BlGa_Fecha { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}