
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ratings;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class RatingsController : Controller
    {

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public RatingsController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        /*
        * Metodo que dependiendo el usuario que se loguee realiza una instancia de ese usuario
        * para que puedan acceder a su información
        */
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        /*
        * Este metodo se utiliza para Obtener el logo de la compañia del usuario actual,
        * para que se vea en todas las vistas.
        */
        private string GetUrlLogo()
        {
            var companyId = (int)GetActualUserId().CompanyId;
            string Logo = "";
            StylesLogos CompanyLogo = ApplicationDbContext.StylesLogos.Where(x => x.companyId == companyId).FirstOrDefault();
            if (CompanyLogo != null)
            {
                Logo = CompanyLogo.UrlLogo;
            }
            else
            {
                Logo = ApplicationDbContext.StylesLogos.Find(1).UrlLogo;
            }
            return Logo;
        }
        //Este metodo toma la ip de la cual se está ingresando y la retorna en una variable String.
        public string IpUser()
        {
            string ip = Request.UserHostAddress;
            string szXForwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string Ipserver;
            if (szXForwardedFor != null)
            {
                Ipserver = szXForwardedFor;
            }
            else
            {
                Ipserver = ip;
            }
            return Ipserver;
        }
        // GET: Ratings
        public ActionResult Index()
        {
            return View();
        }
        /*
         * Metodo utilizado para añadir un nuevo recurso a un tema 
         * especifico
         * Esté metodo recibe un modelo con todos los datos necesarios para
         * la creación del nuevo recurso
         */
        [HttpPost]
        public ActionResult AddJob(AdminTrainingGeneralViewModel model)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            if (ModelState.IsValid)
            {
                var top = ApplicationDbContext.TopicsCourses.Find(model.ToCo_Id);
                var job = new Job
                {
                    Job_Name = model.Job_Name,
                    Job_Content = model.Job_Content,
                    Job_Description = model.Job_Description,
                    Job_InitDate = model.Job_InitDate,
                    Job_FinishDate = model.Job_FinishDate,
                    Job_Points = model.Job_Points,
                    Job_StateJob = model.Job_StateJob,
                    Job_TypeJob = model.Job_TypeJob,
                    ToCo_Id = model.ToCo_Id,
                    TopicsCourse = top,
                    ApplicationUser = user,
                    User_Id = user.Id
                };
                ApplicationDbContext.Job.Add(job);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
            }
            return RedirectToAction("Grades", "AdminTraining", new { id = model.Modules.Modu_Id });
        }
        /*
         * Metodo utilizado para ctivar un formulario
         * que permita modificar los cambios de 
         * un recurso especifico
         * Esté metodo recibe el id del recurso que se 
         * va a modificicar
         */
        [Authorize]
        public ActionResult EditJob(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            var jobs = ApplicationDbContext.Job.Find(id);
            TempData["UpdateJob"] = "esta es una prueba para editar";
            var GetModule = ApplicationDbContext.Modules.Find(jobs.TopicsCourse.Modu_Id);
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Job_Id = jobs.Job_Id,
                Job_Name = jobs.Job_Name,
                Job_Description = jobs.Job_Description,
                Job_Content = jobs.Job_Content,
                Job_InitDate = jobs.Job_InitDate,
                Job_FinishDate = jobs.Job_FinishDate,
                Job_Points = jobs.Job_Points,
                Job_StateJob = jobs.Job_StateJob,
                Job_TypeJob = jobs.Job_TypeJob,
                Modules = GetModule,
                baseUrl = url,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("Grades", "AdminTraining", model);
        }
        /*
         * Metodo utilizado para modificar los datos de
         * un recurso especifico
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para la modificación del recurso
         */
        [HttpPost]
        [Authorize]
        public ActionResult EditJob(AdminTrainingGeneralViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Info"] = "Tema modificado con éxito";
                var jobs = ApplicationDbContext.Job.Find(model.Job_Id);
                jobs.Job_Name = model.Job_Name;
                jobs.Job_Description = model.Job_Description;
                jobs.Job_Content = model.Job_Content;
                jobs.Job_FinishDate = model.Job_FinishDate;
                jobs.Job_InitDate = model.Job_InitDate;
                jobs.Job_Points = model.Job_Points;
                jobs.Job_StateJob = model.Job_StateJob;
                jobs.Job_TypeJob = model.Job_TypeJob;
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("grades", "AdminTraining", new { id = model.Modules.Modu_Id });
        }
        /*
         * Metodo utilizado para retornar la vista
         * del recurso dependiendo el tipo de 
         * recurso al que pertenezca
         */
        [Authorize]
        public ActionResult ViewJob(int id)
        {
            var jobs = ApplicationDbContext.Job.Find(id);

            if (jobs.Job_TypeJob == TYPEJOB.Tarea)
            {
                return RedirectToAction("ViewJobsT", new { id = id });
            }
            else
            {
                return RedirectToAction("ViewJobsF", new { id = id });
            }
        }
        /*
         *Metodo utilizado para visualizar los recursos de 
         * tipo tareas 
         * Esté metodo recibe el id del recurso
         */
        [Authorize]
        public ActionResult ViewJobsT(int id)
        {
            var job = ApplicationDbContext.Job.Find(id);


            RatingAdminViewJobsT model = new RatingAdminViewJobsT
            {
                ActualRole=GetActualUserId().Role,
                JOBS = job,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo()
            };
            if (job.Job_Ext != null)
            {
                switch (job.Job_Ext)
                {
                    case ".pdf":
                        model.Image = "pdf.png";
                        break;
                    case ".doc":
                        model.Image = "word.png";
                        break;
                    case ".xls":
                        model.Image = "xls.png";
                        break;
                    case ".pptx":
                        model.Image = "pptx.png";
                        break;
                    case ".xlsx":
                        model.Image = "xls.png";
                        break;
                    case ".docx":
                        model.Image = "word.png";
                        break;
                }
            }
            return View(model);
        }
        /*
        *Metodo utilizado para visualizar los recursos de 
        * tipo foro 
        * Esté metodo recibe el id del recurso
        */
        [Authorize]
        public ActionResult ViewJobsF(int id)
        {
            var job = ApplicationDbContext.Job.Find(id);
            RatingForum model = new RatingForum
            {
                ActualRole = GetActualUserId().Role,
                JOBS = job,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo()
            };
            if (job.Job_Ext != null)
            {
                switch (job.Job_Ext)
                {
                    case ".pdf":
                        model.Image = "pdf.png";
                        break;
                    case ".doc":
                        model.Image = "word.png";
                        break;
                    case ".xls":
                        model.Image = "xls.png";
                        break;
                    case ".pptx":
                        model.Image = "pptx.png";
                        break;
                    case ".docx":
                        model.Image = "word.png";
                        break;
                }
            }
            return View(model);
        }
        /*
         * Metodo utilizado para visualiar
         * las respuestas de un comentario 
         * de foro
         * Esté metodo recibe el id del foro seleccionado
         * para consultar sus respuestas
         */
        [Authorize]
        public ActionResult AnswerForm(int id)
        {
            var refo = ApplicationDbContext.ResourceForum.Find(id);
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            AnswerFormView model = new AnswerFormView
            {
                Refo = refo,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                user = user
            };
            return View(model);
        }
        /*
        * Metodo utilizado para visualiar
        * las respuestas de un comentario 
        * de foro en la vista del usuario
        * Esté metodo recibe el id del foro seleccionado
        * para consultar sus respuestas
        */
        [Authorize]
        public ActionResult AnswerFormUser(int id)
        {
            var refo = ApplicationDbContext.ResourceForum.Find(id);
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            AnswerFormView model = new AnswerFormView
            {
                Refo = refo,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                user = user
            };
            return View(model);
        }
        /*
         * Metodo utilizado para crear las resuestas 
         * de un comentaro de foro
         * Esté metodo recibe un modelo con todos los 
         * datos necesarios para la creación de la 
         * respuesta al comentario
        */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnswerComment(AnswerFormView model)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            if (ModelState.IsValid)
            {

                var Refo = ApplicationDbContext.ResourceForum.Find(model.Refo.ReFo_Id);
                var AnFo = new AnswersForum
                {
                    ReFo_Id = Refo.ReFo_Id,
                    AnFo_InitDate = DateTime.Now,
                    AnFo_Content = model.AnFo_Content,
                    ApplicationUser = user,
                    User_Id = user.Id,
                    ResourceForum = Refo,
                };
                ApplicationDbContext.AnswersForum.Add(AnFo);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("AnswerForm", new { id = model.Refo.ReFo_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("AnswerForm", new { id = model.Refo.ReFo_Id });
            }
        }
        /*
         * Metodo utilizado para eliminar las 
         * respuestas a un comentario de foro
         * Esté metodo recibe el id de la respuesta
         * para eliminar
         */
        [Authorize]
        public ActionResult DeleteAnswerComm(int id)
        {
            var AnFo = ApplicationDbContext.AnswersForum.Find(id);
            int job = AnFo.ReFo_Id;
            ApplicationDbContext.AnswersForum.Remove(AnFo);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("AnswerForm", new { id = job });
        }
        /*
         * Metodo utilizado para crear una respuesta a 
         * un comentario por parte del usuario
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para la creación de l respuesta al comentario
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnswerCommentUser(AnswerFormView model)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            if (ModelState.IsValid)
            {

                var Refo = ApplicationDbContext.ResourceForum.Find(model.Refo.ReFo_Id);
                var AnFo = new AnswersForum
                {
                    ReFo_Id = Refo.ReFo_Id,
                    AnFo_InitDate = DateTime.Now,
                    AnFo_Content = model.AnFo_Content,
                    ApplicationUser = user,
                    User_Id = user.Id,
                    ResourceForum = Refo,
                };
                ApplicationDbContext.AnswersForum.Add(AnFo);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("AnswerFormUser", new { id = model.Refo.ReFo_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("AnswerFormUser", new { id = model.Refo.ReFo_Id });
            }
        }
        /*
        * Metodo utilizado para eliminar las 
        * respuestas a un comentario de foro
        * realizada por el usuario
        * Esté metodo recibe el id de la respuesta
        * para eliminar
        */
        [Authorize]
        public ActionResult DeleteAnswerCommUser(int id)
        {
            var AnFo = ApplicationDbContext.AnswersForum.Find(id);
            int job = AnFo.ReFo_Id;
            ApplicationDbContext.AnswersForum.Remove(AnFo);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("AnswerFormUser", new { id = job });
        }
        /*
         * Metodo utilizado para añadir un archivo a un 
         * recurso de tipo tarea
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para añadir el archivo al recurso
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ResourceJobsT(RatingAdminViewJobsT model, HttpPostedFileBase upload)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                            jobs.Job_Resource = ruta;
                            jobs.Job_Ext = ext;
                            ApplicationDbContext.SaveChanges();
                            SendEmail(upload.FileName, GetActualUserId().FirstName+ " "+ GetActualUserId().LastName);
                            return RedirectToAction("ViewJobsT", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewJobsT", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    TempData["Info"] = "No ha seleccionado un archivo o el archivo que inteta subir es demasiado pesado ";
                    return RedirectToAction("ViewJobsT", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewJobsT", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metodo utilizado para crear un comentario a 
         * un foro por parte del administrador o docente
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para la creación del comentario
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOFComment(RatingForum model, HttpPostedFileBase upload)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                            var ReFo = new ResourceForum
                            {
                                ReFo_Content = model.ReFo_Content,
                                ReFo_InitDate = DateTime.Now,
                                ReFo_Name = model.ReFo_Name,
                                ReFo_Resource = ruta,
                                ApplicationUser = user,
                                User_Id = user.Id,
                                Job_Id = jobs.Job_Id,
                                Job = jobs
                            };
                            ApplicationDbContext.ResourceForum.Add(ReFo);
                            ApplicationDbContext.SaveChanges();
                            var boock = new BookRatings
                            {
                                BoRa_StateScore = STATESCORE.No_Calificado,
                                ResourceForum = ReFo,
                                ReFo_Id = ReFo.ReFo_Id
                            };
                            ApplicationDbContext.BookRatings.Add(boock);
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("ViewJobsF", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewJobsF", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                    var ReFo = new ResourceForum
                    {
                        ReFo_Content = model.ReFo_Content,
                        ReFo_InitDate = DateTime.Now,
                        ReFo_Name = model.ReFo_Name,
                        ApplicationUser = user,
                        User_Id = user.Id,
                        Job_Id = jobs.Job_Id,
                        Job = jobs
                    };
                    ApplicationDbContext.ResourceForum.Add(ReFo);
                    ApplicationDbContext.SaveChanges();
                    var boock = new BookRatings
                    {
                        BoRa_StateScore = STATESCORE.No_Calificado,
                        ResourceForum = ReFo,
                        ReFo_Id = ReFo.ReFo_Id
                    };
                    ApplicationDbContext.BookRatings.Add(boock);
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("ViewJobsF", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewJobsF", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metodo utilizado para eliminar un comentario 
         * de un foro
         * Esté metodo recibe el id del comentario para 
         * eliminar
         */
        [Authorize]
        public ActionResult DeleteForum(int id)
        {
            var ReFo = ApplicationDbContext.ResourceForum.Find(id);
            int job = ReFo.Job_Id;
            if (ReFo.BookRatings.Count != 0)
            {
                foreach (var item2 in ReFo.BookRatings)
                {
                    var a = ApplicationDbContext.BookRatings.Find(item2.BoRa_Id);
                    int? i = a.ReFo_Id;
                    ApplicationDbContext.BookRatings.Remove(a);
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("DeleteForum", new { id = i });
                }
            }
            ApplicationDbContext.ResourceForum.Remove(ReFo);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("ViewJobsF", new { id = job });
        }
        /*
         * Metodo utilizado para borra las 
         * respuestas que tenga un comentarios y 
         * el registro del libro de calificaciones
         * Esté metodo recibe el id del comentario
         */
        private bool VrReFo(int id)
        {
            var ReFo = ApplicationDbContext.ResourceForum.Find(id);
            if (ReFo.BookRatings.Count != 0)
            {
                foreach (var item2 in ReFo.BookRatings)
                {
                    var a = ApplicationDbContext.BookRatings.Find(item2.BoRa_Id);
                    ApplicationDbContext.BookRatings.Remove(a);
                    ApplicationDbContext.SaveChanges();
                }
            }
            return true;
        }
        /*
         * Metodo utilizado para visualizar el libro de calificaciones
         * de los aportes realizados de 
         * un foro 
         * Esté metodo recibe el id del recurso
         * para consultar los foros disponibles
         */
        [Authorize]
        public ActionResult ScoreF(int id, int? page)
        {
            var jo = ApplicationDbContext.Job.Find(id);
            IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceForum.ApplicationUser.UserName).Where(x => x.ResourceForum.Job_Id == id).ToList().ToPagedList(page ?? 1, 15);
            ScoreF model = new ScoreF
            {
                ListBookRatings = ListBook,
                Logo = GetUrlLogo(),
                JOBS = jo
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        /*
         * Metodo utilizado para buscar un usuario en 
         * especifico en el libro de calificaciones de 
         * foros 
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para realizar la busqueda del usuario
         */
        public ActionResult SearchScoreF(ScoreF model, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.SearchUser) || string.IsNullOrWhiteSpace(model.SearchUser))
            {

                return RedirectToAction("ScoreF", new { id = model.JOBS.Job_Id });
            }
            else
            {
                IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceForum.ReFo_Name).Where(x => x.ResourceForum.Job_Id == model.JOBS.Job_Id && (x.ResourceForum.ReFo_Name.Contains(model.SearchUser) || x.ResourceForum.ApplicationUser.FirstName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 15);
                model = new ScoreF
                {
                    ListBookRatings = ListBook,
                    Logo = GetUrlLogo(),
                };
                model.Sesion = GetActualUserId().SesionUser;
                return View("ScoreF", model);
            }
        }
        /*
         * Metodo utilizado para activar el formulario
         * de clificación de un usuario especifico 
         * Esté metodo recibe el id del aporte del usuario
         */
        [Authorize]
        public ActionResult UpdateBook(int id, int? page)
        {
            var fo = ApplicationDbContext.BookRatings.Find(id);
            IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceForum.ApplicationUser.UserName).Where(x => x.ResourceForum.Job_Id == fo.ResourceForum.Job_Id).ToList().ToPagedList(page ?? 1, 15);
            ScoreF model = new ScoreF
            {
                ListBookRatings = ListBook,
                Logo = GetUrlLogo(),
                BoRa_Id = id,
                JOBS = ApplicationDbContext.Job.Find(fo.ResourceForum.Job_Id)
            };
            model.Sesion = GetActualUserId().SesionUser;
            TempData["Edit"] = "Tema modificado con éxito";
            return View("ScoreF", model);

        }
        /*
         * Metodo utilizado para modificar los datos de un libro 
         * de calificaciones de un usuario especifico
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para modificar la información
        */
        [HttpPost]
        [Authorize]
        public ActionResult UpdateBook(ScoreF model)
        {
            var book = ApplicationDbContext.BookRatings.Find(model.BoRa_Id);
            if (ModelState.IsValid)
            {
                TempData["Info"] = "Tema modificado con éxito";
                book.BoRa_Score = model.BoRa_Score;
                double puntos = book.ResourceForum.Job.Job_Points * model.BoRa_Score;
                double puntos2 = puntos / 5;
                int puntos3 = (int)puntos2;
                book.BoRa_Point = puntos3;
                book.BoRa_InitDate = DateTime.Now;
                book.BoRa_Description = model.BoRa_Description;
                book.BoRa_StateScore = STATESCORE.Calificado;
                ApplicationDbContext.SaveChanges();
                if (book.BoRa_StateScore == STATESCORE.Calificado)
                {
                    var cate = ApplicationDbContext.TypePoints.Find(49);
                    var point = new Point
                    {
                        ApplicationUser = book.ResourceForum.ApplicationUser,
                        TypePoint = cate,
                        TyPo_Id = cate.TyPo_Id,
                        Quantity_Points = puntos3,
                        User_Id = book.ResourceForum.ApplicationUser.Id,
                        Poin_Date = DateTime.Now
                    };
                    ApplicationDbContext.Points.Add(point);
                    ApplicationDbContext.SaveChanges();
                }

            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("ScoreF", new { id = book.ResourceForum.Job_Id });
        }
        /*
        * Metodo utilizado para visualizar el libro de calificaciones
        * de los aportes realizados de 
        * una tarea 
        * Esté metodo recibe el id del recurso
        * para consultar los foros disponibles
        */
        [Authorize]
        public ActionResult ScoreT(int id, int? page)
        {
            var jo = ApplicationDbContext.Job.Find(id);
            IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceJobs.ApplicationUser.UserName).Where(x => x.ResourceJobs.Job_Id == id).ToList().ToPagedList(page ?? 1, 15);
            ScoreT model = new ScoreT
            {
                ListBookRatings = ListBook,
                Logo = GetUrlLogo(),
                JOBS = jo
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        /*
         * Metodo utilizado para buscar un usuario en 
         * especifico en el libro de calificaciones de 
         * tareas 
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para realizar la busqueda del usuario
         */
        public ActionResult SearchScoreT(ScoreT model, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.SearchUser) || string.IsNullOrWhiteSpace(model.SearchUser))
            {
                return RedirectToAction("ScoreT", new { id = model.JOBS.Job_Id });
            }
            else
            {
                IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceJobs.ReJo_Name).Where(x => x.ResourceJobs.Job_Id == model.JOBS.Job_Id && (x.ResourceJobs.ReJo_Name.Contains(model.SearchUser) || x.ResourceJobs.ApplicationUser.FirstName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 15);
                model = new ScoreT
                {
                    ListBookRatings = ListBook,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                return View("ScoreT", model);
            }
        }
        /*
         * Metodo utilizado para activar el formulario
         * de clificación de un usuario especifico 
         * Esté metodo recibe el id del aporte del usuario
         */
        [Authorize]
        public ActionResult UpdateBookT(int id, int? page)
        {
            var fo = ApplicationDbContext.BookRatings.Find(id);
            IPagedList<BookRatings> ListBook = ApplicationDbContext.BookRatings.OrderBy(x => x.ResourceJobs.ApplicationUser.UserName).Where(x => x.ResourceJobs.Job_Id == fo.ResourceJobs.Job_Id).ToList().ToPagedList(page ?? 1, 15);
            ScoreT model = new ScoreT
            {
                ListBookRatings = ListBook,
                Logo = GetUrlLogo(),
                BoRa_Id = id,
                JOBS = ApplicationDbContext.Job.Find(fo.ResourceJobs.Job_Id)
            };
            model.Sesion = GetActualUserId().SesionUser;
            TempData["Edit"] = "Tema modificado con éxito";
            return View("ScoreT", model);

        }
        /*
         * Metodo utilizado para modificar los datos de un libro 
         * de calificaciones de un usuario especifico
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para modificar la información
        */
        [HttpPost]
        [Authorize]
        public ActionResult UpdateBookT(ScoreT model)
        {
            var book = ApplicationDbContext.BookRatings.Find(model.BoRa_Id);
            if (ModelState.IsValid)
            {
                TempData["Info"] = "Tema modificado con éxito";
                book.BoRa_Score = model.BoRa_Score;
                double puntos = book.ResourceJobs.Job.Job_Points * model.BoRa_Score;
                double puntos2 = puntos / 5;
                int puntos3 = book.ResourceJobs.Job.Job_Points;
                book.BoRa_InitDate = DateTime.Now;
                if(book.BoRa_Score >= 3)
                {
                    book.BoRa_Point = puntos3;
                    book.BoRa_StateScore = STATESCORE.Aceptado;
                }
                else
                {
                    book.BoRa_Point = 0;
                    book.BoRa_StateScore = STATESCORE.No_Aceptado;

                }
                book.BoRa_Description = model.BoRa_Description;
                ApplicationDbContext.SaveChanges();
                if (book.BoRa_StateScore == STATESCORE.Calificado)
                {
                    var cate = ApplicationDbContext.TypePoints.Find(53);
                    var point = new Point
                    {
                        ApplicationUser = book.ResourceJobs.ApplicationUser,
                        TypePoint = cate,
                        TyPo_Id = cate.TyPo_Id,
                        Quantity_Points = puntos3,
                        User_Id = book.ResourceJobs.ApplicationUser.Id,
                        Poin_Date = DateTime.Now,
                        Poin_End_Date = DateTime.Now.AddYears(1)
                    };
                    ApplicationDbContext.Points.Add(point);
                    ApplicationDbContext.SaveChanges();
                }

            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("ScoreT", new { id = book.ResourceJobs.Job_Id });
        }
        /*
         *Metodo utilizado para visualizar los recursos de 
         * tipo tareas en la vista del usuario
         * Esté metodo recibe el id del recurso
         */
        [Authorize]
        public ActionResult ViewJobUser(int id)
        {
            var job = ApplicationDbContext.Job.Find(id);
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var listT = ApplicationDbContext.ResourceJobs.Where(x => x.User_Id == user.Id && x.Job_Id == job.Job_Id).ToList();

            RatingUserViewJobsT model = new RatingUserViewJobsT
            {
                JOBS = job,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                ListJobsT = listT
            };
            if (job.Job_Ext != null)
            {
                switch (job.Job_Ext)
                {
                    case ".pdf":
                        model.Image = "pdf.png";
                        break;
                    case ".doc":
                        model.Image = "word.png";
                        break;
                    case ".xls":
                        model.Image = "xls.png";
                        break;
                    case ".pptx":
                        model.Image = "pptx.png";
                        break;
                    case ".xlsx":
                        model.Image = "xls.png";
                        break;
                    case ".docx":
                        model.Image = "word.png";
                        break;
                }
            }
            if (listT.Count != 0)
            {
                switch (listT.FirstOrDefault().ReJo_Ext)
                {
                    case ".pdf":
                        model.ImageR = "pdf.png";
                        break;
                    case ".doc":
                        model.ImageR = "word.png";
                        break;
                    case ".xls":
                        model.ImageR = "xls.png";
                        break;
                    case ".pptx":
                        model.ImageR = "pptx.png";
                        break;
                    case ".xlsx":
                        model.ImageR = "xls.png";
                        break;
                    case ".docx":
                        model.ImageR = "word.png";
                        break;
                }
            }
            return View(model);
        }
        /*
        *Metodo utilizado para visualizar los recursos de 
        * tipo foro en la vista del usuario
        * Esté metodo recibe el id del recurso
        */
        [Authorize]
        public ActionResult ViewFobUser(int id)
        {
            var job = ApplicationDbContext.Job.Find(id);
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            RatingUserViewJobsF model = new RatingUserViewJobsF
            {
                JOBS = job,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                User = user
            };
            return View(model);
        }
        /*
         * Metodo utilizado para crear un comentario a 
         * un foro por parte del usuario
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para la creación del comentario
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOFCommentUser(RatingForum model, HttpPostedFileBase upload)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                            var ReFo = new ResourceForum
                            {
                                ReFo_Content = model.ReFo_Content,
                                ReFo_InitDate = DateTime.Now,
                                ReFo_Name = model.ReFo_Name,
                                ReFo_Resource = ruta,
                                ApplicationUser = user,
                                User_Id = user.Id,
                                Job_Id = jobs.Job_Id,
                                Job = jobs
                            };
                            ApplicationDbContext.ResourceForum.Add(ReFo);
                            ApplicationDbContext.SaveChanges();
                            var boock = new BookRatings
                            {
                                BoRa_StateScore = STATESCORE.No_Calificado,
                                ResourceForum = ReFo,
                                ReFo_Id = ReFo.ReFo_Id
                            };
                            ApplicationDbContext.BookRatings.Add(boock);
                            ApplicationDbContext.SaveChanges();
                            SendEmailForm(GetActualUserId().FirstName + " " + GetActualUserId().LastName, jobs.Job_Name, jobs.TopicsCourse.Module.Modu_Name);
                            return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    TempData["Info"] = "El archivo excede el peso permitido.";
                    var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                    var ReFo = new ResourceForum
                    {
                        ReFo_Content = model.ReFo_Content,
                        ReFo_InitDate = DateTime.Now,
                        ReFo_Name = model.ReFo_Name,
                        ApplicationUser = user,
                        User_Id = user.Id,
                        Job_Id = jobs.Job_Id,
                        Job = jobs
                    };
                    ApplicationDbContext.ResourceForum.Add(ReFo);
                    ApplicationDbContext.SaveChanges();
                    var boock = new BookRatings
                    {
                        BoRa_StateScore = STATESCORE.No_Calificado,
                        ResourceForum = ReFo,
                        ReFo_Id = ReFo.ReFo_Id
                    };
                    ApplicationDbContext.BookRatings.Add(boock);
                    ApplicationDbContext.SaveChanges();
                    SendEmailForm(GetActualUserId().FirstName + " " + GetActualUserId().LastName, jobs.Job_Name, jobs.TopicsCourse.Module.Modu_Name);
                    return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metodo utilizado para añadir un archivo a un 
         * recurso de tipo tarea en la vista del usuario
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para añadir el archivo al recurso
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ResourceUserJobsT(RatingUserViewJobsT model, HttpPostedFileBase upload)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                            var reso = new ResourceJobs
                            {
                                Job = jobs,
                                Job_Id = jobs.Job_Id,
                                ReJo_Content = model.ReJo_Content,
                                ReJo_Description = model.ReJo_Description,
                                ReJo_Ext = ext,
                                ReJo_InitDate = DateTime.Now,
                                ReJo_Name = model.ReJo_Name,
                                ReJo_Resource = ruta,
                                User_Id = user.Id,
                                ApplicationUser = user
                            };
                            ApplicationDbContext.ResourceJobs.Add(reso);
                            ApplicationDbContext.SaveChanges();
                            var boock = new BookRatings
                            {
                                BoRa_StateScore = STATESCORE.No_Calificado,
                                ResourceJobs = reso,
                                ReJo_Id = reso.ReJo_Id
                            };
                            ApplicationDbContext.BookRatings.Add(boock);
                            ApplicationDbContext.SaveChanges();
                            SendEmailForm(GetActualUserId().FirstName + " " + GetActualUserId().LastName, jobs.Job_Name, jobs.TopicsCourse.Module.Modu_Name);
                            TempData["Info"] = "Archivo cargado exitosamente";
                            return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    TempData["Info"] = "El archivo excede el peso permitido.";
                    var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                    var reso = new ResourceJobs
                    {
                        Job = jobs,
                        Job_Id = jobs.Job_Id,
                        ReJo_Content = model.ReJo_Content,
                        ReJo_Description = model.ReJo_Description,
                        ReJo_InitDate = DateTime.Now,
                        ReJo_Name = model.ReJo_Name,
                        User_Id = user.Id,
                        ApplicationUser = user
                    };
                    ApplicationDbContext.ResourceJobs.Add(reso);
                    ApplicationDbContext.SaveChanges();
                    var boock = new BookRatings
                    {
                        BoRa_StateScore = STATESCORE.No_Calificado,
                        ResourceJobs = reso,
                        ReJo_Id = reso.ReJo_Id
                    };
                    ApplicationDbContext.BookRatings.Add(boock);
                    ApplicationDbContext.SaveChanges();
                    SendEmailForm(GetActualUserId().FirstName + " " + GetActualUserId().LastName, jobs.Job_Name, jobs.TopicsCourse.Module.Modu_Name);
                    return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metodo utilizado para modificar el archivo 
         * que ha subido el usuario a un recurso de tipo 
         * tarea
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para modificar el archivo
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditResourceUserJobsT(RatingUserViewJobsT model, HttpPostedFileBase upload)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            var jobs = ApplicationDbContext.Job.Find(model.JOBS.Job_Id);
                            var RJ = ApplicationDbContext.ResourceJobs.Find(model.ReJo_Id);
                            RJ.ReJo_Resource = ruta;
                            RJ.ReJo_Ext = ext;
                            RJ.ReJo_InitDate = DateTime.Now;
                            ApplicationDbContext.SaveChanges();
                            SendEmailForm(GetActualUserId().FirstName + " " + GetActualUserId().LastName, jobs.Job_Name, jobs.TopicsCourse.Module.Modu_Name);
                            return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    TempData["Info"] = "Debe seleccioanr un archivo o el archivo es demasiado pesado";
                    return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewJobUser", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metod utilizado para activar un 
         * formulario que permita modificar los
         * datos de un comentario de un foro en 
         * especifico
         * Esté metodo recibe el id del comentario 
         * para llamar la información a la vista
         */
        [Authorize]
        public ActionResult EditUserForum(int id)
        {
            var RF = ApplicationDbContext.ResourceForum.Find(id);
            var job = RF.Job;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            RatingUserViewJobsF model = new RatingUserViewJobsF
            {
                JOBS = job,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                User = user,
                ReFo_Id = RF.ReFo_Id,
                ReFo_Content = RF.ReFo_Content,
                ReFo_Description = RF.ReFo_Description,
                ReFo_Name = RF.ReFo_Name
            };
            TempData["Edit"] = "Tema modificado con éxito";
            return View("ViewFobUser", model);

        }
        /*
         * Metodo utilizado para modificar la 
         * infirmación de un comentario de foro 
         * especifico
         * Esté metodo recibe un modelo con todos los datos
         * necesarios para modificar el comentario
         */
        [HttpPost]
        [Authorize]
        public ActionResult EditUserForum(RatingUserViewJobsF model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    var ext = file;
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourcesJobs/" + file));
                            string ruta = file;
                            var RF = ApplicationDbContext.ResourceForum.Find(model.ReFo_Id);
                            RF.ReFo_Name = model.ReFo_Name;
                            RF.ReFo_Resource = ruta;
                            RF.ReFo_Content = model.ReFo_Content;
                            RF.ReFo_Description = model.ReFo_Description;
                            RF.ReFo_InitDate = DateTime.Now;
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                        }
                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                }
                else
                {
                    var RF = ApplicationDbContext.ResourceForum.Find(model.ReFo_Id);
                    RF.ReFo_Name = model.ReFo_Name;
                    RF.ReFo_Content = model.ReFo_Content;
                    RF.ReFo_Description = model.ReFo_Description;
                    RF.ReFo_InitDate = DateTime.Now;
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("ViewFobUser", new { id = model.JOBS.Job_Id });
            }
        }
        /*
         * Metodo utilizado para enviar un correo
         * al administrador y al docente encargado de 
         * un curso cuando el usuario realice un aporte 
         * Esté metodo recibe una serie de datos que ayudan 
         * al envio del correo
         */
        private void SendEmailForm(string NameUser, string Recurso, string Module)
        {
            int a = GetActualUserId().Company.CompanyId;
            var correo = ApplicationDbContext.Enrollments.Where(x => x.Module.Modu_Name == Module && x.Enro_RoleEnrollment == ROLEENROLLMENT.Docente).ToList();
            var admin = ApplicationDbContext.Users.Where(x => x.Company.CompanyId == a && x.Role == ROLES.AdministradordeFormacion || x.Role == ROLES.AdministradoGeneral).ToList();

            if (correo.Count != 0)
            {
                foreach (var item in correo)
                {
                    MailMessage solicitud = new MailMessage();
                    solicitud.Subject = "Aporte Recurso, curso de " + Module;
                    solicitud.Body = "Cordial saludo " + "<br/>" +
                        "Sr(a). " + item.ApplicationUser.FirstName +" "+ item.ApplicationUser.LastName + "<br/>" +
                       "<br/>" + "El usuario : " + NameUser + " ha realizado un aporte en la actividad de " + Recurso + " perteneciente al curso de " + Module + " el "+ DateTime.Now.ToString("yyyyMMddHHmmss")+
                       "<br/>" + "Por favor verifique el aporte realizado" +
                       "<br/>" +
                       "<br/>" + "Equipo de Soporte Bureau Veritas Trainning Community";

                    solicitud.To.Add(item.ApplicationUser.Email);
                    solicitud.IsBodyHtml = true;
                    var smtp2 = new SmtpClient();
                    smtp2.Send(solicitud);
                }
            }
        }
        /*
         * Metodo utilizado para consultar las 
         * calificaciones que tenga el usuario en 
         * relación a todos los recursos que tenga un curso
         */
        [Authorize]
        public ActionResult Result(int id)
        {


            var Module = ApplicationDbContext.Modules.Find(id);
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var jobs = ApplicationDbContext.Job.Where(x => x.TopicsCourse.Modu_Id == id).ToList();
            //list of test
            List<resultado> listTest = new List<resultado>();

            List<AdvanceUser> Test = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == user.Id).ToList();
            int eva = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Modu_Id == Module.Modu_Id).ToList().Count;
            double pointEvalution = Module.Modu_Points / eva;
            // List of test v.2 
            List<BankQuestion> ListBakn = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Module.Modu_Id == Module.Modu_Id).ToList();
            foreach (var item in ListBakn)
            {
                if (Test.Find(x => x.ToCo_id == item.ToCo_Id && x.User_Id == user.Id) != null)
                {
                    double PointOfUser = (((Test.Find(x => x.ToCo_id == item.ToCo_Id && x.User_Id == user.Id).AdUs_ScoreObtained * 100) / pointEvalution)/2)/10;

                    listTest.Add(
                        new resultado
                        {
                            Nombre = item.TopicsCourse.ToCo_Name,
                            BoRa_Score = Convert.ToDouble((int)Math.Round(PointOfUser)),
                            BoRa_StateScore = STATESCORE.Calificado
                        });
                }
                else
                {
                    listTest.Add(
                            new resultado
                            {
                                Nombre = item.TopicsCourse.ToCo_Name,
                                BoRa_Score = 0,
                                BoRa_StateScore = STATESCORE.No_Calificado
                            });
                }
            }

            //list of job
            List<resultado> listresultado = new List<resultado>();
            foreach (var item in jobs)
            {
                double califi = 0;
                DateTime? fecha = DateTime.Now;
                STATESCORE estado = STATESCORE.No_Calificado;
                string Retroalimentacion = "";
                var rf = ApplicationDbContext.ResourceForum.Where(x => x.Job.Job_Id == item.Job_Id && x.User_Id == user.Id).FirstOrDefault();
                var rj = ApplicationDbContext.ResourceJobs.Where(x => x.Job.Job_Id == item.Job_Id && x.User_Id == user.Id).FirstOrDefault();
                if (rf != null)
                {
                    var book = rf.BookRatings.Count;
                    if (book != 0)
                    {
                        califi = rf.BookRatings.FirstOrDefault().BoRa_Score;
                        fecha = rf.BookRatings.FirstOrDefault().BoRa_InitDate;
                        estado = rf.BookRatings.FirstOrDefault().BoRa_StateScore;
                        Retroalimentacion = rf.BookRatings.FirstOrDefault().BoRa_Description;
                    }
                }
                if (rj != null)
                {
                    var book = rj.BookRatings.Count;
                    if (book != 0)
                    {
                        califi = rj.BookRatings.FirstOrDefault().BoRa_Score;
                        fecha = rj.BookRatings.FirstOrDefault().BoRa_InitDate;
                        estado = rj.BookRatings.FirstOrDefault().BoRa_StateScore;
                        Retroalimentacion = rj.BookRatings.FirstOrDefault().BoRa_Description;
                    }
                }

                listresultado.Add(new resultado
                {
                    Nombre = item.Job_Name,
                    BoRa_InitDate = fecha,
                    BoRa_Score = califi,
                    BoRa_StateScore = estado,
                    BoRa_Description = Retroalimentacion,
                });
            }
            RatingUserViewJobs model = new RatingUserViewJobs
            {
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                listresultado = listresultado,
                listTestUs= listTest
            };
            return View(model);
        }
        private void SendEmail(string nombreArchivo, string nombreUsuario)
        {
            try
            {
                MailMessage solicitud = new MailMessage();
                solicitud.Subject = "Se subio un archivo";
                solicitud.Body =
                    "El usuario "+nombreUsuario+" subio un archivo de nombre "+nombreArchivo +", el "+ DateTime.Now.ToString("yyyyMMddHHmmss");

                solicitud.To.Add("bureauveritassoporte@gmail.com");
                solicitud.IsBodyHtml = true;
                var smtp2 = new SmtpClient();

                smtp2.Send(solicitud);
            }
            catch (Exception)
            {


            }

        }
    }
}