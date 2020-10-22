using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Games
{
    public class Game
    {
        [Key]
        public int Game_Id { get; set; }
        public string Game_Nombre { get; set; }


        public virtual ICollection<LockGame> LockGame { get; set; }
    }
}