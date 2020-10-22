using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Lms;
using SCORM1.Models.ViewModel;
using SCORM1.Models.Engagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using SCORM1.Models.PageCustomization;
using PagedList;
using SCORM1.Models.Logs;
using SCORM1.Models.ratings;

namespace SCORM1.Controllers
{
    public class AdminTrainingController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


        public AdminTrainingController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: Admintraining
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        private IEnumerable<SelectListItem> GetCategoryModuleIEnum()
        {
            var ModuleCompany = GetActualUserId().CompanyId;
            var dbcategorymodule = ApplicationDbContext.CategoryModules.Where(x => x.CompanyId == ModuleCompany);
            var categorymodule = dbcategorymodule
                        .Select(categorymodules =>
                                new SelectListItem
                                {
                                    Value = categorymodules.CaMo_Id.ToString(),
                                    Text = categorymodules.CaMo_Category
                                });

            return new SelectList(categorymodule, "Value", "Text");
        }

        private IEnumerable<SelectListItem> GetModuleIEnum()
        {
            var ModuleCompany = GetActualUserId().CompanyId;
            var dbmodule = ApplicationDbContext.Modules.Where(x => x.CompanyId == ModuleCompany);
            var module = dbmodule
                        .Select(modules =>
                                new SelectListItem
                                {
                                    Value = modules.Modu_Id.ToString(),
                                    Text = modules.Modu_Name
                                });

            return new SelectList(module, "Value", "Text");
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

        [Authorize]
        public ActionResult TermsandConditions()
        {
            AdminTrainingProfileViewModel model = new AdminTrainingProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
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
        public ActionResult Validateterms(AdminTrainingProfileViewModel model)
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
            AdminTrainingProfileViewModel model = new AdminTrainingProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
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
        public ActionResult Validatevideos(AdminTrainingProfileViewModel model)
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
        public ActionResult Modules()
        {
            var role = GetActualUserId().Role;
            AdminTrainingModuleViewModel model = new AdminTrainingModuleViewModel();
            if (role == ROLES.AdministradoGeneral)
            {
                var GetModuleCompanyId = GetActualUserId().CompanyId;
                model = new AdminTrainingModuleViewModel { ActualRole = GetActualUserId().Role, ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList(), CategoryModule = GetCategoryModuleIEnum(), Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;

            }
            else
            {
                var GetModuleCompanyId = GetActualUserId().Id;
                var consulta = ApplicationDbContext.Enrollments.Where(w => w.User_Id == GetModuleCompanyId).ToList();

                int i = 0;

                foreach (var resultado in consulta)
                {
                    model = new AdminTrainingModuleViewModel { ActualRole = GetActualUserId().Role, CategoryModule = GetCategoryModuleIEnum(), ListModules = ApplicationDbContext.Modules.Where(m => m.Modu_Id == resultado.Modu_Id).ToList(), Logo = GetUrlLogo() };
                    foreach (var resultado1 in consulta)
                    {
                        if (i == 0)
                        {

                        }
                        else
                        {
                            model.ListModules.Add(ApplicationDbContext.Modules.Where(m => m.Modu_Id == resultado1.Modu_Id).FirstOrDefault());

                        }
                        i++;
                    }
                    break;
                }

                model.Sesion = GetActualUserId().SesionUser;
            }
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(169);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de gestión de cursos, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachModule(AdminTrainingModuleViewModel model)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchModules) || string.IsNullOrEmpty(model.SearchModules))
            {

                model = new AdminTrainingModuleViewModel { ActualRole = GetActualUserId().Role, ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompany).ToList(), CategoryModule = GetCategoryModuleIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
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
                return View("Modules", model);
            }
            else
            {
                List<Module> SearchedModules = ApplicationDbContext.Modules.Where(m => m.Modu_Name.Contains(model.SearchModules) && m.CompanyId == GetModuleCompany).ToList();
                model = new AdminTrainingModuleViewModel { ActualRole = GetActualUserId().Role, ListModules = SearchedModules, CategoryModule = GetCategoryModuleIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
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
                return View("Modules", model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddModule(AdminTrainingModuleViewModel model, HttpPostedFileBase upload)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/" + file));
                            TempData["add"] = "El curso se ha creado con éxito";
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().CompanyId;
                            Module module = new Module { Modu_Name = model.Modu_Name, Modu_Description = model.Modu_Description, Modu_Statemodule = (MODULESTATE)model.Modu_Statemodule, Modu_TypeOfModule = (CURSO)model.Modu_TypeOfModule, Modu_Forum = (FORO)model.Modu_Forum, Modu_BetterPractice = (FORO)model.Modu_BetterPractice, Modu_Improvement = (FORO)model.Modu_Improvement, Modu_Test = (FORO)model.Modu_Test, Modu_InitDate = model.Modu_InitDate, Modu_Validity = model.Modu_Validity, Modu_Period = (VIGENCIA)model.Modu_Period, Modu_Points = model.Modu_Points, Company = GetActualUserId().Company, Modu_ImageName = ruta };
                            if (model.QSMActive == FORO.Si)
                            {
                                module.QSMActive = 1;
                            }
                            else
                            {
                                module.QSMActive = 0;
                            }
                            ApplicationDbContext.Modules.Add(module);
                            ApplicationDbContext.SaveChanges();
                            model.ListModules = ApplicationDbContext.Modules.ToList();
                            var table = ApplicationDbContext.TableChanges.Find(34);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(171);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = module.Modu_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear un curso con id " + module.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Modules");
                        }
                    }
                    TempData["add"] = "El formato del archivo no es valido";
                    model.ActualRole = GetActualUserId().Role;
                    model.ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList();
                    model.CategoryModule = GetCategoryModuleIEnum();
                    model.Sesion = GetActualUserId().SesionUser;
                    return View("Modules", model);
                }
                else
                {
                    TempData["add"] = "No ha seleccionado un archivo o el archivo que inteta subir es demasiado pesado ";
                    model.ActualRole = GetActualUserId().Role;
                    model.ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList();
                    model.CategoryModule = GetCategoryModuleIEnum();
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(34);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(171);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento crear un curso,pero no selecciono una imagen, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View("Modules", model);
                }
            }
            else
            {
                TempData["add"] = "Los campos no pueden estar vacios";
                model.ActualRole = GetActualUserId().Role;
                model.ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList();
                model.CategoryModule = GetCategoryModuleIEnum();
                model.Sesion = GetActualUserId().SesionUser;
                return View("Modules", model);
            }
        }

        [Authorize]
        public ActionResult DeleteModule(int id)
        {
            Module deletedModule = ApplicationDbContext.Modules.Find(id);
            if (deletedModule.TopicsCourse.Count != 0)
            {
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(173);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = deletedModule.Modu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar un curso,pero este curso tiene temas creados, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Info"] = "El curso no puede ser eliminado debido a que, tiene temas asociados.Debe eliminar los temas primero";
            }
            else
            {
                TempData["Info"] = "Curso Eliminado";
                var path = Server.MapPath("~/Resources");
                var fullpath = Path.Combine(path, deletedModule.Modu_ImageName);
                System.IO.File.Delete(fullpath);
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(173);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = deletedModule.Modu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino el  curso con id " + deletedModule.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Modules.Remove(deletedModule);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("Modules");
        }
        [Authorize]
        public ActionResult UpdateModule(int id)
        {

            var GetModuleCompanyId = GetActualUserId().CompanyId;
            Module updatedModule = ApplicationDbContext.Modules.Find(id);
            TempData["UpdateModulo"] = "esta es una prueba para editar";
            AdminTrainingModuleViewModel model = new AdminTrainingModuleViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList(),
                Modu_Id = updatedModule.Modu_Id,
                Modu_Name = updatedModule.Modu_Name,
                Modu_Description = updatedModule.Modu_Description,
                Modu_Statemodule = updatedModule.Modu_Statemodule,
                Modu_InitDate = updatedModule.Modu_InitDate,
                Modu_Validity = updatedModule.Modu_Validity,
                Modu_Period = updatedModule.Modu_Period,
                Modu_Points = updatedModule.Modu_Points,
                CategoryModule = GetCategoryModuleIEnum(),
                Modu_ImageName = updatedModule.Modu_ImageName,
                Modu_Forum = updatedModule.Modu_Forum,
                Modu_Improvement = updatedModule.Modu_Improvement,
                Modu_BetterPractice = updatedModule.Modu_BetterPractice,
                Modu_TypeOfModule = updatedModule.Modu_TypeOfModule,
                Modu_Test = updatedModule.Modu_Test,
                Logo = GetUrlLogo()
            };

            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(172);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono el curso para modificar con id " + id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();

                return View("Modules", model);
            }
            return RedirectToAction("Modules");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateModule(AdminTrainingModuleViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            Module updatedModule = ApplicationDbContext.Modules.Find(model.Modu_Id);
                            var path = Server.MapPath("~/Resources");
                            var fullpath = Path.Combine(path, updatedModule.Modu_ImageName);
                            System.IO.File.Delete(fullpath);
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/" + file));
                            TempData["add"] = "El curso se ha modificado con éxito";
                            string ruta = file;
                            updatedModule.Modu_Name = model.Modu_Name;
                            updatedModule.Modu_Description = model.Modu_Description;
                            updatedModule.Modu_Statemodule = model.Modu_Statemodule;
                            updatedModule.Modu_Validity = model.Modu_Validity;
                            updatedModule.Modu_Period = model.Modu_Period;
                            updatedModule.Modu_Points = model.Modu_Points;
                            updatedModule.Modu_TypeOfModule = model.Modu_TypeOfModule;
                            updatedModule.Modu_Improvement = model.Modu_Improvement;
                            updatedModule.Modu_Forum = model.Modu_Forum;
                            updatedModule.Modu_Test = model.Modu_Test;
                            updatedModule.Modu_BetterPractice = model.Modu_BetterPractice;
                            updatedModule.Modu_ImageName = ruta;
                            if (model.QSMActive == FORO.Si)
                            {
                                updatedModule.QSMActive = 1;
                            }
                            else
                            {
                                updatedModule.QSMActive = 0;
                            }
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(34);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(172);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = updatedModule.Modu_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico el curso con id " + updatedModule.Modu_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Modules");
                        }
                    }
                    TempData["add"] = "El formato del archivo no es valido";
                    model.ActualRole = GetActualUserId().Role;
                    model.ListModules = ApplicationDbContext.Modules.ToList();
                    model.CategoryModule = GetCategoryModuleIEnum();
                    model.Sesion = GetActualUserId().SesionUser;
                    model.Sesion = GetActualUserId().SesionUser;
                    return View("Modules", model);
                }
                else
                {
                    Module updatedModule = ApplicationDbContext.Modules.Find(model.Modu_Id);
                    TempData["add"] = "El curso se ha modificado con éxito";
                    updatedModule.Modu_Name = model.Modu_Name;
                    updatedModule.Modu_Description = model.Modu_Description;
                    updatedModule.Modu_Statemodule = model.Modu_Statemodule;
                    updatedModule.Modu_Validity = model.Modu_Validity;
                    updatedModule.Modu_Period = model.Modu_Period;
                    updatedModule.Modu_Points = model.Modu_Points;
                    updatedModule.Modu_TypeOfModule = model.Modu_TypeOfModule;
                    updatedModule.Modu_Improvement = model.Modu_Improvement;
                    updatedModule.Modu_Forum = model.Modu_Forum;
                    updatedModule.Modu_Test = model.Modu_Test;
                    updatedModule.Modu_BetterPractice = model.Modu_BetterPractice;
                    if (model.QSMActive == FORO.Si)
                    {
                        updatedModule.QSMActive = 1;
                    }
                    else
                    {
                        updatedModule.QSMActive = 0;
                    }
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(34);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(172);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = updatedModule.Modu_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico el curso con id " + updatedModule.Modu_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Modules");
                }
            }
            else
            {
                TempData["add"] = "Los campos no pueden estar vacios";
                model.ActualRole = GetActualUserId().Role;
                model.ListModules = ApplicationDbContext.Modules.ToList();
                model.CategoryModule = GetCategoryModuleIEnum();
                model.Sesion = GetActualUserId().SesionUser;
                return View("Modules", model);
            }
        }

        [Authorize]
        public ActionResult AddResources(int id)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            TopicsCourse topic = ApplicationDbContext.TopicsCourses.Find(id);
            TempData["Resource"] = "resource";
            var GetModule = ApplicationDbContext.Modules.Find(topic.Module.Modu_Id);
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Modules = GetModule,
                ToCo_Id = topic.ToCo_Id,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(62);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(178);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = topic.ToCo_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + "Ingreso a la vista de cargar documentos de apoyo al tema con id " + topic.ToCo_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Grades", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddResource(AdminTrainingGeneralViewModel model, HttpPostedFileBase upload)
        {
            var topic = ApplicationDbContext.TopicsCourses.Find(model.ToCo_Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (5 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".doc", ".pdf", ".xlsx ", ".docx", ".pptx", ".mp3", ".mp4", ".m4a", ".avi", ".wmv", ".wma" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ResourceTopic/" + file));
                            TempData["Info"] = "Documento de apoyo agregado";
                            string ruta = file;
                            ResourceTopics resource = new ResourceTopics { ReMo_NameResource = ruta, TopicsCourse = topic, ReMo_InitDate = DateTime.Now, ReMo_Name = model.ReMo_Name };
                            ApplicationDbContext.ResourceTopicss.Add(resource);
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(54);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(178);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = resource.ReMo_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Agrego un documentos de apoyo con id " + resource.ReMo_Id + "al tema con id " + topic.ToCo_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Grades", new { id = topic.Module.Modu_Id });
                        }

                    }
                    TempData["Info"] = "El formato del archivo no es valido";
                    return RedirectToAction("Grades", new { id = topic.Module.Modu_Id });
                }
                else
                {
                    var table = ApplicationDbContext.TableChanges.Find(54);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(178);
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
                            Log_StateLogs = LOGSTATE.Error,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar un documentos de apoyo al tema con id " + topic.ToCo_Id + " pero el archivo que intenta subir es demaciado pesado, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    TempData["Info"] = "No ha seleccionado un archivo o el archivo que inteta subir es demasiado pesado ";
                    return RedirectToAction("Grades", new { id = topic.Module.Modu_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return RedirectToAction("Grades", new { id = topic.Module.Modu_Id });
            }
        }
        [Authorize]
        public ActionResult DeleteResource(int id)
        {
            TempData["Delete"] = "Documento Eliminado";

            var deletedCategoryModule = ApplicationDbContext.ResourceTopicss.Find(id);
            var idg = deletedCategoryModule.TopicsCourse.Module.Modu_Id;
            ApplicationDbContext.ResourceTopicss.Remove(deletedCategoryModule);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("Grades", new { id = idg });
        }

        [Authorize]
        public ActionResult CategoryModules()
        {
            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            AdminTrainingCategoryModuleViewModel model = new AdminTrainingCategoryModuleViewModel { ActualRole = GetActualUserId().Role, ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SeachCategoryModule(AdminTrainingCategoryModuleViewModel model)
        {
            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchCategoryModule) || string.IsNullOrEmpty(model.SearchCategoryModule))
            {

                model = new AdminTrainingCategoryModuleViewModel { ActualRole = GetActualUserId().Role, ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList() };
                model.Sesion = GetActualUserId().SesionUser;
                return View("CategoryModules", model);
            }
            else
            {
                List<CategoryModule> SearchedCategoryModules = ApplicationDbContext.CategoryModules.Where(m => m.CaMo_Category.Contains(model.SearchCategoryModule)).ToList();
                model = new AdminTrainingCategoryModuleViewModel { ActualRole = GetActualUserId().Role, ListCategoryModule = SearchedCategoryModules };
                model.Sesion = GetActualUserId().SesionUser;
                return View("CategoryModules", model);
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryModule(AdminTrainingCategoryModuleViewModel model)
        {
            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {

                TempData["Add"] = "La Categoria se ha creado con exito";
                CategoryModule categorymodule = new CategoryModule { CaMo_Category = model.CaMo_Category, Company = GetActualUserId().Company };
                ApplicationDbContext.CategoryModules.Add(categorymodule);
                ApplicationDbContext.SaveChanges();
                model.ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList();
                return RedirectToAction("CategoryModules");
            }
            else
            {
                TempData["Add"] = "Ingrese Nombre de Categoria";
                model.ActualRole = GetActualUserId().Role;
                model.ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                return View("CategoryModules", model);
            }
        }
        [Authorize]
        public ActionResult DeleteCategoryModule(int id)
        {
            TempData["Delete"] = "Categoria eliminada";
            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            CategoryModule deletedCategoryModule = ApplicationDbContext.CategoryModules.Find(id);
            ApplicationDbContext.CategoryModules.Remove(deletedCategoryModule);
            ApplicationDbContext.SaveChanges();
            AdminTrainingCategoryModuleViewModel model = new AdminTrainingCategoryModuleViewModel { ActualRole = GetActualUserId().Role, ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList() };
            return RedirectToAction("CategoryModules");
        }

        [Authorize]
        public ActionResult UpdateCategoryModule(int id)
        {

            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            CategoryModule updatedcategoryModule = ApplicationDbContext.CategoryModules.Find(id);
            TempData["UpdateCategoryModule"] = "Categoria Modificada con exito";
            AdminTrainingCategoryModuleViewModel model = new AdminTrainingCategoryModuleViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList(),
                CaMo_Id = updatedcategoryModule.CaMo_Id,
                CaMo_Category = updatedcategoryModule.CaMo_Category

            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("CategoryModules", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCategoryModule(AdminTrainingCategoryModuleViewModel model)
        {
            var GetCategoryModuleCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                CategoryModule updatedCategoryModule = ApplicationDbContext.CategoryModules.Find(model.CaMo_Id);
                TempData["Edit"] = "La categoria se ha Modificado con exito";
                updatedCategoryModule.CaMo_Category = model.CaMo_Category;
                ApplicationDbContext.SaveChanges();
                model.ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                return RedirectToAction("CategoryModules", model);
            }

            else
            {
                TempData["Edit"] = "Debe ingresar un nombre de Categoria";
                model.ActualRole = GetActualUserId().Role;
                model.ListCategoryModule = ApplicationDbContext.CategoryModules.Where(c => c.CompanyId == GetCategoryModuleCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                return View("categoryModules", model);
            }
        }

        [Authorize]
        public ActionResult Topics()
        {
            var GetTopicCompanyId = GetActualUserId().CompanyId;
            AdminTrainingTopicViewModel model = new AdminTrainingTopicViewModel { ActualRole = GetActualUserId().Role, ListTopic = ApplicationDbContext.TopicsCourses.Where(c => c.Module.CompanyId == GetTopicCompanyId).ToList(), modules = GetModuleIEnum() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult SeachTopic(AdminTrainingTopicViewModel model)
        {
            var GetTopicCompanyId = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchTopic) || string.IsNullOrEmpty(model.SearchTopic))
            {

                model = new AdminTrainingTopicViewModel { ActualRole = GetActualUserId().Role, ListTopic = ApplicationDbContext.TopicsCourses.Where(m => m.Module.CompanyId == GetTopicCompanyId).ToList(), modules = GetModuleIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                return View("Topics", model);
            }
            else
            {
                List<TopicsCourse> SearchedTopic = ApplicationDbContext.TopicsCourses.Where(m => m.ToCo_Name.Contains(model.SearchTopic) && m.Module.CompanyId == GetTopicCompanyId).ToList();
                model = new AdminTrainingTopicViewModel { ActualRole = GetActualUserId().Role, ListTopic = SearchedTopic, modules = GetModuleIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                return View("Topics", model);
            }
        }
        [Authorize]
        public ActionResult ListModules()
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            AdminTrainingModuleViewModel model = new AdminTrainingModuleViewModel { ActualRole = GetActualUserId().Role, ListModules = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompany).ToList(), CategoryModule = GetCategoryModuleIEnum() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [Authorize]
        public ActionResult ListTopic()
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            AdminTrainingTopicViewModel model = new AdminTrainingTopicViewModel { ActualRole = GetActualUserId().Role, ListTopic = ApplicationDbContext.TopicsCourses.Where(m => m.Module.CompanyId == GetModuleCompany).ToList(), modules = GetModuleIEnum() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [Authorize]
        public new ActionResult Profile(AdminTrainingProfileViewModel model)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var GetModuleCompany = GetActualUserId().CompanyId;
            var EnrollmentVirtual = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var EnrollmentEvaluative = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            int CompanyUser = (int)GetActualUserId().CompanyId;
            List<Banner> banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();
            model = new AdminTrainingProfileViewModel { ActualRole = GetActualUserId().Role, Listmodulevirtual = EnrollmentVirtual, Listmoduleevaluative = EnrollmentEvaluative };
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
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_Profile", model);
        }
        [Authorize]
        public ActionResult Grades(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Module getModule = ApplicationDbContext.Modules.Find(id);
            var test = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Modu_Id == getModule.Modu_Id).ToList();
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Modules = getModule,
                baseUrl = url,
                Logo = GetUrlLogo(),
                listtest = test
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = new CodeLogs();
            if (getModule.Modu_TypeOfModule == CURSO.Evaluativo)
            {
                code = ApplicationDbContext.CodeLogs.Find(161);
            }
            else
            {
                code = ApplicationDbContext.CodeLogs.Find(159);
            }

            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = getModule.Modu_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso al curso con id " + getModule.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult Form(int id)
        {
            Module moduled = ApplicationDbContext.Modules.Find(id);
            var AdvanceUser = ApplicationDbContext.AdvanceUsers.Where(x => x.TopicsCourse.Modu_Id == moduled.Modu_Id).ToList().Count();
            var Attempts = ApplicationDbContext.Attempts.Where(x => x.BankQuestion.TopicsCourse.Modu_Id == moduled.Modu_Id).ToList().Count();
            if (AdvanceUser != 0 || Attempts != 0)
            {
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(174);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento crear un tema en el curso con id " + id + " pero ya hay evaluaciones realizadas en este curso, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Info"] = "No se pueden crear mas temas debido a que un usuario ya realizo una evaluación";
            }
            else
            {

                TempData["form"] = "virtual";
                var table = ApplicationDbContext.TableChanges.Find(34);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(174);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de crear un tema en el curso con id " + id + " pero ya hay evaluaciones realizadas en este curso, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel { ActualRole = GetActualUserId().Role, Modules = moduled, Modu_Id = id };


            }
            return RedirectToAction("Grades", new { id = moduled.Modu_Id });

        }
        [HttpPost]
        public ActionResult UploadTopic()
        {
            var GetTopicCompanyId = GetActualUserId().CompanyId;
            var file = Request.Files["Filedata"];
            var fileid = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName).ToLower();
            string savePath = Server.MapPath(@"~\ResourcesTopics\" + fileid);
            file.SaveAs(savePath);
            ResourceTopic newResourceTopic = new ResourceTopic { ReTo_Name = fileid, ReTo_Location = savePath, Company = GetActualUserId().Company };
            ApplicationDbContext.ResourceTopics.Add(newResourceTopic);
            ApplicationDbContext.SaveChanges();
            return Content(Url.Content(@"~\ResourcesTopics\" + fileid));
        }

        [HttpPost]
        public ActionResult AddGeneralTopicVirtual(AdminTrainingGeneralViewModel model, HttpPostedFileBase Img)
        {
            var GetModules = ApplicationDbContext.Modules.Find(model.Modules.Modu_Id);
            if (ModelState.IsValid)
            {
                TempData["Info"] = "El Tema se ha creado con éxito";
                if (model.ToCo_Type == TYPE.Evaluativo)
                {
                    var a = REQUIREDEVALUATION.Si;
                    TopicsCourse topics = new TopicsCourse { ToCo_Name = model.ToCo_Name, ToCo_Description = model.ToCo_Description, Module = GetModules, ToCo_ContentVirtual = model.ToCo_ContentVirtual, ToCo_Type = model.ToCo_Type, ToCo_RequiredEvaluation = a, ToCo_Attempt = model.ToCo_Attempt, ToCo_Visible = model.ToCo_Visible };
                    ApplicationDbContext.TopicsCourses.Add(topics);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(62);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(174);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = topics.ToCo_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de  crear un tema con id " + topics.ToCo_Id + " perteneciente al curso con id " + GetModules.Modu_Id + " ,en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Grades", new { id = GetModules.Modu_Id });

                }
                else
                {
                    TopicsCourse topics = new TopicsCourse { ToCo_Name = model.ToCo_Name, ToCo_TotalQuestion = model.ToCo_TotalQuestion, ToCo_Description = model.ToCo_Description, Module = GetModules, ToCo_ContentVirtual = model.ToCo_ContentVirtual, ToCo_Type = model.ToCo_Type, ToCo_RequiredEvaluation = model.ToCo_RequiredEvaluation, ToCo_Attempt = model.ToCo_Attempt, ToCo_Visible = model.ToCo_Visible };
                    ApplicationDbContext.TopicsCourses.Add(topics);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(62);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(174);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = topics.ToCo_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de  crear un tema con id " + topics.ToCo_Id + " perteneciente al curso con id " + GetModules.Modu_Id + " ,en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Grades", new { id = GetModules.Modu_Id });
                }
            }
            else
            {
                TempData["Info"] = "Los campos no puedes ser vacios";
                model.Modu_Id = model.Modules.Modu_Id;
                return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
            }
        }

        [Authorize]
        public ActionResult DeleteTopicVirtual(int id)
        {
            TopicsCourse deletedTopic = ApplicationDbContext.TopicsCourses.Find(id);
            var AdvanceUser = ApplicationDbContext.AdvanceUsers.Where(x => x.TopicsCourse.Modu_Id == deletedTopic.Modu_Id).ToList().Count();
            var Module = deletedTopic.Modu_Id;
            var Attempts = ApplicationDbContext.Attempts.Where(x => x.BankQuestion.TopicsCourse.Modu_Id == deletedTopic.Modu_Id).ToList().Count();
            if (AdvanceUser != 0 || Attempts != 0)
            {
                var table = ApplicationDbContext.TableChanges.Find(62);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(176);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = deletedTopic.ToCo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar un tema con id " + deletedTopic.ToCo_Id + " perteneciente al curso con id " + deletedTopic.Modu_Id + " pero este tema ya tiene evaluaciones registradas ,en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Info"] = "El tema no puede ser eliminado dedido a que, ya se realiazaron evaluaciones asociadas a este curso.";
            }
            else
            {
                TempData["Info"] = "Tema Eliminado";
                var table = ApplicationDbContext.TableChanges.Find(62);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(176);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = deletedTopic.ToCo_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Error,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino un tema con id " + deletedTopic.ToCo_Id + " perteneciente al curso con id " + deletedTopic.Modu_Id + ",en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.TopicsCourses.Remove(deletedTopic);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("Grades", new { id = Module });
        }

        [Authorize]
        public ActionResult UpdateTopicVirtual(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            TopicsCourse topic = ApplicationDbContext.TopicsCourses.Find(id);
            var AdvanceUser = ApplicationDbContext.AdvanceUsers.Where(x => x.TopicsCourse.Modu_Id == topic.Modu_Id).ToList().Count();
            var Attempts = ApplicationDbContext.Attempts.Where(x => x.BankQuestion.TopicsCourse.Modu_Id == topic.Modu_Id).ToList().Count();
            if (topic.BankQuestion.Count != 0)
            {
                TempData["UpdateTopicVirtual"] = "esta es una prueba para editar";
                var GetModule = ApplicationDbContext.Modules.Find(topic.Module.Modu_Id);
                AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
                {
                    ActualRole = GetActualUserId().Role,
                    ToCo_Id = topic.ToCo_Id,
                    ToCo_Name = topic.ToCo_Name,
                    ToCo_Description = topic.ToCo_Description,
                    ToCo_ContentVirtual = topic.ToCo_ContentVirtual,
                    ToCo_Visible = topic.ToCo_Visible,
                    Modules = GetModule,
                    topics = topic,
                    baseUrl = url,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(62);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(175);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = topic.ToCo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono un tema para ,modificar con id " + topic.ToCo_Id + " perteneciente al curso con id " + topic.Modu_Id + ",en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("grades", model);
            }
            else
            {
                TempData["UpdateTopicVirtual"] = "esta es una prueba para editar";
                var GetModule = ApplicationDbContext.Modules.Find(topic.Module.Modu_Id);
                AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
                {
                    ActualRole = GetActualUserId().Role,
                    ToCo_Id = topic.ToCo_Id,
                    ToCo_Name = topic.ToCo_Name,
                    ToCo_Description = topic.ToCo_Description,
                    ToCo_Attempt = topic.ToCo_Attempt,
                    ToCo_Type = topic.ToCo_Type,
                    ToCo_RequiredEvaluation = topic.ToCo_RequiredEvaluation,
                    ToCo_ContentVirtual = topic.ToCo_ContentVirtual,
                    ToCo_Visible = topic.ToCo_Visible,
                    Modules = GetModule,
                    topics = topic,
                    baseUrl = url,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(62);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(175);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = topic.ToCo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono un tema para ,modificar con id " + topic.ToCo_Id + " perteneciente al curso con id " + topic.Modu_Id + ",en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("grades", model);
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateTopicVirtual(AdminTrainingGeneralViewModel model, HttpPostedFileBase upload)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            if (ModelState.IsValid)
            {
                TempData["Info"] = "Tema modificado con éxito";
                TopicsCourse updatedTopic = ApplicationDbContext.TopicsCourses.Find(model.ToCo_Id);
                updatedTopic.ToCo_Name = model.ToCo_Name;
                updatedTopic.ToCo_Description = model.ToCo_Description;
                updatedTopic.ToCo_ContentVirtual = model.ToCo_ContentVirtual;
                updatedTopic.ToCo_Visible = model.ToCo_Visible;
                if (updatedTopic.BankQuestion.Count != 0)
                {
                }
                else
                {
                    updatedTopic.ToCo_Attempt = model.ToCo_Attempt;
                    updatedTopic.ToCo_Type = model.ToCo_Type;
                    updatedTopic.ToCo_RequiredEvaluation = model.ToCo_RequiredEvaluation;
                }
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(62);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(175);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedTopic.ToCo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico un tema con id " + updatedTopic.ToCo_Id + " perteneciente al curso con id " + updatedTopic.Modu_Id + ",en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                model.Modules = model.Modules;
                model.baseUrl = url;
                model.Sesion = GetActualUserId().SesionUser;
                return View("Grades", model);
            }
        }

        [Authorize]
        public ActionResult EditBetterPractice(int id)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            BetterPractice betterpracticed = ApplicationDbContext.BetterPractices.Find(id);
            TempData["EditBetterPractice"] = "BetterPractice";
            var GetModule = ApplicationDbContext.Modules.Find(betterpracticed.Module.Modu_Id);
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Modules = GetModule,
                BePr_TiTle = betterpracticed.BePr_TiTle,
                BePr_Id = betterpracticed.BePr_Id,
                BePr_StateBetterpractice = betterpracticed.BePr_StateBetterpractice,
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(10);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(306);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = betterpracticed.BePr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ha seleccionado una buena practica para modificar con id " + betterpracticed.BePr_Id + " perteneciente al curso con id " + betterpracticed.Modu_Id + ",en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Grades", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditBetterPractice(AdminTrainingGeneralViewModel model)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (model.BePr_StateBetterpractice == BETTERPRACTICESTATE.Autorizado)
                {
                    var PointForComm = ApplicationDbContext.PointManagerCategory.Where(x => x.Company.CompanyId == GetModuleCompany).ToList();
                    if (PointForComm.Count == 0)
                    {
                        var bp = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                        var table = ApplicationDbContext.TableChanges.Find(10);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(179);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = model.BePr_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento aprobar un aporte de buena practica con id " + model.BePr_Id + " perteneciente al curso con id " + bp.Modu_Id + " pero no hay puntos asignados,en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        TempData["Info"] = "No se han asignado puntos";
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                    else
                    {
                        var PointB = PointForComm.Single(x => x.Company.CompanyId == GetModuleCompany);
                        var UpdatedBetterPracticed = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                        TempData["Info"] = "Aporte Autorizado";
                        UpdatedBetterPracticed.BePr_StateBetterpractice = model.BePr_StateBetterpractice;
                        UpdatedBetterPracticed.BePr_FinishDate = DateTime.Now;
                        UpdatedBetterPracticed.BePr_Points = PointB.PoMaCa_course;
                        ApplicationDbContext.SaveChanges();
                        var UserSelected = ApplicationDbContext.Users.Find(UpdatedBetterPracticed.User_Id);
                        var Point = UserSelected.Point.Where(x => x.User_Id == UserSelected.Id && x.TypePoint.Poin_TypePoints == TYPEPOINTS.LMS).ToList();
                        if (Point.Count == 0)
                        {
                            TempData["Info"] = "Aporte Autorizado";
                            var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.Poin_TypePoints == TYPEPOINTS.LMS);
                            Point Points = new Point { ApplicationUser = UserSelected, TypePoint = cate, TyPo_Id = cate.TyPo_Id, Poin_Date = DateTime.Now, Quantity_Points = PointB.PoMaCa_course };
                            ApplicationDbContext.Points.Add(Points);
                            ApplicationDbContext.SaveChanges();
                            var asunto = "Programa ECO: ¡Seguimos creciendo gracias a ti! – Tu vinculación ha sido aprobada";
                            var mensaje = "Agradecemos tu contribución al programa, tenemos un usuario más gracias a ti. Tus puntos han sido cargados en tu perfil. ¡Sigue participando y hagamos ECO juntos!";
                            var solicitud = new MailMessage();
                            solicitud.Body = "<br/>" + mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: univida.cap@saludvidaeps.com";
                            solicitud.Subject = asunto;
                            solicitud.To.Add(UpdatedBetterPracticed.ApplicationUser.Email);
                            solicitud.IsBodyHtml = true;
                            var smtp = new SmtpClient();
                            smtp.Send(solicitud);
                            var bp = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                            var table = ApplicationDbContext.TableChanges.Find(10);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(179);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = model.BePr_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Aprobo un aporte de buena practica con id " + model.BePr_Id + " perteneciente al curso con id " + bp.Modu_Id + ",en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                        }
                        else
                        {
                            TempData["Info"] = "Aporte Autorizado";
                            var Points = Point.Single(x => x.User_Id == x.User_Id);
                            var a = Points.Poin_Id;
                            Point PoinTNew = ApplicationDbContext.Points.Find(a);
                            PoinTNew.Quantity_Points = PoinTNew.Quantity_Points + PointB.PoMaCa_course;
                            ApplicationDbContext.SaveChanges();
                            var asunto = "Aporte De Evidencia, Vinculación Nuevos Miembros de Asociaciones";
                            var mensaje = "Felicidades el contenido enviado es me mucha utilidad para nosotros, por eso te has ganado unos puntos.";
                            var solicitud = new MailMessage();
                            solicitud.Body = "Hola " + UpdatedBetterPracticed.ApplicationUser.FirstName + "<br/>" + mensaje + "<br/>" + "Gracias por participar";
                            solicitud.Subject = asunto;
                            solicitud.To.Add(UpdatedBetterPracticed.ApplicationUser.Email);
                            solicitud.IsBodyHtml = true;
                            var smtp = new SmtpClient();
                            smtp.Send(solicitud);
                            var bp = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                            var table = ApplicationDbContext.TableChanges.Find(10);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(179);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = model.BePr_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Aprobo un aporte de buena practica con id " + model.BePr_Id + " perteneciente al curso con id " + bp.Modu_Id + ",en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                }
                else
                {
                    if (model.BePr_StateBetterpractice == BETTERPRACTICESTATE.Rechazado)
                    {
                        TempData["DeleteBetterPractice"] = "Aporte Rechazado";
                        BetterPractice betterpracticed = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                        var GetModule = ApplicationDbContext.Modules.Find(betterpracticed.Module.Modu_Id);
                        model.Modules = GetModule;
                        model.BePr_Id = model.BePr_Id;
                        model.Sesion = GetActualUserId().SesionUser;
                        return View("Grades", model);
                    }
                    else
                    {
                        TempData["Info"] = "Aporte En Espera";
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                }

            }

            else
            {
                return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteBetterPractice(AdminTrainingGeneralViewModel model)
        {
            BetterPractice UpdatedBetterPracticed = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
            var path = Server.MapPath("~/ResourceBetterPractice");
            if (UpdatedBetterPracticed.BePr_Resource != null)
            {
                var fullpath = Path.Combine(path, UpdatedBetterPracticed.BePr_Resource);
                System.IO.File.Delete(fullpath);
            }
            var asunto = "Programa ECO: Tu solicitud de vinculación ha sido rechazada";
            var mensaje = model.BePr_CommentAdmin;
            var solicitud = new MailMessage();
            solicitud.Body = "<br/>" + mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: univida.cap@saludvidaeps.com";
            solicitud.Subject = asunto;
            solicitud.To.Add(UpdatedBetterPracticed.ApplicationUser.Email);
            solicitud.IsBodyHtml = true;
            var smtp = new SmtpClient();
            smtp.Send(solicitud);
            var bp = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
            var table = ApplicationDbContext.TableChanges.Find(10);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(179);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = UpdatedBetterPracticed.BePr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Rechazo un aporte de buena practica con id " + model.BePr_Id + " perteneciente al curso con id " + bp.Modu_Id + ",en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.BetterPractices.Remove(UpdatedBetterPracticed);
            ApplicationDbContext.SaveChanges();
            TempData["Info"] = "Aporte Eliminado";
            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
        }


        [Authorize]
        public ActionResult FormJob(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            var topic = ApplicationDbContext.TopicsCourses.Find(id);
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ToCo_Id = id,
                Logo = GetUrlLogo(),
                Modules = topic.Module,
                baseUrl = url
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
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
                    Job_Visible = model.Job_Visible,
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
            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
        }

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
                Job_Visible = jobs.Job_Visible,
                Modules = GetModule,
                baseUrl = url,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("Grades", model);
        }
        public ActionResult ViewTest(int id, int? page)
        {
            var theme = ApplicationDbContext.BankQuestions.Find(id);
            var module = ApplicationDbContext.Modules.Find(theme.TopicsCourse.Modu_Id);
            int eva = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Modu_Id == module.Modu_Id).ToList().Count;

            double pun = module.Modu_Points / eva;

            IPagedList<AdvanceUser> ListUsers = ApplicationDbContext.AdvanceUsers.Where(x => x.ToCo_id == theme.ToCo_Id).ToList().ToPagedList(page ?? 1, 20); ;
            AdminTrainingViewTest model = new AdminTrainingViewTest()
            {
                ListTest = ListUsers,
                cali = pun,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditJob(AdminTrainingGeneralViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Info"] = "Recurso modificado con éxito";
                var jobs = ApplicationDbContext.Job.Find(model.Job_Id);
                jobs.Job_Name = model.Job_Name;
                jobs.Job_Description = model.Job_Description;
                jobs.Job_Content = model.Job_Content;
                jobs.Job_FinishDate = model.Job_FinishDate;
                jobs.Job_InitDate = model.Job_InitDate;
                jobs.Job_Points = model.Job_Points;
                jobs.Job_StateJob = model.Job_StateJob;
                jobs.Job_TypeJob = model.Job_TypeJob;
                jobs.Job_Visible = model.Job_Visible;
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("grades", new { id = model.Modules.Modu_Id });
        }

        [Authorize]
        public ActionResult DeleteJob(int id)
        {
            var jobs = ApplicationDbContext.Job.Find(id);
            int module = jobs.TopicsCourse.Modu_Id;
            if (VrJobs(id) == true)
            {
                TempData["Info"] = "Recurso Eliminado";
                ApplicationDbContext.Job.Remove(jobs);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("Grades", new { id = module });
        }
        private bool VrJobs(int id)
        {
            var jobs = ApplicationDbContext.Job.Find(id);
            if (jobs.ResourceForum.Count != 0)
            {
                foreach (var item in jobs.ResourceForum)
                {
                    if (item.AnswersForum.Count != 0)
                    {
                        foreach (var item2 in item.AnswersForum)
                        {
                            ApplicationDbContext.AnswersForum.Remove(item2);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    if (item.BookRatings.Count != 0)
                    {
                        foreach (var item2 in item.BookRatings)
                        {
                            ApplicationDbContext.BookRatings.Remove(item2);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                }
            }
            if (jobs.ResourceJobs.Count != 0)
            {
                foreach (var item in jobs.ResourceJobs)
                {
                    if (item.BookRatings.Count != 0)
                    {
                        foreach (var item2 in item.BookRatings)
                        {
                            ApplicationDbContext.BookRatings.Remove(item2);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    ApplicationDbContext.ResourceJobs.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            return true;
        }

        [Authorize]
        public ActionResult EditImprovement(int id)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            Improvement improvemented = ApplicationDbContext.Improvements.Find(id);
            TempData["EditImprovement"] = "Improvement";
            var GetModule = ApplicationDbContext.Modules.Find(improvemented.Module.Modu_Id);
            AdminTrainingGeneralViewModel model = new AdminTrainingGeneralViewModel
            {
                ActualRole = GetActualUserId().Role,
                Modules = GetModule,

                Impr_Id = improvemented.Impr_Id,
                Impr_StateImprovement = improvemented.Impr_StateImprovement
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(27);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(308);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = improvemented.Impr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ha seleccionado una mejora para modificar con id " + improvemented.Impr_Id + " perteneciente al curso con id " + improvemented.Modu_Id + ",en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Grades", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditImprovement(AdminTrainingGeneralViewModel model)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                if (model.Impr_StateImprovement == IMPROVEMENTSTATE.Autorizado)
                {
                    var PointForComm = ApplicationDbContext.PointManagerCategory.Where(x => x.Company.CompanyId == GetModuleCompany).ToList();
                    if (PointForComm.Count == 0)
                    {
                        TempData["Info"] = "No se han asignado puntos";
                        var bp = ApplicationDbContext.BetterPractices.Find(model.BePr_Id);
                        var table = ApplicationDbContext.TableChanges.Find(27);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(182);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = model.Impr_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento aprobar un aporte de mejora con id " + model.Impr_Id + " perteneciente al curso con id " + bp.Modu_Id + " pero no hay puntos asignados,en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                    else
                    {
                        var PointB = PointForComm.Single(x => x.Company.CompanyId == GetModuleCompany);
                        var UpdatedImprovement = ApplicationDbContext.Improvements.Find(model.Impr_Id);
                        TempData["Info"] = "Aporte Autorizado";
                        UpdatedImprovement.Impr_StateImprovement = model.Impr_StateImprovement;
                        UpdatedImprovement.Impr_FinishDate = DateTime.Now;
                        UpdatedImprovement.Impr_Points = PointB.PoMaCa_Improvements;
                        ApplicationDbContext.SaveChanges();
                        var UserSelected = ApplicationDbContext.Users.Find(UpdatedImprovement.User_Id);
                        var Point = UserSelected.Point.Where(x => x.User_Id == UserSelected.Id && x.TypePoint.Poin_TypePoints == TYPEPOINTS.LMS).ToList();
                        if (Point.Count == 0)
                        {
                            TempData["Info"] = "Aporte Autorizado";
                            var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.Poin_TypePoints == TYPEPOINTS.LMS);
                            Point Points = new Point { ApplicationUser = UserSelected, TypePoint = cate, TyPo_Id = cate.TyPo_Id, Poin_Date = DateTime.Now, Quantity_Points = PointB.PoMaCa_Improvements };
                            ApplicationDbContext.Points.Add(Points);
                            ApplicationDbContext.SaveChanges();
                            var asunto = "Programa ECO: ¡Ahora todos pueden ver tú comentario!";
                            var mensaje = "Tu comentario en el periódico ha sido aprobado por el administrador y está visible a todos los usuarios de la plataforma. Muchas gracias por unirte a SaludVida para hacer ECO.";
                            var solicitud = new MailMessage();
                            solicitud.Body = "<br/>" + mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: univida.cap@saludvidaeps.com";
                            solicitud.Subject = asunto;
                            solicitud.To.Add(UpdatedImprovement.ApplicationUser.Email);
                            solicitud.IsBodyHtml = true;
                            var smtp = new SmtpClient();
                            smtp.Send(solicitud);
                            var bp = ApplicationDbContext.Improvements.Find(model.Impr_Id);
                            var table = ApplicationDbContext.TableChanges.Find(27);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(182);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = model.Impr_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Aprobo un aporte de mejora con id " + model.Impr_Id + " perteneciente al curso con id " + bp.Modu_Id + ",en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                        }
                        else
                        {
                            TempData["Info"] = "Aporte Autorizado";
                            var Points = Point.Single(x => x.User_Id == x.User_Id);
                            var a = Points.Poin_Id;
                            Point PoinTNew = ApplicationDbContext.Points.Find(a);
                            PoinTNew.Quantity_Points = PoinTNew.Quantity_Points + PointB.PoMaCa_Improvements;
                            ApplicationDbContext.SaveChanges();
                            var asunto = "Aporte De Foro Autorizada";
                            var mensaje = "Felicidades el contenido enviado es me mucha utilidad para nosotros, por eso te has ganado unos puntos.";
                            var solicitud = new MailMessage();
                            solicitud.Body = "Hola " + UpdatedImprovement.ApplicationUser.FirstName + "<br/>" + mensaje + "<br/>" + "Gracias por participar";
                            solicitud.Subject = asunto;
                            solicitud.To.Add(UpdatedImprovement.ApplicationUser.Email);
                            solicitud.IsBodyHtml = true;
                            var smtp = new SmtpClient();
                            smtp.Send(solicitud);
                        }
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                }
                else
                {
                    if (model.Impr_StateImprovement == IMPROVEMENTSTATE.Rechazado)
                    {
                        TempData["DeleteImprovement"] = "Aporte Rechazado";
                        Improvement improvement = ApplicationDbContext.Improvements.Find(model.Impr_Id);
                        var GetModule = ApplicationDbContext.Modules.Find(improvement.Module.Modu_Id);
                        model.Modules = GetModule;
                        model.Impr_Id = model.Impr_Id;
                        model.Sesion = GetActualUserId().SesionUser;
                        return View("Grades", model);
                    }
                    else
                    {
                        TempData["Info"] = "Aporte En Espera";
                        return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
                    }
                }
            }

            else
            {
                return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteImprovement(AdminTrainingGeneralViewModel model)
        {
            Improvement UpdatedImprovement = ApplicationDbContext.Improvements.Find(model.Impr_Id);
            var path = Server.MapPath("~/ResourceImprovement");
            if (UpdatedImprovement.Impr_Resource != null)
            {
                var fullpath = Path.Combine(path, UpdatedImprovement.Impr_Resource);
                System.IO.File.Delete(fullpath);
            }
            var asunto = "Programa ECO: ¡Sigue participando en nuestros Foros!";
            var mensaje = model.Impr_CommentAdmin;
            var solicitud = new MailMessage();
            solicitud.Body = mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: univida.cap@saludvidaeps.coms";
            solicitud.Subject = asunto;
            solicitud.To.Add(UpdatedImprovement.ApplicationUser.Email);
            solicitud.IsBodyHtml = true;
            var smtp = new SmtpClient();
            smtp.Send(solicitud);
            var bp = ApplicationDbContext.Improvements.Find(model.Impr_Id);
            var table = ApplicationDbContext.TableChanges.Find(27);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(183);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = UpdatedImprovement.Impr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Rechazo un aporte de mejora con id " + model.Impr_Id + " perteneciente al curso con id " + bp.Modu_Id + ",en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.Improvements.Remove(UpdatedImprovement);
            ApplicationDbContext.SaveChanges();
            TempData["Info"] = "Aporte Eliminado";
            return RedirectToAction("Grades", new { id = model.Modules.Modu_Id });
        }

        [Authorize]
        public ActionResult Test(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            TopicsCourse actualtopic = ApplicationDbContext.TopicsCourses.Find(id);
            var bankquestion = ApplicationDbContext.BankQuestions.Where(x => x.ToCo_Id == actualtopic.ToCo_Id).ToList();
            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel { ActualRole = GetActualUserId().Role, topic = actualtopic, Logo = GetUrlLogo() };
            if (bankquestion.Count != 0)
            {
                var a = bankquestion.Single(x => x.ToCo_Id == actualtopic.ToCo_Id);
                model.BaQu_Id = a.BaQu_Id;
            }
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(8);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(177);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de creación de evaluación del tema con id " + actualtopic.ToCo_Id + " perteneciente al curso con id " + actualtopic.Modu_Id + ",en la compañía con id " + company.CompanyId,
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
        public ActionResult AddBanQuestion(AdminTrainingTestViewModel model)
        {

            var GetTopic = ApplicationDbContext.TopicsCourses.Find(model.topic.ToCo_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "El Banco se ha creado con exito";
                BankQuestion bankquestion = new BankQuestion
                {
                    BaQu_Name = model.BaQu_Name,
                    BaQu_Porcentaje = model.BaQu_Porcentaje,
                    BaQu_Attempts = GetTopic.ToCo_Attempt,
                    BaQu_InintDate = model.BaQu_InintDate,
                    BaQu_FinishDate = model.BaQu_FinishDate,
                    TopicsCourse = GetTopic,
                    Company = GetActualUserId().Company,
                    BaQu_QuestionUser = model.BaQu_QuestionUser,
                    BaQu_SelectQuestion = model.BaQu_SelectQuestion

                };
                ApplicationDbContext.BankQuestions.Add(bankquestion);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(8);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(177);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = bankquestion.BaQu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una evaluación con id " + bankquestion.BaQu_Id + " del tema con id " + bankquestion.ToCo_Id + " perteneciente al curso con id " + bankquestion.TopicsCourse.Modu_Id + ",en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }

                if (bankquestion.BaQu_SelectQuestion == FORO.Si)
                {
                    var totalbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id != bankquestion.BaQu_Id && x.Company.CompanyId == bankquestion.Company.CompanyId).ToList();
                    if (totalbank.Count == 0)
                    {
                        TempData["Alert"] = "No hay Banco de preguntas Creado";
                        return RedirectToAction("Questions", new { id = bankquestion.BaQu_Id });
                    }
                    else
                    {
                        var table1 = ApplicationDbContext.TableChanges.Find(8);
                        var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code1 = ApplicationDbContext.CodeLogs.Find(310);
                        var idcompany1 = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = bankquestion.BaQu_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de banco de preguntas existentes con id de evaluación " + bankquestion.BaQu_Id + " del tema con id " + bankquestion.ToCo_Id + " perteneciente al curso con id " + bankquestion.TopicsCourse.Modu_Id + ",en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        return RedirectToAction("ManagementOfBankQuestions", new { id = bankquestion.BaQu_Id });
                    }

                }
                else
                {
                    return RedirectToAction("Questions", new { id = bankquestion.BaQu_Id });
                }

            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                return RedirectToAction("Test", new { id = GetTopic.ToCo_Id });
            }
        }

        [Authorize]
        public ActionResult BankQuestions(int id)
        {
            var GetActualCompany = GetActualUserId().Company.CompanyId;
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(id);
            var TotalBank = ApplicationDbContext.BankQuestions.Where(x => x.Company.CompanyId == GetActualCompany && x.BaQu_Id != actualbank.BaQu_Id).ToList();

            AdminTrainingBankQuestionViewModel model = new AdminTrainingBankQuestionViewModel
            {
                bankquestion = actualbank,
                Listbank = TotalBank
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [Authorize]
        public ActionResult Questions(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de preguntas de la evaluación con id " + actualbank.BaQu_Id + " del tema con id " + actualbank.ToCo_Id + " perteneciente al curso con id " + actualbank.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult AddOptionMultiple(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Pregunta Creada con éxito";
                OptionMultiple optionmultiple = new OptionMultiple { OpMu_Question = model.OpMu_Question, OpMu_Description = model.OpMu_Description, OpMult_Content = model.OpMult_Content, BankQuestion = Getbank };
                ApplicationDbContext.OptionMultiples.Add(optionmultiple);
                ApplicationDbContext.SaveChanges();
                var OptioMultipleId = ApplicationDbContext.OptionMultiples.Find(optionmultiple.OpMu_Id);
                AnswerOptionMultiple answeroptionmeultiple = new AnswerOptionMultiple { AnOp_OptionAnswer = model.AnOp_OptionAnswer, AnOp_TrueAnswer = model.AnOp_TrueAnswer, OptionMultiple = OptioMultipleId };
                ApplicationDbContext.AnswerOptionMultiples.Add(answeroptionmeultiple);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(38);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(185);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = optionmultiple.OpMu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una pregunta de opción múltiple con id " + optionmultiple.OpMu_Id + " en la evaluación con id " + optionmultiple.BaQu_Id + " del tema con id " + optionmultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + optionmultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
        }
        [Authorize]
        public ActionResult AddOptionMultiples(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getoptionmultiple = ApplicationDbContext.OptionMultiples.Find(id);
            TempData["Add"] = "Respuesta creada con éxito";
            List<Models.Lms.AnswerOptionMultiple> listansweroptionmultiple = Getoptionmultiple.AnswerOptionMultiple.Where(x => x.OpMu_Id == Getoptionmultiple.OpMu_Id && x.OptionMultiple.BankQuestion.Company.CompanyId == Getoptionmultiple.BankQuestion.Company.CompanyId).ToList();
            model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == Getoptionmultiple.OpMu_Id && x.BankQuestion.Company.CompanyId == Getoptionmultiple.BankQuestion.Company.CompanyId).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getoptionmultiple.BankQuestion.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getoptionmultiple.BankQuestion;
            model.BaQu_Id = Getoptionmultiple.BankQuestion.BaQu_Id;
            model.OpMu_Id = Getoptionmultiple.OpMu_Id;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddAnswerOptionMultiple", model);
        }
        [Authorize]
        public ActionResult AddOtherOptionMultiples(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbankquestion = ApplicationDbContext.BankQuestions.Find(id);
            TempData["Add"] = "Pregunta Creada";
            List<Models.Lms.OptionMultiple> listoptionmultiple = Getbankquestion.OptionMultiple.Where(x => x.BaQu_Id == Getbankquestion.BaQu_Id && x.BankQuestion.Company.CompanyId == Getbankquestion.Company.CompanyId).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbankquestion.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbankquestion;
            model.BaQu_Id = Getbankquestion.BaQu_Id;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddOptionMultiple", model);
        }
        [Authorize]
        public ActionResult EditOptionMultiple(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            OptionMultiple OptionMultiple = ApplicationDbContext.OptionMultiples.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(OptionMultiple.BaQu_Id);
            TempData["Form"] = "Activar Formulario";
            List<OptionMultiple> listoptionmultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == OptionMultiple.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.ListOptionMultiple = listoptionmultiple;
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.ActualRole = GetActualUserId().Role;
            model.OpMu_Id = OptionMultiple.OpMu_Id;
            model.OpMu_Question = OptionMultiple.OpMu_Question;
            model.OpMu_Description = OptionMultiple.OpMu_Description;
            model.OpMult_Content = OptionMultiple.OpMult_Content;
            return RedirectToAction("EditOMultiple", new { id = model.OpMu_Id });

        }
        [Authorize]
        public ActionResult EditOMultiple(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            OptionMultiple OptionMultipleId = ApplicationDbContext.OptionMultiples.Find(id);
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(OptionMultipleId.BankQuestion.BaQu_Id);
            List<TopicsCourse> listtopic = ApplicationDbContext.TopicsCourses.Where(x => x.ToCo_Id == actualbank.ToCo_Id && x.Module.Company.CompanyId == GetModuleCompanyId).ToList();
            List<BankQuestion> listbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == actualbank.BaQu_Id && x.Company.CompanyId == GetModuleCompanyId).ToList();
            List<Models.Lms.OptionMultiple> listoptionmultiple = ApplicationDbContext.OptionMultiples.Where(x => x.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId && x.BankQuestion.BaQu_Id == actualbank.BaQu_Id).ToList();
            List<Models.Lms.AnswerOptionMultiple> listansweroptionmultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.OptionMultiple.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId).ToList();
            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListTopic = listtopic,
                BaQu_Id = actualbank.BaQu_Id,
                ListBankQuestion = listbank,
                ListOptionMultiple = listoptionmultiple,
                ListAnswerOptionMultiple = listansweroptionmultiple,
                OpMu_Id = OptionMultipleId.OpMu_Id,
                OpMu_Question = OptionMultipleId.OpMu_Question,
                OpMu_Description = OptionMultipleId.OpMu_Description,
                OpMult_Content = OptionMultipleId.OpMult_Content,
                baseUrl = url,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(38);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(186);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = OptionMultipleId.OpMu_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar una pregunta para modificar de opción múltiple con id " + OptionMultipleId.OpMu_Id + " en la evaluación con id " + OptionMultipleId.BaQu_Id + " del tema con id " + OptionMultipleId.BankQuestion.ToCo_Id + " perteneciente al curso con id " + OptionMultipleId.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult EditOptionMultiples(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                OptionMultiple UpdatedOptionMultiple = ApplicationDbContext.OptionMultiples.Find(model.OpMu_Id);
                TempData["Add"] = "Pregunta modificada con éxito";
                UpdatedOptionMultiple.OpMu_Question = model.OpMu_Question;
                UpdatedOptionMultiple.OpMu_Description = model.OpMu_Description;
                UpdatedOptionMultiple.OpMult_Content = model.OpMult_Content;
                model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == UpdatedOptionMultiple.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.OpMu_Id = UpdatedOptionMultiple.OpMu_Id;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(38);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(186);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UpdatedOptionMultiple.OpMu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico una pregunta de opción múltiple con id " + UpdatedOptionMultiple.OpMu_Id + " en la evaluación con id " + UpdatedOptionMultiple.BaQu_Id + " del tema con id " + UpdatedOptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + UpdatedOptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == model.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.OpMu_Id = model.OpMu_Id;
                return RedirectToAction("EditOMultiple", new { id = model.OpMu_Id });
            }
        }
        [Authorize]
        public ActionResult AddAnswerOptionMultiple(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Respuesta Creada";
                var OptioMultipleId = ApplicationDbContext.OptionMultiples.Find(model.OpMu_Id);
                AnswerOptionMultiple answeroptionmeultiple = new AnswerOptionMultiple { AnOp_OptionAnswer = model.AnOp_OptionAnswer, AnOp_TrueAnswer = model.AnOp_TrueAnswer, OptionMultiple = OptioMultipleId };
                ApplicationDbContext.AnswerOptionMultiples.Add(answeroptionmeultiple);
                ApplicationDbContext.SaveChanges();
                model.ListAnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.AnOp_Id == answeroptionmeultiple.AnOp_Id && x.OptionMultiple.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == OptioMultipleId.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.OpMu_Id = OptioMultipleId.OpMu_Id;
                model.AnOp_Id = answeroptionmeultiple.AnOp_Id;
                var table = ApplicationDbContext.TableChanges.Find(3);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(188);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = answeroptionmeultiple.AnOp_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Creo una respuesta de opción múltiple con id " + answeroptionmeultiple.AnOp_Id + " perteneciente a una pregunta de opción múltiple con id " + answeroptionmeultiple.OpMu_Id + " en la evaluación con id " + answeroptionmeultiple.OptionMultiple.BaQu_Id + " del tema con id " + answeroptionmeultiple.OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + answeroptionmeultiple.OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                model.ActualRole = GetActualUserId().Role;
                model.Sesion = GetActualUserId().SesionUser;
                return View("Tests", model);
            }
        }
        [Authorize]
        public ActionResult EditAnswerOptionMultiple(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            AnswerOptionMultiple AnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Find(id);
            OptionMultiple OptionMultiple = ApplicationDbContext.OptionMultiples.Find(AnswerOptionMultiple.OptionMultiple.OpMu_Id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(OptionMultiple.BaQu_Id);
            TempData["FormAnswer"] = "Activar Formulario";
            List<AnswerOptionMultiple> listansweroptionmultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.AnOp_Id == AnswerOptionMultiple.AnOp_Id && x.OptionMultiple.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            List<OptionMultiple> listoptionmultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == OptionMultiple.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.ListOptionMultiple = listoptionmultiple;
            model.ListAnswerOptionMultiple = listansweroptionmultiple;
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.ActualRole = GetActualUserId().Role;
            model.AnOp_Id = AnswerOptionMultiple.AnOp_Id;
            model.AnOp_OptionAnswer = AnswerOptionMultiple.AnOp_OptionAnswer;
            model.AnOp_TrueAnswer = AnswerOptionMultiple.AnOp_TrueAnswer;
            return RedirectToAction("EditAnswerOMultiple", new { id = model.AnOp_Id });

        }
        [Authorize]
        public ActionResult EditAnswerOMultiple(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            AnswerOptionMultiple AnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Find(id);
            OptionMultiple OptionMultipleId = ApplicationDbContext.OptionMultiples.Find(AnswerOptionMultiple.OptionMultiple.OpMu_Id);
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(OptionMultipleId.BankQuestion.BaQu_Id);
            List<TopicsCourse> listtopic = ApplicationDbContext.TopicsCourses.Where(x => x.ToCo_Id == actualbank.ToCo_Id && x.Module.Company.CompanyId == GetModuleCompanyId).ToList();
            List<BankQuestion> listbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == actualbank.BaQu_Id && x.Company.CompanyId == GetModuleCompanyId).ToList();
            List<Models.Lms.OptionMultiple> listoptionmultiple = ApplicationDbContext.OptionMultiples.Where(x => x.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId && x.BankQuestion.BaQu_Id == actualbank.BaQu_Id).ToList();
            List<Models.Lms.AnswerOptionMultiple> listansweroptionmultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.OptionMultiple.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId).ToList();
            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListTopic = listtopic,
                BaQu_Id = actualbank.BaQu_Id,
                ListBankQuestion = listbank,
                ListOptionMultiple = listoptionmultiple,
                ListAnswerOptionMultiple = listansweroptionmultiple,
                OpMu_Id = AnswerOptionMultiple.OptionMultiple.OpMu_Id,
                AnOp_Id = AnswerOptionMultiple.AnOp_Id,
                AnOp_OptionAnswer = AnswerOptionMultiple.AnOp_OptionAnswer,
                AnOp_TrueAnswer = AnswerOptionMultiple.AnOp_TrueAnswer,
                Answer_OpMult_Content = AnswerOptionMultiple.Answer_OpMult_Content,

                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(3);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(189);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = AnswerOptionMultiple.AnOp_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Seleccionó una  respuesta de opción múltiple para modificar con id " + AnswerOptionMultiple.AnOp_Id + " perteneciente a una pregunta de opción múltiple con id " + AnswerOptionMultiple.OpMu_Id + " en la evaluación con id " + AnswerOptionMultiple.OptionMultiple.BaQu_Id + " del tema con id " + AnswerOptionMultiple.OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + AnswerOptionMultiple.OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult EditAnswerOptionMultiples(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            var OptioMultipleId = ApplicationDbContext.OptionMultiples.Find(model.OpMu_Id);
            if (ModelState.IsValid)
            {
                AnswerOptionMultiple UpdatedAnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Find(model.AnOp_Id);
                TempData["Add"] = "Respuesta modificada con éxito";
                UpdatedAnswerOptionMultiple.AnOp_OptionAnswer = model.AnOp_OptionAnswer;
                UpdatedAnswerOptionMultiple.AnOp_TrueAnswer = model.AnOp_TrueAnswer;
                UpdatedAnswerOptionMultiple.Answer_OpMult_Content = model.Answer_OpMult_Content;
                model.ListAnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.AnOp_Id == UpdatedAnswerOptionMultiple.AnOp_Id && x.OptionMultiple.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == UpdatedAnswerOptionMultiple.OptionMultiple.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.OpMu_Id = UpdatedAnswerOptionMultiple.OptionMultiple.OpMu_Id;
                model.AnOp_Id = UpdatedAnswerOptionMultiple.AnOp_Id;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(3);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(189);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UpdatedAnswerOptionMultiple.AnOp_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modificó una respuesta de opción múltiple con id " + UpdatedAnswerOptionMultiple.AnOp_Id + " perteneciente a una pregunta de opción múltiple con id " + UpdatedAnswerOptionMultiple.OpMu_Id + " en la evaluación con id " + UpdatedAnswerOptionMultiple.OptionMultiple.BaQu_Id + " del tema con id " + UpdatedAnswerOptionMultiple.OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + UpdatedAnswerOptionMultiple.OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ListAnswerOptionMultiple = ApplicationDbContext.AnswerOptionMultiples.Where(x => x.AnOp_Id == model.AnOp_Id && x.OptionMultiple.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListOptionMultiple = ApplicationDbContext.OptionMultiples.Where(x => x.OpMu_Id == model.OpMu_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.OpMu_Id = model.OpMu_Id;
                model.AnOp_Id = model.AnOp_Id;
                return RedirectToAction("EditAnswerOMultiple", new { id = model.AnOp_Id });
            }
        }
        [Authorize]
        public ActionResult DeleteOptionMultiple(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            OptionMultiple OptionMultiple = ApplicationDbContext.OptionMultiples.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(OptionMultiple.BaQu_Id);
            var answer = OptionMultiple.AnswerOptionMultiple.Where(x => x.OpMu_Id == OptionMultiple.OpMu_Id).Sum(x => x.AnOp_Id);
            if (answer == 0)
            {
                TempData["Add"] = "Pregunta eliminada con éxito";
                var table = ApplicationDbContext.TableChanges.Find(38);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(187);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = OptionMultiple.OpMu_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Eliminó una pregunta de opción múltiple con id " + OptionMultiple.OpMu_Id + " del tema con id " + OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.OptionMultiples.Remove(OptionMultiple);
                ApplicationDbContext.SaveChanges();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                return RedirectToAction("Questions", new { id = model.BaQu_Id });

            }
            else
            {
                TempData["Add"] = "No se puede eliminar la pregunta hasta que elimine las respuestas ";
                var table = ApplicationDbContext.TableChanges.Find(38);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(187);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = OptionMultiple.OpMu_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.NoRealizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar una pregunta de opción múltiple con id " + OptionMultiple.OpMu_Id + " del tema con id " + OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + " pero hay respuestas asociadas a la pregunta, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }

        }
        [Authorize]
        public ActionResult DeleteAnswerOptionMultiple(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            AnswerOptionMultiple AnswerOptionMultipled = ApplicationDbContext.AnswerOptionMultiples.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(AnswerOptionMultipled.OptionMultiple.BankQuestion.BaQu_Id);
            TempData["Add"] = "Respuesta Eliminada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(3);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(190);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = AnswerOptionMultipled.AnOp_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Eliminó una respuesta con id " + AnswerOptionMultipled.AnOp_Id + " perteneciente a una pregunta de opción múltiple con id " + AnswerOptionMultipled.OpMu_Id + " del tema con id " + AnswerOptionMultipled.OptionMultiple.BankQuestion.ToCo_Id + " perteneciente al curso con id " + AnswerOptionMultipled.OptionMultiple.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.AnswerOptionMultiples.Remove(AnswerOptionMultipled);
            ApplicationDbContext.SaveChanges();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            return RedirectToAction("Questions", new { id = model.BaQu_Id });
        }



        [Authorize]
        public ActionResult AddPairing(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Pregunta Creada";
                Pairing pairing = new Pairing { Pair_Question = model.Pair_Question, Pair_Description = model.Pair_Description, BankQuestion = Getbank };
                ApplicationDbContext.Pairings.Add(pairing);
                ApplicationDbContext.SaveChanges();
                var PairingId = ApplicationDbContext.Pairings.Find(pairing.Pair_Id);
                AnswerPairing answerpairing = new AnswerPairing { AnPa_OptionAnswer = model.AnPa_OptionAnswer, AnPa_OptionsQuestion = model.AnPa_OptionsQuestion, Pairing = PairingId };
                ApplicationDbContext.AnswerPairings.Add(answerpairing);
                ApplicationDbContext.SaveChanges();
                model.BaQu_Id = Getbank.BaQu_Id;
                var table = ApplicationDbContext.TableChanges.Find(39);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(191);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = pairing.Pair_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una pregunta de apareamiento con id " + pairing.Pair_Id + " en la evaluación con id " + pairing.BaQu_Id + " del tema con id " + pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                model.ActualRole = GetActualUserId().Role;
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
        }
        [Authorize]
        public ActionResult AddPairings(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getpairing = ApplicationDbContext.Pairings.Find(id);
            TempData["Add"] = "Pregunta Creada";
            List<Models.Lms.AnswerPairing> listanswerpairing = Getpairing.AnswerPairing.Where(x => x.Pair_Id == Getpairing.Pair_Id && x.Pairing.BankQuestion.Company.CompanyId == Getpairing.BankQuestion.Company.CompanyId).ToList();
            model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == Getpairing.Pair_Id && x.BankQuestion.Company.CompanyId == Getpairing.BankQuestion.Company.CompanyId).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getpairing.BankQuestion.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getpairing.BankQuestion;
            model.BaQu_Id = Getpairing.BankQuestion.BaQu_Id;
            model.Pair_Id = Getpairing.Pair_Id;
            model.Sesion = GetActualUserId().SesionUser;

            return PartialView("_AddAnswerPairing", model);
        }
        [Authorize]
        public ActionResult AddOtherPairings(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbankquestion = ApplicationDbContext.BankQuestions.Find(id);
            TempData["Add"] = "Pregunta Creada";
            List<Models.Lms.Pairing> listpairing = Getbankquestion.Pairing.Where(x => x.BaQu_Id == Getbankquestion.BaQu_Id && x.BankQuestion.Company.CompanyId == Getbankquestion.Company.CompanyId).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbankquestion.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbankquestion;
            model.BaQu_Id = Getbankquestion.BaQu_Id;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddPairing", model);
        }
        [Authorize]
        public ActionResult EditPairing(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            Pairing Pairing = ApplicationDbContext.Pairings.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(Pairing.BaQu_Id);
            TempData["Form"] = "Activar Formulario";
            List<Pairing> listpairing = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == Pairing.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.ListPairings = listpairing;
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.ActualRole = GetActualUserId().Role;
            model.Pair_Id = Pairing.Pair_Id;
            model.Pair_Question = Pairing.Pair_Question;
            model.Pair_Description = Pairing.Pair_Description;
            return RedirectToAction("EditPairingQ", new { id = model.Pair_Id });
        }
        [Authorize]
        public ActionResult EditPairingQ(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            Pairing PairingId = ApplicationDbContext.Pairings.Find(id);
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(PairingId.BankQuestion.BaQu_Id);
            List<TopicsCourse> listtopic = ApplicationDbContext.TopicsCourses.Where(x => x.ToCo_Id == actualbank.ToCo_Id && x.Module.Company.CompanyId == GetModuleCompanyId).ToList();
            List<BankQuestion> listbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == actualbank.BaQu_Id && x.Company.CompanyId == GetModuleCompanyId).ToList();
            List<Models.Lms.Pairing> listpairing = ApplicationDbContext.Pairings.Where(x => x.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId && x.BankQuestion.BaQu_Id == actualbank.BaQu_Id).ToList();
            List<Models.Lms.AnswerPairing> listanswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.Pairing.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId).ToList();
            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListTopic = listtopic,
                BaQu_Id = actualbank.BaQu_Id,
                ListBankQuestion = listbank,
                ListPairings = listpairing,
                ListAnswerpairing = listanswerpairing,
                Pair_Id = PairingId.Pair_Id,
                Pair_Question = PairingId.Pair_Question,
                Pair_Description = PairingId.Pair_Description,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(38);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(192);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = PairingId.Pair_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar una pregunta para modificar de apareamiento con id " + PairingId.Pair_Id + " en la evaluación con id " + PairingId.BaQu_Id + " del tema con id " + PairingId.BankQuestion.ToCo_Id + " perteneciente al curso con id " + PairingId.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult EditPairings(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                Pairing UpdatedPairing = ApplicationDbContext.Pairings.Find(model.Pair_Id);
                TempData["Add"] = "Preguta Modificada";
                UpdatedPairing.Pair_Question = model.Pair_Question;
                UpdatedPairing.Pair_Description = model.Pair_Description;
                model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == UpdatedPairing.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.Pair_Id = UpdatedPairing.Pair_Id;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(39);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(192);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UpdatedPairing.Pair_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico una pregunta de apareamiento con id " + UpdatedPairing.Pair_Id + " en la evaluación con id " + UpdatedPairing.BaQu_Id + " del tema con id " + UpdatedPairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + UpdatedPairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == model.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.Pair_Id = model.Pair_Id;
                return RedirectToAction("EditPairingQ", new { id = model.Pair_Id });
            }
        }
        [Authorize]
        public ActionResult AddAnswerPairing(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Respuesta Creada";
                var PairingId = ApplicationDbContext.Pairings.Find(model.Pair_Id);
                AnswerPairing answerpairing = new AnswerPairing { AnPa_OptionsQuestion = model.AnPa_OptionsQuestion, AnPa_OptionAnswer = model.AnPa_OptionAnswer, Pairing = PairingId };
                ApplicationDbContext.AnswerPairings.Add(answerpairing);
                ApplicationDbContext.SaveChanges();
                model.ListAnswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.AnPa_Id == answerpairing.AnPa_Id && x.Pairing.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == PairingId.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.Pair_Id = PairingId.Pair_Id;
                model.AnPa_Id = answerpairing.AnPa_Id;
                var table = ApplicationDbContext.TableChanges.Find(4);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(194);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = answerpairing.AnPa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Creo una respuesta de apareamiento con id " + answerpairing.AnPa_Id + " perteneciente a una pregunta de apareamientos con id " + answerpairing.AnPa_Id + " en la evaluación con id " + answerpairing.Pairing.BaQu_Id + " del tema con id " + answerpairing.Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + answerpairing.Pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                model.ActualRole = GetActualUserId().Role;
                model.Sesion = GetActualUserId().SesionUser;
                return View("Questions", model);
            }
        }
        [Authorize]
        public ActionResult EditAnswerPairing(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            AnswerPairing AnswerPairing = ApplicationDbContext.AnswerPairings.Find(id);
            Pairing Pairing = ApplicationDbContext.Pairings.Find(AnswerPairing.Pairing.Pair_Id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(Pairing.BaQu_Id);
            TempData["FormAnswer"] = "Activar Formulario";
            List<AnswerPairing> listanswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.AnPa_Id == AnswerPairing.AnPa_Id && x.Pairing.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            List<Pairing> listpairing = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == Pairing.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.ListPairings = listpairing;
            model.ListAnswerpairing = listanswerpairing;
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.ActualRole = GetActualUserId().Role;
            model.AnPa_Id = AnswerPairing.AnPa_Id;
            model.AnPa_OptionAnswer = AnswerPairing.AnPa_OptionAnswer;
            model.AnPa_OptionsQuestion = AnswerPairing.AnPa_OptionsQuestion;
            return RedirectToAction("EditAnswerP", new { id = model.AnPa_Id });
        }
        [Authorize]
        public ActionResult EditAnswerP(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            AnswerPairing AnswerPairing = ApplicationDbContext.AnswerPairings.Find(id);
            Pairing PairingId = ApplicationDbContext.Pairings.Find(AnswerPairing.Pairing.Pair_Id);
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(PairingId.BankQuestion.BaQu_Id);
            List<TopicsCourse> listtopic = ApplicationDbContext.TopicsCourses.Where(x => x.ToCo_Id == actualbank.ToCo_Id && x.Module.Company.CompanyId == GetModuleCompanyId).ToList();
            List<BankQuestion> listbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == actualbank.BaQu_Id && x.Company.CompanyId == GetModuleCompanyId).ToList();
            List<Models.Lms.Pairing> listpairing = ApplicationDbContext.Pairings.Where(x => x.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId && x.BankQuestion.BaQu_Id == actualbank.BaQu_Id).ToList();
            List<Models.Lms.AnswerPairing> listanswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.Pairing.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId).ToList();
            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListTopic = listtopic,
                BaQu_Id = actualbank.BaQu_Id,
                ListBankQuestion = listbank,
                ListPairings = listpairing,
                ListAnswerpairing = listanswerpairing,
                Pair_Id = AnswerPairing.Pairing.Pair_Id,
                AnPa_Id = AnswerPairing.AnPa_Id,
                AnPa_OptionAnswer = AnswerPairing.AnPa_OptionAnswer,
                AnPa_OptionsQuestion = AnswerPairing.AnPa_OptionsQuestion,
                Logo = GetUrlLogo()

            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(4);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(195);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = AnswerPairing.AnPa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Seleccionó una  respuesta de apareamiento para modificar con id " + AnswerPairing.AnPa_Id + " perteneciente a una pregunta de apareamiento con id " + AnswerPairing.Pair_Id + " en la evaluación con id " + AnswerPairing.Pairing.BaQu_Id + " del tema con id " + AnswerPairing.Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + AnswerPairing.Pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult EditAnswerPairings(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            var PairingId = ApplicationDbContext.Pairings.Find(model.Pair_Id);
            if (ModelState.IsValid)
            {
                AnswerPairing UpdatedAnswerPairing = ApplicationDbContext.AnswerPairings.Find(model.AnPa_Id);
                TempData["Add"] = "Respuesta Modificada";
                UpdatedAnswerPairing.AnPa_OptionAnswer = model.AnPa_OptionAnswer;
                UpdatedAnswerPairing.AnPa_OptionsQuestion = model.AnPa_OptionsQuestion;
                model.ListAnswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.AnPa_Id == UpdatedAnswerPairing.AnPa_Id && x.Pairing.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == UpdatedAnswerPairing.Pairing.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.Pair_Id = UpdatedAnswerPairing.Pairing.Pair_Id;
                model.AnPa_Id = UpdatedAnswerPairing.AnPa_Id;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(4);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(195);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UpdatedAnswerPairing.AnPa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modificó una respuesta de apareamiento con id " + UpdatedAnswerPairing.AnPa_Id + " perteneciente a una pregunta de apareamiento con id " + UpdatedAnswerPairing.Pair_Id + " en la evaluación con id " + UpdatedAnswerPairing.Pairing.BaQu_Id + " del tema con id " + UpdatedAnswerPairing.Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + UpdatedAnswerPairing.Pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ListAnswerpairing = ApplicationDbContext.AnswerPairings.Where(x => x.AnPa_Id == model.AnPa_Id && x.Pairing.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListPairings = ApplicationDbContext.Pairings.Where(x => x.Pair_Id == model.Pair_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.Pair_Id = model.Pair_Id;
                model.AnPa_Id = model.AnPa_Id;
                return RedirectToAction("EditAnswerP", new { id = model.AnPa_Id });
            }
        }
        [Authorize]
        public ActionResult DeletePairing(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            Pairing Pairing = ApplicationDbContext.Pairings.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(Pairing.BaQu_Id);
            var answer = Pairing.AnswerPairing.Where(x => x.Pair_Id == Pairing.Pair_Id).Sum(x => x.AnPa_Id);
            if (answer == 0)
            {
                TempData["Add"] = "Pregunta Eliminada";
                var table = ApplicationDbContext.TableChanges.Find(39);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(193);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = Pairing.Pair_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Eliminó una pregunta de apareamiento con id " + Pairing.Pair_Id + " del tema con id " + Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + Pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Pairings.Remove(Pairing);
                ApplicationDbContext.SaveChanges();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;

                return RedirectToAction("Questions", new { id = model.BaQu_Id });

            }
            else
            {
                TempData["Add"] = "No se puede eliminar la pregunta hasta que elimine las respuestas.";
                var table = ApplicationDbContext.TableChanges.Find(39);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(193);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = Pairing.Pair_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.NoRealizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar una pregunta de apareamiento con id " + Pairing.Pair_Id + " del tema con id " + Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + Pairing.BankQuestion.TopicsCourse.Modu_Id + " pero hay respuestas asociadas a la pregunta, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
        }
        [Authorize]
        public ActionResult DeleteAnswerPairing(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            AnswerPairing AnswerPairing = ApplicationDbContext.AnswerPairings.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(AnswerPairing.Pairing.BankQuestion.BaQu_Id);
            TempData["Add"] = "Respuesta Eliminada";
            var table = ApplicationDbContext.TableChanges.Find(4);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(196);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = AnswerPairing.AnPa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Eliminó una respuesta con id " + AnswerPairing.AnPa_Id + " perteneciente a una pregunta de apareamiento con id " + AnswerPairing.Pair_Id + " del tema con id " + AnswerPairing.Pairing.BankQuestion.ToCo_Id + " perteneciente al curso con id " + AnswerPairing.Pairing.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.AnswerPairings.Remove(AnswerPairing);
            ApplicationDbContext.SaveChanges();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            return RedirectToAction("Questions", new { id = model.BaQu_Id });
        }



        [Authorize]
        public ActionResult AddTrueorFlase(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Pregunta creada con éxito";
                TrueOrFalse trueorfalse = new TrueOrFalse { TrFa_Question = model.TrFa_Question, TrFa_Description = model.TrFa_Description, TrFa_Content = model.TrFa_Content, TrFa_State = model.TrFa_State, BankQuestion = Getbank };
                ApplicationDbContext.TrueOrFalses.Add(trueorfalse);
                ApplicationDbContext.SaveChanges();
                model.ListTrueOrFalse = ApplicationDbContext.TrueOrFalses.Where(x => x.TrFa_Id == trueorfalse.TrFa_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.TrFa_Id = trueorfalse.TrFa_Id;
                var table = ApplicationDbContext.TableChanges.Find(63);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(197);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = trueorfalse.TrFa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una pregunta de verdadero-falso con id " + trueorfalse.TrFa_Id + " en la evaluación con id " + trueorfalse.TrFa_Id + " del tema con id " + trueorfalse.BankQuestion.ToCo_Id + " perteneciente al curso con id " + trueorfalse.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                model.ActualRole = GetActualUserId().Role;
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
        }

        [Authorize]
        public ActionResult AddTrueorFalses(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(id);
            TempData["Add"] = "Pregunta creada con éxito";
            List<Models.Lms.TrueOrFalse> listtrueorfalse = Getbank.TrueOrFalse.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.BankQuestion.Company.CompanyId == Getbank.Company.CompanyId).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddTrueOrFalse", model);
        }
        [Authorize]
        public ActionResult EditTrueorFalses(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            TrueOrFalse TrueOrFalse = ApplicationDbContext.TrueOrFalses.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(TrueOrFalse.BaQu_Id);
            TempData["Form"] = "Activar Formulario";
            List<TrueOrFalse> listtrueorfalse = ApplicationDbContext.TrueOrFalses.Where(x => x.TrFa_Id == TrueOrFalse.TrFa_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.ListTrueOrFalse = listtrueorfalse;
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            model.ActualRole = GetActualUserId().Role;
            model.TrFa_Id = TrueOrFalse.TrFa_Id;
            model.TrFa_Question = TrueOrFalse.TrFa_Question;
            model.TrFa_State = TrueOrFalse.TrFa_State;
            return RedirectToAction("EditTrueOrFalse", new { id = model.TrFa_Id });
        }
        [Authorize]
        public ActionResult EditTrueOrFalse(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            TrueOrFalse TrueOrFalseId = ApplicationDbContext.TrueOrFalses.Find(id);
            BankQuestion actualbank = ApplicationDbContext.BankQuestions.Find(TrueOrFalseId.BankQuestion.BaQu_Id);
            List<TopicsCourse> listtopic = ApplicationDbContext.TopicsCourses.Where(x => x.ToCo_Id == actualbank.ToCo_Id && x.Module.Company.CompanyId == GetModuleCompanyId).ToList();
            List<BankQuestion> listbank = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == actualbank.BaQu_Id && x.Company.CompanyId == GetModuleCompanyId).ToList();
            List<Models.Lms.TrueOrFalse> listtrueorfalse = ApplicationDbContext.TrueOrFalses.Where(x => x.BankQuestion.Company.CompanyId == actualbank.Company.CompanyId && x.BankQuestion.BaQu_Id == actualbank.BaQu_Id).ToList();

            AdminTrainingTestViewModel model = new AdminTrainingTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListTopic = listtopic,
                BaQu_Id = actualbank.BaQu_Id,
                ListBankQuestion = listbank,
                ListTrueOrFalse = listtrueorfalse,
                TrFa_Id = TrueOrFalseId.TrFa_Id,
                TrFa_Question = TrueOrFalseId.TrFa_Question,
                TrFa_Description = TrueOrFalseId.TrFa_Description,
                TrFa_Content = TrueOrFalseId.TrFa_Content,
                TrFa_State = TrueOrFalseId.TrFa_State,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(63);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(198);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = TrueOrFalseId.TrFa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar una pregunta para modificar de verdadero-falso con id " + TrueOrFalseId.TrFa_Id + " en la evaluación con id " + TrueOrFalseId.BaQu_Id + " del tema con id " + TrueOrFalseId.BankQuestion.ToCo_Id + " perteneciente al curso con id " + TrueOrFalseId.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult EditTrueOrFalses(AdminTrainingTestViewModel model)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getbank = ApplicationDbContext.BankQuestions.Find(model.BaQu_Id);
            if (ModelState.IsValid)
            {
                TrueOrFalse UpdatedTrueOrFalse = ApplicationDbContext.TrueOrFalses.Find(model.TrFa_Id);
                TempData["Add"] = "Pregunta modificada con éxito";
                UpdatedTrueOrFalse.TrFa_Question = model.TrFa_Question;
                UpdatedTrueOrFalse.TrFa_Description = model.TrFa_Description;
                UpdatedTrueOrFalse.TrFa_Content = model.TrFa_Content;
                UpdatedTrueOrFalse.TrFa_State = model.TrFa_State;
                model.ListTrueOrFalse = ApplicationDbContext.TrueOrFalses.Where(x => x.TrFa_Id == UpdatedTrueOrFalse.TrFa_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.TrFa_Id = UpdatedTrueOrFalse.TrFa_Id;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(63);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(198);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UpdatedTrueOrFalse.TrFa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico una pregunta de verdadero-falso con id " + UpdatedTrueOrFalse.TrFa_Id + " en la evaluación con id " + UpdatedTrueOrFalse.BaQu_Id + " del tema con id " + UpdatedTrueOrFalse.BankQuestion.ToCo_Id + " perteneciente al curso con id " + UpdatedTrueOrFalse.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Questions", new { id = model.BaQu_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ListTrueOrFalse = ApplicationDbContext.TrueOrFalses.Where(x => x.TrFa_Id == model.TrFa_Id && x.BankQuestion.Company.CompanyId == GetTopicCompany).ToList();
                model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
                model.BankQuestion = Getbank;
                model.BaQu_Id = Getbank.BaQu_Id;
                model.TrFa_Id = model.TrFa_Id;
                return RedirectToAction("EditTrueOrFalse", new { id = model.OpQu_Id });
            }
        }


        [Authorize]
        public ActionResult DeleteTrueorFalse(AdminTrainingTestViewModel model, int id)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            TrueOrFalse TrueOrFalse = ApplicationDbContext.TrueOrFalses.Find(id);
            var Getbank = ApplicationDbContext.BankQuestions.Find(TrueOrFalse.BaQu_Id);
            TempData["Add"] = "Pregunta eliminada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(63);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(199);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = TrueOrFalse.TrFa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Eliminó una pregunta de verdadero-falso con id " + TrueOrFalse.TrFa_Id + " del tema con id " + TrueOrFalse.BankQuestion.ToCo_Id + " perteneciente al curso con id " + TrueOrFalse.BankQuestion.TopicsCourse.Modu_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.TrueOrFalses.Remove(TrueOrFalse);
            ApplicationDbContext.SaveChanges();
            model.ListBankQuestion = ApplicationDbContext.BankQuestions.Where(x => x.BaQu_Id == Getbank.BaQu_Id && x.Company.CompanyId == GetTopicCompany).ToList();
            model.BankQuestion = Getbank;
            model.BaQu_Id = Getbank.BaQu_Id;
            return RedirectToAction("Questions", new { id = model.BaQu_Id });
        }

        [Authorize]
        public ActionResult Enrollment(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(205);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de matriculas, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachUser(AdminTrainingEnrollmentViewModel model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario, sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Enrollment");
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser;
                var conteo = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser))).ToList();
                if (conteo.Count > 0)
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, conteo.Count);
                }
                else
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 6);
                }

                model = new AdminTrainingEnrollmentViewModel { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("Enrollment", model);
            }
        }

        [Authorize]
        public ActionResult AddEnrollment(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrolmentV = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var enrolmentE = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            //var enrolmentA = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentvirtual = enrolmentV,
                listenrollmentevaluative = enrolmentE,

            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(208);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de matricula indivial, en la compañía con id " + company.CompanyId,
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
        public ActionResult EnrollmentD(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.AdministradordeFormacion).ToList().ToPagedList(page ?? 1, 6);
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(205);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de matriculas de docentes, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachUserD(AdminTrainingEnrollmentViewModel model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario, sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("EnrollmentD");
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser)) && x.Role == ROLES.AdministradordeFormacion).ToList().ToPagedList(page ?? 1, 6);
                model = new AdminTrainingEnrollmentViewModel { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("EnrollmentD", model);
            }
        }

        [Authorize]
        public ActionResult AddEnrollmentD(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrolmentV = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var enrolmentE = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            //var enrolmentA = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentvirtual = enrolmentV,
                listenrollmentevaluative = enrolmentE,

            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(208);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de matricula indivial, en la compañía con id " + company.CompanyId,
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
        public ActionResult CancelEnrollment(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrolmentV = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var enrolmentE = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            //var enrolmentA = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentvirtual = enrolmentV,
                listenrollmentevaluative = enrolmentE,
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(213);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de  des matriculación indivial, en la compañía con id " + company.CompanyId,
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
        public ActionResult CancelEnrollmentmVirtuals(int Modu_Id, string User_Id)
        {
            var GetEnrollment = ApplicationDbContext.Enrollments.Find(Modu_Id);
            if (ModelState.IsValid)
            {
                var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == User_Id && x.TopicsCourse.Module.Modu_Id == GetEnrollment.Module.Modu_Id).ToList();
                var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == GetEnrollment.Modu_Id).ToList();
                if (advanceuser.Count != 0 || attempts.Count != 0)
                {
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = new CodeLogs();
                    if (GetEnrollment.Module.Modu_TypeOfModule == CURSO.Virtual)
                    {
                        code = ApplicationDbContext.CodeLogs.Find(214);
                    }
                    else
                    {
                        code = ApplicationDbContext.CodeLogs.Find(215);
                    }
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = GetEnrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular el curso con id " + GetEnrollment.Enro_Id + " asociado al usuario con id " + User_Id + "pero este usuario ya ha empezado este curso, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    TempData["Add"] = "No se puede cancelar la matricula debido a que este usuario ya ha iniciado este curso.";
                    return RedirectToAction("CancelEnrollment", new { id = User_Id });
                }
                else
                {
                    TempData["Add"] = "Usuario desmatriculado.";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = new CodeLogs();
                    if (GetEnrollment.Module.Modu_TypeOfModule == CURSO.Virtual)
                    {
                        code = ApplicationDbContext.CodeLogs.Find(214);
                    }
                    else
                    {
                        code = ApplicationDbContext.CodeLogs.Find(215);
                    }
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = GetEnrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Des matriculo el curso con id " + GetEnrollment.Enro_Id + " asociado al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.Enrollments.Remove(GetEnrollment);
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("CancelEnrollment", new { id = User_Id });
                }
            }
            else
            {
                return RedirectToAction("CancelEnrollment", new { id = User_Id });
            }
        }

        [Authorize]
        public ActionResult ModuleVirtual(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Virtual && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentvirtual = enrrolment,
                listmodule = module,
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(209);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos virtuales disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return PartialView("_ModuleVirtuals", model);
        }


        [Authorize]
        public ActionResult EnrollmentmModuleVirtuals(int Modu_Id, string User_Id)
        {

            var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var getuser = ApplicationDbContext.Users.Find(User_Id);
            if (ModelState.IsValid)
            {
                var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (GetModule.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime a = finish;
                    TempData["Add"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo, Enro_RoleEnrollment = ROLEENROLLMENT.Estudiante };
                    ApplicationDbContext.Enrollments.Add(enrollment);
                    if (GetModule.QSMActive == 1)
                    {
                        QuienSabeMasPuntaje quienSabeMasPuntaje = new QuienSabeMasPuntaje()
                        {
                            User_Id = GetActualUserId().Id,
                            User_Id_QSM = GetActualUserId().Document,
                            Mudole_Id = GetModule.Modu_Id,
                            FechaPresentacion = DateTime.Now,
                            Puntaje = 0,
                            PorcentajeAprobacion = 0
                        };
                        ApplicationDbContext.QuienSabeMasPuntajes.Add(quienSabeMasPuntaje);
                    }
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(211);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = enrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollment", new { id = getuser.Id });
                }
                else
                {
                    TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(211);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollment", new { id = getuser.Id });

                }

            }
            else
            {
                TempData["Add"] = "Problemas al registrar matricula";
                return RedirectToAction("AddEnrollment", new { id = getuser.Id });
            }
        }

        [Authorize]
        public ActionResult ModuleEvaluative(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Evaluativo && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentevaluative = enrrolment,
                listmodule = module,
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(210);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos Evaluativos disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return PartialView("_ModuleEvaluatives", model);
        }


        [Authorize]
        public ActionResult EnrollmentmModuleEvaluatives(int Modu_Id, string User_Id)
        {

            var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var getuser = ApplicationDbContext.Users.Find(User_Id);
            if (ModelState.IsValid)
            {
                var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (GetModule.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime a = finish;
                    TempData["Add"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo, Enro_RoleEnrollment = ROLEENROLLMENT.Estudiante };
                    ApplicationDbContext.Enrollments.Add(enrollment);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(212);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = enrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollment", new { id = getuser.Id });
                }
                else
                {
                    TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(212);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollment", new { id = getuser.Id });
                }
            }
            else
            {
                TempData["Add"] = "Problemas al registrar matricula";
                return RedirectToAction("AddEnrollment", new { id = getuser.Id });
            }
        }

        [Authorize]
        public ActionResult ModuleVirtualD(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Virtual).ToList();
            var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Virtual && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentvirtual = enrrolment,
                listmodule = module,
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(209);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos virtuales disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return PartialView("_ModuleVirtualsD", model);
        }


        [Authorize]
        public ActionResult EnrollmentmModuleVirtualsD(int Modu_Id, string User_Id)
        {

            var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var getuser = ApplicationDbContext.Users.Find(User_Id);
            if (ModelState.IsValid)
            {
                var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (GetModule.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime a = finish;
                    TempData["Add"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo, Enro_RoleEnrollment = ROLEENROLLMENT.Docente };
                    ApplicationDbContext.Enrollments.Add(enrollment);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(211);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = enrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollmentD", new { id = getuser.Id });
                }
                else
                {
                    TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(211);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollmentD", new { id = getuser.Id });

                }

            }
            else
            {
                TempData["Add"] = "Problemas al registrar matricula";
                return RedirectToAction("AddEnrollment", new { id = getuser.Id });
            }
        }

        [Authorize]
        public ActionResult ModuleEvaluativeD(string id)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Evaluativo).ToList();
            var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Evaluativo && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
            AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
            {
                ActualRole = GetActualUserId().Role,
                User = user,
                listenrollmentevaluative = enrrolment,
                listmodule = module,
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(210);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos Evaluativos disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return PartialView("_ModuleEvaluativesD", model);
        }


        [Authorize]
        public ActionResult EnrollmentmModuleEvaluativesD(int Modu_Id, string User_Id)
        {

            var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var getuser = ApplicationDbContext.Users.Find(User_Id);
            if (ModelState.IsValid)
            {
                var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (GetModule.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime a = finish;
                    TempData["Add"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo, Enro_RoleEnrollment = ROLEENROLLMENT.Docente };
                    ApplicationDbContext.Enrollments.Add(enrollment);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(212);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = enrollment.Enro_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollmentD", new { id = getuser.Id });
                }
                else
                {
                    TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(212);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("AddEnrollmentD", new { id = getuser.Id });
                }
            }
            else
            {
                TempData["Add"] = "Problemas al registrar matricula";
                return RedirectToAction("AddEnrollmentD", new { id = getuser.Id });
            }
        }

        //[Authorize]
        //public ActionResult ModuleAplicative(string id)
        //{
        //    var user = ApplicationDbContext.Users.Find(id);
        //    var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //    var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Aplicativo && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
        //    AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
        //    {
        //        ActualRole = GetActualUserId().Role,
        //        User = user,
        //        listenrollmentevaluative = enrrolment,
        //        listmodule = module,
        //    };
        //    model.Sesion = GetActualUserId().SesionUser;
        //    model.Logo = GetUrlLogo();
        //    var table = ApplicationDbContext.TableChanges.Find(22);
        //    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //    var code = ApplicationDbContext.CodeLogs.Find(210);
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
        //            Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos Evaluativos disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
        //            Company = company,
        //            Company_Id = company.CompanyId,
        //            Log_Ip = ip
        //        };
        //        ApplicationDbContext.Logs.Add(logsesiontrue);
        //        ApplicationDbContext.SaveChanges();
        //    }
        //    return PartialView("_ModuleAplicatives", model);
        //}
        //[Authorize]
        //public ActionResult EnrollmentmModuleAplicatives(int Modu_Id, string User_Id)
        //{

        //    var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
        //    var getuser = ApplicationDbContext.Users.Find(User_Id);
        //    if (ModelState.IsValid)
        //    {
        //        var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
        //        if (getEnrollment.Count == 0)
        //        {
        //            DateTime finish = new DateTime();
        //            switch (GetModule.Modu_Period)
        //            {
        //                case VIGENCIA.Dias:
        //                    finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Meses:
        //                    finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Años:
        //                    finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            DateTime a = finish;
        //            TempData["Add"] = "Matricula Registrada";
        //            Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
        //            ApplicationDbContext.Enrollments.Add(enrollment);
        //            ApplicationDbContext.SaveChanges();
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(212);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = enrollment.Enro_Id.ToString()
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //            return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //        }
        //        else
        //        {
        //            TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(212);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = null
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //            return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //        }
        //    }
        //    else
        //    {
        //        TempData["Add"] = "Problemas al registrar matricula";
        //        return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //    }
        //}
        [Authorize]
        public ActionResult NewAttempts()
        {
            var company = GetActualUserId().CompanyId;
            var newattempts = ApplicationDbContext.NewAttempts.Where(x => x.ApplicationUser.CompanyId == company).ToList();
            AdminTrainingNewAttemptsViewMode model = new AdminTrainingNewAttemptsViewMode
            {
                NewAttempts = newattempts
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(7);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(231);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company1 = ApplicationDbContext.Companies.Find(idcompany);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingresó a la vista de gestión de intentos, en la compañía con id " + company1.CompanyId,
                    Company = company1,
                    Company_Id = company1.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
        [Authorize]
        public ActionResult NewAttemptsUser(int BaQu_Id, string User_Id)
        {
            if (ModelState.IsValid)
            {
                var GetAttempts = ApplicationDbContext.Attempts.Where(x => x.BaQu_Id == BaQu_Id && x.UserId == User_Id).OrderByDescending(x => x.Atte_Id).First();
                if (GetAttempts != null)
                {
                    var newattempts = ApplicationDbContext.NewAttempts.FirstOrDefault(x => x.ApplicationUser.Id == User_Id && x.BaQu_Id == BaQu_Id);
                    newattempts.Attempts = ATTEMPTS.Si;
                    var table = ApplicationDbContext.TableChanges.Find(7);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(232);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company1 = ApplicationDbContext.Companies.Find(idcompany);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Aprobó un nuevo intento de evaluación con id " + newattempts.NeAt_Id + ", en la compañía con id " + company1.CompanyId,
                            Company = company1,
                            Company_Id = company1.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.Attempts.Remove(GetAttempts);
                    ApplicationDbContext.SaveChanges();
                    TempData["Info"] = "Nuevo Intento Asignado";
                }
            }
            return RedirectToAction("NewAttempts");
        }

        // desde aqui el copiado de banco de pregunta
        public ActionResult ManagementOfBankQuestions(int id)
        {
            AdminManagementBankQuestions model = new AdminManagementBankQuestions
            {
                ActualRole = GetActualUserId().Role,
                bankquestion = ApplicationDbContext.BankQuestions.ToList(),
                bankReceiving = id
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(8);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(310);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company1 = ApplicationDbContext.Companies.Find(idcompany);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingresó a la vista de selección de banco de preguntas, en la compañía con id " + company1.CompanyId,
                    Company = company1,
                    Company_Id = company1.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }
        public ActionResult ShowBankQuestions(int id, int idBanckReiceiving)
        {
            BankQuestion bank = ApplicationDbContext.BankQuestions.Find(id);
            AdminManagementBankQuestions model = new AdminManagementBankQuestions
            {
                ActualRole = GetActualUserId().Role,
                bank = bank,
                OptionMultiple = FillListOptionmultiple(bank.OptionMultiple),
                Pairing = FillListPairing(bank.Pairing),
                TrueOrFalse = FillListTrueorfalse(bank.TrueOrFalse),
                bankReceiving = idBanckReiceiving
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(8);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(311);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company1 = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = bank.BaQu_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingresó a la vista de preguntas del banco de preguntas seleccionado con id " + bank.BaQu_Id + ", en la compañía con id " + company1.CompanyId,
                    Company = company1,
                    Company_Id = company1.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return PartialView("_ViewQuestions", model);
        }
        public List<ListOptionmultiple> FillListOptionmultiple(ICollection<OptionMultiple> multiple)
        {
            List<ListOptionmultiple> ListToFill = new List<ListOptionmultiple>();
            foreach (OptionMultiple item in multiple)
            {
                ListToFill.Add(new ListOptionmultiple
                {
                    OpMu_Id = item.OpMu_Id,
                    OpMu_Description = item.OpMu_Description,
                    OpMu_Score = item.OpMu_Score,
                    BaQu_Id = item.BaQu_Id,
                    OpMu_Question = item.OpMu_Question,
                    Check = false
                });
            }
            return ListToFill;
        }
        public List<ListPairing> FillListPairing(ICollection<Pairing> pairing)
        {
            List<ListPairing> ListToFill = new List<ListPairing>();
            foreach (Pairing item in pairing)
            {
                ListToFill.Add(new ListPairing
                {
                    Pair_Id = item.Pair_Id,
                    Pair_Question = item.Pair_Question,
                    Pair_Score = item.Pair_Score,
                    Pair_Description = item.Pair_Description,
                    BaQu_Id = item.BaQu_Id,
                    Check = false
                });
            }
            return ListToFill;
        }
        public List<ListTrueorfalse> FillListTrueorfalse(ICollection<TrueOrFalse> TrueOrFalse)
        {
            List<ListTrueorfalse> ListToFill = new List<ListTrueorfalse>();
            foreach (TrueOrFalse item in TrueOrFalse)
            {
                ListToFill.Add(new ListTrueorfalse
                {
                    BaQu_Id = item.BaQu_Id,
                    TrFa_Id = item.TrFa_Id,
                    TrFa_Question = item.TrFa_Question,
                    TrFa_Description = item.TrFa_Description,
                    TrFa_Content = item.TrFa_Content,
                    TrFa_FalseAnswer = item.TrFa_FalseAnswer,
                    TrFa_TrueAnswer = item.TrFa_TrueAnswer,
                    TrFa_Score = item.TrFa_Score,
                    TrFa_State = item.TrFa_State,
                    Check = false
                });
            }
            return ListToFill;
        }
        public ActionResult SelectdQuestions(AdminManagementBankQuestions model)
        {

            if (ModelState.IsValid)
            {
                var bankkk = ApplicationDbContext.BankQuestions.Find(model.bank.BaQu_Id);
                if (model.OptionMultiple != null)
                {
                    foreach (var x in model.OptionMultiple)
                    {
                        if (x.Check == true)
                        {
                            OptionMultiple questionToCreate = ApplicationDbContext.OptionMultiples.Find(x.OpMu_Id);
                            questionToCreate.BaQu_Id = model.bankReceiving;
                            ApplicationDbContext.OptionMultiples.Add(questionToCreate);
                            List<AnswerOptionMultiple> answesOfQuestion = ApplicationDbContext.AnswerOptionMultiples.Where(z => z.OpMu_Id == x.OpMu_Id).ToList();
                            foreach (var answer in answesOfQuestion)
                            {
                                answer.OpMu_Id = questionToCreate.OpMu_Id;
                                ApplicationDbContext.AnswerOptionMultiples.Add(answer);
                            }
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                }
                if (model.Pairing != null)
                {
                    foreach (var x in model.Pairing)
                    {
                        if (x.Check == true)
                        {
                            Pairing questionToCreate = ApplicationDbContext.Pairings.Find(x.Pair_Id);
                            questionToCreate.BaQu_Id = model.bankReceiving;
                            ApplicationDbContext.Pairings.Add(questionToCreate);
                            List<AnswerPairing> answesOfQuestion = ApplicationDbContext.AnswerPairings.Where(z => z.Pair_Id == x.Pair_Id).ToList();

                            foreach (var answer in answesOfQuestion)
                            {
                                AnswerPairing answerSelected = ApplicationDbContext.AnswerPairings.Find(answer.Pair_Id);
                                answerSelected.Pair_Id = questionToCreate.Pair_Id;
                                ApplicationDbContext.AnswerPairings.Add(answerSelected);
                            }
                            ApplicationDbContext.SaveChanges();

                        }
                    }
                }

                if (model.TrueOrFalse != null)
                {
                    foreach (var x in model.TrueOrFalse)
                    {
                        if (x.Check == true)
                        {
                            TrueOrFalse questionToCreate = ApplicationDbContext.TrueOrFalses.Find(x.TrFa_Id);
                            questionToCreate.BaQu_Id = model.bankReceiving;
                            ApplicationDbContext.TrueOrFalses.Add(questionToCreate);
                        }
                        ApplicationDbContext.SaveChanges();
                    }
                }
                var table = ApplicationDbContext.TableChanges.Find(8);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(313);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company1 = ApplicationDbContext.Companies.Find(idcompany);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Agregó nuevas preguntas desde un banco de preguntas seleccionado a un nuevo banco de preguntas con id " + bankkk.BaQu_Id + ", en la compañía con id " + company1.CompanyId,
                        Company = company1,
                        Company_Id = company1.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }

            }
            return RedirectToAction("Questions", new { id = model.bankReceiving });
        }
        // aqui finaliza el copiado del banco de pregunta

        public ActionResult AcountSSt()
        {
            var a = GetActualUserId();
            return RedirectToAction("../sst/Home/AcountScore/" + a.Id);
        }

        //[Authorize]
        //public ActionResult ModuleAplicative(string id)
        //{
        //    var user = ApplicationDbContext.Users.Find(id);
        //    var enrrolment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_TypeOfModule == CURSO.Aplicativo).ToList();
        //    var module = ApplicationDbContext.Modules.Where(x => x.Company.CompanyId == user.Company.CompanyId && x.Modu_TypeOfModule == CURSO.Aplicativo && x.Modu_Statemodule == MODULESTATE.Activo).ToList();
        //    AdminTrainingEnrollmentViewModel model = new AdminTrainingEnrollmentViewModel
        //    {
        //        ActualRole = GetActualUserId().Role,
        //        User = user,
        //        listenrollmentevaluative = enrrolment,
        //        listmodule = module,
        //    };
        //    model.Sesion = GetActualUserId().SesionUser;
        //    model.Logo = GetUrlLogo();
        //    var table = ApplicationDbContext.TableChanges.Find(22);
        //    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //    var code = ApplicationDbContext.CodeLogs.Find(210);
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
        //            Log_Description = "El usuario con id: " + UserCurrent.Id + " Desplegó los cursos Evaluativos disponibles para matricular al usuario con id " + id + ", en la compañía con id " + company.CompanyId,
        //            Company = company,
        //            Company_Id = company.CompanyId,
        //            Log_Ip = ip
        //        };
        //        ApplicationDbContext.Logs.Add(logsesiontrue);
        //        ApplicationDbContext.SaveChanges();
        //    }
        //    return PartialView("_ModuleAplicatives", model);
        //}
        //[Authorize]
        //public ActionResult EnrollmentmModuleAplicatives(int Modu_Id, string User_Id)
        //{

        //    var GetModule = ApplicationDbContext.Modules.Find(Modu_Id);
        //    var getuser = ApplicationDbContext.Users.Find(User_Id);
        //    if (ModelState.IsValid)
        //    {
        //        var getEnrollment = getuser.Enrollment.Where(x => x.ApplicationUser.Id == getuser.Id && x.Module.Modu_Id == GetModule.Modu_Id).ToList();
        //        if (getEnrollment.Count == 0)
        //        {
        //            DateTime finish = new DateTime();
        //            switch (GetModule.Modu_Period)
        //            {
        //                case VIGENCIA.Dias:
        //                    finish = DateTime.Now.AddDays(GetModule.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Meses:
        //                    finish = DateTime.Now.AddMonths(GetModule.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Años:
        //                    finish = DateTime.Now.AddYears(GetModule.Modu_Validity);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            DateTime a = finish;
        //            TempData["Add"] = "Matricula Registrada";
        //            Enrollment enrollment = new Enrollment { Module = GetModule, ApplicationUser = getuser, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = a, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
        //            ApplicationDbContext.Enrollments.Add(enrollment);
        //            ApplicationDbContext.SaveChanges();
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(212);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = enrollment.Enro_Id.ToString()
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Matriculó el curso con id " + enrollment.Modu_Id + "al usuario con id " + User_Id + ", en la compañía con id " + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //            return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //        }
        //        else
        //        {
        //            TempData["Add"] = "Este Usuario ya tiene Matriculada esta materia";
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(212);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = null
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intentó´matricular el curso con id " + GetModule.Modu_Id + "al usuario con id " + User_Id + " pero este usuario ya tiene una matricula registrada con estos datos, en la compañía con id " + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //            return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //        }
        //    }
        //    else
        //    {
        //        TempData["Add"] = "Problemas al registrar matricula";
        //        return RedirectToAction("AddEnrollment", new { id = getuser.Id });
        //    }
        //}

        //este metodo se encarga de mostrar el listado de los cursos que tiene matriculado el administrador de formación


        [Authorize]
        public ActionResult ModuleAdmin()
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var enrrolment = ApplicationDbContext.Enrollments.Where(x => x.ApplicationUser.Id == user.Id).ToList();
            AdminModuleViewModel model = new AdminModuleViewModel
            {
                listenrrolment = enrrolment
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            return View(model);
        }

        [Authorize]
        public ActionResult UserEnrollment(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            var user = ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == id && x.Enro_RoleEnrollment == ROLEENROLLMENT.Estudiante).ToList();
            AdminModuleViewModel model = new AdminModuleViewModel
            {
                baseUrl = url,
                listenrrolment = user
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EmailUser(AdminModuleViewModel model)
        {
            var user = ApplicationDbContext.Enrollments.Find(model.idEnroll);
            var asunto = model.Asunto;
            var mensaje = model.Mensaje;
            var userac = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var solicitud = new MailMessage();
            solicitud.Body = "<br/>" + mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dude en comunicarse al siguiente correo: " + userac.Email;
            solicitud.Subject = asunto;
            solicitud.To.Add(user.ApplicationUser.Email);
            solicitud.IsBodyHtml = true;
            var smtp = new SmtpClient();
            smtp.Send(solicitud);
            return RedirectToAction("UserEnrollment", new { id = user.Modu_Id });
        }


        [Authorize]
        public ActionResult Resultt(int id, int idd)
        {

            var Enrollments = ApplicationDbContext.Enrollments.Find(id);
            var Module = ApplicationDbContext.Modules.Find(idd);

            //var Module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);


            var jobs = ApplicationDbContext.Job.Where(x => x.TopicsCourse.Modu_Id == idd).ToList();
            //list of test
            List<resultado> listTest = new List<resultado>();

            List<AdvanceUser> Test = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == Enrollments.User_Id).ToList();
            int eva = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Modu_Id == Module.Modu_Id).ToList().Count;
            double pointEvalution = Module.Modu_Points / eva;
            // List of test v.2 
            List<BankQuestion> ListBakn = ApplicationDbContext.BankQuestions.Where(x => x.TopicsCourse.Module.Modu_Id == Module.Modu_Id).ToList();
            foreach (var item in ListBakn)
            {
                if (Test.Find(x => x.ToCo_id == item.ToCo_Id && x.User_Id == Enrollments.User_Id) != null)
                {
                    double PointOfUser = (Test.Find(x => x.ToCo_id == item.ToCo_Id && x.User_Id == Enrollments.User_Id).AdUs_ScoreObtained * 5) / pointEvalution;

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
                var rf = ApplicationDbContext.ResourceForum.Where(x => x.Job.Job_Id == item.Job_Id && x.User_Id == Enrollments.User_Id).FirstOrDefault();
                var rj = ApplicationDbContext.ResourceJobs.Where(x => x.Job.Job_Id == item.Job_Id && x.User_Id == Enrollments.User_Id).FirstOrDefault();
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

            var enrrolment = ApplicationDbContext.Enrollments.Where(x => x.ApplicationUser.Id == Enrollments.User_Id).ToList();
            var Jobs = ApplicationDbContext.Job.Where(x => x.TopicsCourse.Modu_Id == idd).ToList();
            var user = ApplicationDbContext.Users.Where(x => x.Id == Enrollments.User_Id).ToList();




            RatingUserViewJobs model = new RatingUserViewJobs
            {
                ApplicationUser = user,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                listresultado = listresultado,
                listTestUs = listTest,
                listenrrolment = enrrolment,
                listjobs = Jobs,

            };
            return View(model);
        }


    }
}

