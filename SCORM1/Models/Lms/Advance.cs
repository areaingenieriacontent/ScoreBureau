using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Advance
    {
        [Key]
        public int Id { get; set; }

        public int Modulo_Id { get; set; }

        public float Score { get; set; }

        public DateTime FechaActualizacion { get; set; }
        
        public string Usuario_Id { get; set; }


    }
}