using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Lms;
using SCORM1.Models.Logs;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.SCORM1;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class MassiveEnrollmentController : Controller
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

        public MassiveEnrollmentController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        // GET: MassiveEnrollment
        public ActionResult Index()
        {
            return View();
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
        public ActionResult ModulesEnrollment()
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            AdminTrainingMassiveEnrollmentModuleViewMode model = new AdminTrainingMassiveEnrollmentModuleViewMode { ActualRole = GetActualUserId().Role, ListModuleEnrollment = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompanyId).ToList()};
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(216);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de matricula masiva, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachModuleEnrollment(AdminTrainingMassiveEnrollmentModuleViewMode model)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchModuleEnrollment) || string.IsNullOrEmpty(model.SearchModuleEnrollment))
            {

                model = new AdminTrainingMassiveEnrollmentModuleViewMode { ActualRole = GetActualUserId().Role, ListModuleEnrollment = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompany).ToList() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(22);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(217);
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
                return View("ModulesEnrollment", model);
            }
            else
            {
                List<Module> SearchedModules = ApplicationDbContext.Modules.Where(m => m.Modu_Name.Contains(model.SearchModuleEnrollment) && m.CompanyId == GetModuleCompany).ToList();
                model = new AdminTrainingMassiveEnrollmentModuleViewMode { ActualRole = GetActualUserId().Role, ListModuleEnrollment = SearchedModules };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(22);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(217);
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
                return View("ModulesEnrollment", model);
            }
        }

        [Authorize]
        public ActionResult Enrollments(int Modu_Id)
        {

            var GetModuleCompany = GetActualUserId().CompanyId;
            Module getModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in Users)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id= item.Id,
                    Modu_Id= Modu_Id
                });
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();
         
                ListAreas.Add(new Areas
                {
                    Modu_Id = Modu_Id,
                    Listareas =areas
                });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = Modu_Id,
                Listpositions= positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id= Modu_Id,
                Listcities= city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id=Modu_Id,
                Listlocations= location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser= Listalluser,
                ListAreas= ListAreas,
                ListPositions= ListPositions,
                ListCitices= ListCities,
                ListLocations= ListLocations
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(219);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar un curso para matricular con id " + getModule.Modu_Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Enrollments", model);
        }
        [Authorize]
        public ActionResult CancelEnrollments(int Modu_Id)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            Module getModule = ApplicationDbContext.Modules.Find(Modu_Id);
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in Users)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = Modu_Id
                });
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = Modu_Id,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListPositions.Add(new Positions
            {
                Modu_Id = Modu_Id,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = Modu_Id,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = Modu_Id,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(220);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar un curso para des matricular con id " + getModule.Modu_Id + ", en la compañía con id" + company.CompanyId,
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
        public ActionResult AllUsers()
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role==ROLES.Usuario).ToList();
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                listuser= Users
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            return PartialView("_AllUser", model);
        }

        [Authorize]
        public ActionResult EnrollmentAllUser(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            foreach (var item in Enrollments.ListAllUser)
            {
                var user = ApplicationDbContext.Users.Find(item.User_Id);
                var getEnrollment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (module.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(module.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(module.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(module.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime b = finish;
                    TempData["Info"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = user, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };

                    // QuienSabeMas
                    if (module.QSMActive == 1)
                    {
                        QuienSabeMasPuntaje quienSabeMasPuntaje = new QuienSabeMasPuntaje()
                        {
                            User_Id = GetActualUserId().Id,
                            User_Id_QSM = GetActualUserId().Document,
                            Mudole_Id = module.Modu_Id,
                            FechaPresentacion = DateTime.Now,
                            Puntaje = 0,
                            PorcentajeAprobacion = 0
                        };
                        ApplicationDbContext.QuienSabeMasPuntajes.Add(quienSabeMasPuntaje);
                    }

                    ApplicationDbContext.Enrollments.Add(enrollment);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(221);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular el curso con id " + enrollment.Modu_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
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
                    TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                    var table = ApplicationDbContext.TableChanges.Find(22);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(221);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular el curso con id " + item.Modu_Id + " al usuario con id " + item.User_Id + "pero este usuario ya tiene este curso matriculado, en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                }           
            }    
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentAllUser(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListAllUser.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in Users)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            foreach (var item in Enrollments.ListAllUser)
            {
                int Modu_Id = item.Modu_Id;
                string User_Id = item.User_Id;
                var user = ApplicationDbContext.Users.Find(User_Id);
                var module = ApplicationDbContext.Modules.Find(Modu_Id);

                var getEnrollment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                if (getEnrollment != null)
                {
                    var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == user.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                    var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                    if (advanceuser.Count != 0 || attempts.Count != 0)
                    {
                        ListUserCancel.Add(new user
                        {
                            UserName = user.UserName,
                            Name = user.FirstName + user.LastName,
                            Email = user.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(226);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + " pero este usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
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
                        ListUserCancel1.Add(new user1
                        {
                            UserName = user.UserName,
                            Name = user.FirstName + user.LastName,
                            Email = user.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info1"] = "Usuarios desmatriculados.";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(226);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        ApplicationDbContext.Enrollments.Remove(getEnrollment);
                        ApplicationDbContext.SaveChanges();
                    }
                }
                else
                {
                    ListUserCancel2.Add(new user2
                    {
                        UserName = user.UserName,
                        Name = user.FirstName + user.LastName,
                        Email = user.Email
                    });
                    TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                    TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    TempData["Info3"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                }
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }


        [Authorize]
        public ActionResult EnrollmentAreas(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var A = Enrollments.ListAreas.Take(1);
            int d = A.Select(x => x.Area_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.AreaId == d && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {            
            foreach (var item in user)
            {
                var useractual = ApplicationDbContext.Users.Find(item.Id);
                var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                if (getEnrollment.Count == 0)
                {
                    DateTime finish = new DateTime();
                    switch (module.Modu_Period)
                    {
                        case VIGENCIA.Dias:
                            finish = DateTime.Now.AddDays(module.Modu_Validity);
                            break;
                        case VIGENCIA.Meses:
                            finish = DateTime.Now.AddMonths(module.Modu_Validity);
                            break;
                        case VIGENCIA.Años:
                            finish = DateTime.Now.AddYears(module.Modu_Validity);
                            break;
                        default:
                            break;
                    }
                    DateTime b = finish;
                    TempData["Info"] = "Matricula Registrada";
                    Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                    ApplicationDbContext.Enrollments.Add(enrollment);
                    ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(222);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de  matricular por área el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
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
                    TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(222);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por área el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
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
                TempData["Info"] = "No hay usuarios asociados con esta area";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentAreas(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListAreas.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Area_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.AreaId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(227);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por área el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
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
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(227);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por área el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con esta area";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }


        [Authorize]
        public ActionResult EnrollmentPositions(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var A = Enrollments.ListPositions.Take(1);           
            int d = A.Select(x => x.Posi_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.PositionId == d && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(223);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por cargo el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
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
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(223);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por cargo el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
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
                TempData["Info"] = "No hay usuarios asociados con este cargo";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentPositions(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListPositions.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Posi_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.PositionId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(228);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por cargo el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
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
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(228);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por cargo el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }

        [Authorize]
        public ActionResult EnrollmentCities(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListCitices.Take(1);
            int d = A.Select(x => x.City_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.CityId == d && x.Role == ROLES.Usuario && x.CompanyId == GetModuleCompany).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(224);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por ciudad el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
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
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(224);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por ciudad el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
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
                TempData["Info"] = "No hay usuarios asociados con esta ciudad";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentCities(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListCitices.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.City_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.CityId == d && x.Role == ROLES.Usuario && x.Company.CompanyId == GetModuleCompany).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(229);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por ciudad el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
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
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(229);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por ciudad el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }

        [Authorize]
        public ActionResult EnrollmentLocations(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var A = Enrollments.ListLocations.Take(1);
            int d = A.Select(x => x.Loca_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.LocationId == d && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(225);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por ubicación el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
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
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(225);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por ubicación el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
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
                TempData["Info"] = "No hay usuarios asociados con esta ubicación";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }


        [Authorize]
        public ActionResult CancelEnrollmentLocations(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListLocations.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Loca_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.LocationId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(230);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por ubicación el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
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
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(230);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
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
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por ubicación el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }


    }
}