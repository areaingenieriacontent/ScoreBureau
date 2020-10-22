using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using SCORM1.Models;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.SCORM1;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Enum;
using System.IO;
using SCORM1.Models.Logs;

namespace SCORM1.Controllers
{
    public class AdminPageCustomizationController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
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

        public AdminPageCustomizationController()
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

        // GET: AdminPageCustomization
        public ActionResult BannerAndLogos()
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            int CompanyUser = (int)GetActualUserId().CompanyId;
            List<Banner> banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();
            AdminPageCustomizationViewModel model = new AdminPageCustomizationViewModel
            {
                baseUrl = url,
                Logo=GetUrlLogo()
            };
            if (banners != null && banners.Count != 0)
            {
                model.Banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();
            }
            else
            {
                model.Banners = ApplicationDbContext.Banners.Where(x => x.Bann_Id <= 4).ToList();
            }
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(9);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(245);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de personalización de banners, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }


        //aqui se crean o se actualizan los datos de los banner
        [HttpPost]
        public ActionResult UpdateOrCreationBanner(AdminPageCustomizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                int[] idPrimary = { 1, 2, 3, 4 };
                int CompanyUser = (int)GetActualUserId().CompanyId;
                foreach (Banner banners in model.Banners)
                {
                    if (idPrimary.Contains(banners.Bann_Id))
                    {
                        Banner NewImage = new Banner
                        {
                            Bann_Name = banners.Bann_Name,
                            Bann_Description = banners.Bann_Description,
                            Bann_Link = banners.Bann_Link,
                            Bann_Image = banners.Bann_Image,
                            Bann_Date = DateTime.Now,
                            companyId = CompanyUser
                        };
                        ApplicationDbContext.Banners.Add(NewImage);
                        var table = ApplicationDbContext.TableChanges.Find(9);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(246);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = NewImage.Bann_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de actualizar los banners pertenecientes al id " + NewImage.Bann_Id + ", en la compañía con id " + company.CompanyId,
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
                        Banner bannerUpdate = ApplicationDbContext.Banners.Find(banners.Bann_Id);
                        bannerUpdate.Bann_Name = banners.Bann_Name;
                        bannerUpdate.Bann_Description = banners.Bann_Description;
                        bannerUpdate.Bann_Link = banners.Bann_Link;
                        bannerUpdate.Bann_Image = banners.Bann_Image;
                        var table = ApplicationDbContext.TableChanges.Find(9);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(246);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = bannerUpdate.Bann_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de actualizar los banners pertenecientes al id " + bannerUpdate.Bann_Id + ", en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        //aqui se crean o se actualizan los datos de los logos 
        [Authorize]
        public ActionResult Logo()
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            int CompanyUser = (int)GetActualUserId().CompanyId;

            StylesLogos companiesActuality = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();

            AdminPageLogoViewModel model = new AdminPageLogoViewModel();

            if (companiesActuality != null)
            {

            }
            else
            {
                companiesActuality = ApplicationDbContext.StylesLogos.Find(1);
            }
            model.baseUrl = url;
            model.companyId = companiesActuality.companyId;
            model.UrlLogo = companiesActuality.UrlLogo;
            model.UrlImage1 = companiesActuality.UrlImage1;
            model.UrlImage2 = companiesActuality.UrlImage2;
            model.UrlImage3 = companiesActuality.UrlImage3;
            model.UrlImage4 = companiesActuality.UrlImage4;
            model.Title1 = companiesActuality.Title1;
            model.Title2 = companiesActuality.Title2;
            model.Title3 = companiesActuality.Title3;
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(59);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(247);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = companiesActuality.Styl_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de personalización de logos,imagenes y titulos pertenecientes al id " + companiesActuality.Styl_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult Logo(AdminPageLogoViewModel model)
        {
            if (ModelState.IsValid)
            {
                int[] idPrimary = { 1 };
                int CompanyUser = (int)GetActualUserId().CompanyId;
                if (model.companyId == null)
                {
                    StylesLogos style = new StylesLogos
                    {
                        UrlLogo = model.UrlLogo,
                        UrlImage1 = model.UrlImage1,
                        UrlImage2 = model.UrlImage2,
                        UrlImage3 = model.UrlImage3,
                        UrlImage4 = model.UrlImage4,
                        Title1 = model.Title1,
                        Title2 = model.Title2,
                        Title3 = model.Title3,
                        companyId = CompanyUser
                    };
                    ApplicationDbContext.StylesLogos.Add(style);
                    var table = ApplicationDbContext.TableChanges.Find(59);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(248);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = style.Styl_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de actualizar los logos,titulos e imagenes pertenecientes al id " + style.Styl_Id + ", en la compañía con id " + company.CompanyId,
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
                    StylesLogos styleToUpdate = ApplicationDbContext.StylesLogos.Where(x => x.companyId == model.companyId).First();
                    styleToUpdate.UrlLogo = model.UrlLogo;
                    styleToUpdate.UrlImage1 = model.UrlImage1;
                    styleToUpdate.UrlImage2 = model.UrlImage2;
                    styleToUpdate.UrlImage3 = model.UrlImage3;
                    styleToUpdate.UrlImage4 = model.UrlImage4;
                    styleToUpdate.Title1 = model.Title1;
                    styleToUpdate.Title2 = model.Title2;
                    styleToUpdate.Title3 = model.Title3;
                    var table = ApplicationDbContext.TableChanges.Find(59);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(248);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = styleToUpdate.Styl_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de actualizar los logos,titulos e imagenes pertenecientes al id " + styleToUpdate.Styl_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                }
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            return View(model);
        }

        [Authorize]
        public ActionResult TermsandConditions()
        {
            var url = HttpRuntime.AppDomainAppVirtualPath;
            int CompanyUser = (int)GetActualUserId().CompanyId;
            StylesLogos companiesActuality = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            AdminPageTermsandConditionsViewModel model = new AdminPageTermsandConditionsViewModel();
            if (companiesActuality != null)
            {

            }
            else
            {
                companiesActuality = ApplicationDbContext.StylesLogos.Find(1);
            }
            model.baseUrl = url;
            model.companyId = companiesActuality.companyId;
            model.FileTerms = companiesActuality.colorsTittle;
            model.Logo = companiesActuality.UrlLogo;
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(59);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(249);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = companiesActuality.Styl_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de personalización de terminos y condiciones pertenecientes al id " + companiesActuality.Styl_Id + ", en la compañía con id " + company.CompanyId,
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
        public ActionResult TermsandConditions(AdminPageTermsandConditionsViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                int[] idPrimary = { 1 };
                int CompanyUser = (int)GetActualUserId().CompanyId;
                if (model.companyId == null)
                {
                    StylesLogos style = new StylesLogos
                    {
                        colorsTittle = model.FileTerms,
                        companyId = CompanyUser
                    };
                    ApplicationDbContext.StylesLogos.Add(style);
                }
                else
                {
                    if (upload != null && upload.ContentLength <= (1 * 1000000))
                    {
                        string[] allowedExtensions = new[] { ".pdf" };
                        var imagen = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                        foreach (var Ext in allowedExtensions)
                        {
                            if (Ext.Contains(imagen))
                            {
                                imagen = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                                upload.SaveAs(Server.MapPath("~/TermsandConditions/" + imagen));
                                string ruta = imagen;
                                StylesLogos styleToUpdate = ApplicationDbContext.StylesLogos.Where(x => x.companyId == model.companyId).First();
                                styleToUpdate.colorsTittle = ruta;
                                ApplicationDbContext.SaveChanges();
                                var table = ApplicationDbContext.TableChanges.Find(59);
                                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                                var code = ApplicationDbContext.CodeLogs.Find(250);
                                var idcompany = UserCurrent.CompanyId;
                                if (idcompany != null)
                                {
                                    var company = ApplicationDbContext.Companies.Find(idcompany);
                                    string ip = IpUser();
                                    var idchange = new IdChange
                                    {
                                        IdCh_IdChange = styleToUpdate.Styl_Id.ToString()
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
                                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de actualizar la información de los terminos y condiciones pertenecientes al id " + styleToUpdate.Styl_Id + ", en la compañía con id " + company.CompanyId,
                                        Company = company,
                                        Company_Id = company.CompanyId,
                                        Log_Ip = ip
                                    };
                                    ApplicationDbContext.Logs.Add(logsesiontrue);
                                    ApplicationDbContext.SaveChanges();
                                }
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        TempData["Info"] = "El formato del archivo no es valido";
                        return RedirectToAction("TermsandConditions");
                    }
                    else
                    {
                        StylesLogos styleToUpdate = ApplicationDbContext.StylesLogos.Where(x => x.companyId == model.companyId).First();
                        TempData["Info"] = "No se ha seleccionado un archivo o el archivo es muy pesado";
                        var table = ApplicationDbContext.TableChanges.Find(59);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(250);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = styleToUpdate.Styl_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento actualizar la información de los terminos y condiciones pertenecientes al id " + styleToUpdate.Styl_Id + " pero no ha seleccionado un archivo, en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        return RedirectToAction("TermsandConditions");
                    }
                }
            }
            return RedirectToAction("TermsandConditions");
        }


        //public JsonResult CreatingCssFilesToJson()
        //{
        //    StylesLogos page = new StylesLogos
        //    {
        //        UrlLogo = "fixLogo",
        //        UrlImage1 = "fixeImg1",
        //        UrlImage2 = "fixIMg2",
        //        UrlImage3 = "fixImg3",
        //        UrlImage4 = "fiximg4",
        //        width = "200px"
        //    };

        //    var a = JsonConvert.SerializeObject(page);
        //    var y = Json(page);
        //    var z = JsonConvert.DeserializeObject<StylesLogos>(a);
        //    var x = JsonConvert.DeserializeObject<List<string>>(a);

        //    var styleStr = "";
        //    foreach (var item in z)
        //    {
        //        styleStr += item + "{\n";
        //        styleStr += "}\n";
        //    }

        //    ReadJsonConvertToFileCss();
        //    return Json(page, JsonRequestBehavior.AllowGet);
        //}

        public void ReadJsonConvertToFileCss(JsonResult JsonFile)
        {
            string pah = Server.MapPath("~/Content/Testing.css");
            //string FileJson = System.IO.File.ReadAllText(pah);

        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

    }
}