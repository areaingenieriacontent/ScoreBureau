
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class LogsComunidad
    {
        [Key]
        public string IdUsuario { get; set; }
        public int ContOBL { get; set; }
        public int ContSoftKills { get; set; }
        public int ContBiblioteca { get; set; }
        public int ContPeriodico { get; set; }
        public int ContJuegos { get; set; }
        public int ContVideoteca { get; set; }

    }
}