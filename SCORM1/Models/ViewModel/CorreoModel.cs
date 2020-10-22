
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace SCORM1.Models.Lms
{
    public class CorreoModel
    {
        [Key]
        public string IdMensaje { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Documento { get; set; }
        public string Empresa { get; set; }
        public string Mensaje { get; set; }
        public string Correos { get; set; }


    }
}