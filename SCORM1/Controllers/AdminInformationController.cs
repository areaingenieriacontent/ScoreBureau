using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SCORM1.Models;
using SCORM1.Models.ViewModel;
using SCORM1.Models.Newspaper;
using SCORM1.Enum;
using SCORM1.Models.Lms;
using System.IO;
using System.Net.Mail;
using SCORM1.Models.Engagement;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.Logs;

namespace SCORM1.Controllers
{

    public class AdminInformationController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        [Authorize]
        public ActionResult habeasdata()
        {
            return File(Server.MapPath("~/App_Files/Upload/info.json"), "application/json");
        }

        public AdminInformationController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

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
            AdminInformationProfileViewModel model = new AdminInformationProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
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
        public ActionResult Validateterms(AdminInformationProfileViewModel model)
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
            AdminInformationProfileViewModel model = new AdminInformationProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
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
        }
        [Authorize]
        public ActionResult Validatevideos(AdminInformationProfileViewModel model)
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
        public void UpdateEditionsToShows()
        {
            List<Edition> CurrenEditions = ApplicationDbContext.Editions.Where(x => x.Edit_StateEdition != EDITIONSTATE.Inactivo && x.Edit_InintDate > DateTime.Now && x.Edit_FinishDate < DateTime.Now).ToList();
            foreach (var EditionToUpdate in CurrenEditions)
            {
                EditionToUpdate.Edit_StateEdition = EDITIONSTATE.Inactivo;
                ApplicationDbContext.SaveChanges();
            }
        }

        [Authorize]
        public ActionResult Editions()
        {
            int CompanyOfUser = (int)GetActualUserId().CompanyId;
            UpdateEditionsToShows();
            List<Edition> CurrentEditions = ApplicationDbContext.Editions.Where(x => x.CompanyId == CompanyOfUser).ToList();
            AdminInformationEditionViewModel model = new AdminInformationEditionViewModel { ActualRole = (ROLES)GetActualUserId().Role, Editions = CurrentEditions, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
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
            return View(model);
        }
        [Authorize]
        public ActionResult Edition()
        {
            int CompanyOfUser = (int)GetActualUserId().CompanyId;
            UpdateEditionsToShows();
            List<Edition> CurrentEditions = ApplicationDbContext.Editions.Where(x => x.CompanyId == CompanyOfUser).ToList();
            AdminInformationEditionViewModel model = new AdminInformationEditionViewModel { ActualRole = (ROLES)GetActualUserId().Role, Editions = CurrentEditions, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
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
            return PartialView("_Editions",model);
        }

        public int VerifyIfEditionActive()
        {

            var companyCurrentOfUser = GetActualUserId().CompanyId;
            List<Edition> Editions = ApplicationDbContext.Editions.Where(x => x.CompanyId == companyCurrentOfUser).ToList();
            int SumOfEditionActive = Editions.Count(x => x.Edit_StateEdition == EDITIONSTATE.Activo);
            return SumOfEditionActive;
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddEdition(AdminInformationEditionViewModel model, HttpPostedFileBase upload)
        {
            var CompanyUserCurrent = GetActualUserId().CompanyId;
            var userCompanyRegister = GetActualUserId().Company;
            AdminInformationEditionViewModel modelRetun = new AdminInformationEditionViewModel { ActualRole = (ROLES)GetActualUserId().Role, Logo=GetUrlLogo() };
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Extns in allowedExtensions)
                    {
                        if (Extns.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/") + file);

                            Edition addEditionts = new Edition { Edit_Name = model.name, Edit_Description = model.descriptions, Edit_InintDate = model.InintDate, Edit_FinishDate = model.FinishDate, Company = userCompanyRegister, Edit_ImageName = file, Edit_Points = 0 };
                            if (VerifyIfEditionActive() <= 0)
                            {
                                addEditionts.Edit_StateEdition = model.state;
                                TempData["Menssage"] = "la edición se ha creado con éxito";
                            }
                            else
                            {
                                TempData["Menssage"] = "No pueden haber dos ediciones activas por tal motivo se creó con estado inactivos";
                                addEditionts.Edit_StateEdition = EDITIONSTATE.Inactivo;
                            }
                            ApplicationDbContext.Editions.Add(addEditionts);
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(21);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(233);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = addEditionts.Edit_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una edición con id " + addEditionts.Edit_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            return RedirectToAction("Editions");
                        }
                    }
                    model.Sesion = GetActualUserId().SesionUser;
                    return RedirectToAction("Editions", model);
                }
                else
                {
                    var table = ApplicationDbContext.TableChanges.Find(21);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(233);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Trato de crear una edición, pero no ha seleccionado una imagen de portada, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    TempData["Menssage"] = "El formato del archivo no es valido";
                    return RedirectToAction("Editions");
                }

            }
            else
            {
                return RedirectToAction("Editions");
            }
        }

        [Authorize]
        public ActionResult UpdateEditionCurrent(int id)
        {
            var CompanyOfUser = GetActualUserId().CompanyId;
            Edition updated = ApplicationDbContext.Editions.Find(id);
            TempData["editarEdition"] = "Editar";
            List<Edition> EditionsCurrents = ApplicationDbContext.Editions.Where(x => x.CompanyId == CompanyOfUser).ToList();
            AdminInformationEditionViewModel model = new AdminInformationEditionViewModel
            {
                ActualRole = GetActualUserId().Role,
                editionsForUpdate = updated,
                Editions = EditionsCurrents,
                Logo=GetUrlLogo()

            };
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
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(21);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(234);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updated.Edit_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono una edición para modificar con id " + updated.Edit_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Editions", model);
        }

        [HttpPost]
        public ActionResult UpdateEditionCurrent(AdminInformationEditionViewModel model, HttpPostedFileBase upload)
        {
            int CompanyUserCurrent = (int)GetActualUserId().CompanyId;
            var userCompanyRegister = GetActualUserId().Company;
            AdminInformationEditionViewModel modelRetun = new AdminInformationEditionViewModel { ActualRole = (ROLES)GetActualUserId().Role };
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Extns in allowedExtensions)
                    {
                        if (Extns.Contains(file))
                        {
                            Edition EditionToUpdate = ApplicationDbContext.Editions.Find(model.editionsForUpdate.Edit_Id);
                            string path = Server.MapPath("~/Resources/SourceEdition/");
                            string fullpath = Path.Combine(path, EditionToUpdate.Edit_ImageName);
                            System.IO.File.Delete(fullpath);
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/" + file));
                            string ruta = file;
                            EditionToUpdate.Edit_Name = model.editionsForUpdate.Edit_Name;
                            EditionToUpdate.Edit_Description = model.editionsForUpdate.Edit_Description;
                            EditionToUpdate.Edit_ImageName = ruta;
                            EditionToUpdate.Edit_InintDate = model.editionsForUpdate.Edit_InintDate;
                            EditionToUpdate.Edit_FinishDate = model.editionsForUpdate.Edit_FinishDate;
                            if (model.editionsForUpdate.Edit_StateEdition == EDITIONSTATE.Activo)
                            {
                                if (VerifyIfEditionActive() <= 0)
                                {
                                    TempData["Menssage"] = "La edición se ha modificado";
                                    EditionToUpdate.Edit_StateEdition = model.editionsForUpdate.Edit_StateEdition;
                                }
                                else
                                {
                                    TempData["Menssage"] = "No pueden haber dos ediciones activas por tal motivo se actualizo con estado inactivo";
                                    EditionToUpdate.Edit_StateEdition = EDITIONSTATE.Inactivo;
                                }
                            }
                            else
                            {
                                EditionToUpdate.Edit_StateEdition = model.state;
                            }
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(21);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(234);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = EditionToUpdate.Edit_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar la edición con id " + EditionToUpdate.Edit_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }

                            return RedirectToAction("Editions");
                        }
                    }
                    return RedirectToAction("Editions", model);
                }
                else
                {
                    Edition EditionToUpdate = ApplicationDbContext.Editions.Find(model.editionsForUpdate.Edit_Id);
                    EditionToUpdate.Edit_Name = model.editionsForUpdate.Edit_Name;
                    EditionToUpdate.Edit_Description = model.editionsForUpdate.Edit_Description;
                    EditionToUpdate.Edit_InintDate = model.editionsForUpdate.Edit_InintDate;
                    EditionToUpdate.Edit_FinishDate = model.editionsForUpdate.Edit_FinishDate;
                    if (model.editionsForUpdate.Edit_StateEdition == EDITIONSTATE.Activo)
                    {
                        if (VerifyIfEditionActive() <= 0)
                        {
                            TempData["Menssage"] = "La edición se ha modificado";
                            EditionToUpdate.Edit_StateEdition = model.editionsForUpdate.Edit_StateEdition;
                        }
                        else
                        {
                            TempData["Menssage"] = "No pueden haber dos ediciones activas por tal motivo se actualizo con estado inactivo";
                            EditionToUpdate.Edit_StateEdition = EDITIONSTATE.Inactivo;
                        }
                    }
                    else
                    {
                        EditionToUpdate.Edit_StateEdition = model.state;
                    }
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(21);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(234);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = EditionToUpdate.Edit_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar la edición con id " + EditionToUpdate.Edit_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Editions");
                }
            }
            else
            {
                return RedirectToAction("Editions");
            }
        }


        [Authorize]
        public ActionResult DeleteEdition(int id)
        {
            var GetEditionCompany = GetActualUserId().CompanyId;
            Edition EditionToDelete = ApplicationDbContext.Editions.Find(id);
            var path = Server.MapPath("~/Resources/SourceEdition/");
            var fullpath = Path.Combine(path, EditionToDelete.Edit_ImageName);
            System.IO.File.Delete(fullpath);
            foreach (var DeleteSectionOfEdicion in EditionToDelete.Sections.ToList())
            {
                foreach (var articleDeleteOfSection in DeleteSectionOfEdicion.Article.ToList())
                {
                    Article articleDelete = ApplicationDbContext.Articles.Find(articleDeleteOfSection.Arti_Id);

                    foreach (var commentDelete in articleDelete.Comments.ToList())
                    {
                        ApplicationDbContext.Comments.Remove(commentDelete);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.Articles.Remove(articleDelete);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Section.Remove(DeleteSectionOfEdicion);
                ApplicationDbContext.SaveChanges();
            }
            TempData["Menssage"] = "La edición ha sido eliminada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(21);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(235);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = EditionToDelete.Edit_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono una edición para eliminar con id " + EditionToDelete.Edit_Id + " y con nombre " + EditionToDelete.Edit_Name + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.Editions.Remove(EditionToDelete);
            ApplicationDbContext.SaveChanges();
            List<Edition> Editions = ApplicationDbContext.Editions.Where(x => x.CompanyId == GetEditionCompany).ToList();
            AdminInformationEditionViewModel model = new AdminInformationEditionViewModel { ActualRole = GetActualUserId().Role, Editions = Editions, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            return View("Editions", model);
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        public ActionResult FileManager()
        {
            return View();
        }


        [Authorize]
        public ActionResult ShowArticles(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Edition editionCurrent = ApplicationDbContext.Editions.Find(id);
            List<Section> sections = editionCurrent.Sections.ToList();
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = (ROLES)GetActualUserId().Role, editions = editionCurrent, sectionsList = sections, baseUrl=url, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(21);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(303);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = editionCurrent.Edit_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la edición con id " + editionCurrent.Edit_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult CreateSection(AdminInformationArticles SectionOfView)
        {
            if (ModelState.IsValid)
            {
                
                    Edition editionCurrent = ApplicationDbContext.Editions.Find(SectionOfView.editions.Edit_Id);
                    Section sectionToCreate = new Section { sect_name = SectionOfView.Sect_Name, Edition = editionCurrent };
                    ApplicationDbContext.Section.Add(sectionToCreate);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(58);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(236);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = sectionToCreate.sect_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una sección con id " + sectionToCreate.sect_Id + " en la edición con id " + editionCurrent.Edit_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    TempData["Menssage"] = "La sección fue creada con éxito";
              
            }

            return RedirectToAction("ShowArticles", new { id = SectionOfView.editions.Edit_Id });
        }

        public int CheckForCommentOfArticle(int idEdition)
        {
            var editionToSearch = ApplicationDbContext.Editions.Find(idEdition);
            int totalCommenta = editionToSearch.Sections.Select(x => x.Article.Sum(y => y.Comments.Count)).Sum();
            return totalCommenta;
        }

        public ActionResult CreateArticle(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Section sectionToCreate = ApplicationDbContext.Section.Find(id);
            int editionID = sectionToCreate.Edition.Edit_Id;
            Edition editionCurrent = ApplicationDbContext.Editions.Find(editionID);
            List<Section> sections = editionCurrent.Sections.ToList();
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = (ROLES)GetActualUserId().Role, editions = editionCurrent, sectionsList = sections, SectionId = id, baseUrl=url, Logo = GetUrlLogo() };
            TempData["AddArticle"] = "Crear article";
            model.Sesion = GetActualUserId().SesionUser;
            return View("ShowArticles", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateArticle(AdminInformationArticles Articles, HttpPostedFileBase upload)
        {
            var CompanyUserCurrent = GetActualUserId().CompanyId;
            var userCompanyRegister = GetActualUserId().Company;

            if (ModelState.IsValid)
            {
                 if (upload != null && upload.ContentLength <= (2 * 1000000))
                    {
                        string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        string file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                        foreach (var Extns in allowedExtensions)
                        {
                            if (Extns.Contains(file))
                            {
                                file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                                upload.SaveAs(Server.MapPath("~/Resources/SourceSection/") + file);
                                string UserCurrent = GetActualUserId().FirstName + GetActualUserId().LastName;
                                Section sectionCurrent = ApplicationDbContext.Section.Find(Articles.SectionId);
                                Article ArticleToCreate = new Article
                                {
                                    Arti_Name = Articles.arti_Name,
                                    Arti_Description = Articles.arti_Description,
                                    Arti_Author = UserCurrent,
                                    Arti_Content = Articles.arti_Content,
                                    Arti_StateArticle = (ARTICLESTATE)Articles.arti_State,
                                    Section = sectionCurrent,
                                    Arti_imagen = file,
                                    ArticleWithComment = (ARTICLEWITHCOMMENT)Articles.ArticleWithComment
                                };
                                ApplicationDbContext.Articles.Add(ArticleToCreate);
                                ApplicationDbContext.SaveChanges();
                                var table = ApplicationDbContext.TableChanges.Find(6);
                                var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                                var code = ApplicationDbContext.CodeLogs.Find(239);
                                var idcompany = UserCurrent1.CompanyId;
                                if (idcompany != null)
                                {
                                    var company = ApplicationDbContext.Companies.Find(idcompany);
                                    string ip = IpUser();
                                    var idchange = new IdChange
                                    {
                                        IdCh_IdChange = ArticleToCreate.Arti_Id.ToString()
                                    };
                                    ApplicationDbContext.IdChanges.Add(idchange);
                                    ApplicationDbContext.SaveChanges();
                                    Log logsesiontrue = new Log
                                    {
                                        ApplicationUser = UserCurrent1,
                                        CoLo_Id = code.CoLo_Id,
                                        CodeLogs = code,
                                        Log_Date = DateTime.Now,
                                        Log_StateLogs = LOGSTATE.Realizado,
                                        TableChange = table,
                                        TaCh_Id = table.TaCh_Id,
                                        IdChange = idchange,
                                        IdCh_Id = idchange.IdCh_Id,
                                        User_Id = UserCurrent1.Id,
                                        Log_Description = "El usuario con id: " + UserCurrent1.Id + " Acaba de crear un articulo con id " + ArticleToCreate.Arti_Id + " en la edición con id " + sectionCurrent.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                                        Company = company,
                                        Company_Id = company.CompanyId,
                                        Log_Ip = ip
                                    };
                                    ApplicationDbContext.Logs.Add(logsesiontrue);
                                    ApplicationDbContext.SaveChanges();
                                }
                                TempData["Menssage"] = "Articulo creado con éxito";
                                return RedirectToAction("ShowArticles", new { id = Articles.editions.Edit_Id });
                            }
                        }

                    }
                    else
                    {
                        TempData["Menssage"] = "El formato del archivo no es valido";
                        return RedirectToAction("ShowArticles", new { id = Articles.editions.Edit_Id });
                    }
            }
            return RedirectToAction("ShowArticles", new { id = Articles.editions.Edit_Id });
        }

        public ActionResult UpdateSectionCurrent(int id)
        {
            Section sectionToUpdate = ApplicationDbContext.Section.Find(id);
            int EditionID = sectionToUpdate.Edition.Edit_Id;
            Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
            List<Section> sections = editionCurrent.Sections.ToList();
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sections, SectionId = id,Logo = GetUrlLogo() };
            TempData["UpdateSection"] = "Editar";
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(58);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(237);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = sectionToUpdate.sect_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono una sección para modificar en la edición con id " + editionCurrent.Edit_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ShowArticles", model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UpdateSectionCurrent(AdminInformationArticles SectionOfView)
        {

            if (ModelState.IsValid)
            {
                Section sectionToUpdate = ApplicationDbContext.Section.Find(SectionOfView.SectionId);
                sectionToUpdate.sect_name = SectionOfView.Sect_Name;
                ApplicationDbContext.SaveChanges();
                int EditionID = sectionToUpdate.Edition.Edit_Id;
                Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                List<Section> sections = editionCurrent.Sections.ToList();
                AdminInformationArticles model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sections, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(58);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(237);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = sectionToUpdate.sect_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico la sección con id " + sectionToUpdate.sect_Id + " en la edición con id " + editionCurrent.Edit_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ShowArticles", model);
            }
            return View();
        }


        public ActionResult DeleteSectionCurrent(int id)
        {
            Section sectionToDelete = ApplicationDbContext.Section.Find(id);
            int EditionID = sectionToDelete.Edition.Edit_Id;
            AdminInformationArticles model = new AdminInformationArticles();

            if (sectionToDelete.Article.Select(x => x.Comments.Count).Sum() <= 0)
            {

                foreach (var deleteArticlesOfSection in sectionToDelete.Article.ToList())
                {
                    Article deleteArticle = ApplicationDbContext.Articles.Find(deleteArticlesOfSection.Arti_Id);
                    ApplicationDbContext.Articles.Remove(deleteArticle);
                    ApplicationDbContext.SaveChanges();
                }
                var table = ApplicationDbContext.TableChanges.Find(58);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(238);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = sectionToDelete.sect_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino una sección con id " + sectionToDelete.sect_Id + " en la edición con id " + sectionToDelete.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Section.Remove(sectionToDelete);
                ApplicationDbContext.SaveChanges();

                Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                List<Section> sections = editionCurrent.Sections.ToList();
                model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sections, Logo = GetUrlLogo() };
                TempData["Menssage"] = "La sección se eliminó con todos los artículos";
                model.Sesion = GetActualUserId().SesionUser;
                return View("ShowArticles", model);
            }
            else
            {
                TempData["Menssage"] = "La sección no puede ser eliminada debido a que ya hay comentarios";
                Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                List<Section> sections = editionCurrent.Sections.ToList();
                model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sections };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(58);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(238);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = sectionToDelete.sect_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Trato de eliminar una sección con id " + sectionToDelete.sect_Id + " en la edición con id " + editionCurrent.Edit_Id + ",la sección no puede ser eliminada debido a que ya hay comentarios en los artículos, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ShowArticles", model);
            }
        }
        [Authorize]
        public ActionResult ViewArticle(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Article articleVisualized = ApplicationDbContext.Articles.Find(id);
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = (ROLES)GetActualUserId().Role, ViewArticle = articleVisualized, baseUrl= url, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(6);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(303);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso al articulo con id " + articleVisualized.Arti_Id + "en la edición con id " + articleVisualized.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        public ActionResult DeleteArticleCurrent(int id)
        {
            Article articleToDelete = ApplicationDbContext.Articles.Find(id);
            int EditionID = articleToDelete.Section.Edition.Edit_Id;
            AdminInformationArticles model = new AdminInformationArticles();

            string path = Server.MapPath("~/Resources/SourceEdition/");
            string fullpath = Path.Combine(path, articleToDelete.Arti_imagen);

            if (articleToDelete.Comments.Count <= 0)
            {
                if (CheckForCommentOfArticle(articleToDelete.Section.Edition.Edit_Id) <= 0)
                {
                    System.IO.File.Delete(fullpath);
                    var table = ApplicationDbContext.TableChanges.Find(6);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(241);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = articleToDelete.Arti_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de eliminar el articulo con id " + articleToDelete.Arti_Id + "en la edición con id " + articleToDelete.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.Articles.Remove(articleToDelete);
                    ApplicationDbContext.SaveChanges();
                    TempData["Menssage"] = "Articulo eliminado";
                    Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                    List<Section> sectionsToSend = editionCurrent.Sections.ToList();
                    model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sectionsToSend, Logo = GetUrlLogo() };
                    model.Sesion = GetActualUserId().SesionUser;

                    return View("ShowArticles", model);
                }
                else
                {
                    TempData["Menssage"] = "El artículo no puede ser eliminado debido a que ya hay comentarios en otros artículos";
                    Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                    List<Section> sectionsToSend = editionCurrent.Sections.ToList();
                    model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sectionsToSend,Logo = GetUrlLogo() };
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(6);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(241);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = articleToDelete.Arti_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar el articulo con id " + articleToDelete.Arti_Id + "en la edición con id " + articleToDelete.Section.Edition.Edit_Id + " el artículo no puede ser eliminado debido a que ya hay comentarios en otros artículos, en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View("ShowArticles", model);
                }

            }
            else
            {
                TempData["Menssage"] = "El artículo no puede ser eliminado debido a que ya hay comentarios en este artículo";
                Edition editionCurrent = ApplicationDbContext.Editions.Find(EditionID);
                List<Section> sectionsToSend = editionCurrent.Sections.ToList();
                model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, editions = editionCurrent, sectionsList = sectionsToSend, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(6);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(241);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = articleToDelete.Arti_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar el articulo con id " + articleToDelete.Arti_Id + "en la edición con id " + articleToDelete.Section.Edition.Edit_Id + " el artículo no puede ser eliminado debido a que ya hay comentarios en este artículo, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ShowArticles", model);
            }
        }



        public ActionResult UpdateArticleCurrent(AdminInformationArticles article, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Extns in allowedExtensions)
                    {
                        if (Extns.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Resources/SourceSection/") + file);
                            string ruta = file;
                            string UserCurrent = GetActualUserId().FirstName + GetActualUserId().LastName;
                            Article articleToUpdate = ApplicationDbContext.Articles.Find(article.ViewArticle.Arti_Id);
                            articleToUpdate.Arti_Name = article.ViewArticle.Arti_Name;
                            articleToUpdate.Arti_Description = article.ViewArticle.Arti_Description;
                            articleToUpdate.Arti_Content = article.ViewArticle.Arti_Content;
                            articleToUpdate.Arti_StateArticle = (ARTICLESTATE)article.ViewArticle.Arti_StateArticle;
                            articleToUpdate.Arti_imagen = ruta;
                            ApplicationDbContext.SaveChanges();
                            var table = ApplicationDbContext.TableChanges.Find(6);
                            var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(240);
                            var idcompany = UserCurrent1.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = articleToUpdate.Arti_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent1,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent1.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent1.Id + " Modifico al articulo con id " + articleToUpdate.Arti_Id + "en la edición con id " + articleToUpdate.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    Article articleToUpdate = ApplicationDbContext.Articles.Find(article.ViewArticle.Arti_Id);
                    articleToUpdate.Arti_Name = article.ViewArticle.Arti_Name;
                    articleToUpdate.Arti_Description = article.ViewArticle.Arti_Description;
                    articleToUpdate.Arti_Content = article.ViewArticle.Arti_Content;
                    articleToUpdate.Arti_StateArticle = (ARTICLESTATE)article.ViewArticle.Arti_StateArticle;
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(6);
                    var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(240);
                    var idcompany = UserCurrent1.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = articleToUpdate.Arti_Id.ToString()
                        };
                        ApplicationDbContext.IdChanges.Add(idchange);
                        ApplicationDbContext.SaveChanges();
                        Log logsesiontrue = new Log
                        {
                            ApplicationUser = UserCurrent1,
                            CoLo_Id = code.CoLo_Id,
                            CodeLogs = code,
                            Log_Date = DateTime.Now,
                            Log_StateLogs = LOGSTATE.Realizado,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent1.Id,
                            Log_Description = "El usuario con id: " + UserCurrent1.Id + " Modifico al articulo con id " + articleToUpdate.Arti_Id + "en la edición con id " + articleToUpdate.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ViewArticle/" + article.ViewArticle.Arti_Id);
        }

        [Authorize]
        public ActionResult CreateOFCommentToArticle(AdminInformationArticles commentsToArticle)
        {
            var articleCurrent = ApplicationDbContext.Articles.Find(commentsToArticle.ViewArticle.Arti_Id);

            if (ModelState.IsValid)
            {
                string Id = GetActualUserId().Id;
                ApplicationUser author = UserManager.FindById(Id);

                Comments comment = new Comments { Comm_value = 0, comm_Title = commentsToArticle.comm_Title, comm_Description = commentsToArticle.comm_Description, Comm_Date = DateTime.Now, Arti_Id = commentsToArticle.ViewArticle.Arti_Id, Comm_StateComment = (COMMENTSTATE)COMMENTSTATE.Inactivo, comm_Author = author };
                PointsComment pointsObtained = new PointsComment { PoCo_Date = DateTime.Now, ApplicationUser = GetActualUserId(), Comments = comment };
                ApplicationDbContext.Comments.Add(comment);
                ApplicationDbContext.PointsComments.Add(pointsObtained);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(18);
                var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(305);
                var idcompany = UserCurrent1.CompanyId;
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
                        ApplicationUser = UserCurrent1,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent1.Id,
                        Log_Description = "El usuario con id: " + UserCurrent1.Id + " Acaba de crear un comentario con id " + comment.Comm_Id + " en el articulo con id " + comment.Article.Arti_Id + "en la edición con id " + comment.Article.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }

            }
            TempData["Menssage"] = "Su comentario fue creado con éxito. Será validado por el administrador para poderlo publicar y ser visto por todos los usuarios.";
            return RedirectToAction("ViewArticle", new { id = commentsToArticle.ViewArticle.Arti_Id });
        }

        public ActionResult AcceptedCommentCurrent(int IdComment)
        {
            Comments commentToUpdate = ApplicationDbContext.Comments.Find(IdComment);
            int Article = commentToUpdate.Article.Arti_Id;
            if (AssignPointToArticle(IdComment) == true)
            {
                SendEmailAccept(commentToUpdate.comm_Author.Email);
                var table = ApplicationDbContext.TableChanges.Find(18);
                var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(242);
                var idcompany = UserCurrent1.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = commentToUpdate.Comm_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent1,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent1.Id,
                        Log_Description = "El usuario con id: " + UserCurrent1.Id + " Acaba de aprobar un comentario con id " + commentToUpdate.Comm_Id + " en el articulo con id " + commentToUpdate.Article.Arti_Id + "en la edición con id " + commentToUpdate.Article.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Menssage"] = "Puntos asignados";
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(18);
                var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(242);
                var idcompany = UserCurrent1.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = commentToUpdate.Comm_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent1,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.NoRealizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent1.Id,
                        Log_Description = "El usuario con id: " + UserCurrent1.Id + " Trato de aprobar un comentario con id " + commentToUpdate.Comm_Id + " en el articulo con id " + commentToUpdate.Article.Arti_Id + "en la edición con id " + commentToUpdate.Article.Section.Edition.Edit_Id + " pero no hay puntos asignados, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Menssage"] = "No hay puntos para asignar";
            }
            Article articleTosendToView = ApplicationDbContext.Articles.Find(Article);
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, ViewArticle = articleTosendToView, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            return View("ViewArticle", model);
        }

        public bool AssignPointToArticle(int IdComment)
        {
            int ValueOfPoint = ApplicationDbContext.PointManagerCategory.ToList().FirstOrDefault().PoMaCa_Periodical;
            if (ValueOfPoint > 0)
            {
                Comments comment = ApplicationDbContext.Comments.Find(IdComment);
                comment.Comm_value = ValueOfPoint;
                comment.Comm_StateComment = COMMENTSTATE.aceptado;
                ApplicationDbContext.SaveChanges();
                var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.Poin_TypePoints == TYPEPOINTS.PERIODICO);
                Point pointsForComment = new Point { User_Id = comment.comm_Author.Id, Poin_Date = DateTime.Now, Quantity_Points = ValueOfPoint,TypePoint=cate,TyPo_Id=cate.TyPo_Id};
                var pointactual = ApplicationDbContext.Points.Where(x => x.TypePoint.Poin_TypePoints == TYPEPOINTS.PERIODICO && x.User_Id == comment.comm_Author.Id).ToList();
                if ( pointactual.Count != 0)
                {
                    int i = ApplicationDbContext.Points.Where(x => x.TypePoint.Poin_TypePoints == TYPEPOINTS.PERIODICO && x.User_Id==comment.comm_Author.Id ).ToList().FirstOrDefault().Poin_Id;
                    
                    Point pointToUpdate = ApplicationDbContext.Points.Find(i);
                    pointToUpdate.Quantity_Points = pointToUpdate.Quantity_Points + ValueOfPoint;
                    ApplicationDbContext.SaveChanges();
                }
                else
                {
                    ApplicationDbContext.Points.Add(pointsForComment);
                    ApplicationDbContext.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult RefuseCommentCurrent(int id)
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            Comments commentToUpdate = ApplicationDbContext.Comments.Find(id);
            int Article = commentToUpdate.Article.Arti_Id;
            Article articleTosendToView = ApplicationDbContext.Articles.Find(Article);
            TempData["RefuseComment"] = "Editar";
            AdminInformationArticles model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, commentToUpdate = commentToUpdate, ViewArticle = articleTosendToView, Logo = GetUrlLogo(),baseUrl=url };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(18);
            var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(243);
            var idcompany = UserCurrent1.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = commentToUpdate.Comm_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent1,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = UserCurrent1.Id,
                    Log_Description = "El usuario con id: " + UserCurrent1.Id + " Ha seleccionado un comentario para eliminar con id " + commentToUpdate.Comm_Id + " en el articulo con id " + commentToUpdate.Article.Arti_Id + "en la edición con id " + commentToUpdate.Article.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ViewArticle", model);
        }

        [HttpPost]
        public ActionResult RefuseCommentCurrent(AdminInformationArticles comment)
        {
            if (ModelState.IsValid)
            {
                var url = HttpRuntime.AppDomainAppVirtualPath;
                Comments commentToUpdate = ApplicationDbContext.Comments.Find(comment.commentToUpdate.Comm_Id);
                int Article = commentToUpdate.Article.Arti_Id;
                var table = ApplicationDbContext.TableChanges.Find(18);
                var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(244);
                var idcompany = UserCurrent1.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = commentToUpdate.Comm_Id.ToString()
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                    Log logsesiontrue = new Log
                    {
                        ApplicationUser = UserCurrent1,
                        CoLo_Id = code.CoLo_Id,
                        CodeLogs = code,
                        Log_Date = DateTime.Now,
                        Log_StateLogs = LOGSTATE.Realizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent1.Id,
                        Log_Description = "El usuario con id: " + UserCurrent1.Id + " Acaba de eliminar un comentario y enviar un correo con id " + commentToUpdate.Comm_Id + " en el articulo con id " + commentToUpdate.Article.Arti_Id + "en la edición con id " + commentToUpdate.Article.Section.Edition.Edit_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                SendEmail(commentToUpdate.comm_Title, commentToUpdate.comm_Author.FirstName + " " + commentToUpdate.comm_Author.LastName, commentToUpdate.comm_Author.Email, comment.commentOfAdmin);
                DeleteCommentCurrent(comment.commentToUpdate.Comm_Id);
                Article articleTosendToView = ApplicationDbContext.Articles.Find(Article);
                AdminInformationArticles model = new AdminInformationArticles { ActualRole = GetActualUserId().Role, ViewArticle = articleTosendToView, Logo = GetUrlLogo(),baseUrl=url };
                TempData["Menssage"] = "Comentario eliminado";
                model.Sesion = GetActualUserId().SesionUser;
                return View("ViewArticle", model);

            }
            return View();
        }

        public void DeleteCommentCurrent(int id)
        {
            Comments CommentToDelete = ApplicationDbContext.Comments.Find(id);
            ApplicationDbContext.Comments.Remove(CommentToDelete);
            ApplicationDbContext.SaveChanges();
        }

        public void SendEmail(string TitleCommentary, string NameUser, string Email, string Mesagge)
        {
            MailMessage solicitud2 = new MailMessage();
            solicitud2.Subject = "UnAulaMi: ¡Sigue participando en nuestro periódico!";
            solicitud2.Body = Mesagge + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: UnAulaMI@gmail.com";
            solicitud2.To.Add(Email);
            solicitud2.IsBodyHtml = true;
            var smtp2 = new SmtpClient();
            smtp2.Send(solicitud2);
        }
        public void SendEmailAccept( string Email)
        {
            MailMessage solicitud2 = new MailMessage();
            solicitud2.Subject = "UnAulaMi: ¡Ahora todos pueden ver tú comentario!";
            solicitud2.Body = "<br/>"+ "Tu comentario en el periódico ha sido aprobado por el administrador y está visible a todos los usuarios de la plataforma. Muchas gracias por unirte a UnAula MI." + "<br/>"+ "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: UnAulaMI@gmail.com";
            solicitud2.To.Add(Email);
            solicitud2.IsBodyHtml = true;
            var smtp2 = new SmtpClient();
            smtp2.Send(solicitud2);
        }

    }
}