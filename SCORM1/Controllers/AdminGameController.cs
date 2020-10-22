using Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.MainGame;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class AdminGameController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public AdminGameController()
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
        /*
         * Metodo que se encarga de llenar una lista con el nombre de las plantillas que se pueden usar para implementar
         * en el juego.
         * para poderlo utilizar en una vista de formulario como lista desplegable
         */
        public IEnumerable<SelectListItem> GetTemplate()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<MG_Template> Templates = ApplicationDbContext.MG_Template.ToList();
            IEnumerable<SelectListItem> Template = Templates.Select(x =>
           new SelectListItem
           {
               Value = x.Plant_Id.ToString(),
               Text = x.Plant_Color
           });
            return new SelectList(Template, "Value", "Text");
        }
        /*
         * Metodo que se encarga de llenar una lista con el nombre de los audios que se pueden usar para implementar
         * en el juego.
         * para poderlo utilizar en una vista de formulario como lista desplegable
         */
        public IEnumerable<SelectListItem> GetAudios()
        {
            
            List<string> audios = new List<string>();
            audios.Add("Instrucciones.mp3");
            audios.Add("respuesta.mp3");
            audios.Add("inicio.mp3");
            audios.Add("pregunta.mp3");
            audios.Add("ganador.mp3");
            IEnumerable<SelectListItem> audio = audios.Select(x =>
           new SelectListItem
           {
               Value = x.ToString(),
               Text = x.ToString()
           });
            return new SelectList(audio, "Value", "Text");
        }
        /*
         * Metodo utilizado para abrir la gestión del 
         * juego por compañias
         */ 
        [Authorize]
        public ActionResult Setting()
        {
            int company = (int)GetActualUserId().CompanyId;
            var setting = ApplicationDbContext.MG_SettingMp.Where(x => x.Company_Id == company).ToList();
            var a = "";
            if (setting.Count !=0)
            {
                a = setting.FirstOrDefault().Sett_Instruction;
            }else
            {
                var settings = ApplicationDbContext.MG_SettingMp.FirstOrDefault();
                a = settings.Sett_Instruction;
            }
            AdminGameSetting model = new AdminGameSetting
            {
                ActualRole = GetActualUserId().Role,
                Sesion=GetActualUserId().SesionUser,
                Logo=GetUrlLogo(),
                ListSetting=setting,
                Sett_Templates=GetTemplate(),
                Sett_Audios=GetAudios(),
                TermsGame=a
            };
            return View(model);
        }
        /*
         * Metodo utilizado para añadir una nueva 
         * configuración del juego
         * Esté metodo recibe un modelo con la información 
         * necesaria para añadir una nueva configuración
         */ 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddSetting(AdminGameSetting model)
        {
            int GetCompanyId = (int)GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                var template = ApplicationDbContext.MG_Template.Find(model.Plan_Id);                           
                    MG_SettingMp Setting = new MG_SettingMp
                    {
                        Sett_Attemps = 3,
                        Sett_NumberOfQuestions = 15,
                        Sett_InitialDate = model.Sett_InitialDate,
                        Sett_CloseDate = model.Sett_CloseDate,
                        MG_Template=template,
                        Company_Id=GetCompanyId,
                        Company = GetActualUserId().Company,
                        Sett_Instruction= "20171017152016-administracion formacion - matriculas.pdf",                       
                        Sett_Audio1=model.Sett_Audio1,
                        Sett_Audio2=model.Sett_Audio2,
                        Sett_Audio3=model.Sett_Audio3,
                        Sett_Audio4=model.Sett_Audio4,
                        Sett_Audio5=model.Sett_Audio5
                    };
                    ApplicationDbContext.MG_SettingMp.Add(Setting);         
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("Setting");
                }
                TempData["AddMessageError"] = "Se debe Crear una plantilla primero";                    
                return View("Setting", model);
            }
        /*
         * Metodo utilizado para eliminar 
         * una configuración de juego creada
         * Esté metodo recibe el id de la configuración 
         * del juego seleccionada para eliminar
         */ 
        [Authorize]
        public ActionResult DeleteSetting(int id)
        {
     
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            MG_SettingMp deletedSetting = ApplicationDbContext.MG_SettingMp.Find(id);
            if (deletedSetting.MG_MultipleChoice.Count != 0 || deletedSetting.MG_Order.Count != 0 || deletedSetting.MG_Pairing.Count != 0 || deletedSetting.MG_Point.Count != 0)
            {
                TempData["Info"] = "No se puede eliminar la configuración del juego debido a que, se encuentra información asociada";
            }
            else
            {
                TempData["Info"] = "Configuración eliminada con éxito";
                ApplicationDbContext.MG_SettingMp.Remove(deletedSetting);
                ApplicationDbContext.SaveChanges();
            }

            return RedirectToAction("Setting");
        }
        /*
         * Metodo utilizado para activar un formulario 
         * para modificar la información de la configuración del juego
         * Esté metodo recibe el id del juego para objener la información de
         * la configuración
         */ 
        [Authorize]
        public ActionResult UpdateSetting(int id)
        {
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            MG_SettingMp updatedSetting = ApplicationDbContext.MG_SettingMp.Find(id);
            TempData["UpdateSetting"] = "Modificada con exito";
            AdminGameSetting model = new AdminGameSetting
            {
                ActualRole = GetActualUserId().Role,
                ListSetting= ApplicationDbContext.MG_SettingMp.Where(c => c.Company_Id == GetPrizeCompanyId).ToList(),
                Sett_Id = updatedSetting.Sett_Id,
                Sett_InitialDate = updatedSetting.Sett_InitialDate,
                Sett_CloseDate = updatedSetting.Sett_CloseDate,
                Plan_Id = updatedSetting.Plan_Id,
                Sett_Audio1=updatedSetting.Sett_Audio1,
                Sett_Audio2=updatedSetting.Sett_Audio2,
                Sett_Audio3=updatedSetting.Sett_Audio3,
                Sett_Audio4=updatedSetting.Sett_Audio4,
                Sett_Audio5=updatedSetting.Sett_Audio5,
                Sett_Templates = GetTemplate(),
                Sett_Audios=GetAudios()               
            };
            model.Logo = GetUrlLogo(); 
            model.Sesion = GetActualUserId().SesionUser;   
            return View("Setting", model);
        }
        /*
         * Metodo utlización para modificar la información de una configuración
         * de juego 
         * Esté metodo recibe un modelo con la información
         * necesaria para modificar la información
         */ 
        [HttpPost]
        [Authorize]
        public ActionResult UpdateSetting(AdminGameSetting model)
        {
            MG_SettingMp updatedSetting = ApplicationDbContext.MG_SettingMp.Find(model.Sett_Id);
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
              
                TempData["Info"] = "La configuración se ha modificado con éxito.";              
                updatedSetting.Sett_InitialDate = model.Sett_InitialDate;
                updatedSetting.Sett_CloseDate = model.Sett_CloseDate;
                updatedSetting.Plan_Id = model.Plan_Id;
                updatedSetting.MG_Template = ApplicationDbContext.MG_Template.Find(model.Plan_Id);    
                updatedSetting.Sett_Audio1 = model.Sett_Audio1;
                updatedSetting.Sett_Audio2 = model.Sett_Audio2;
                updatedSetting.Sett_Audio3 = model.Sett_Audio3;
                updatedSetting.Sett_Audio4 = model.Sett_Audio4;
                updatedSetting.Sett_Audio5 = model.Sett_Audio5;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Setting");
            }
            else
            {
                TempData["Info"] = "Los campos no pueden ser vacios ";
                model.ActualRole = GetActualUserId().Role;
                model.ListSetting = ApplicationDbContext.MG_SettingMp.Where(c => c.Company_Id == GetPrizeCompanyId).ToList();
                model.Sett_Templates = GetTemplate();
                model.Sett_Audios = GetAudios();
                model.Logo = GetUrlLogo();
                model.Sesion = GetActualUserId().SesionUser;
                return View("Setting", model);
            }
        }
        /*
         * Metod utilizado para ingresar a la 
         * gestión de plantillas de un juego
         */ 
        [Authorize]
        public ActionResult Template()
        {
            int company = (int)GetActualUserId().CompanyId;
            var template = ApplicationDbContext.MG_Template.Where(x => x.Company_Id == company).ToList();
            AdminTemplate model = new AdminTemplate
            {
                ActualRole = GetActualUserId().Role,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                ListTemplate=template
            };
            return View(model);
        }
        /*
         * Metodo utilizado para agregar nuevas plantillas
         * a un juego por compañia
         * Esté metodo recibe un modelo con toda la información necesaria
         * para la creación de plantillas
         */ 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddTemplate(AdminTemplate model, HttpPostedFileBase upload)
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
                            upload.SaveAs(Server.MapPath("~/ImgTemplate/" + file));
                            TempData["Info"] = "Plantilla cargada con éxito";
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().Company;
                            MG_Template template = new MG_Template
                            {
                                Company=GetModuleCompany,
                                Company_Id=GetModuleCompany.CompanyId,
                                Plant_Img_instructions=ruta
                            };
                            ApplicationDbContext.MG_Template.Add(template);
                            ApplicationDbContext.SaveChanges();                          
                            return RedirectToAction("Template");
                        }
                    }
                    TempData["Info"] = "El formato de la imagen cargada no es valida";
                }
                else
                {
                    TempData["Info"] = "No ha seleccionado un archivo o el archivo que inteta subir es demasiado pesado ";
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("Template");
        }
        /*
         * Metodo utilizado para eliminar plantillas 
         * Esté metodo recibe el id de una plantilla 
         * para eliminar
         */ 
        [Authorize]
        public ActionResult DeleteTemplate(int id)
        {
            MG_Template deletedtemplate = ApplicationDbContext.MG_Template.Find(id);
            var setting=deletedtemplate.MG_SettingMp.Count;
            if (setting != 0)
            {
                TempData["Info"] = "No se puede eliminar la plantilla por que esta siendo utilizada.";
            }
            else
            {
                TempData["Info"] = "Plantilla eliminada";
                var path = Server.MapPath("~/ImgTemplate");
                var fullpath = Path.Combine(path, deletedtemplate.Plant_Img_instructions);
                System.IO.File.Delete(fullpath);
                ApplicationDbContext.MG_Template.Remove(deletedtemplate);
                ApplicationDbContext.SaveChanges();
            }   
            return RedirectToAction("Template");
        }
        /*
        * Metodo utilizado para activar un formulario 
        * para modificar la información de la plantilla del juego
        * Esté metodo recibe el id de la plantilla para objener la información de
        * la plantilla
        */
        [Authorize]
        public ActionResult UpdateTemplate(int id)
        {
            var GetCompanyId = GetActualUserId().CompanyId;
            MG_Template updatedTemplate = ApplicationDbContext.MG_Template.Find(id);
            TempData["UpdateTemplate"] = "Modificada con exito";
            AdminTemplate model = new AdminTemplate
            {
                ActualRole = GetActualUserId().Role,
                ListTemplate = ApplicationDbContext.MG_Template.Where(c => c.Company_Id == GetCompanyId).ToList(),
                Plant_Id = updatedTemplate.Plant_Id,
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("Template", model);
        }
        /*
         * Metodo utlización para modificar la información de una plantilla
         * de juego 
         * Esté metodo recibe un modelo con la información
         * necesaria para modificar la información
         */
        [HttpPost]
        [Authorize]
        public ActionResult UpdateTemplate(AdminTemplate model, HttpPostedFileBase upload)
        {
            MG_Template updatedtemplate = ApplicationDbContext.MG_Template.Find(model.Plant_Id);
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
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
                            var path = Server.MapPath("~/ImgTemplate");
                            var fullpath = Path.Combine(path, updatedtemplate.Plant_Img_instructions);
                            System.IO.File.Delete(fullpath);
                            ApplicationDbContext.SaveChanges();
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/ImgTemplate/" + file));
                            TempData["Info"] = "Plantilla modificada con éxito";
                            string ruta = file;
                            var GetModuleCompany = GetActualUserId().Company;
                            updatedtemplate.Plant_Img_instructions = ruta;
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("Template");
                        }
                    }
                    TempData["Info"] = "El formato de la imagen cargada no es valida";
                }
                else
                {
                    TempData["Info"] = "No ha seleccionado un archivo o el archivo que inteta subir es demasiado pesado ";
                }
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
            }
            return RedirectToAction("Template");
        }
        /*
         * Metodo utilizado para abrir la gestión de preguntas 
         * de una juego
         * Esté metodo recibe el id de la configuración del juego para 
         * retornarlo a la vista
         */
        [Authorize]
        public ActionResult MgQuestions(int id)
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            MG_SettingMp setting = ApplicationDbContext.MG_SettingMp.Find(id);         
                int TotalQuestion = setting.MG_MultipleChoice.Count() + setting.MG_Pairing.Count() + setting.MG_Order.Count();
                AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
                {
                    ActualRole = GetActualUserId().Role,
                    Sett_Id = id,
                    Setting = setting,
                    TotalQuestion = TotalQuestion,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                return View(model);
          
        }
        /*
         * Metodo utilizado para crear una pregunta de opción multiple
         * Esté metodo recibe un modelo con los datos necesarios para crear la pregunta
         */
        [Authorize]
        public ActionResult AddMgMultipleChoice(AdminGameQuestionViewModel model, HttpPostedFileBase upload)
        {
            var GetTopicCompany = GetActualUserId().CompanyId;
            var Getsetting = ApplicationDbContext.MG_SettingMp.Find(model.Sett_Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png",".gif" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Mg_Game_Image/" + file));
                            TempData["add"] = "El curso se ha creado con éxito";
                            string ruta = file;
                            TempData["Add"] = "Pregunta Creada con éxito";
                            MG_MultipleChoice multiplechouse = new MG_MultipleChoice
                            {
                                MuCh_NameQuestion = model.MuCh_NameQuestion,
                                MuCh_Description = model.MuCh_Description,
                                MuCh_Level = model.MuCh_Level,
                                MG_SettingMp = Getsetting,
                                Sett_Id = Getsetting.Sett_Id,
                                MuCh_Feedback = model.MuCh_Feedback,
                                MuCh_ImageQuestion=ruta
                            };
                            ApplicationDbContext.MG_MultipleChoice.Add(multiplechouse);
                            ApplicationDbContext.SaveChanges();
                            var MultiplechoiseId = ApplicationDbContext.OptionMultiples.Find(multiplechouse.MuCh_ID);
                            MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                            {
                                AnMul_Description = model.AnMul_Description,
                                AnMul_TrueAnswer = model.AnMul_TrueAnswer,
                                MG_MultipleChoice = multiplechouse,
                                MuCh_ID = multiplechouse.MuCh_ID
                            };
                            ApplicationDbContext.MG_AnswerMultipleChoice.Add(answeroptionmeultiple);
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
                        }
                    }
                    TempData["add"] = "El formato del archivo no es valido";
                }else
                {
                    TempData["Add"] = "Pregunta Creada con éxito";
                    MG_MultipleChoice multiplechouse = new MG_MultipleChoice
                    {
                        MuCh_NameQuestion = model.MuCh_NameQuestion,
                        MuCh_Description = model.MuCh_Description,
                        MuCh_Level = model.MuCh_Level,
                        MG_SettingMp = Getsetting,
                        Sett_Id = Getsetting.Sett_Id,
                        MuCh_Feedback = model.MuCh_Feedback
                    };
                    ApplicationDbContext.MG_MultipleChoice.Add(multiplechouse);
                    ApplicationDbContext.SaveChanges();
                    var MultiplechoiseId = ApplicationDbContext.OptionMultiples.Find(multiplechouse.MuCh_ID);
                    MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                    {
                        AnMul_Description = model.AnMul_Description,
                        AnMul_TrueAnswer = model.AnMul_TrueAnswer,
                        MG_MultipleChoice = multiplechouse,
                        MuCh_ID = multiplechouse.MuCh_ID
                    };
                    ApplicationDbContext.MG_AnswerMultipleChoice.Add(answeroptionmeultiple);
                    ApplicationDbContext.SaveChanges();            
                }
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }
        /*
        * Metodo utilizado para activar el formulario de creación de otra respuesta 
        * de pregunta de opción multiple
        * Esté metodo recibe un modelo con todos los datos necesarios para la creación 
        * de la pregunta
        */
        [Authorize]
        public ActionResult AddAnswerMultiplesChoise(AdminGameQuestionViewModel model, int id)
        {
            var Getmultiplechoise = ApplicationDbContext.MG_MultipleChoice.Find(id);
            TempData["Add"] = "Pregunta Creada";
            model.Setting = Getmultiplechoise.MG_SettingMp;
            model.Sett_Id = Getmultiplechoise.Sett_Id;
            model.MuCh_ID = Getmultiplechoise.MuCh_ID;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddAnswerMCh", model);
        }
        /*
        * Metodo utilizado para crear una respuesta de opción multiple
        * Esté metodo recibe eun modelo con los datos necesarios para 
        * la creación de una respuesta de opción multiple
        */
        [Authorize]
        public ActionResult AddAnswerMultiplesChoises(AdminGameQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Respuesta Creada";
                var MultipleChoise = ApplicationDbContext.MG_MultipleChoice.Find(model.MuCh_ID);
                var answermultiplechoice = new MG_AnswerMultipleChoice
                {
                    AnMul_Description = model.AnMul_Description,
                    AnMul_TrueAnswer = model.AnMul_TrueAnswer,
                    MuCh_ID = MultipleChoise.MuCh_ID,
                    MG_MultipleChoice=MultipleChoise
                };
                ApplicationDbContext.MG_AnswerMultipleChoice.Add(answermultiplechoice);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new {id =model.Sett_Id});
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }
        /*
         * Metodo utilizado para activar el formulario de creación de otra
         * pregunta de opción multiple
         * Esté metodo recibe un modelo con todos los datos necesarios para la creación 
         * de la pregunta
         */
        [Authorize]
        public ActionResult AddOtherMultipleChoice(AdminGameQuestionViewModel model, int id)
        {         
            var Getsetting = ApplicationDbContext.MG_SettingMp.Find(id);
            TempData["Add"] = "Pregunta Creada";
            model.Setting = Getsetting;
            model.Sett_Id = Getsetting.Sett_Id;
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AddMultipleChoice", model);
        }
        /*
         * Metodo utilizado para seleccionar una pregunta de 
         * opción multiple para modificar
         * Esté metodo recibe el id de la pregunta seleccionada y retorna 
         * una vista con los datos para modificar
         */
        [Authorize]
        public ActionResult EditMultipleCh(int id)
        { 
            var MultipleId = ApplicationDbContext.MG_MultipleChoice.Find(id);
            var setting = ApplicationDbContext.MG_SettingMp.Find(MultipleId.Sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                ActualRole = GetActualUserId().Role,
                Setting=setting,
                Sett_Id = setting.Sett_Id,
                MuCh_ID = MultipleId.MuCh_ID,
                MuCh_NameQuestion = MultipleId.MuCh_NameQuestion,
                MuCh_Description = MultipleId.MuCh_Description,
                MuCh_Level=MultipleId.MuCh_Level,
                MuCh_Feedback=MultipleId.MuCh_Feedback,                
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            TempData["Form"] = "Activar Formulario";
            return View(model);
        }
        /*
        * Metodo utilizado para modificar los datos de 
        * una pregunta de opción multiple seleccionada
        * Esté metodo recibe un modelo con la información necesaria para modificar la pregunta 
        * de opción multiple
        */
        [HttpPost]
        [Authorize]
        public ActionResult EditMultipleChoices(AdminGameQuestionViewModel model, HttpPostedFileBase upload)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(model.Sett_Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Mg_Game_Image/" + file));
                            TempData["add"] = "El curso se ha creado con éxito";
                            string ruta = file;
                            var UpdatedOptionMultiple = ApplicationDbContext.MG_MultipleChoice.Find(model.MuCh_ID);
                            TempData["Add"] = "Pregunta Modificada con éxito";
                            UpdatedOptionMultiple.MuCh_NameQuestion = model.MuCh_NameQuestion;
                            UpdatedOptionMultiple.MuCh_Description = model.MuCh_Description;
                            UpdatedOptionMultiple.MuCh_Feedback = model.MuCh_Feedback;
                            UpdatedOptionMultiple.MuCh_Level = model.MuCh_Level;
                            UpdatedOptionMultiple.MuCh_ImageQuestion = ruta;
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
                        }
                    }
                    TempData["add"] = "El formato del archivo no es valido";
                }
                else
                {
                    var UpdatedOptionMultiple = ApplicationDbContext.MG_MultipleChoice.Find(model.MuCh_ID);
                    TempData["Add"] = "Pregunta Modificada con éxito";
                    UpdatedOptionMultiple.MuCh_NameQuestion = model.MuCh_NameQuestion;
                    UpdatedOptionMultiple.MuCh_Description = model.MuCh_Description;
                    UpdatedOptionMultiple.MuCh_Feedback = model.MuCh_Feedback;
                    UpdatedOptionMultiple.MuCh_Level = model.MuCh_Level;
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });                  
            }
            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
        }
        /*
        * Metodo utilizado para seleccionar una respuesta a una 
        * pregunta de opción multiple para modificar
        * Esté metodo recibe el id de la respuesta y lo retorna a una vista
        */
        [Authorize]
        public ActionResult EditAnswerOMultiple(int id)
        {
            var AnswerOptionMultiple = ApplicationDbContext.MG_AnswerMultipleChoice.Find(id);
            var setting = ApplicationDbContext.MG_SettingMp.Find(AnswerOptionMultiple.MG_MultipleChoice.Sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                ActualRole = GetActualUserId().Role,
                Setting=setting,
                Sett_Id=setting.Sett_Id,
                AnMu_Id = AnswerOptionMultiple.AnMul_ID,
                AnMul_Description = AnswerOptionMultiple.AnMul_Description,
                AnMul_TrueAnswer = AnswerOptionMultiple.AnMul_TrueAnswer,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            TempData["FormAnswer"] = "Activar Formulario";
            return View(model);
        }
        /*
        * Metodo utilizado para modificar los datos de 
        * una respuesta de una pregunta de opción multiple
        * Esté metodo recibe un modelo con los datos necesarios
        * para modificar la respuesta
        */
        [HttpPost]
        [Authorize]
        public ActionResult EditAnswerOptionMultiples(AdminGameQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
               var UpdatedAnswerOptionMultiple = ApplicationDbContext.MG_AnswerMultipleChoice.Find(model.AnMu_Id);
                TempData["Add"] = "Respuesta Modificada";
                UpdatedAnswerOptionMultiple.AnMul_Description = model.AnMul_Description;
                UpdatedAnswerOptionMultiple.AnMul_TrueAnswer = model.AnMul_TrueAnswer;
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
        }
        /*
         * Metodo utilizado para eliminar una pregunta de opción multiple
         * Esté metodo recibe un id de la pregunta para elimiar una 
         * pregunta de opción multiple
         */
        [Authorize]
        public ActionResult DeleteMultipleChoice( int id)
        {
            var OptionMultiple = ApplicationDbContext.MG_MultipleChoice.Find(id);
            var answer = OptionMultiple.MG_AnswerMultipleChoice.Count;
            if (answer == 0)
            {
                TempData["Add"] = "Pregunta Eliminada";             
                ApplicationDbContext.MG_MultipleChoice.Remove(OptionMultiple);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = OptionMultiple.Sett_Id});
            }
            else
            {
                TempData["Add"] = "No se puede eliminar la pregunta hasta que elimine las resuestas ";
                return RedirectToAction("MgQuestions", new { id = OptionMultiple.Sett_Id });
            }
        }
        /*
         * Metodo utilizado para elimianr una respuesta de 
         * una pregunta de opción multiple
         * Esté metodo recibe un id de la respuesta
         * para eliminar la respuesta
         */
        [Authorize]
        public ActionResult DeleteAnswerMultipleChoice(int id)
        {
            var AnswerOptionMultipled = ApplicationDbContext.MG_AnswerMultipleChoice.Find(id);
            var settingId = AnswerOptionMultipled.MG_MultipleChoice.Sett_Id;
            TempData["Add"] = "Respuesta Eliminada";
            ApplicationDbContext.MG_AnswerMultipleChoice.Remove(AnswerOptionMultipled);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("MgQuestions", new { id = settingId});
        }
        /*
        * Metodo utilizado para realizar la gestión de carga masiva de las pregunta que van a pertenecer a un juego
        * Esté metodo recibe el id de la configuración del juego
        */
        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public ActionResult MassiveQuestions(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(Id);
            AdmingameMassiveQuestion model = new AdmingameMassiveQuestion
            {
                Setting=setting,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser; 
            return View(model);
        }
        /*
        * Metodo utilizado para guardar las preguntas qus se cargan en un archivo excel
        * a una juego
        * Esté metodo recibe un modelo con los datos necesarios para el registro de las preguntas,
        * ademas recibe un archivo excel que se almacena en la variable excelUpload
        */
        [HttpPost]
        [AllowAnonymous]
        public ActionResult MassiveQuestions(AdmingameMassiveQuestion model, HttpPostedFileBase excelUpload)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(model.Setting.Sett_Id);
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
                    model = new AdmingameMassiveQuestion
                    {
                        Setting=setting,
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
                                    string Feedback = table.Rows[i].ItemArray[4].ToString();
                                    int Level = Int32.Parse(table.Rows[i].ItemArray[5].ToString());
                                    string[] a = AnswerOM.Split(';');
                                    string[] t = TrueAnswer.Split(';');
                                    MG_MultipleChoice optionmultiples = new MG_MultipleChoice
                                    {
                                        MuCh_NameQuestion = TitleOpenMultiple,
                                        MuCh_Description = DescriptionOptionMultiple,
                                        Sett_Id = setting.Sett_Id,
                                        MuCh_Feedback=Feedback,
                                        MuCh_Level=(LEVEL)Level,
                                        MG_SettingMp = setting,                                       
                                    };
                                    ApplicationDbContext.MG_MultipleChoice.Add(optionmultiples);
                                    ApplicationDbContext.SaveChanges();
                                    for (int c = 0; c < a.Length; c++)
                                    {
                                        var m = a[c];
                                        var n = t[c];
                                        int Z = Int32.Parse(n);
                                        MG_AnswerMultipleChoice AnswerOptionMultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = m,
                                            AnMul_TrueAnswer = (OPTIONANSWER)Z,
                                            MuCh_ID = optionmultiples.MuCh_ID,
                                            MG_MultipleChoice = optionmultiples
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoice.Add(AnswerOptionMultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                }
                                break;                          
                        }
                    }
                    reader.Close();
                    ModelState.AddModelError("File", "Preguntas cargadas con exito");
                    model = new AdmingameMassiveQuestion
                    {
                        Setting = setting,
                        Logo = GetUrlLogo()
                    };
                    model.Sesion = GetActualUserId().SesionUser;                    
                    return View(model);
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " Por este motivo no se pueden cargar las preguntas,por favor verifique las respuestas y vuelvlo a intentar";
                    model.Sesion = GetActualUserId().SesionUser;
                    model.Setting = setting;
                    model.Logo = GetUrlLogo();
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("File", "No se a seleccionado un archivo");
                model = new AdmingameMassiveQuestion
                {
                    Setting = setting,
                    Logo = GetUrlLogo()
                };
                model.Sesion = GetActualUserId().SesionUser;
                return View(model);
            }
        }
        /*
        * Metodo utilizado para verificar que los datos cargados en el archivo excel
        * no esten vacios
        * Esté metodo recibe los datos del archivo excel que se carga 
        */
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
                        if (m == null)
                        {
                            return "Deben Haber respustas creadas";
                        }
                    }
                }
            }
            return "success";
        }
        /*
         * Metodo utilizado para cargar el documento
         * de los terminos y condiciones del juego
         * Esté metodo recibe un modelo con los datos necesarios 
         * para la creación del documento
         */ 
        [HttpPost]
        public ActionResult TermsGame(AdminGameSetting model, HttpPostedFileBase upload)
        {   
                int CompanyUser = (int)GetActualUserId().CompanyId;
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
                                var terms = ApplicationDbContext.MG_SettingMp.Where(x => x.Company_Id == CompanyUser).FirstOrDefault();
                                terms.Sett_Instruction = ruta;
                                ApplicationDbContext.SaveChanges();
                                return RedirectToAction("Setting");
                            }
                        }
                        TempData["Info"] = "El formato del archivo no es valido";
                        return RedirectToAction("Setting");
                    }
                    else
                    {
                        TempData["Info"] = "No se ha seleccionado un archivo o el archivo es muy pesado";
                        return RedirectToAction("Setting");
                    }                             
                    }
    }
}