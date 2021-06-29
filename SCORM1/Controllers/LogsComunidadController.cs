using Microsoft.AspNet.Identity;
using SCORM1.Models;
using SCORM1.Models.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class LogsComunidadController : Controller
    {
        public ApplicationDbContext ApplicationDbContext { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        // GET: LogsComunidad

        public ActionResult Index()
        {
            return View();
        }
        public RedirectResult ClickObl()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());
            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 1,
                    ContBiblioteca = 0,
                    ContJuegos = 0,
                    ContPeriodico = 0,
                    ContSoftKills = 0

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int OBL = Consulta1.ContOBL;


                ConsultaUnica.FirstOrDefault().ContOBL = OBL + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://aprendeyavanza2.com.co/contentGroup/bureau/COMUNIDAD/ABE/contenedor/index.html");


        }
        public RedirectResult ClickBiblioteca()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());
            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 0,
                    ContBiblioteca = 1,
                    ContJuegos = 0,
                    ContPeriodico = 0,
                    ContSoftKills = 0

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int Biblioteca = Consulta1.ContBiblioteca;


                ConsultaUnica.FirstOrDefault().ContBiblioteca = Biblioteca + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://www.aprendeyavanza2.com.co/contentGroup/bureau/comunidad/biblioteca/paginabiblioteca/biblioteca.html");


        }
        public RedirectResult ClickVideoteca()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());
            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 0,
                    ContBiblioteca = 0,
                    ContJuegos = 0,
                    ContPeriodico = 0,
                    ContSoftKills = 0,
                    ContVideoteca = 1

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int Videoteca = Consulta1.ContVideoteca;


                ConsultaUnica.FirstOrDefault().ContVideoteca = Videoteca + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://www.aprendeyavanza2.com.co/contentGroup/bureau/comunidad/videoteca/paginaVideoteca/videoteca.html");


        }

        public RedirectResult ClickPeriodico()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());
            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 0,
                    ContBiblioteca = 0,
                    ContJuegos = 0,
                    ContPeriodico = 1,
                    ContSoftKills = 0,
                    ContVideoteca = 0

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int Periodico = Consulta1.ContPeriodico;


                ConsultaUnica.FirstOrDefault().ContPeriodico = Periodico + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://aprendeyavanza2.com.co/contentGroup/bureau/COMUNIDAD/Revista/contenedor/revisiones.html");

        }

        public RedirectResult ClickJuego()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());
            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 0,
                    ContBiblioteca = 0,
                    ContJuegos = 1,
                    ContPeriodico = 0,
                    ContSoftKills = 0,
                    ContVideoteca = 0

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int Juego = Consulta1.ContJuegos;


                ConsultaUnica.FirstOrDefault().ContJuegos = Juego + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://aprendeyavanza2.com.co/contentGroup/bureau/COMUNIDAD/Juegos/index.html");


        }

        public RedirectResult ClickSoftkills()
        {
            ApplicationDbContext = new ApplicationDbContext();
            var Consulta = ApplicationDbContext.LogsComunidad.ToList();
            var ConsultaUnica = Consulta.Where(x => x.IdUsuario == User.Identity.GetUserId());

            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(User.Identity.Name);
            result = Convert.ToBase64String(encryted);

            if (ConsultaUnica.Count() == 0)
            {

                LogsComunidad logsobl = new LogsComunidad
                {
                    IdUsuario = User.Identity.GetUserId(),
                    ContOBL = 0,
                    ContBiblioteca = 0,
                    ContJuegos = 0,
                    ContPeriodico = 0,
                    ContSoftKills = 1,
                    ContVideoteca = 0

                };
                ApplicationDbContext.LogsComunidad.Add(logsobl);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                var Consulta1 = ConsultaUnica.FirstOrDefault();

                int SoftKills = Consulta1.ContSoftKills;


                ConsultaUnica.FirstOrDefault().ContSoftKills = SoftKills + 1;

                ApplicationDbContext.SaveChanges();
            }
            return Redirect("https://aprendeyavanza2.com.co/DCOBureau/home/Loginexterno/" + result);


        }
    }
}