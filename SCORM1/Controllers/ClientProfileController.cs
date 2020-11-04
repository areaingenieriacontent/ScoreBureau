using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.ClientProfile;
using SCORM1.Models.ViewModel;

namespace SCORM1.Controllers
{
    public class ClientProfileController : Controller
    {
        protected ApplicationDbContext db { get; set; }
        //protected UserManager<ApplicationUser> UserManager { get; set; }

        public ClientProfileController()
        {
            db = new ApplicationDbContext();
            //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
        }

        // GET: PerfilamientoCliente
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            ClienteViewModel cl = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays=db.Dias.ToList()
            };
            cl.Sesion = db.Users.Find(userId).SesionUser;
            return View(cl);
        }

        //To-Do
        [Authorize]
        [HttpPost]
        public ActionResult SearchClient(string name)
        {
            ClienteViewModel cl = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.firstName == name).ToList()
            };
            return View("Index", cl);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient(ClienteViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                Cliente cl = new Cliente
                {
                    firstName = cvm.cliente.firstName,
                    lastName = cvm.cliente.lastName,
                    identification = cvm.cliente.identification,
                    enterpriseName = cvm.cliente.enterpriseName,
                    idClasificacion = cvm.cliente.idClasificacion,
                    idDia = cvm.cliente.idDia
                };
                cl.userId= User.Identity.GetUserId();
                db.Clientes.Add(cl);
                db.SaveChanges();
                ViewBag.ClientInformation= "Cliente creado exitosamente";
            }
            else
            {
                ViewBag.ClientInformation = "Error al agregar al cliente";
            }
            var userId = User.Identity.GetUserId();
            cvm = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View("Index", cvm);
        }

        [Authorize]
        public ActionResult ViewClient(int id)
        {
            var userId = User.Identity.GetUserId();
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = db.Clientes.Find(id),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View(cvm);
        }

        [Authorize]
        public ActionResult DeleteClient(int id)
        {
            Cliente c = db.Clientes.Find(id);
            db.Clientes.Remove(c);
            var userId = User.Identity.GetUserId();
            db.SaveChanges();
            ViewBag.ClientInformation = "Cliente eliminado exitosamente";
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View("Index", cvm);
        }

        [Authorize]
        public ActionResult EditClient(int id)
        {

            var userId = User.Identity.GetUserId();
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = db.Clientes.Find(id),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View("ViewClient", cvm);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient(ClienteViewModel cl)
        {
            if (ModelState.IsValid)
            {
                Cliente clientToUpdate = db.Clientes.Find(cl.cliente.id);
                clientToUpdate.firstName = cl.cliente.firstName;
                clientToUpdate.lastName = cl.cliente.lastName;
                clientToUpdate.identification = cl.cliente.identification;
                clientToUpdate.enterpriseName = cl.cliente.enterpriseName;
                clientToUpdate.idDia = cl.cliente.idDia;
                clientToUpdate.idClasificacion = cl.cliente.idClasificacion;
                db.SaveChanges();
            }

            var userId = User.Identity.GetUserId();
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View("Index", cvm);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MassiveRegister1()
        {
            var userId = User.Identity.GetUserId();
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
            return View(cvm);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult MassiveRegister1(HttpPostedFileBase excelUpload)
        {
            var userId = User.Identity.GetUserId();
            ClienteViewModel cvm = new ClienteViewModel
            {
                cliente = new Cliente(),
                listOfClients = db.Clientes.Where(x => x.userId == userId).ToList(),
                listOfCalification = db.Clasificaciones.ToList(),
                listOfDays = db.Dias.ToList()
            };
            cvm.Sesion = db.Users.Find(userId).SesionUser;
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
                    return View();
                }

                reader.IsFirstRowAsColumnNames = true;

                DataSet result = reader.AsDataSet();
                string next = VerifyFields(result);
                if (next == "success")
                {
                    foreach (DataTable table in result.Tables)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            if (table.Rows[i].ItemArray[4].ToString() != null && table.Rows[i].ItemArray[4].ToString() != "")
                            {
                                string userNameFile = table.Rows[i].ItemArray[4].ToString();
                                string userIdToIntroduce = db.Users.Where(x => x.UserName == userNameFile).FirstOrDefault().Id;
                                Cliente cli = new Cliente
                                {
                                    firstName = table.Rows[i].ItemArray[0].ToString(),
                                    lastName = table.Rows[i].ItemArray[1].ToString(),
                                    identification = table.Rows[i].ItemArray[2].ToString(),
                                    enterpriseName = table.Rows[i].ItemArray[3].ToString(),
                                    userId = userIdToIntroduce,
                                    idDia = Int32.Parse(table.Rows[i].ItemArray[5].ToString()),
                                    idClasificacion = Int32.Parse(table.Rows[i].ItemArray[6].ToString())
                                };
                                db.Clientes.Add(cli);
                                db.SaveChanges();
                            }
                            else
                            {
                                return View(cvm);
                            }
                        }
                    }
                        reader.Close();
                        return View(cvm);
                }
            }
            else
            {
                return View(cvm);
            }
            return View(cvm);
        }
        private string VerifyFields(DataSet result)
        {
            return "success";
        }
    }
}