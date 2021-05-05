using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Logs;
using SCORM1.Models.Newspaper;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using SCORM1.Models.RigidCourse;

namespace SCORM1.Controllers
{

    public class UserController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


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

        public UserController()
        {

            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
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
        [HttpGet]
        [Authorize]
        public ActionResult ProfileUser()
        {
            string Id = GetActualUserId().Id;
            ApplicationUser User = UserManager.FindById(Id);
            UserProfileViewModel userCurrent = new UserProfileViewModel { ActualRole = GetActualUserId().Role, user = User, Logo = GetUrlLogo() };
            userCurrent.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(155);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = Id
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de información del usuario actual, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
                //userCurrent.ComunidadActiva = 
            }
            return View(userCurrent);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateUser(UserProfileViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {


                if (model.Password != null)
                {

                    if (upload != null && upload.ContentLength <= (2 * 1000000))
                    {
                        string[] allowedExtensions = new[] { ".png" };
                        var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();

                        foreach (var Ext in allowedExtensions)
                        {

                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/" + file));
                            TempData["add"] = "El curso se ha creado con éxito";
                            string ruta = file;

                            ApplicationUser updatedUser = UserManager.FindById(model.user.Id);
                            UserManager.RemovePassword(updatedUser.Id);
                            UserManager.AddPassword(updatedUser.Id, model.Password);
                            updatedUser.Id = model.user.Id;
                            updatedUser.UserName = model.user.UserName;
                            UserManager.AddPassword(updatedUser.Id, model.Password);
                            updatedUser.Email = model.user.Email;
                            updatedUser.FirstName = model.user.FirstName;
                            updatedUser.LastName = model.user.LastName;
                            updatedUser.Document = model.user.Document;
                            updatedUser.Foto_perfil = ruta;
                            UserManager.Update(updatedUser);
                            ApplicationDbContext.SaveChanges();
                            TempData["Result"] = "Modificación realizada con éxito.";
                            var table = ApplicationDbContext.TableChanges.Find(72);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(156);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = updatedUser.Id
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar su información personal, en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {

                        ApplicationUser updatedUser = UserManager.FindById(model.user.Id);
                        UserManager.RemovePassword(updatedUser.Id);
                        UserManager.AddPassword(updatedUser.Id, model.Password);
                        updatedUser.Id = model.user.Id;
                        updatedUser.UserName = model.user.UserName;
                        UserManager.AddPassword(updatedUser.Id, model.Password);
                        updatedUser.Email = model.user.Email;
                        updatedUser.FirstName = model.user.FirstName;
                        updatedUser.LastName = model.user.LastName;
                        updatedUser.Document = model.user.Document;
                        UserManager.Update(updatedUser);
                        ApplicationDbContext.SaveChanges();
                        TempData["Result"] = "Modificación realizada con éxito.";
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(156);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = updatedUser.Id
                            };
                            ApplicationDbContext.IdChanges.Add(idchange);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar su información personal, en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }

                    }



                    return RedirectToAction("ProfileUser");

                }

                else
                {
                    TempData["Result"] = "Debe escribir la contraseña.";
                    model.Sesion = GetActualUserId().SesionUser;
                    model.Logo = GetUrlLogo();
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(156);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = UserCurrent.Id
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento modificar su información personal pero no asigno una contraseña, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View("ProfileUser", model);
                }

            }
            else
            {
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(156);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UserCurrent.Id
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento modificar su información personal pero no ingreso ninguna información, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ProfileUser", model);
            }
        }

        [Authorize]
        public ActionResult TermsandConditions()
        {
            UserProfileViewModel model = new UserProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
            int CompanyUser = (int)GetActualUserId().CompanyId;
            var terms = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            if (terms != null)
            {
                model.FileTerms = terms.colorsTittle;
            }
            else
            {
                model.FileTerms = ApplicationDbContext.StylesLogos.Find(1).colorsTittle;
            }
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_TermsandConditions", model);
        }
        [Authorize]
        public ActionResult Validateterms(UserProfileViewModel model)
        {
            if (model.termsandconditions == true)
            {
                var id = GetActualUserId().Id;
                ApplicationUser user = ApplicationDbContext.Users.Find(id);
                user.TermsandConditions = Terms_and_Conditions.Aceptado;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(302);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de aceptar los terminos y condiciones, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Info"] = "Para ingresar al contenido debe aceptar los términos y condiciones.";
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        public ActionResult Videos()
        {
            UserProfileViewModel model = new UserProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };

            //test anclar el pdf a la vista de Módulo intróductorio
            int CompanyUser = (int)GetActualUserId().CompanyId;
            var terms = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            if (terms != null)
            {
                model.FileTerms = terms.colorsBacgraundTitles;
            }
            else
            {
                model.FileTerms = ApplicationDbContext.StylesLogos.Find(1).colorsBacgraundTitles;
            }
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_Videos", model);
            //fin test
        }
        [Authorize]
        public ActionResult Validatevideos(UserProfileViewModel model)
        {
            if (model.videos == true)
            {
                var id = GetActualUserId().Id;
                ApplicationUser user = ApplicationDbContext.Users.Find(id);
                user.Videos = VIDEOS.Opción1;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(302);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de garantizar que vio los videos introductorios a la plataforma, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Info"] = "Para ingresar al contenido debe ver primero los videos";
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        public ActionResult Panel()
        {
            UserProfileViewModel model = new UserProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_Panel", model);
        }
        [Authorize]
        public ActionResult Validateview(UserProfileViewModel model)
        {
            if (model.Opción1 == true)
            {
                var id = GetActualUserId().Id;
                ApplicationUser user = ApplicationDbContext.Users.Find(id);
                user.Videos = VIDEOS.Opción1;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(302);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de cofirmar su ingreso a la plataforma por la vista de opción1, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }

            }
            else
            {
                if (model.Opción2 == true)
                {
                    var id = GetActualUserId().Id;
                    ApplicationUser user = ApplicationDbContext.Users.Find(id);
                    user.Videos = VIDEOS.Opción2;
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(302);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = id
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de cofirmar su ingreso a la plataforma por la vista de opción2, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }

                }
            }
            return RedirectToAction("Index", "Home");
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public IEnumerable<SelectListItem> GetModule()
        {

            var Modules = ApplicationDbContext.Modules.ToList();
            IEnumerable<SelectListItem> Cursos = Modules.Select(x =>
           new SelectListItem
           {
               Value = x.Modu_Id.ToString(),
               Text = x.Modu_Name
           });
            return new SelectList(Cursos, "Value", "Text");
        }

        private IEnumerable<SelectListItem> GetAnswerPairing(int id, int idpairing)
        {
            var dbanswer = ApplicationDbContext.AnswerPairings.Where(x => x.Pairing.Pair_Id == idpairing);
            var answerpairing = dbanswer
                        .Select(answers =>
                                new SelectListItem
                                {
                                    Value = answers.AnPa_Id.ToString(),
                                    Text = answers.AnPa_OptionAnswer
                                }).OrderByDescending(x => x.Value);

            return new SelectList(answerpairing, "Value", "Text");
        }

        [Authorize]
        public new ActionResult Profile(UserProfileViewModel model)
        {
            var ComunidadActiva = ApplicationDbContext.Users.Find(GetActualUserId().Id).ComunidadActiva;
            model.ComunidadActiva = ComunidadActiva;
            var GetModuleCompany = GetActualUserId().CompanyId;
            try
            {
                Edition EditionToShow = ApplicationDbContext.Editions.Where(x => x.Edit_StateEdition == EDITIONSTATE.Activo && x.CompanyId == GetModuleCompany).First();
                List<Article> ListArticlesToSend = ApplicationDbContext.Articles.Where(x => x.Section.Edition.Edit_Id == EditionToShow.Edit_Id).ToList();
                var GetUserProfile = GetActualUserId().Id;
                model = new UserProfileViewModel
                {
                    ActualRole = GetActualUserId().Role,
                    EditionCurrentToActive = EditionToShow,
                    ListArticles = ListArticlesToSend,
                    Logo = GetUrlLogo(),
                    Form = new FormViewModel(),
                    ComunidadActiva = ComunidadActiva
                };
                model.Form.ListModule = GetModule();
            }
            catch (Exception)
            {

            }
            ApplicationUser Enrollment = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            //var mvirtual = ApplicationDbContext.Modules.Where(x => x.CompanyId == Enrollment.CompanyId && x.Modu_TypeOfModule==CURSO.Virtual).ToList();
            //if (mvirtual.Count!=Enrollment.Enrollment.Where(x => x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList().Count)
            //{
            //    if (mvirtual.Count != 0)
            //    {
            //        foreach (var item in mvirtual)
            //        {
            //            var e = ApplicationDbContext.Enrollments.Where(x => x.User_Id == Enrollment.Id && x.Modu_Id == item.Modu_Id).ToList();
            //            if (e.Count == 0)
            //            {
            //                DateTime finish = new DateTime();
            //                switch (item.Modu_Period)
            //                {
            //                    case VIGENCIA.Dias:
            //                        finish = DateTime.Now.AddDays(item.Modu_Validity);
            //                        break;
            //                    case VIGENCIA.Meses:
            //                        finish = DateTime.Now.AddMonths(item.Modu_Validity);
            //                        break;
            //                    case VIGENCIA.Años:
            //                        finish = DateTime.Now.AddYears(item.Modu_Validity);
            //                        break;
            //                    default:
            //                        break;
            //                }
            //                DateTime a = finish;
            //                Enrollment enro = new Enrollment
            //                {
            //                    ApplicationUser = Enrollment,
            //                    User_Id = Enrollment.Id,
            //                    Modu_Id = item.Modu_Id,
            //                    Module = item,
            //                    CompanyId = item.CompanyId,
            //                    Company = item.Company,
            //                    Enro_InitDateModule = DateTime.Now,
            //                    Enro_FinishDateModule = a,
            //                    Enro_StateEnrollment = ENROLLMENTSTATE.Activo
            //                };
            //                ApplicationDbContext.Enrollments.Add(enro);
            //                ApplicationDbContext.SaveChanges();
            //            }
            //        }

            //    }
            //}


            //var meval = ApplicationDbContext.Modules.Where(x => x.CompanyId == Enrollment.CompanyId && x.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            //if (meval.Count!=Enrollment.Enrollment.Where(x=>x.Module.Modu_TypeOfModule==CURSO.Evaluativo).ToList().Count)
            //{
            //    if (meval.Count != 0)
            //    {
            //        foreach (var item in meval)
            //        {
            //            var e = ApplicationDbContext.Enrollments.Where(x => x.User_Id == Enrollment.Id && x.Modu_Id == item.Modu_Id).ToList();
            //            if (e.Count == 0)
            //            {
            //                DateTime finish = new DateTime();
            //                switch (item.Modu_Period)
            //                {
            //                    case VIGENCIA.Dias:
            //                        finish = DateTime.Now.AddDays(item.Modu_Validity);
            //                        break;
            //                    case VIGENCIA.Meses:
            //                        finish = DateTime.Now.AddMonths(item.Modu_Validity);
            //                        break;
            //                    case VIGENCIA.Años:
            //                        finish = DateTime.Now.AddYears(item.Modu_Validity);
            //                        break;
            //                    default:
            //                        break;
            //                }
            //                DateTime a = finish;
            //                Enrollment enro = new Enrollment
            //                {
            //                    ApplicationUser = Enrollment,
            //                    User_Id = Enrollment.Id,
            //                    Modu_Id = item.Modu_Id,
            //                    Module = item,
            //                    CompanyId = item.CompanyId,
            //                    Company = item.Company,
            //                    Enro_InitDateModule = DateTime.Now,
            //                    Enro_FinishDateModule = a,
            //                    Enro_StateEnrollment = ENROLLMENTSTATE.Activo
            //                };
            //                ApplicationDbContext.Enrollments.Add(enro);
            //                ApplicationDbContext.SaveChanges();
            //            }
            //        }
            //    }
            //}

            bool game = activeGame();
            model.game = game;
            int Idgame = GameId();
            var se = ApplicationDbContext.MG_SettingMp.Find(Idgame);
            if (se != null)
            {
                model.fechajuego = se.Sett_CloseDate;
            }
            model.Idgame = Idgame;
            int CompanyUser = (int)GetActualUserId().CompanyId;
            List<Banner> banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();
            if (banners != null && banners.Count != 0)
            {
                model.Banners = banners;
            }
            else
            {
                model.Banners = ApplicationDbContext.Banners.Where(x => x.Bann_Id <= 4).ToList();
            }
            var titles = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            if (titles != null)
            {
                model.Title1 = titles.Title1;
                model.Title2 = titles.Title2;
                model.Title3 = titles.Title3;
            }
            else
            {
                model.Title1 = ApplicationDbContext.StylesLogos.Find(1).Title1;
                model.Title2 = ApplicationDbContext.StylesLogos.Find(1).Title2;
                model.Title3 = ApplicationDbContext.StylesLogos.Find(1).Title3;
            }
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            var EnrollmentVirtual = Enrollment.Enrollment.Where(x => x.ApplicationUser.Id == Enrollment.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var EnrollmentEvaluative = Enrollment.Enrollment.Where(x => x.ApplicationUser.Id == Enrollment.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            model.Listmoduleevaluative = EnrollmentEvaluative;
            model.Listmodulevirtual = EnrollmentVirtual;
            model.juego = GetActualUserId().Company.CompanyGame;
            var block = ApplicationDbContext.MG_BlockGameUser.Where(x => x.User_Id == Enrollment.Id).OrderByDescending(x => x.BlGa_Id).ToList();
            if (block.Count != 0)
            {
                var bu = block.FirstOrDefault();
                model.BlockUser = bu.BlGa_Fecha;
            }
            model.ComunidadActiva = ComunidadActiva;



            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(Enrollment.UserName);
            result = Convert.ToBase64String(encryted);
            ViewData["Usuario"] = result;

            ////string result = string.Empty;
            //byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            ////result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
            //result = System.Text.Encoding.Unicode.GetString(decryted);
            //return result;

            return PartialView("_Profile", model);
        }
        public bool activeGame()
        {
            int company = (int)GetActualUserId().CompanyId;
            var setting = ApplicationDbContext.MG_SettingMp.FirstOrDefault(x => x.Company_Id == company);
            int facil = 0;
            int medio = 0;
            int dificil = 0;
            if (setting != null)
            {
                if (setting.MG_MultipleChoice.Count != 0)
                {
                    foreach (var item in setting.MG_MultipleChoice)
                    {
                        switch (item.MuCh_Level)
                        {
                            case Enum.LEVEL.Fácil:
                                facil = facil + 1;
                                break;
                            case Enum.LEVEL.Medio:
                                medio = 1 + medio;
                                break;
                            case Enum.LEVEL.Difícil:
                                dificil = 1 + dificil;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (facil >= 15 && medio >= 18 && dificil >= 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GameId()
        {
            int company = (int)GetActualUserId().CompanyId;
            var setting = ApplicationDbContext.MG_SettingMp.FirstOrDefault(x => x.Company_Id == company);
            if (setting != null)
            {
                return setting.Sett_Id;
            }
            else
            {
                return 0;
            }
        }
        //[Authorize]
        //public new ActionResult Profile2(UserProfileViewModel model)
        //{
        //    var GetModuleCompany = GetActualUserId().CompanyId;
        //    try
        //    {
        //        Edition EditionToShow = ApplicationDbContext.Editions.Where(x => x.Edit_StateEdition == EDITIONSTATE.Activo && x.CompanyId == GetModuleCompany).First();
        //        List<Article> ListArticlesToSend = ApplicationDbContext.Articles.Where(x => x.Section.Edition.Edit_Id == EditionToShow.Edit_Id).ToList();
        //        var GetUserProfile = GetActualUserId().Id;
        //        model = new UserProfileViewModel
        //        {
        //            ActualRole = GetActualUserId().Role,
        //            EditionCurrentToActive = EditionToShow,
        //            ListArticles = ListArticlesToSend,
        //            Logo = GetUrlLogo()
        //        };
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    ApplicationUser Enrollment = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //    var mapli = ApplicationDbContext.Modules.Where(x => x.CompanyId == Enrollment.CompanyId && x.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //    if (mapli.Count != Enrollment.Enrollment.Where(x => x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList().Count)
        //    {
        //        if (mapli.Count != 0)
        //        {
        //            foreach (var item in mapli)
        //            {
        //                var e = ApplicationDbContext.Enrollments.Where(x => x.User_Id == Enrollment.Id && x.Modu_Id == item.Modu_Id).ToList();
        //                if (e.Count == 0)
        //                {
        //                    DateTime finish = new DateTime();
        //                    switch (item.Modu_Period)
        //                    {
        //                        case VIGENCIA.Dias:
        //                            finish = DateTime.Now.AddDays(item.Modu_Validity);
        //                            break;
        //                        case VIGENCIA.Meses:
        //                            finish = DateTime.Now.AddMonths(item.Modu_Validity);
        //                            break;
        //                        case VIGENCIA.Años:
        //                            finish = DateTime.Now.AddYears(item.Modu_Validity);
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    DateTime a = finish;
        //                    Enrollment enro = new Enrollment
        //                    {
        //                        ApplicationUser = Enrollment,
        //                        User_Id = Enrollment.Id,
        //                        Modu_Id = item.Modu_Id,
        //                        Module = item,
        //                        CompanyId = item.CompanyId,
        //                        Company = item.Company,
        //                        Enro_InitDateModule = DateTime.Now,
        //                        Enro_FinishDateModule = a,
        //                        Enro_StateEnrollment = ENROLLMENTSTATE.Activo
        //                    };
        //                    ApplicationDbContext.Enrollments.Add(enro);
        //                    ApplicationDbContext.SaveChanges();
        //                }
        //            }
        //        }
        //    }


        //    var meval = ApplicationDbContext.Modules.Where(x => x.CompanyId == Enrollment.CompanyId && x.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
        //    if (meval.Count != Enrollment.Enrollment.Where(x => x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList().Count)
        //    {
        //        if (meval.Count != 0)
        //        {
        //            foreach (var item in meval)
        //            {
        //                var e = ApplicationDbContext.Enrollments.Where(x => x.User_Id == Enrollment.Id && x.Modu_Id == item.Modu_Id).ToList();
        //                if (e.Count == 0)
        //                {
        //                    DateTime finish = new DateTime();
        //                    switch (item.Modu_Period)
        //                    {
        //                        case VIGENCIA.Dias:
        //                            finish = DateTime.Now.AddDays(item.Modu_Validity);
        //                            break;
        //                        case VIGENCIA.Meses:
        //                            finish = DateTime.Now.AddMonths(item.Modu_Validity);
        //                            break;
        //                        case VIGENCIA.Años:
        //                            finish = DateTime.Now.AddYears(item.Modu_Validity);
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    DateTime a = finish;
        //                    Enrollment enro = new Enrollment
        //                    {
        //                        ApplicationUser = Enrollment,
        //                        User_Id = Enrollment.Id,
        //                        Modu_Id = item.Modu_Id,
        //                        Module = item,
        //                        CompanyId = item.CompanyId,
        //                        Company = item.Company,
        //                        Enro_InitDateModule = DateTime.Now,
        //                        Enro_FinishDateModule = a,
        //                        Enro_StateEnrollment = ENROLLMENTSTATE.Activo
        //                    };
        //                    ApplicationDbContext.Enrollments.Add(enro);
        //                    ApplicationDbContext.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //    bool game = activeGame();
        //    model.game = game;
        //    int Idgame = GameId();
        //    var se = ApplicationDbContext.MG_SettingMp.Find(Idgame);
        //    model.fechajuego = se.Sett_CloseDate;
        //    model.Idgame = Idgame;
        //    var EnrollmentApli = Enrollment.Enrollment.Where(x => x.ApplicationUser.Id == Enrollment.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //    var EnrollmentEvaluative = Enrollment.Enrollment.Where(x => x.ApplicationUser.Id == Enrollment.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
        //    model.Listmoduleevaluative = EnrollmentEvaluative;
        //    model.Listmoduleapplicative = EnrollmentApli;
        //    int CompanyUser = (int)GetActualUserId().CompanyId;
        //    List<Banner> banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();
        //    if (banners != null && banners.Count != 0)
        //    {
        //        model.Banners = banners;
        //    }
        //    else
        //    {
        //        model.Banners = ApplicationDbContext.Banners.Where(x => x.Bann_Id <= 4).ToList();
        //    }
        //    var titles = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
        //    if (titles != null)
        //    {
        //        model.Title1 = titles.Title1;
        //        model.Title2 = titles.Title2;
        //        model.Title3 = titles.Title3;
        //    }
        //    else
        //    {
        //        model.Title1 = ApplicationDbContext.StylesLogos.Find(1).Title1;
        //        model.Title2 = ApplicationDbContext.StylesLogos.Find(1).Title2;
        //        model.Title3 = ApplicationDbContext.StylesLogos.Find(1).Title3;
        //    }
        //    model.Logo = GetUrlLogo();
        //    model.Sesion = GetActualUserId().SesionUser;
        //    model.juego = GetActualUserId().Company.CompanyGame;
        //    var block = ApplicationDbContext.MG_BlockGameUser.Where(x => x.User_Id == Enrollment.Id).OrderByDescending(x => x.BlGa_Id).ToList();
        //    if (block.Count != 0)
        //    {
        //        var bu = block.FirstOrDefault();
        //        model.BlockUser = bu.BlGa_Fecha;
        //    }
        //    return PartialView("_Profile2", model);
        //}

        //Section of artticles 

        [Authorize]
        public ActionResult ViewArticle(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Article articleVisualized = ApplicationDbContext.Articles.Find(id);
            UserInformationArticles model = new UserInformationArticles { ActualRole = (ROLES)GetActualUserId().Role, ViewArticle = articleVisualized, baseUrl = url };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(6);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(304);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = articleVisualized.Arti_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de un articulo con id " + articleVisualized.Arti_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        [Authorize]
        public ActionResult CreateOFCommentToArticle(UserInformationArticles commentsToArticle)
        {
            var articleCurrent = ApplicationDbContext.Articles.Find(commentsToArticle.ViewArticle.Arti_Id);
            if (ModelState.IsValid)
            {
                string user = GetActualUserId().Id;
                ApplicationUser author = UserManager.FindById(user);
                Comments comment = new Comments { Comm_value = 0, comm_Title = commentsToArticle.comm_Title, comm_Description = commentsToArticle.comm_Description, Comm_Date = DateTime.Now, Arti_Id = commentsToArticle.ViewArticle.Arti_Id, Comm_StateComment = (COMMENTSTATE)COMMENTSTATE.Inactivo, comm_Author = author };
                PointsComment pointsObtained = new PointsComment { PoCo_Date = DateTime.Now, ApplicationUser = GetActualUserId(), Comments = comment };
                ApplicationDbContext.Comments.Add(comment);
                ApplicationDbContext.PointsComments.Add(pointsObtained);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(18);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(305);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = comment.Comm_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear un comentario con id " + comment.Comm_Id + " perteneciente al articulo con id " + articleCurrent.Arti_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                SendEmailComment(GetActualUserId().FirstName + " " + GetActualUserId().LastName, GetActualUserId().Company.CompanyName, comment.Article.Arti_Name);
            }
            TempData["Menssage"] = "Su comentario fue creado con éxito. Será analizado por el Administrador para poder ser visto por todos los usuarios.";
            return RedirectToAction("ViewArticle", new { id = commentsToArticle.ViewArticle.Arti_Id });
        }

        public ActionResult UpdateCommentCurrent(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Comments commentToUpdate = ApplicationDbContext.Comments.Find(id);
            int Article = commentToUpdate.Article.Arti_Id;
            Article articleTosendToView = ApplicationDbContext.Articles.Find(Article);
            TempData["UpdateComment"] = "Editar";
            UserInformationArticles model = new UserInformationArticles { ActualRole = GetActualUserId().Role, commentToUpdate = commentToUpdate, ViewArticle = articleTosendToView, baseUrl = url };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            return View("ViewArticle", model);
        }

        [HttpPost]
        public ActionResult UpdateCommentCurrent(UserInformationArticles comment)
        {
            if (ModelState.IsValid)
            {
                var url = HttpRuntime.AppDomainAppVirtualPath;
                Comments commentToUpdate = ApplicationDbContext.Comments.Find(comment.commentToUpdate.Comm_Id);
                int Article = commentToUpdate.Article.Arti_Id;
                Article articleTosendToView = new Article();
                UserInformationArticles model = new UserInformationArticles();
                commentToUpdate.comm_Title = comment.commentToUpdate.comm_Title;
                commentToUpdate.comm_Description = comment.commentToUpdate.comm_Description;
                ApplicationDbContext.SaveChanges();
                articleTosendToView = ApplicationDbContext.Articles.Find(Article);
                model = new UserInformationArticles { ActualRole = GetActualUserId().Role, ViewArticle = articleTosendToView, baseUrl = url };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("ViewArticle", model);
            }
            return View();
        }
        // finish of article
        [Authorize]
        public ActionResult ListModuleVirtual()
        {
            var id = GetActualUserId().Id;
            var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id).ToList();
            UserEnrollmentViewmodel model = new UserEnrollmentViewmodel
            {
                ActualRole = GetActualUserId().Role,
                enrollment = enrollments,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(158);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos virtuales matriculados, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
        public ActionResult ListModuleVirtualAdmin()
        {
            var id = GetActualUserId().Id;
            var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id).ToList();
            UserEnrollmentViewmodel model = new UserEnrollmentViewmodel
            {
                ActualRole = GetActualUserId().Role,
                enrollment = enrollments,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(158);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos virtuales matriculados, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult SearchMVirtual(UserEnrollmentViewmodel model)
        {
            var id = GetActualUserId().Id;
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchModuleVirtual) || string.IsNullOrEmpty(model.SearchModuleVirtual))
            {
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(170);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso  sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ListModuleVirtual");
            }
            else
            {
                List<Enrollment> SearchedModules = ApplicationDbContext.Enrollments.Where(m => m.Module.Modu_Name.Contains(model.SearchModuleVirtual) && m.CompanyId == GetModuleCompany && m.User_Id == id && m.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
                model = new UserEnrollmentViewmodel { ActualRole = GetActualUserId().Role, enrollment = SearchedModules, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(170);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ListModuleVirtual", model);
            }
        }

        //[Authorize]
        //public ActionResult Listmoduleapplicative()
        //{
        //    var id = GetActualUserId().Id;
        //    var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id & x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //    UserEnrollmentViewmodel model = new UserEnrollmentViewmodel
        //    {
        //        ActualRole = GetActualUserId().Role,
        //        enrollment = enrollments,
        //        Logo = GetUrlLogo()
        //    };
        //    model.Sesion = GetActualUserId().SesionUser;
        //    var table = ApplicationDbContext.TableChanges.Find(34);
        //    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //    var code = ApplicationDbContext.CodeLogs.Find(158);
        //    var idcompany = UserCurrent.CompanyId;
        //    if (idcompany != null)
        //    {
        //        var company = ApplicationDbContext.Companies.Find(idcompany);
        //        string ip = IpUser();
        //        var idchange = new IdChange
        //        {
        //            IdCh_IdChange = null
        //        };
        //        ApplicationDbContext.IdChanges.Add(idchange);
        //        ApplicationDbContext.SaveChanges();
        //        Log logsesiontrue = new Log
        //        {
        //            ApplicationUser = UserCurrent,
        //            CoLo_Id = code.CoLo_Id,
        //            CodeLogs = code,
        //            Log_Date = DateTime.Now,
        //            Log_StateLogs = LOGSTATE.Realizado,
        //            TableChange = table,
        //            TaCh_Id = table.TaCh_Id,
        //            IdChange = idchange,
        //            IdCh_Id = idchange.IdCh_Id,
        //            User_Id = UserCurrent.Id,
        //            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos virtuales matriculados, en la compañía con id " + company.CompanyId,
        //            Company = company,
        //            Company_Id = company.CompanyId,
        //            Log_Ip = ip
        //        };
        //        ApplicationDbContext.Logs.Add(logsesiontrue);
        //        ApplicationDbContext.SaveChanges();
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //[Authorize]
        //public ActionResult SearchMApplicative(UserEnrollmentViewmodel model)
        //{
        //    var id = GetActualUserId().Id;
        //    var GetModuleCompany = GetActualUserId().CompanyId;
        //    if (string.IsNullOrWhiteSpace(model.SearchModuleVirtual) || string.IsNullOrEmpty(model.SearchModuleVirtual))
        //    {
        //        var table = ApplicationDbContext.TableChanges.Find(34);
        //        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //        var code = ApplicationDbContext.CodeLogs.Find(170);
        //        var idcompany = UserCurrent.CompanyId;
        //        if (idcompany != null)
        //        {
        //            var company = ApplicationDbContext.Companies.Find(idcompany);
        //            string ip = IpUser();
        //            var idchange = new IdChange
        //            {
        //                IdCh_IdChange = null
        //            };
        //            ApplicationDbContext.IdChanges.Add(idchange);
        //            ApplicationDbContext.SaveChanges();
        //            Log logsesiontrue = new Log
        //            {
        //                ApplicationUser = UserCurrent,
        //                CoLo_Id = code.CoLo_Id,
        //                CodeLogs = code,
        //                Log_Date = DateTime.Now,
        //                Log_StateLogs = LOGSTATE.Realizado,
        //                TableChange = table,
        //                TaCh_Id = table.TaCh_Id,
        //                IdChange = idchange,
        //                IdCh_Id = idchange.IdCh_Id,
        //                User_Id = UserCurrent.Id,
        //                Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso  sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
        //                Company = company,
        //                Company_Id = company.CompanyId,
        //                Log_Ip = ip
        //            };
        //            ApplicationDbContext.Logs.Add(logsesiontrue);
        //            ApplicationDbContext.SaveChanges();
        //        }
        //        return RedirectToAction("Listmoduleapplicative");
        //    }
        //    else
        //    {
        //        List<Enrollment> SearchedModules = ApplicationDbContext.Enrollments.Where(m => m.Module.Modu_Name.Contains(model.SearchModuleVirtual) && m.CompanyId == GetModuleCompany && m.User_Id == id && m.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //        model = new UserEnrollmentViewmodel { ActualRole = GetActualUserId().Role, enrollment = SearchedModules, Logo = GetUrlLogo() };
        //        model.Sesion = GetActualUserId().SesionUser;
        //        var table = ApplicationDbContext.TableChanges.Find(34);
        //        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //        var code = ApplicationDbContext.CodeLogs.Find(170);
        //        var idcompany = UserCurrent.CompanyId;
        //        if (idcompany != null)
        //        {
        //            var company = ApplicationDbContext.Companies.Find(idcompany);
        //            string ip = IpUser();
        //            var idchange = new IdChange
        //            {
        //                IdCh_IdChange = null
        //            };
        //            ApplicationDbContext.IdChanges.Add(idchange);
        //            ApplicationDbContext.SaveChanges();
        //            Log logsesiontrue = new Log
        //            {
        //                ApplicationUser = UserCurrent,
        //                CoLo_Id = code.CoLo_Id,
        //                CodeLogs = code,
        //                Log_Date = DateTime.Now,
        //                Log_StateLogs = LOGSTATE.Realizado,
        //                TableChange = table,
        //                TaCh_Id = table.TaCh_Id,
        //                IdChange = idchange,
        //                IdCh_Id = idchange.IdCh_Id,
        //                User_Id = UserCurrent.Id,
        //                Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
        //                Company = company,
        //                Company_Id = company.CompanyId,
        //                Log_Ip = ip
        //            };
        //            ApplicationDbContext.Logs.Add(logsesiontrue);
        //            ApplicationDbContext.SaveChanges();
        //        }
        //        return View("Listmoduleapplicative", model);
        //    }
        //}
        [Authorize]
        public ActionResult ListModuleEvaluative()
        {
            var id = GetActualUserId().Id;
            var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id & x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            UserEnrollmentViewmodel model = new UserEnrollmentViewmodel
            {
                ActualRole = GetActualUserId().Role,
                enrollment = enrollments,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(160);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos evaluativos matriculados, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult SearchMEvaluative(UserEnrollmentViewmodel model)
        {
            var id = GetActualUserId().Id;
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchModuleEvaluative) || string.IsNullOrEmpty(model.SearchModuleEvaluative))
            {
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(170);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso  sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ListModuleEvaluative");
            }
            else
            {
                List<Enrollment> SearchedModules = ApplicationDbContext.Enrollments.Where(m => m.Module.Modu_Name.Contains(model.SearchModuleEvaluative) && m.CompanyId == GetModuleCompany && m.User_Id == id && m.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
                model = new UserEnrollmentViewmodel { ActualRole = GetActualUserId().Role, enrollment = SearchedModules, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(170);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ListModuleEvaluative", model);
            }
        }

        [Authorize]
        public ActionResult ListArticle()
        {
            var table = ApplicationDbContext.TableChanges.Find(6);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(314);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los articulos disponibles, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            var id = GetActualUserId().Id;
            var articles = ApplicationDbContext.Articles.Where(x => x.Section.Edition.CompanyId == idcompany).ToList();
            UserArticleViewModel model = new UserArticleViewModel
            {
                ActualRole = GetActualUserId().Role,
                articles = articles,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchArticle(UserArticleViewModel model)
        {
            var id = GetActualUserId().Id;
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchArticle) || string.IsNullOrEmpty(model.SearchArticle))
            {
                var table = ApplicationDbContext.TableChanges.Find(6);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(315);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un articulo sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ListArticle");
            }
            else
            {
                List<Article> SearchedArticle = ApplicationDbContext.Articles.Where(m => m.Arti_Name.Contains(model.SearchArticle) && m.Section.Edition.CompanyId == GetModuleCompany).ToList();
                model = new UserArticleViewModel { ActualRole = GetActualUserId().Role, articles = SearchedArticle, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(6);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(315);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un articulo ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ListArticle", model);
            }
        }

        [Authorize]
        public ActionResult Grades(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            ApplicationUser user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            Module mod = ApplicationDbContext.Modules.Find(id);
            var enrollments = ApplicationDbContext.Enrollments.Single(x => x.Modu_Id == id && x.User_Id == user.Id);
            var AdvanceUser = user.AdvanceUser.Where(x => x.User_Id == user.Id && x.TopicsCourse.Modu_Id == enrollments.Modu_Id).ToList();
            var Attempt = user.Attempts.Where(x => x.UserId == user.Id && x.BankQuestion.TopicsCourse.Modu_Id == enrollments.Modu_Id).ToList();
            var listenrollments = enrollments.Module.TopicsCourse.Where(x => x.Modu_Id == id && x.ToCo_Visible == FORO.Si).OrderBy(x => x.ToCo_Id).ToList();
            var listCompletedFlashTests = ApplicationDbContext.UserModuleAdvances.Where(x => x.Enro_id == enrollments.Enro_Id).ToList();
            var flashtestloaded = ApplicationDbContext.FlashTest.Where(x => x.TopicsCourse.Modu_Id == enrollments.Modu_Id).ToList();
            UserGeneralViewModel model = new UserGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Enrollment = enrollments,
                user = user,
                listattempts = Attempt,
                listadvanceuser = AdvanceUser,
                baseUrl = url,
                listenrollment = listenrollments,
                userFlashTestResults = listCompletedFlashTests,
                flashTests = flashtestloaded,
                Modules = mod
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = new CodeLogs();
            code = ApplicationDbContext.CodeLogs.Find(159);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = enrollments.Enro_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un curso ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Grades", model);
        }

        [Authorize]
        public ActionResult BetterPractice(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            ApplicationUser user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var module = ApplicationDbContext.Modules.Find(id);
            TempData["BetterPractice"] = "BetterPractice";
            UserGeneralViewModel model = new UserGeneralViewModel { ActualRole = GetActualUserId().Role, Modu_Id = id, Modules = module };
            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
        }


        [Authorize]
        public ActionResult Improvement(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            ApplicationUser user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            Module moduled = ApplicationDbContext.Modules.Find(id);
            TempData["Improvement"] = "Improvement";
            UserGeneralViewModel model = new UserGeneralViewModel { ActualRole = GetActualUserId().Role, Modu_Id = id, Modules = moduled };
            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddBetterPractice(UserGeneralViewModel model, HttpPostedFileBase upload)
        {
            var GetModules = ApplicationDbContext.Modules.Find(model.Enrollment.Module.Modu_Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (5 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".doc", ".pdf", ".xlsx ", ".docx", ".pptx", ".mp3" };
                    var files = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(files))
                        {
                            files = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourceBetterPractice/" + files));
                            string ruta = files;
                            TempData["Result"] = "El documento que has adjuntado ha sido subido con éxito.";
                            TempData["datos1"] = "Recuerda que puedes realizar el número de vinculaciones que desees y recibirás puntos por todas. Continúa con UnAula y beneficia con este conocimiento a toda la comunidad.";

                            ApplicationUser getuseractual = GetActualUserId();
                            BetterPractice betterpractice = new BetterPractice { BePr_TiTle = model.BePr_TiTle, BePr_InitDate = DateTime.Now, BePr_Comment = model.BePr_Comment, BePr_Resource = ruta, BePr_Name = model.BePr_Name, Module = GetModules, ApplicationUser = getuseractual };
                            ApplicationDbContext.BetterPractices.Add(betterpractice);
                            ApplicationDbContext.SaveChanges();
                            SendEmailBetterPractice(GetActualUserId().FirstName + " " + GetActualUserId().LastName, GetActualUserId().Company.CompanyName, betterpractice.Module.Modu_Name);
                            var table = ApplicationDbContext.TableChanges.Find(10);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(162);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = betterpractice.BePr_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar un aporte de buena practica con id " + betterpractice.BePr_Id + " perteneciente al curso con id " + betterpractice.Module.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                        }
                    }
                    TempData["Result"] = "El formato del archivo no es valido";
                    return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                }
                else
                {
                    TempData["Result"] = "El documento que has adjuntado ha sido subido con éxito.";
                    TempData["datos1"] = "Recuerda que puedes realizar el número de vinculaciones que desees y recibirás puntos por todas. Continúa haciendo ECO junto a UnAula y beneficia con este conocimiento a toda la comunidad.";
                    ApplicationUser getuseractual = GetActualUserId();
                    BetterPractice betterpractice = new BetterPractice { BePr_TiTle = model.BePr_TiTle, BePr_InitDate = DateTime.Now, BePr_Comment = model.BePr_Comment, BePr_Name = model.BePr_Name, Module = GetModules, ApplicationUser = getuseractual };
                    ApplicationDbContext.BetterPractices.Add(betterpractice);
                    ApplicationDbContext.SaveChanges();
                    SendEmailBetterPractice(GetActualUserId().FirstName + " " + GetActualUserId().LastName, GetActualUserId().Company.CompanyName, betterpractice.Module.Modu_Name);
                    var table = ApplicationDbContext.TableChanges.Find(10);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(162);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = betterpractice.BePr_Id.ToString()
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar un aporte de buena practica con id " + betterpractice.BePr_Id + " perteneciente al curso con id " + betterpractice.Module.Modu_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                }
            }
            else
            {
                TempData["Result"] = "Los campos no pueden estar vacios";
                return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddImprovement(UserGeneralViewModel model, HttpPostedFileBase carga)
        {
            var GetModules = ApplicationDbContext.Modules.Find(model.Enrollment.Module.Modu_Id);
            if (ModelState.IsValid)
            {
                if (carga != null && carga.ContentLength <= (5 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".doc", ".pdf", ".xlsx ", ".docx", ".pptx", ".mp3" };
                    var imagen = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + carga.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(imagen))
                        {
                            imagen = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + carga.FileName).ToLower();
                            carga.SaveAs(Server.MapPath("~/ResourcesImprovement/" + imagen));
                            string ruta = imagen;
                            TempData["Result"] = "Programa ECO: ";
                            TempData["datos1"] = "Tu comentario ha sido enviado para aprobación al administrador. Agradecemos tu comentario, este tipo de interacción nos ayuda a mantenernos conectados y haciendo ECO. En un máximo de 5 días hábiles recibirás respuesta.";
                            TempData["datos2"] = "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: UnAulaMI@gmail.com";
                            ApplicationUser getuseractual = GetActualUserId();
                            Improvement improvement = new Improvement { Impr_Title = model.Impr_Title, Impr_InitDate = DateTime.Now, Impr_Comment = model.Impr_Comment, Impr_Comment2 = model.Impr_Comment2, Impr_Resource = ruta, Impr_Name = model.Impr_Name, Module = GetModules, ApplicationUser = getuseractual };
                            ApplicationDbContext.Improvements.Add(improvement);
                            ApplicationDbContext.SaveChanges();
                            SendEmailImprovement(GetActualUserId().FirstName + " " + GetActualUserId().LastName, GetActualUserId().Company.CompanyName, improvement.Module.Modu_Name);
                            var table = ApplicationDbContext.TableChanges.Find(27);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(163);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = improvement.Impr_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar un aporte de mejora con id " + improvement.Impr_Id + " perteneciente al curso con id " + improvement.Module.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                        }
                    }
                    TempData["Result"] = "El formato del archivo no es valido";
                    return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                }
                else
                {
                    TempData["Result"] = "Programa ECO: ";
                    TempData["datos1"] = "Tu comentario ha sido enviado para aprobación al administrador. Agradecemos tu comentario, este tipo de interacción nos ayuda a mantenernos conectados y haciendo ECO. En un máximo de 5 días hábiles recibirás respuesta.";
                    ApplicationUser getuseractual = GetActualUserId();
                    Improvement improvement = new Improvement { Impr_Title = model.Impr_Title, Impr_InitDate = DateTime.Now, Impr_Comment = model.Impr_Comment, Impr_Comment2 = model.Impr_Comment2, Impr_Name = model.Impr_Name, Module = GetModules, ApplicationUser = getuseractual };
                    ApplicationDbContext.Improvements.Add(improvement);
                    ApplicationDbContext.SaveChanges();
                    SendEmailImprovement(GetActualUserId().FirstName + " " + GetActualUserId().LastName, GetActualUserId().Company.CompanyName, improvement.Module.Modu_Name);
                    var table = ApplicationDbContext.TableChanges.Find(27);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(163);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = improvement.Impr_Id.ToString()
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar un aporte de mejora con id " + improvement.Impr_Id + " perteneciente al curso con id " + improvement.Module.Modu_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
                }
            }
            else
            {
                TempData["Result"] = "Los campos no pueden estar vacios";
                return RedirectToAction("Grades", new { id = model.Enrollment.Module.Modu_Id });
            }
        }

        [Authorize]
        public ActionResult Exchange()
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            List<Prize> PrizeOfTheCompany = ApplicationDbContext.Prizes.Where(x => x.CompanyId == GetModuleCompanyId).ToList();
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            int PointOfUserActuallity = UserActuallity.Point.Sum(x => x.Quantity_Points);
            var l = ApplicationDbContext.Points.Where(x => x.User_Id == UserActuallity.Id).ToList();
            //int i = UserActuallity.PointsComment.Sum(x => x.Comments.comm_Author.Point);
            UserExchangeViewModel model = new UserExchangeViewModel { ActualRole = GetActualUserId().Role, ListPoint = l, ListPrize = PrizeOfTheCompany, TotalPointUser = PointOfUserActuallity };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(46);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(317);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de premios y puntos, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        public ActionResult Report()
        {
            var UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                            from u in ApplicationDbContext.Points.Where(x => x.User_Id == UserActuallity.Id).ToList()
                            select new
                            {
                                UserName = u.ApplicationUser.UserName,
                                Documento = u.ApplicationUser.Document,
                                Nombre = u.ApplicationUser.FirstName + " " + u.ApplicationUser.LastName,
                                Cargo = u.ApplicationUser.Position.Posi_Description,
                                Area = u.ApplicationUser.Area.AreaName,
                                Ubicacion = u.ApplicationUser.Location.Loca_Description,
                                Pais = u.ApplicationUser.Country,
                                Ciudad = u.ApplicationUser.City.City_Name,
                                Tipo_de_Usuario = u.ApplicationUser.Role,
                                Tipo_punto = u.TypePoint.Poin_TypePoints,
                                Categoría = u.TypePoint.TyPo_Description,
                                Puntos_Obtenidos = u.Quantity_Points,
                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=PUNTOS USUARIO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE PUNTOS USUARIO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Puntos usuario ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Exchange");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Exchange");
        }

        [Authorize]
        public ActionResult ExchangeUser(int id)
        {
            int? GetModuleCompanyId = GetActualUserId().CompanyId;
            ApplicationUser getUserActual = UserManager.FindById(GetActualUserId().Id);
            Prize PrizeUser = ApplicationDbContext.Prizes.Find(id);
            int PointOfUserActuallity = getUserActual.Point.Sum(x => x.Quantity_Points);
            int ValorProducto = PrizeUser.Priz_RequiredPoints;
            if (PointOfUserActuallity < ValorProducto)
            {
                TempData["Menssages"] = "No cuenta con los  puntos suficientes para postularse a este premio ";
            }
            else
            {
                if (VerifyRequestExchange(id) <= 0)
                {
                    Exchange exchange = new Exchange { User = getUserActual, Prize = PrizeUser, Exch_PointUser = PointOfUserActuallity, Exch_date = DateTime.Now, StateExchange = Enum.STATEEXCHANGE.Pendiente };
                    ApplicationDbContext.Exchanges.Add(exchange);
                    ApplicationDbContext.SaveChanges();
                    TempData["Menssages"] = "Programa ECO: Tu solicitud de participación ha sido recibida";
                    TempData["Menssages1"] = "¡Felicitaciones! Has solicitado participar en el sorteo mensual de premios. ¡Sigue participando y hagamos ECO juntos!";
                    TempData["Menssages2"] = "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: UnAulaMi@gmail.com";
                    SendEmailExchange(GetActualUserId().FirstName + " " + GetActualUserId().LastName);
                    var table = ApplicationDbContext.TableChanges.Find(23);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(318);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = exchange.Exch_Id.ToString()
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una solicitud de canje con id " + exchange.Exch_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }

                }
                else
                {
                    TempData["Menssages"] = "Usted ya se ha postulado para este premio y está en espera de respuesta por parte del administrador";
                }
            }
            return RedirectToAction("Exchange");
        }
        public int VerifyRequestExchange(int id)
        {
            string getUserActual = GetActualUserId().Id;
            int RequestSendForUSer = ApplicationDbContext.Exchanges.Where(x => x.User.Id == getUserActual && x.Prize.Priz_Id == id && x.StateExchange == Enum.STATEEXCHANGE.Pendiente).Count();
            return RequestSendForUSer;
        }

        [Authorize]
        public ActionResult TestUser(int id)
        {

            TopicsCourse topic = ApplicationDbContext.TopicsCourses.Find(id);
            var bankquestioid = topic.BankQuestion.Where(x => x.ToCo_Id == topic.ToCo_Id).ToDictionary(x => x.BaQu_Id);
            int idbank = bankquestioid.Select(X => X.Value.BaQu_Id).Single();
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(idbank);

            ApplicationUser UserActual = GetActualUserId();
            AdvanceUser userAdva = ApplicationDbContext.AdvanceUsers.Where(z => z.ToCo_id == id && z.User_Id == UserActual.Id).FirstOrDefault();
            var usuario = User.Identity.GetUserId();
            var Attempt = ApplicationDbContext.Attempts.Where(x => x.UserId == usuario && x.BankQuestion.TopicsCourse.ToCo_Id == topic.ToCo_Id).ToList();

            if (topic.ToCo_Attempt <= Attempt.Count)
            {
                return RedirectToAction("ListModuleVirtual");
            }
            if (userAdva != null)
            {
                var module = ApplicationDbContext.Modules.Find(actualbank.TopicsCourse.Modu_Id);
                int eva = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Modu_Id == module.Modu_Id).ToList().Count;
                double pointEvalution = module.Modu_Points / eva;
                double PointOfUser = ((userAdva.AdUs_ScoreObtained * 100) / pointEvalution);


                UserTestViewModel model = new UserTestViewModel
                {
                    acces = false,
                    UserLog = UserActual.FirstName + " " + UserActual.LastName,
                    pointsObtained = (int)Math.Round(PointOfUser),
                    ActualRole = UserActual.Role
                };
                model.Logo = GetUrlLogo();
                model.Sesion = GetActualUserId().SesionUser;

                return View(model);
            }
            else
            {
                var pairing = actualbank.Pairing.ToList();
                List<GeneralQuestions> modelgeneralquestions = new List<GeneralQuestions>();
                List<answerpairing> modelgeneralanswerpairing = new List<answerpairing>();
                List<optionmultiple> modeloptionmultiple = new List<optionmultiple>();
                foreach (OptionMultiple optionmultiple in actualbank.OptionMultiple)
                {
                    modeloptionmultiple.Add(new optionmultiple
                    {
                        BaQu_Id = optionmultiple.BaQu_Id,
                        OpMu_Id = optionmultiple.OpMu_Id,
                        OpMu_Question = optionmultiple.OpMu_Question,
                        OpMu_Description = optionmultiple.OpMu_Description,
                        OpMult_Content = optionmultiple.OpMult_Content,
                        listanswer = optionmultiple.AnswerOptionMultiple.ToList()

                    });
                }
                foreach (OptionMultiple optionmultiple in actualbank.OptionMultiple)
                {
                    modelgeneralquestions.Add(new GeneralQuestions
                    {
                        TypeQuestion = TYPEQUESTIONS.OptionMultiple,
                        BaQu_Id = optionmultiple.BaQu_Id,
                        OpMu_Id = optionmultiple.OpMu_Id,
                        OpMu_Question = optionmultiple.OpMu_Question,
                        OpMu_Description = optionmultiple.OpMu_Description,
                        OpMult_Content = optionmultiple.OpMult_Content,
                        listanswerOM = optionmultiple.AnswerOptionMultiple.ToList()

                    });
                }
                List<pairing> modelpairing = new List<pairing>();
                foreach (Pairing pairings in actualbank.Pairing)
                {
                    modelpairing.Add(new pairing
                    {
                        BaQu_Id = pairings.BaQu_Id,
                        Pair_Id = pairings.Pair_Id,
                        Pair_Question = pairings.Pair_Question,
                        Pair_Description = pairings.Pair_Description,
                        AnswerPairing = GetAnswerPairing(pairings.BaQu_Id, pairings.Pair_Id),
                        listanswerpairing = pairings.AnswerPairing.ToList(),

                    });
                }
                foreach (Pairing pairings in actualbank.Pairing)
                {
                    modelgeneralquestions.Add(new GeneralQuestions
                    {
                        TypeQuestion = TYPEQUESTIONS.Pairing,
                        BaQu_Id = pairings.BaQu_Id,
                        Pair_Id = pairings.Pair_Id,
                        Pair_Question = pairings.Pair_Question,
                        Pair_Description = pairings.Pair_Description,
                        AnswerPairing = GetAnswerPairing(pairings.BaQu_Id, pairings.Pair_Id),
                        listanswerpairing = pairings.AnswerPairing.ToList()
                    });
                }
                List<trueorfalse> modeltrueorfalse = new List<trueorfalse>();
                foreach (TrueOrFalse trueorfalse in actualbank.TrueOrFalse)
                {
                    modeltrueorfalse.Add(new trueorfalse
                    {
                        TrFa_Id = trueorfalse.TrFa_Id,
                        TrFa_Question = trueorfalse.TrFa_Question,
                        TrFa_Description = trueorfalse.TrFa_Description,
                        TrFa_Content = trueorfalse.TrFa_Content,
                        BaQu_Id = trueorfalse.BaQu_Id,
                        TrFa_State = trueorfalse.TrFa_State
                    });
                }
                foreach (TrueOrFalse trueorfalse in actualbank.TrueOrFalse)
                {
                    modelgeneralquestions.Add(new GeneralQuestions
                    {
                        TypeQuestion = TYPEQUESTIONS.TrueorFalse,
                        BaQu_Id = trueorfalse.BaQu_Id,
                        TrFa_Id = trueorfalse.TrFa_Id,
                        TrFa_Question = trueorfalse.TrFa_Question,
                        TrFa_Description = trueorfalse.TrFa_Description,
                        TrFa_Content = trueorfalse.TrFa_Content,
                        TrFa_State = trueorfalse.TrFa_State
                    });
                }
                var rnd = new Random();
                var shuffledList = modelgeneralquestions.OrderBy(x => rnd.Next()).ToList();
                var modelgeneralquestionsr = shuffledList.Take(actualbank.BaQu_QuestionUser);
                List<GeneralQuestions> modelgeneralquestionsa = new List<GeneralQuestions>(modelgeneralquestionsr);
                foreach (var item in modelgeneralquestionsa)
                {
                    if (item.TypeQuestion == TYPEQUESTIONS.Pairing)
                    {
                        foreach (var item1 in item.listanswerpairing)
                        {
                            modelgeneralanswerpairing.Add(new answerpairing
                            {
                                Pair_Id = item1.Pair_Id,
                                AnPa_IdOption = item1.AnPa_Id,
                                AnPa_OptionsQuestion = item1.AnPa_OptionsQuestion
                            });
                        }
                    }
                }

                UserTestViewModel model = new UserTestViewModel
                {
                    acces = true,
                    ActualRole = GetActualUserId().Role,
                    BankQuestion = actualbank,
                    BaQu_Id = actualbank.BaQu_Id,
                    ToCo_Id = topic.ToCo_Id,
                    topic = topic,
                    listmodeloptionmultiple = modeloptionmultiple,
                    listmodelpairing = modelpairing,
                    listmodeltrueorfalse = modeltrueorfalse,
                    Listgeneralquestion = modelgeneralquestionsa,
                    Listrequestpairing = modelgeneralanswerpairing
                };
                model.Logo = GetUrlLogo();
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(8);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(164);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = actualbank.BaQu_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la evaluación con id " + actualbank.BaQu_Id + "perteneciente al tema con id " + actualbank.ToCo_Id + "del curso con id " + actualbank.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View(model);
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult AnswerTest(UserTestViewModel model)
        {
            var getcompany = GetActualUserId().Company.CompanyId;
            int PointOptionMultiple = resultOptionMultiple(model.Listgeneralquestion);
            int PointPairing = resultPairing(model.Listgeneralquestion, model.Listrequestpairing);
            int PointTrueorFalse = resultTrueOrFalse(model.Listgeneralquestion);
            int TotalPoints = PointOptionMultiple + PointPairing + PointTrueorFalse;
            int TotalQuestions = model.Listgeneralquestion.Count;
            double PointTest = AsignarPuntajedelTest(TotalPoints, model.BaQu_Id, model.topic.Module.Modu_Id, TotalQuestions);
            //return RedirectToAction("Grades", new { id = model.topic.Module.Modu_Id });
            return RedirectToAction("ResulQuestions", new { id = model.BaQu_Id });
        }
        [Authorize]

        public ActionResult ResulQuestions(int id)

        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            string Id = GetActualUserId().Id;
            ApplicationUser User = UserManager.FindById(Id);
            var truefalse = ApplicationDbContext.TrueOrFalseStudent.Where(x => x.User_Id == User.Id).ToList();



            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(id);



             int TotalQuestion = actualbank.OptionMultiple.Count() + actualbank.Pairing.Count() + actualbank.TrueOrFalse.Count();

            List<pairing> modelpairing = new List<pairing>();
            foreach (Pairing pairings in actualbank.Pairing)
            {
                modelpairing.Add(new pairing
                {
                    BaQu_Id = pairings.BaQu_Id,
                    Pair_Id = pairings.Pair_Id,
                    Pair_Question = pairings.Pair_Question,
                    Pair_Description = pairings.Pair_Description,
                    AnswerPairing = GetAnswerPairing(pairings.BaQu_Id, pairings.Pair_Id),
                    listanswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.Pair_Id == pairings.Pair_Id).ToList()
                });

            }

            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                userId = User.Id,
                ActualRole = GetActualUserId().Role,
                BaQu_Id = id,
                BankQuestion = actualbank,
                TotalQuestion = TotalQuestion,
                ListPairing = modelpairing,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(8);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(309);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = actualbank.BaQu_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();


            }

            var porcentajeVerdadero = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == model.userId && x.ToCo_id == actualbank.ToCo_Id).ToList().FirstOrDefault();
            var porcentajeFalso = ApplicationDbContext.AdvanceLoseUser.Where(x => x.User_Id == model.userId && x.ToCo_id == actualbank.ToCo_Id).ToList().FirstOrDefault();

            if (porcentajeVerdadero != null)
            {
                ViewData["Calificacion"] = "Aprobaste el módulo";

            }
            else
            {
                ViewData["Calificacion"] = "No aprobaste el módulo";

            }

            return View(model);

        }

        //Revisar como se realiza el calculo de puntos To-Do
        // (cantidad de respuesta correctas/total de respuestas)*100
        public double AsignarPuntajedelTest(int TotalPoints, int BaQu_Id, int Modu_Id, int TotalQuestions)
        {
            ApplicationUser useractial = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var enrollments = ApplicationDbContext.Enrollments.Single(x => x.Modu_Id == Modu_Id && x.User_Id == useractial.Id);
            BankQuestion BankQuestion = ApplicationDbContext.BankQuestions.Find(BaQu_Id);
            int TotalTopic = ApplicationDbContext.TopicsCourses.Where(x => x.Module.Modu_Id == enrollments.Module.Modu_Id && x.ToCo_RequiredEvaluation == REQUIREDEVALUATION.Si).ToList().Count();
            double PountTest = (double)enrollments.Module.Modu_Points / (double)TotalTopic;
            int TotalQuestion = TotalQuestions;
            double PointForQuestion = PountTest / (double)TotalQuestion;
            double Result = TotalPoints * PointForQuestion;
            double porcentajeRestriccion = (double)BankQuestion.BaQu_Porcentaje / (double)100;
            double porcentajetest = (double)PountTest * porcentajeRestriccion;
            double porcentajeObtenido = (double)Result;
            if (porcentajeObtenido >= porcentajetest)
            {
                int ax = (int)porcentajeObtenido;
                int bx = (int)PountTest;

                TopicsCourse topic = ApplicationDbContext.TopicsCourses.Find(BankQuestion.TopicsCourse.ToCo_Id);

                var vali = ApplicationDbContext.AdvanceUsers.Where(x => x.ToCo_id == topic.ToCo_Id && x.User_Id == useractial.Id).ToList();
                var valiadvance = ApplicationDbContext.Advances.Where(x => x.Modulo_Id == topic.Modu_Id).ToList();
                if (vali.Count == 0)
                {
                    AdvanceUser advanceuser = new AdvanceUser
                    {
                        ApplicationUser = useractial,
                        TopicsCourse = topic,
                        AdUs_PresentDate = DateTime.Now,
                        AdUs_ScoreObtained = porcentajeObtenido,
                    };

                    if (valiadvance.Count == 0)
                    {
                        Advance advance = new Advance
                        {
                            Modulo_Id = enrollments.Modu_Id,
                            FechaActualizacion = DateTime.Now,
                            Score = (float)porcentajeObtenido,
                            Usuario_Id = useractial.Id,
                        };
                        ApplicationDbContext.Advances.Add(advance);
                        ApplicationDbContext.SaveChanges();

                    }
                    else 
                    {
                        ApplicationDbContext.Advances.Find(topic.Modu_Id);
                        Advance advance = new Advance
                        {
                            Modulo_Id = enrollments.Modu_Id,
                            FechaActualizacion = DateTime.Now,
                            Score = (float)porcentajeObtenido,
                            Usuario_Id = useractial.Id,
                        };                        
                        ApplicationDbContext.SaveChanges();
                        
                    }
             
                    ApplicationDbContext.AdvanceUsers.Add(advanceuser);                    
                    ApplicationDbContext.SaveChanges();

                   



                    TempData["Result"] = "¡ Has Realizado tu intento de evaluación";// + "El puntaje obtenido fue : " + ax + "   de un total de  " + bx + " puntos asignados";
                    var advanceusers = useractial.AdvanceUser.Where(x => x.TopicsCourse.Module.Modu_Id == enrollments.Module.Modu_Id).ToList().Count();
                    if (advanceusers == TotalTopic)
                    {
                        var Point = useractial.Point.Where(x => x.User_Id == useractial.Id && x.TypePoint.Poin_TypePoints == TYPEPOINTS.LMS).ToList();
                        if (Point.Count == 0)
                        {
                            TempData["Result"] = "¡ Has Realizado tu intento de evaluación";// + "El puntaje obtenido fue : " + ax + "   de un total de  " + bx + " puntos asignados";
                            var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.Poin_TypePoints == TYPEPOINTS.LMS);
                            Point Points = new Point { ApplicationUser = useractial, TypePoint = cate, TyPo_Id = cate.TyPo_Id, Poin_Date = DateTime.Now, Poin_End_Date = DateTime.Now.AddYears(1), Quantity_Points = 100 };
                            ApplicationDbContext.Points.Add(Points);
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(41);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(165);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = Points.Poin_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ha pasado la evaluación con id " + BankQuestion.BaQu_Id + "perteneciente al tema con id " + BankQuestion.ToCo_Id + "del curso con id " + BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            TempData["Result"] = "¡ Has Realizado tu intento. " + "El puntaje obtenido fue : " + ax + "   de un total de  " + bx + " puntos asignados";
                            var Points = Point.Single(x => x.User_Id == x.User_Id);
                            var a = Points.Poin_Id;
                            Point PoinTNew = ApplicationDbContext.Points.Find(a);
                            PoinTNew.Quantity_Points = PoinTNew.Quantity_Points + Convert.ToInt32(Result);
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(41);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(165);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = PoinTNew.Poin_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ha pasado la evaluación con id " + BankQuestion.BaQu_Id + "perteneciente al tema con id " + BankQuestion.ToCo_Id + "del curso con id " + BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        var certification = useractial.Certification.Where(x => x.Enro_Id == enrollments.Enro_Id).ToList().Count();
                        if (certification == 0)
                        {
                            Certification CertificationUser = new Certification
                            {
                                ApplicationUser = useractial,
                                Cert_Date = DateTime.Now,
                                Enrollment = enrollments
                            };
                          
                            ApplicationDbContext.Certifications.Add(CertificationUser);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    var advanceModule = enrollments.AdvanceCourse.Count();
                    var advanceuser1 = topic.AdvanceUser.Where(x => x.ToCo_id == topic.ToCo_Id && x.User_Id == useractial.Id).ToList();
                    AdvanceUser advanceuseractual = advanceuser1.Single(x => x.ToCo_id == topic.ToCo_Id);
                    if (advanceModule == 0)
                    {
                        int var = (int)advanceuseractual.AdUs_ScoreObtained * 100 / enrollments.Module.Modu_Points;
                        AdvanceCourse advancemodule = new AdvanceCourse
                        {
                            Enrollment = enrollments,
                            ApplicationUser = useractial,
                            AdvanceUser = advanceuseractual,
                            AdCo_ScoreObtanied = var
                        };
                        ApplicationDbContext.AdvanceCourses.Add(advancemodule);
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        int result = (int)advanceuseractual.AdUs_ScoreObtained * 100 / enrollments.Module.Modu_Points;
                        var AdvanceModule = enrollments.AdvanceCourse.Single(x => x.User_Id == advanceuseractual.User_Id);
                        AdvanceModule.AdCo_ScoreObtanied = AdvanceModule.AdCo_ScoreObtanied + result;
                        if (AdvanceModule.AdCo_ScoreObtanied >= 89)
                        {
                            AdvanceModule.AdCo_ScoreObtanied = 100;
                            ApplicationDbContext.SaveChanges();
                        }
                        ApplicationDbContext.SaveChanges();
                    }
                }
                else
                {
                    TempData["Error"] = "USTED YA TIENE UN REGISTRO ASOCIADO A ESTA EVALUACIÓN";
                }
            }
            else
            {

                TopicsCourse topic = ApplicationDbContext.TopicsCourses.Find(BankQuestion.TopicsCourse.ToCo_Id);
                var vali = ApplicationDbContext.AdvanceLoseUser.Where(x => x.ToCo_id == topic.ToCo_Id && x.User_Id == useractial.Id).ToList();

                var valiadvance = ApplicationDbContext.Advances.Where(x => x.Modulo_Id == topic.Modu_Id).ToList();

                AdvanceLoseUser advanceloseuser = new AdvanceLoseUser

                {
                    ApplicationUser = useractial,
                    TopicsCourse = topic,
                    AdLoUs_PresentDate = DateTime.Now,
                    AdLoUs_ScoreObtained = porcentajeObtenido,
                };

                if (valiadvance.Count == 0)
                {
                    Advance advance = new Advance
                    {
                        Modulo_Id = enrollments.Modu_Id,
                        FechaActualizacion = DateTime.Now,
                        Score = (float)porcentajeObtenido,
                        Usuario_Id = useractial.Id,
                    };
                    ApplicationDbContext.Advances.Add(advance);
                    ApplicationDbContext.SaveChanges();

                }
                else
                {
                    ApplicationDbContext.Advances.Find(topic.Modu_Id);
                    Advance advance = new Advance
                    {
                        Modulo_Id = enrollments.Modu_Id,
                        FechaActualizacion = DateTime.Now,
                        Score = (float)porcentajeObtenido,
                        Usuario_Id = useractial.Id,
                    };
                    ApplicationDbContext.SaveChanges();

                }               
                
                ApplicationDbContext.AdvanceLoseUser.Add(advanceloseuser);
                ApplicationDbContext.SaveChanges();
              

                int a = (int)porcentajeObtenido;
                int b = (int)PountTest;
                TempData["Result"] = "El puntaje obtenido fue : " + a + "  de un total de  " + b;
                TempData["Result1"] = "Mal-100.jpg";
                Attempts AttemptsUser = new Attempts
                {
                    BankQuestion = BankQuestion,
                    ApplicationUser = useractial,
                    Atte_FinishDate = DateTime.Now,
                    Atte_InintDate = DateTime.Now,
                    UserId = useractial.Id,
                    BaQu_Id = BankQuestion.BaQu_Id
                };
                ApplicationDbContext.Attempts.Add(AttemptsUser);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(7);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(165);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = AttemptsUser.Atte_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " No a pasado la evaluación con id " + BankQuestion.BaQu_Id + "perteneciente al tema con id " + BankQuestion.ToCo_Id + "del curso con id " + BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
            }


            TopicsCourse topic1 = ApplicationDbContext.TopicsCourses.Find(BankQuestion.TopicsCourse.ToCo_Id);
            var valiUse = ApplicationDbContext.AdvanceUsers.Where(x => x.ToCo_id == topic1.ToCo_Id && x.User_Id == useractial.Id).ToList();
            var valiLose = ApplicationDbContext.AdvanceLoseUser.Where(x => x.ToCo_id == topic1.ToCo_Id && x.User_Id == useractial.Id).ToList();
           


            return Result;
        }
        public int resultOptionMultiple(List<GeneralQuestions> result)
        {
            var sum = 0;
            if (result != null)
            {
                foreach (var item in result)


                {


                    if (item.TypeQuestion == TYPEQUESTIONS.OptionMultiple)
                    {
                        var b = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.OptionMultiple.OpMu_Id == item.OpMu_Id).ToList();

                        var a = ApplicationDbContext.AnswerOptionMultiples.Find(item.AnOp_Id);
                        var c = GetActualUserId();

                        AnswerOptionMultipleStudent module = new AnswerOptionMultipleStudent

                        {
                            User_Id = c.Id,
                            AnOp_Id = a.AnOp_Id,
                            AnOp_OptionAnswer = a.AnOp_OptionAnswer,
                            AnOp_TrueAnswer = a.AnOp_TrueAnswer,
                            Answer_OpMult_Content = a.Answer_OpMult_Content,
                            OpMu_Id = a.OpMu_Id,
                            Date_Present_test = DateTime.Now

                        };

                        ApplicationDbContext.AnswerOptionMultipleStudent.Add(module);
                        ApplicationDbContext.SaveChanges();


                        AnswerOptionMultiple z = b.Single(x => x.AnOp_TrueAnswer == Enum.OPTIONANSWER.Verdadero);
                        if (z.AnOp_Id == item.AnOp_Id)
                        {
                            sum = sum + 1;
                        }
                    }

                }
            }
            return sum;
        }

        public int resultPairing(List<GeneralQuestions> result, List<answerpairing> answer)
        {
            var sum = 0;
            var sum2 = 0;
            if (result != null && answer != null)
            {
                foreach (var item in result)
                {

                    if (item.TypeQuestion == TYPEQUESTIONS.Pairing)
                    {

                        foreach (var item1 in answer)
                        {
                            var b = ApplicationDbContext.AnswerPairings.Find(item1.AnPa_IdRequest);
                            var c = GetActualUserId();



                            AnserPairingStudent module = new AnserPairingStudent

                            {

                                User_Id = c.Id,
                                AnPa_Id = item.AnPa_Id,
                                AnPa_OptionsQuestion = item1.AnPa_OptionsQuestion,
                                AnPa_OptionAnswer = b.AnPa_OptionAnswer,
                                Pair_Id = item1.Pair_Id,
                                Date_Present_test = DateTime.Now,

                            };

                            ApplicationDbContext.AnserPairingStudent.Add(module);
                            ApplicationDbContext.SaveChanges();

                            if (item1.Pair_Id == item.Pair_Id)
                            {
                                var a = ApplicationDbContext.AnswerPairings.Where(x => x.Pair_Id == item.Pair_Id).ToList();

                                if (item1.AnPa_IdOption == item1.AnPa_IdRequest)
                                {


                                    sum2 = sum2 + 1;
                                }
                                if (sum2 == a.Count)
                                {
                                    sum = sum + 1;
                                    sum2 = 0;
                                }
                            }
                            else
                            {
                                sum2 = 0;
                            }
                        }

                    }
                }
            }
            return sum;
        }

        public int resultTrueOrFalse(List<GeneralQuestions> result)
        {
            var sum = 0;
            if (result != null)
            {

                foreach (var item in result)
                {

                    if (item.TypeQuestion == TYPEQUESTIONS.TrueorFalse)
                    {
                        var a = ApplicationDbContext.TrueOrFalses.Where(x => x.TrFa_Id == item.TrFa_Id).ToList();
                        var b = GetActualUserId();



                        TrueOrFalse z = a.Single(x => x.TrFa_State == item.TrFa_State);

                        TrueOrFalseStudent module = new TrueOrFalseStudent
                        {
                            User_Id = b.Id,
                            TrFa_Id = item.TrFa_Id,
                            TrFa_Question = item.TrFa_Question,
                            TrFa_Description = item.TrFa_Description,
                            TrFa_State = item.TrFa_Answer,
                            BaQu_Id = item.BaQu_Id,
                            Date_Present_test = DateTime.Now
                        };

                        ApplicationDbContext.TrueOrFalseStudent.Add(module);
                        ApplicationDbContext.SaveChanges();

                        if (z.TrFa_State == item.TrFa_Answer)
                        {
                            sum = sum + 1;
                        }
                    }
                }
            }
            return sum;
        }

        [Authorize]
        public ActionResult EmailAttempts(int BaQu_Id)
        {

            var bankquestion = ApplicationDbContext.BankQuestions.Find(BaQu_Id);
            var user = GetActualUserId();
            var useradmin = ApplicationDbContext.Users.Where(x => x.CompanyId == user.CompanyId && x.Role == ROLES.AdministradordeFormacion).ToList();
            NewAttempts NewAttempt = new NewAttempts
            {
                BankQuestion = bankquestion,
                BaQu_Id = bankquestion.BaQu_Id,
                NeAt_DateInint = DateTime.Now,
                ApplicationUser = user,
                User_Id = user.Id,
                Attempts = ATTEMPTS.No
            };
            ApplicationDbContext.NewAttempts.Add(NewAttempt);
            ApplicationDbContext.SaveChanges();
            var table = ApplicationDbContext.TableChanges.Find(36);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(166);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = NewAttempt.NeAt_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ha solicitado un nuevo intento para realizar la evaluación con id " + NewAttempt.BaQu_Id + "perteneciente al tema con id " + NewAttempt.BankQuestion.ToCo_Id + "del curso con id " + NewAttempt.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }

            foreach (var item in useradmin)
            {
                var asunto = "Solicitud Nuevos Intentos Curso " + bankquestion.TopicsCourse.Module.Modu_Name;
                var mensaje = "Me gustaria solicitar mas intentos en el tema de " + bankquestion.TopicsCourse.ToCo_Name + " para poderme certificar en el curso de " + bankquestion.TopicsCourse.Module.Modu_Name;
                var solicitud = new MailMessage();
                solicitud.Body = "Hola " + item.FirstName + item.LastName + "<br/>" + "Mi nombre es " + user.FirstName + user.LastName + "<br/>" + mensaje + "<br/>" + "Gracias por su ayuda";
                solicitud.Subject = asunto;
                solicitud.To.Add(item.Email);
                solicitud.IsBodyHtml = true;
                var smtp = new SmtpClient();
                smtp.Send(solicitud);
                TempData["Result"] = "Mensaje enviado. Espere la respuesta del administrador";
            }
            return RedirectToAction("Grades", new { id = bankquestion.TopicsCourse.Module.Modu_Id });
        }

        private void SendEmailBetterPractice(string NameUser, string Company, string Module)
        {
            int a = GetActualUserId().Company.CompanyId;
            var correo = ApplicationDbContext.Enrollments.Where(x => x.Module.Modu_Name == Module).ToList();
            var admin = ApplicationDbContext.Users.Where(x => x.Company.CompanyId == a && x.Role == ROLES.AdministradordeFormacion || x.Role == ROLES.AdministradoGeneral).ToList();

            if (correo.Count != 0)
            {
                foreach (var item in correo)
                {
                    MailMessage solicitud = new MailMessage();
                    solicitud.Subject = "Aporte de Buena Práctica realizado";
                    solicitud.Body = "Cordial saludo " + "<br/>" +
                        "Sr(a). " + item.ApplicationUser.FirstName + item.ApplicationUser.LastName + "<br/>" +
                       "<br/>" + "El usuario : " + NameUser + " ha realizado un aporte de buenas practicas en el curso de ." + Module + "." +
                       "<br/>" + "Por favor verifique el aporte realizado" +
                       "<br/>" +
                       "<br/>" + "Equipo de Soporte SCORE";

                    solicitud.To.Add(item.ApplicationUser.Email);
                    solicitud.IsBodyHtml = true;
                    var smtp2 = new SmtpClient();
                    smtp2.Send(solicitud);
                }
            }

        }

        private void SendEmailImprovement(string NameUser, string Company, string Module)
        {
            int a = GetActualUserId().Company.CompanyId;
            var correo = ApplicationDbContext.Enrollments.Where(x => x.Module.Modu_Name == Module).ToList();
            var admin = ApplicationDbContext.Users.Where(x => x.Company.CompanyId == a && x.Role == ROLES.AdministradordeFormacion || x.Role == ROLES.AdministradoGeneral).ToList();
            if (correo.Count != 0)
            {
                foreach (var item in correo)
                {
                    MailMessage solicitud = new MailMessage();
                    solicitud.Subject = "Nuevo comentario en FORO - Por revisar";
                    solicitud.Body = "Cordial saludo " + "<br/>" +
                        "Sr(a). " + item.ApplicationUser.FirstName + item.ApplicationUser.LastName + "<br/>" +
                       "<br/>" + "El usuario : " + NameUser + " ha realizado un comentario en un foro." +
                       "<br/>" +
                       "<br/>" + "Equipo de Soporte SCORE";

                    solicitud.To.Add(item.ApplicationUser.Email);
                    solicitud.IsBodyHtml = true;
                    var smtp2 = new SmtpClient();
                    smtp2.Send(solicitud);
                }
            }

        }

        private void SendEmailComment(string NameUser, string Company, string Articulo)
        {
            int a = GetActualUserId().Company.CompanyId;
            var admin = ApplicationDbContext.Users.Where(x => x.Company.CompanyId == a && x.Role == ROLES.AdministradordeInformacion || x.Role == ROLES.AdministradoGeneral).ToList();
            if (admin.Count != 0)
            {
                foreach (var item in admin)
                {
                    MailMessage solicitud = new MailMessage();
                    solicitud.Subject = "Nuevo comentario en PERIÓDICO - Por revisar";
                    solicitud.Body = "Cordial saludo " + "<br/>" +
                        "Sr(a). " + item.FirstName + " " + item.LastName + "<br/>" +
                       "<br/>" + "El usuario : " + NameUser + " ha realizado un comentario en el periódico en el articulo sobre: " + Articulo +
                       "<br/>" +
                       "<br/>" + "Equipo de Soporte SCORE";
                    solicitud.To.Add(item.Email);
                    solicitud.IsBodyHtml = true;
                    var smtp2 = new SmtpClient();
                    smtp2.Send(solicitud);
                }
            }

        }
        private void SendEmailExchange(string NameUser)
        {
            int a = GetActualUserId().Company.CompanyId;
            var admin = ApplicationDbContext.Users.Where(x => x.Company.CompanyId == a && x.Role == ROLES.AdministradordeFormacion || x.Role == ROLES.AdministradoGeneral).ToList();
            if (admin.Count != 0)
            {
                foreach (var item in admin)
                {
                    MailMessage solicitud = new MailMessage();
                    solicitud.Subject = "Nueva solicitud de PARTICIPACIÓN EN SORTEO - Por revisar";
                    solicitud.Body = "Cordial saludo " + "<br/>" +
                        "Sr(a). " + item.FirstName + item.LastName + "<br/>" +
                       "<br/>" + "El usuario : " + NameUser + " solicitado participar en el sorteo mensual de premiación." +
                       "<br/>" +
                       "<br/>" + "Equipo de Soporte SCORE";

                    solicitud.To.Add(item.Email);
                    solicitud.IsBodyHtml = true;
                    var smtp2 = new SmtpClient();
                    smtp2.Send(solicitud);
                }
            }

        }
        public ActionResult AcountSSt()
        {
            var a = GetActualUserId();
            return RedirectToAction("../sst/Home/AcountScore/" + a.Id);
        }


        public ActionResult Report_Person(UserProfileViewModel model)
        {
            string Id = GetActualUserId().Id;
            ApplicationUser User = UserManager.FindById(Id);
            var certi = ApplicationDbContext.Certifications.Where(x => x.User_Id == User.Id).ToList();
            UserProfileViewModel userCurrent = new UserProfileViewModel
            {
                ActualRole = GetActualUserId().Role,
                user = User,
                Logo = GetUrlLogo(),
                listcert = certi
            };
            userCurrent.Sesion = GetActualUserId().SesionUser;

            return View(userCurrent);
        }



        //        public ActionResult ExportToExcel6(int id)
        //        {

        //            var cert = ApplicationDbContext.Certifications.Find(id);
        //            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);

        //            var inputString = @"<html>

        //  <body style='background: #FFF9C4;'>
        //< h3 > Bogota D.C </ h3 >
        //<br></br>
        //<div ALIGN='center' >
        //<img src='http://localhost/UnAula/Recursos/logo.png' width='400px' height='300px'/>
        //</div>  
        //<h1 ALIGN='center'>  CERTIFICADO </ h1> 
        // < H3 > Una Aula certifica que: </ H3 >
        // < h3 > El señor(a) " + cert.ApplicationUser.FirstName + @" " + cert.ApplicationUser.LastName + @" identificado(a) con cedula de ciudadanía N " + cert.ApplicationUser.Document + @" </ h3 >
        //<br></br> 
        //< H3 > Aprobó el curso virtual de " + cert.Enrollment.Module.Modu_Name + @"</H3>

        //<h3>En testimonio de lo anterior, se firma el presente en Bogotá a los " + DateTime.Now + @"</h3>
        //<br></br><br></br>
        //<div ALIGN='center' >
        //<h3 ALIGN='center'>________________________</h3>
        //<h3 ALIGN='center'>Firma del Representante</h3>
        //</div>  
        //</body>
        //</html>";



        //            List<string> cssFiles = new List<string>();
        //            cssFiles.Add(@"/Content/bootstrap.css");

        //            var output = new MemoryStream();

        //            var input = new MemoryStream(Encoding.UTF8.GetBytes(inputString));


        //            var document = new Document();
        //            var writer = PdfWriter.GetInstance(document, output);
        //            writer.CloseStream = false;

        //            document.Open();
        //            var htmlContext = new HtmlPipelineContext(null);
        //            htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());

        //            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
        //            cssFiles.ForEach(i => cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath(i), true));

        //            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
        //            var worker = new XMLWorker(pipeline, true);
        //            var p = new XMLParser(worker);
        //            p.Parse(input);

        //            document.Close();
        //            output.Position = 0;


        //            Response.Clear();
        //            Response.ContentType = "application/pdf";
        //            Response.AddHeader("Content-Disposition", "attachment; filename=Certificado.pdf");
        //            Response.BinaryWrite(output.ToArray());
        //            // myMemoryStream.WriteTo(Response.OutputStream); //works too
        //            Response.Flush();
        //            Response.Close();
        //            Response.End();
        //            //var RE = (from k in ApplicationDbContext.Certifications.Where(x => x.User_Id == UserActuallity.Id)
        //            //          select new
        //            //          {
        //            //              Tipodecurso = k.Enrollment.Module.Modu_TypeOfModule,
        //            //              NombreCurso = k.Enrollment.Module.Modu_Name,
        //            //              Contenido = k.Enrollment.Module.Modu_Description,
        //            //              Esperado = "100%",
        //            //              Obtenido = "100%",
        //            //              UltimoIngreso = k.ApplicationUser.lastAccess,
        //            //              FechaCertificación = k.Cert_Date
        //            //          }).ToList();

        //            //if (RE.Count != 0)
        //            //{
        //            //    var GridView1 = new GridView();
        //            //    GridView1.DataSource = RE;
        //            //    GridView1.DataBind();

        //            //Response.ClearContent();
        //            //Response.Buffer = true;
        //            //Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
        //            //Response.ContentType = "application/ms-excel";
        //            //Response.Charset = "";
        //            //StringWriter sw = new StringWriter();
        //            //HtmlTextWriter htw = new HtmlTextWriter(sw);
        //            ////Response.Write("<CENTER><img  src='https://www.aprendeyavanza2.com.co/UnAula/Recursos/logo.png' width='200' height='200' <CENTER>/>");
        //            //Response.Output.Write("<b><CENTER>" + "<br>");
        //            //Response.Output.Write("<img  src='https://www.aprendeyavanza2.com.co/UnAula/Recursos/logo.png' width='1200' height='350' style='right:300px;'<br><br> <H1 style = 'color:#02447E;'/><br><p><CENTER>SALUD VIDA</CENTER></H1> <br> ");
        //            //Response.Output.Write("<b></CENTER>" + "<br>");
        //            //Response.Write("<BODY BACKGROUND='http://www.aprendeyavanza2.com/UnAula/images/white-background.png'> ");
        //            //Response.Output.Write("<H2><b><CENTER>Hace constar que:" + "<br>");
        //            //Response.Output.Write("<H2 <CENTER> El se&ntilde;or(a) " + "<H1>" + cert.ApplicationUser.FirstName + " " + cert.ApplicationUser.LastName + "<br> ");
        //            //Response.Output.Write("<H2> <CENTER>identificado con  n&uacute;mero de c&eacute;dula" + " " + cert.ApplicationUser.Document + "<br>");
        //            //Response.Output.Write("<H2> <CENTER> Curs&oacute; y aprob&oacute; la acci&oacute;n de formaci&oacute;n <br>");
        //            //Response.Output.Write("<H1>" + cert.Enrollment.Module.Modu_Name);
        //            //Response.Output.Write("<H2><CENTER> En testimonio de lo anterior ,se firma el presente en Bogot&aacute; a los" + " " + DateTime.Now + "<br>" + "<br>");
        //            //Response.Output.Write("<H2> <CENTER>_________________________<br>");
        //            //Response.Output.Write("<H2> <CENTER> Firma del Representante <br>");



        //            ////Response.Output.Write("<br>");


        //            //Response.Flush();
        //            //Response.End();
        //            //Response.ContentType = "application/pdf";
        //            //Response.AddHeader("content-disposition", "attachment;filename=certificado.pdf");
        //            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //            //    StringWriter sw = new StringWriter();
        //            //    HtmlTextWriter hw = new HtmlTextWriter(sw);
        //            //    GridView1.RenderControl(hw);
        //            //    StringReader sr = new StringReader(sw.ToString());
        //            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //            //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //            //    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //            //    pdfDoc.Open();
        //            //    htmlparser.Parse(sr);
        //            //    pdfDoc.Close();
        //            //    Response.Write(pdfDoc);
        //            //    Response.End();
        //            //    GridView1.AllowPaging = true;
        //            //    GridView1.DataBind();
        //            //    Response.Output.Write(cert.ApplicationUser.FirstName);
        //            //    return RedirectToAction("Report_Person");
        //            //}
        //            //else
        //            //{



        //            //    TempData["Menssages"] = "No hay datos para mostrar";
        //            //}
        //            return RedirectToAction("Report_Person");
        //        }

        public ActionResult ExportToExcel6(int id)
        {

            var cert = ApplicationDbContext.Certifications.Find(id);
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);

            var inputString = @"<html>

  <body>

    < div ALIGN='left' >
<img src='http://localhost/UnAula/Recursos/logo.png'width='200px';height='200px'/>
</div>  
<h1 ALIGN='center' style='font-family:Arial Black, Gadget, sans-serif; font-size: 40px;color:#000;'><strong>  CERTIFICADO </strong></ h1> 
 < H2 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;' ><strong>Otorgado a:</strong>  </ H2 >
<h3 ALIGN='center' style='font-family:Arial Black, Gadget, sans-serif; font-size: 40px;color:#000;'><strong>" + cert.ApplicationUser.FirstName + @" " + cert.ApplicationUser.LastName + @"</strong></h3>
< h5 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;' >Identificado(a) con cédula número " + cert.ApplicationUser.Document + @" </ h5 >
< H5 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;'> Por haber aprobado el curso virtual de:</H5>
< H3 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif; font-size: 50px;color:#822433;' > RUTAS DE ACCESO DE LOS SERVICIOS DE SALUD </H3>
<h5 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;'>En testimonio de lo anterior ,se firma en Bogotá a los " + DateTime.Now + @"</h5>
<br></br>
<div ALIGN='center' >
<h3 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;'>________________________</h3>
<h3 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;'>Dr.Juan Pablo Silva Roa</h3>
<h5 ALIGN='center'style='font-family:Arial Black, Gadget, sans-serif;color:#000;'>Presidente Un Aula</h5>
</div>  
</body>
</html>";


            List<string> cssFiles = new List<string>();
            cssFiles.Add(@"/Content/bootstrap.css");

            var output = new MemoryStream();

            var input = new MemoryStream(Encoding.UTF8.GetBytes(inputString));

            Document document = new Document(PageSize.A4.Rotate());

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;

            document.Open();
            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());

            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssFiles.ForEach(i => cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath(i), true));

            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
            var worker = new XMLWorker(pipeline, true);
            var p = new XMLParser(worker);
            p.Parse(input);

            document.Close();
            output.Position = 0;


            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=Certificado.pdf");
            Response.BinaryWrite(output.ToArray());
            // myMemoryStream.WriteTo(Response.OutputStream); //works too
            Response.Flush();
            Response.Close();
            Response.End();
            //var RE = (from k in ApplicationDbContext.Certifications.Where(x => x.User_Id == UserActuallity.Id)
            //          select new
            //          {
            //              Tipodecurso = k.Enrollment.Module.Modu_TypeOfModule,
            //              NombreCurso = k.Enrollment.Module.Modu_Name,
            //              Contenido = k.Enrollment.Module.Modu_Description,
            //              Esperado = "100%",
            //              Obtenido = "100%",
            //              UltimoIngreso = k.ApplicationUser.lastAccess,
            //              FechaCertificación = k.Cert_Date
            //          }).ToList();

            //if (RE.Count != 0)
            //{
            //    var GridView1 = new GridView();
            //    GridView1.DataSource = RE;
            //    GridView1.DataBind();

            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            ////Response.Write("<CENTER><img  src='https://www.aprendeyavanza2.com.co/UnAula/Recursos/logo.png' width='200' height='200' <CENTER>/>");
            //Response.Output.Write("<b><CENTER>" + "<br>");
            //Response.Output.Write("<img  src='https://www.aprendeyavanza2.com.co/UnAula/Recursos/logo.png' width='1200' height='350' style='right:300px;'<br><br> <H1 style = 'color:#02447E;'/><br><p><CENTER>SALUD VIDA</CENTER></H1> <br> ");
            //Response.Output.Write("<b></CENTER>" + "<br>");
            //Response.Write("<BODY BACKGROUND='http://www.aprendeyavanza2.com/UnAula/images/white-background.png'> ");
            //Response.Output.Write("<H2><b><CENTER>Hace constar que:" + "<br>");
            //Response.Output.Write("<H2 <CENTER> El se&ntilde;or(a) " + "<H1>" + cert.ApplicationUser.FirstName + " " + cert.ApplicationUser.LastName + "<br> ");
            //Response.Output.Write("<H2> <CENTER>identificado con  n&uacute;mero de c&eacute;dula" + " " + cert.ApplicationUser.Document + "<br>");
            //Response.Output.Write("<H2> <CENTER> Curs&oacute; y aprob&oacute; la acci&oacute;n de formaci&oacute;n <br>");
            //Response.Output.Write("<H1>" + cert.Enrollment.Module.Modu_Name);
            //Response.Output.Write("<H2><CENTER> En testimonio de lo anterior ,se firma el presente en Bogot&aacute; a los" + " " + DateTime.Now + "<br>" + "<br>");
            //Response.Output.Write("<H2> <CENTER>_________________________<br>");
            //Response.Output.Write("<H2> <CENTER> Firma del Representante <br>");



            ////Response.Output.Write("<br>");


            //Response.Flush();
            //Response.End();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=certificado.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //    StringWriter sw = new StringWriter();
            //    HtmlTextWriter hw = new HtmlTextWriter(sw);
            //    GridView1.RenderControl(hw);
            //    StringReader sr = new StringReader(sw.ToString());
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            //    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            //    pdfDoc.Open();
            //    htmlparser.Parse(sr);
            //    pdfDoc.Close();
            //    Response.Write(pdfDoc);
            //    Response.End();
            //    GridView1.AllowPaging = true;
            //    GridView1.DataBind();
            //    Response.Output.Write(cert.ApplicationUser.FirstName);
            //    return RedirectToAction("Report_Person");
            //}
            //else
            //{



            //    TempData["Menssages"] = "No hay datos para mostrar";
            //}
            return RedirectToAction("Report_Person");
        }

        //[AllowAnonymous]
        //public ActionResult Pdf(int id, string User)
        //{
        //    return new Rotativa.ActionAsPdf("GetSamples", new { id = id, USer = User });

        //}
        [Authorize]
        public ActionResult certificado(int id)
        {
            var cert = ApplicationDbContext.Certifications.Find(id);
            ApplicationUser UserActuallity = UserManager.FindById(cert.User_Id);
            UserCertificado model = new UserCertificado
            {
                certificado = cert,
                user = UserActuallity,
                Logo = GetUrlLogo(),
                Sesion = GetActualUserId().SesionUser
            };
            return View(model);
        }

        //[Authorize]
        //public ActionResult Pdfcertificado(int id)
        //{
        //    var user = GetActualUserId().Id;
        //    var userreal = ApplicationDbContext.Users.Find(user);
        //    return new Rotativa.ActionAsPdf("certificado", new { id = id });

        //}

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        /*
         *
         *
         *desde aqui plenamente 
         *
         *
         */
        [Authorize]
        public ActionResult VistaPendientes()
        {
            var id = GetActualUserId().Id;
            var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id).ToList();
            var Toco = ApplicationDbContext.TopicsCourses.ToList();
            var Evaluaciones = ApplicationDbContext.BankQuestions.ToList();
            var AdvanceUser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == id).ToList();
            List<TopicsCourse> EvaluacionesListaToco = new List<TopicsCourse>();
            List<TopicsCourse> TocoMatriculados = new List<TopicsCourse>();
            List<TopicsCourse> EvaluacionesListaDefinitiva = new List<TopicsCourse>();
            for (var i = 0; i < enrollments.Count(); i++)
            {
                for (var a = 0; a < Toco.Count(); a++)
                {
                    if (enrollments[i].Modu_Id == Toco[a].Modu_Id && Toco[a].ToCo_RequiredEvaluation == REQUIREDEVALUATION.Si )
                    {
                        TocoMatriculados.Add(Toco[a]);
                        EvaluacionesListaToco.Add(Toco[a]);

                    }
                }

            }
            for (var i = 0; i < AdvanceUser.Count; i++)
            {
                for (var a = 0; a < TocoMatriculados.Count(); a++)
                {
                    if (AdvanceUser[i].ToCo_id == TocoMatriculados[a].ToCo_Id && TocoMatriculados[a].ToCo_RequiredEvaluation == REQUIREDEVALUATION.Si)
                    {
                        EvaluacionesListaToco.Remove(TocoMatriculados[a]);
                    }
                }
            }

            EvaluacionesListaDefinitiva = EvaluacionesListaToco.DistinctBy(x => x.ToCo_Id).ToList();
            UserPendientes model = new UserPendientes
            {
                CursosFaltantes = EvaluacionesListaDefinitiva
            };

            model.Sesion = GetActualUserId().SesionUser;

            return View(model);
        }
        [Authorize]
        public ActionResult VistaAvances()
        {
            var id = GetActualUserId().Id;

            var enrollments = ApplicationDbContext.Enrollments.Where(x => x.User_Id == id).ToList();
            var modulo = ApplicationDbContext.Modules.Where(x => x.User_Id == id).ToList();
            var advances = ApplicationDbContext.Advances.Where(x=> x.Usuario_Id == id).ToList();
            /*UserEnrollmentViewmodel model = new UserEnrollmentViewmodel
            {
                quienSabeMasPuntajes = enrollmentsQSM,
                ActualRole = GetActualUserId().Role,
                enrollment = enrollments,
                Logo = GetUrlLogo()
            };
            */

            AdvanceViewModel model = new AdvanceViewModel
            {

                Usuario_Id = id,
                Modulo = advances,
                ActualRole = GetActualUserId().Role,
                enrollment = enrollments,
                Logo = GetUrlLogo()

            };

            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(158);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos virtuales matriculados, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
         

        public ActionResult ViewEvaluaciones()
        {
            List<string> listaModulos = new List<string>();
            var id = GetActualUserId().Id;
            int i = 0;
            var enrollments = (from e in ApplicationDbContext.Enrollments where e.User_Id == id select e.Modu_Id).ToList();
            foreach (var enrollment in enrollments)
            {
                var cursos = (from e in ApplicationDbContext.TopicsCourses where e.Modu_Id == enrollment select e.ToCo_Id);
                listaModulos[i] = cursos.ToString();
                i++;
            }
            UserViewEvaluaciones model = new UserViewEvaluaciones();

            for (i = 0; i < listaModulos.Count; i++)
            {
                var consulta = (from e in ApplicationDbContext.BankQuestions where e.ToCo_Id.ToString() == listaModulos[i] select e);

                model.ListBankQuestion.Add(consulta.FirstOrDefault());
            }

            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(158);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = null
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent.Id,
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de los cursos virtuales matriculados, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);

        }
             
     
    }
}

