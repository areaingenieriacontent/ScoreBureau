using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using SCORM1.Models.Lms;
using SCORM1.Models;

namespace SCORM1.Controllers
{
    public class ContactoController : Controller
    {
        protected ApplicationDbContext ApplicationDb { get; set; }

        // GET: Contacto/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Contacto/Create
        [HttpPost]
        public ActionResult Create(string correos,string identificacion, string empresa, string nombres, string categoria, string descripcion)
        {
            ApplicationDb = new ApplicationDbContext();
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("info@bvtrainingcommunity.com");
            correo.To.Add("bureauveritassoporte@gmail.com");
            correo.Subject = categoria + identificacion;
            string caso = "El usuario " + nombres + " identicado con el numero " + identificacion + " genero un nuevo caso de soporte: ";
            correo.Body = caso + descripcion;
            correo.IsBodyHtml = false;
            correo.Priority = MailPriority.Normal;
            //
            var smtp = new SmtpClient();
        
            smtp.Send(correo);
            int count = ApplicationDb.Correos.ToList().Count();
            CorreoModel correoDB;
            if (count == 0)
            {

                correoDB = new CorreoModel
                {
                    Nombre = nombres,
                    Categoria = categoria,
                    Documento = identificacion,
                    Mensaje = descripcion,
                    Empresa = empresa,
                    IdMensaje = "1",
                    Correos=correos
                };
            }
            else
            {
                count = count + 1;
                correoDB = new CorreoModel
                {
                    Nombre = nombres,
                    Categoria = categoria,
                    Documento = identificacion,
                    Mensaje = descripcion,
                    Empresa = empresa,
                    IdMensaje = count.ToString(),
                    Correos=correos
                  
                };
            }

            ApplicationDb.Correos.Add(correoDB);
            ApplicationDb.SaveChanges();

            CreateUser(correos);
            return RedirectToAction("Index", "Home");

        }
        public ActionResult CreateUser(string correos)
        {
            ApplicationDb = new ApplicationDbContext();
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("info@bvtrainingcommunity.com");
            correo.To.Add(correos);
            correo.Subject = "Inquietud a la mesa de servicio Bureau Veritas";
            string caso = "Acabamos de recibir tu mensaje. Nuestro equipo de trabajo analizará tu solicitud para determinar el nivel de complejidad."+'\n'+
                "Te estaremos informando al correo registrado el tiempo de repuesta asignado.";
            correo.Body = caso;
            correo.IsBodyHtml = false;
            correo.Priority = MailPriority.Normal;
            //
            var smtp = new SmtpClient();

            smtp.Send(correo);

            return RedirectToAction("Index", "Home");

        }
    }
}
