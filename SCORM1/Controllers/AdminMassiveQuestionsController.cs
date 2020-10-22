using Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Lms;
using SCORM1.Models.Logs;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class AdminMassiveQuestionsController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationSignInManager _signInManager;

        public AdminMassiveQuestionsController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
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

        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public ActionResult MassiveRegisterQuestions(int BaQu_Id)
        {
            var BankQuestion = ApplicationDbContext.BankQuestions.Find(BaQu_Id);
            AdminMassiveQuestionsViewModel model = new AdminMassiveQuestionsViewModel
            {
                BankQuestion= BankQuestion,
                Logo=GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(8);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(201);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = BankQuestion.BaQu_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de carga masiva de preguntas perteneciente al banco de preguntas con id " + BankQuestion.BaQu_Id + ", en la compañía con id " + company.CompanyId,
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
        [AllowAnonymous]
        public ActionResult MassiveRegisterQuestions(AdminMassiveQuestionsViewModel model, HttpPostedFileBase excelUpload)
        {
            var BankQuestion = ApplicationDbContext.BankQuestions.Find(model.BankQuestion.BaQu_Id);
            if (excelUpload != null && excelUpload.ContentLength > 0)
            {
                Stream stream = excelUpload.InputStream;
                IExcelDataReader reader = null;
                if (excelUpload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (excelUpload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    ModelState.AddModelError("File", "Este formato no es soportado");
                    model = new AdminMassiveQuestionsViewModel
                    {
                        BankQuestion = BankQuestion,
                        Logo = GetUrlLogo()
                    };
                    model.Sesion = GetActualUserId().SesionUser;
                    return View(model);
                }
                reader.IsFirstRowAsColumnNames = true;
                DataSet result = reader.AsDataSet();
                string next = VerifyQuestionsFields(result);
                if (next == "success")
                {
                    foreach (DataTable table in result.Tables)
                    {
                        switch (table.TableName)
                        {
                            case "PreguntaOpcionMultiple":
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    string TitleOpenMultiple = table.Rows[i].ItemArray[0].ToString();
                                    string DescriptionOptionMultiple = table.Rows[i].ItemArray[1].ToString();
                                    string AnswerOM = table.Rows[i].ItemArray[2].ToString();
                                    string TrueAnswer = table.Rows[i].ItemArray[3].ToString();
                                    string[] a = AnswerOM.Split(';');
                                    string[] t = TrueAnswer.Split(';');
                                    OptionMultiple optionmultiples = new OptionMultiple
                                    {
                                        OpMu_Question = TitleOpenMultiple,
                                        OpMu_Description = DescriptionOptionMultiple,
                                        BaQu_Id = BankQuestion.BaQu_Id,
                                        BankQuestion = BankQuestion
                                    };
                                    ApplicationDbContext.OptionMultiples.Add(optionmultiples);
                                    ApplicationDbContext.SaveChanges();
                                    for (int c = 0; c < a.Length; c++)
                                    {
                                        var m = a[c];
                                        var n = t[c];
                                        int Z = Int32.Parse(n);
                                        AnswerOptionMultiple AnswerOptionMultiple = new AnswerOptionMultiple
                                        {
                                            AnOp_OptionAnswer = m,
                                            AnOp_TrueAnswer = (OPTIONANSWER)Z,
                                            OpMu_Id = optionmultiples.OpMu_Id,
                                            OptionMultiple = optionmultiples
                                        };
                                        ApplicationDbContext.AnswerOptionMultiples.Add(AnswerOptionMultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                }
                                break;
                            case "PreguntaApareamiento":
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    string TitlePairing = table.Rows[i].ItemArray[0].ToString();
                                    string DescriptionPairing = table.Rows[i].ItemArray[1].ToString();
                                    string OptionAnswersPairing = table.Rows[i].ItemArray[2].ToString();
                                    string AnswersPairing = table.Rows[i].ItemArray[3].ToString();
                                    string[] a = OptionAnswersPairing.Split(';');
                                    string[] t = AnswersPairing.Split(';');
                                    Pairing Pairing = new Pairing
                                    {
                                        Pair_Question = TitlePairing,
                                        Pair_Description = DescriptionPairing,
                                        BaQu_Id = BankQuestion.BaQu_Id,
                                        BankQuestion = BankQuestion
                                    };
                                    ApplicationDbContext.Pairings.Add(Pairing);
                                    ApplicationDbContext.SaveChanges();
                                    for (int c = 0; c < a.Length; c++)
                                    {
                                        var m = a[c];
                                        var n = t[c];
                                        AnswerPairing AnswerPairing = new AnswerPairing
                                        {
                                            AnPa_OptionsQuestion = m,
                                            AnPa_OptionAnswer = n,
                                            Pair_Id = Pairing.Pair_Id,
                                            Pairing = Pairing,
                                        };
                                        ApplicationDbContext.AnswerPairings.Add(AnswerPairing);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                }
                                break;
                            case "PreguntaVerdaderoFalso":
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    string TitleTrueorFalse = table.Rows[i].ItemArray[0].ToString();
                                    string DescriptionTrueorFalse = table.Rows[i].ItemArray[1].ToString();
                                    int AnswerTrueorFalse = Int32.Parse(table.Rows[i].ItemArray[2].ToString());
                                    TrueOrFalse TrueOrFalse = new TrueOrFalse
                                    {
                                        TrFa_Question = TitleTrueorFalse,
                                        TrFa_Description = DescriptionTrueorFalse,
                                        TrFa_State = (OPTIONANSWER)AnswerTrueorFalse,
                                        BaQu_Id = BankQuestion.BaQu_Id,
                                        BankQuestion = BankQuestion
                                    };
                                    ApplicationDbContext.TrueOrFalses.Add(TrueOrFalse);
                                    ApplicationDbContext.SaveChanges();
                                }
                                break;
                        }
                    }
                    reader.Close();
                    ModelState.AddModelError("File", "Preguntas cargadas con exito");

                    model = new AdminMassiveQuestionsViewModel
                    {
                        BankQuestion = BankQuestion,
                        Logo = GetUrlLogo()
                    };
                    model.Sesion = GetActualUserId().SesionUser;
                    var tables = ApplicationDbContext.TableChanges.Find(8);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(202);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = BankQuestion.BaQu_Id.ToString()
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
                            TableChange = tables,
                            TaCh_Id = tables.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de realizar la carga masiva de preguntas perteneciente al banco de preguntas con id " + BankQuestion.BaQu_Id + ", en la compañía con id " + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View(model);
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " Por este motivo no se pueden cargar las preguntas,por favor verifique las respuestas y vuelvlo a intentar";
                    model.Sesion = GetActualUserId().SesionUser;
                    model.BankQuestion = BankQuestion;
                    model.Logo = GetUrlLogo();
                    var tables = ApplicationDbContext.TableChanges.Find(8);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(202);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = BankQuestion.BaQu_Id.ToString()
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
                            TableChange = tables,
                            TaCh_Id = tables.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento realizar la carga masiva de preguntas perteneciente al banco de preguntas con id " + BankQuestion.BaQu_Id + " pero se presento un error, en la compañía con id " + company.CompanyId,
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
            else
            {
                ModelState.AddModelError("File", "No se a seleccionado un archivo");
                model = new AdminMassiveQuestionsViewModel
                {
                    BankQuestion = BankQuestion,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                var tables = ApplicationDbContext.TableChanges.Find(8);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(202);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = BankQuestion.BaQu_Id.ToString()
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
                        TableChange = tables,
                        TaCh_Id = tables.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento realizar la carga masiva de preguntas perteneciente al banco de preguntas con id " + BankQuestion.BaQu_Id + " pero no ha seleccionado un archivo, en la compañía con id " + company.CompanyId,
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

        private string VerifyQuestionsFields(DataSet result)
        {
            int CompanyId = (int)GetActualUserId().CompanyId;
            foreach (DataTable Table in result.Tables)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    string AnswerOM = Table.Rows[i].ItemArray[2].ToString();
                    string[] a = AnswerOM.Split(';');
                    for (int c = 0; c < a.Length; c++)
                    {
                        var m = a[c];
                        if (m == null || m.Length > 105)
                        {
                            return "El tamaño máximo de la respuesta para una pregunta de opción multiple debe ser de 105 caracteres";
                        }
                    }
                }
            }
            return "success";
        }




    }
}