using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SCORM1.Models;
using SCORM1.Models.ViewModel;

namespace SCORM1.Models.ClientProfile
{
    public class Cliente
    {

        [Key]
        public int id { get; set; }
        [Display(Name ="Nombres")]
        public string firstName { get; set; }
        [Display(Name = "Apellidos")]
        public string lastName { get; set; }
        [Display(Name = "Documento de identificación")]
        public string identification { get; set; }
        [Display(Name = "Razón Social")]
        public string enterpriseName { get; set; }
        [ForeignKey("Clasificacion")]
        [Display(Name = "Tipo Perfil")]
        public int idClasificacion { get; set; }
        [ForeignKey("User")]

        public string userId { get; set; }
        [ForeignKey("Dia")]
        [Display(Name = "Día")]
        public int idDia { get; set; }
        public virtual Clasificacion Clasificacion { get; set; }
        public virtual Dia Dia { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}