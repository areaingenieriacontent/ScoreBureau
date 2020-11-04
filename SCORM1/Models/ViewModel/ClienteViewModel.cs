using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Models.ClientProfile;

namespace SCORM1.Models.ViewModel
{
    public class ClienteViewModel:BaseViewModel
    {
        public string first { get; set; }
        public Cliente cliente { get; set; }
        public Dia day { get; set; }
        public List<Cliente> listOfClients { get; set; }
        public List<Dia> listOfDays { get; set; }
        public List<Clasificacion> listOfCalification { get; set; }
    }
}