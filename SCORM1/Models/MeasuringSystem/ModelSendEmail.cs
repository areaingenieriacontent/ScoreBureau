using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class ModelSendEmail
    {
    }
    public class UsuariosySusPlanes
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nombre_medicion { get; set; }
        public string Correo { get; set; }
        public List<Plan> planes_Asignados { get; set; }
        public string IdUsuario { get; set; }
    }

}