using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Games
{
    public class TypeBaneo
    {
        [Key]
        public int TyBa_Id { get; set; }
        public string TyBa_Type { get; set; }

        public virtual ICollection<LockGame> LockGame { get; set; }
    }
}