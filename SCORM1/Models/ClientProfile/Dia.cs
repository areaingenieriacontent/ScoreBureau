using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ClientProfile
{
    public class Dia
    {
        public Dia()
        {
            this.Clientes = new HashSet<Cliente>();
        }

        [Key]
        public int id { get; set; }
        [Display(Name="Dia")]
        public string name { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}