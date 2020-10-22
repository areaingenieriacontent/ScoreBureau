using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class QuienSabeMasPuntaje
    {
        [Key]
        public int Id { get; set; }
        public string User_Id { get; set; }
        public string User_Id_QSM { get; set; }
        public int Mudole_Id { get; set; }
        public int Mudole_Id_QSM { get; set; }
        public DateTime FechaPresentacion { get; set; }
        public float Puntaje { get; set; }
        public int PorcentajeAprobacion { get; set; }
    }
}