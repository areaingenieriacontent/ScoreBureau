using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SCORM1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Enum;
using System.Data.Entity;
using Chart.Mvc.ComplexChart;
using PagedList;
using SCORM1.Models.MeasuringSystem;
using SCORM1.Models.SCORM1;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace SCORM1.Controllers
{
    public class AdminController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


        public AdminController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin
        [Authorize]
        public ActionResult Users()
        {

            var GetUserCompany = GetActualUserId().CompanyId;
            List<ApplicationUser> usuaurio = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany).ToList();
            Company company = ApplicationDbContext.Companies.Find(GetActualUserId().CompanyId);
            AdminUsersViewModels model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Users = usuaurio, Areas = GetAreasIEnum(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUser(AdminUsersViewModels model)
        {
            if (string.IsNullOrEmpty(model.SearchUser) || string.IsNullOrWhiteSpace(model.SearchUser))
            {
                var GetUserCompany = GetActualUserId().CompanyId;
                List<ApplicationUser> usuaurio = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany).ToList();
                Company company = ApplicationDbContext.Companies.Find(GetActualUserId().CompanyId);
                model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Users = usuaurio, Areas = GetAreasIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
            else
            {
                var GetUserCompany = GetActualUserId().CompanyId;
                List<ApplicationUser> searchedUser = ApplicationDbContext.Users.Where(p => p.UserName.Contains(model.SearchUser)).ToList();
                model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Users = searchedUser, Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
        }


        [Authorize]
        //Get AddUser PartialView
        public ActionResult AddUser()

        {
            AdminUsersViewModels model = new AdminUsersViewModels();
            model.Users = ApplicationDbContext.Users.ToList();
            model.Areas = GetAreasIEnum();
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(AdminUsersViewModels model)
        {
            if (ModelState.IsValid)
            {

                Company company = ApplicationDbContext.Companies.Find(GetActualUserId().Company.CompanyId);
                Area area = ApplicationDbContext.Areas.Find(model.AreaId);
                ApplicationUser user = new ApplicationUser { Role = (ROLES)model.Role, UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Document = model.Document, Company = company, Area = area };
                UserManager.Create(user, model.Password);
                return RedirectToAction("Users");

            }
            else
            {
                model.Users = ApplicationDbContext.Users.ToList();
                model.Areas = GetAreasIEnum();
                return View("Users", model);
            }
        }


        [Authorize]
        public ActionResult DeleteUser(string id)
        {
            ApplicationUser deletedUser = UserManager.FindById(id);
            UserManager.Delete(deletedUser);
            ApplicationDbContext.SaveChanges();
            AdminUsersViewModels model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Areas = GetAreasIEnum(), Sesion = GetActualUserId().SesionUser };
            model.Users = ApplicationDbContext.Users.ToList();
            model.Areas = GetAreasIEnum();
            return RedirectToAction("Users");
        }


        [Authorize]
        public ActionResult UpdateUser(string id)
        {
            ApplicationUser updatedUser = UserManager.FindById(id);
            AdminUsersViewModels model = new AdminUsersViewModels { UserId = id, Role = (ADMIN_CREATE_ROLES)updatedUser.Role, UserName = updatedUser.UserName, Document = updatedUser.Document, Email = updatedUser.Email, FirstName = updatedUser.FirstName, LastName = updatedUser.LastName, Areas = GetAreasIEnum() };
            return PartialView("_UpdateUser", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateUser(AdminUsersViewModels model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser updatedUser = UserManager.FindById(model.UserId);
                UserManager.RemovePassword(updatedUser.Id);
                UserManager.AddPassword(updatedUser.Id, model.Password);
                updatedUser.Id = model.UserId;
                updatedUser.Role = (ROLES)model.Role;
                updatedUser.UserName = model.UserName;
                UserManager.AddPassword(updatedUser.Id, model.Password);
                updatedUser.Email = model.Email;
                updatedUser.FirstName = model.FirstName;
                updatedUser.LastName = model.LastName;
                updatedUser.Document = model.Document;
                Area newArea = ApplicationDbContext.Areas.Find(model.AreaId);
                updatedUser.Area = newArea;
                UserManager.Update(updatedUser);
                ApplicationDbContext.SaveChanges();
                model.Users = ApplicationDbContext.Users.ToList();
                model.Areas = GetAreasIEnum();
                return RedirectToAction("Users", model);
            }
            else
            {
                model.Users = ApplicationDbContext.Users.ToList();
                model.Areas = GetAreasIEnum();
                model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Areas = GetAreasIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
        }

        private IEnumerable<SelectListItem> GetCompaniesIEnum()
        {
            var dbCompanies = ApplicationDbContext.Companies;
            var Companies = dbCompanies
                        .Select(company =>
                                new SelectListItem
                                {
                                    Value = company.CompanyId.ToString(),
                                    Text = company.CompanyName
                                });

            return new SelectList(Companies, "Value", "Text");
        }

        private IEnumerable<SelectListItem> GetAreasIEnum()
        {
            var userCompany = GetActualUserId().CompanyId;
            var dbAreas = ApplicationDbContext.Areas.Where(x => x.CompanyId == userCompany);
            var Areas = dbAreas
                        .Select(area =>
                                new SelectListItem
                                {
                                    Value = area.AreaId.ToString(),
                                    Text = area.AreaName
                                });
            return new SelectList(Areas, "Value", "Text");
        }

        [Authorize]
        public ActionResult HandleUsers()
        {
            AdminUsersViewModels model = new AdminUsersViewModels { ActualRole = GetActualUserId().Role, Sesion = GetActualUserId().SesionUser };
            return View(model);
        }
        [Authorize]
        public List<Result> ResultOfCompany()
        {
            var GetUserProfile = GetActualUserId().Id;
            var companyId = GetActualUserId().CompanyId;
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.CompanyId == companyId).ToList();

            return allresults;
        }
        [Authorize]
        public List<Score> AverageOfScoresOfCompany()
        {
            var companyId = GetActualUserId().CompanyId;
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.CompanyId == companyId).ToList();
            List<Score> finalScores = new List<Score>();
            try
            {
                foreach (Proficiency proficiency in ApplicationDbContext.Proficiencies.ToList())
                {
                    List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency).ToList();
                    finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value) });
                }
            }
            catch (Exception)
            {
                return finalScores;
            }
            return finalScores;
        }

        [Authorize]
        public List<Score> AverageOfScoreOfSpecificCompany(List<Measure> measures, Proficiency proficiency)
        {
            var companyId = GetActualUserId().CompanyId;
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.CompanyId == companyId).ToList();
            List<Score> finalScores = new List<Score>();
            foreach (Measure measure in measures)
            {
                List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency && s.Results.MeasureId == measure.MeasureId).ToList();
                finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value), Results = proficiencyScores.Where(pr => pr.Results.MeasureId == measure.MeasureId).ToList().FirstOrDefault().Results });

            }
            return finalScores;
        }

        public List<Score> AverageScoresOfCompany()
        {
            var GetUserProfile = GetActualUserId().Id;
            var companyId = GetActualUserId().CompanyId;
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.CompanyId == companyId).ToList();
            List<Score> finalScores = new List<Score>();
            try
            {
                foreach (Proficiency proficiency in ApplicationDbContext.Proficiencies.ToList())
                {
                    List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency).ToList();
                    finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value) });
                }
            }
            catch (Exception)
            {
                return finalScores;
            }
            return finalScores;
        }

        [Authorize]
        public ActionResult Results()
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            var finishScores = AverageOfScoresOfCompany();
            var UserCompany = ApplicationDbContext.Users.Include(x => x.Area).Where(p => p.CompanyId == GetUserCompany && (p.Role == ROLES.UsuarioJefe || p.Role == ROLES.Usuario)).ToList();
            AdminResultsViewModel model = new AdminResultsViewModel { ActualRole = GetActualUserId().Role, FirstScore = new Result { Scores = finishScores }, ResultChart = JsonGraphics(), Users = UserCompany, Sesion = GetActualUserId().SesionUser };
            // model.Areas = GetAreasIEnum();
            return View(model);
        }
        [Authorize]
        public BarChart JsonGraphics()
        {
            var ListJsonTest = AverageOfScoresOfCompany();
            List<double> x = new List<double>();
            List<ComplexDataset> list = new List<ComplexDataset>();
            List<String> labels = new List<string>();
            List<double> scoresValues = new List<double>();
            foreach (var Score in ListJsonTest.ToList())
            {
                scoresValues.Add(Score.Value);
                labels.Add("competencia" + Score.Proficiency.ProficiencyId);
            }
            list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(255, 86, 3, 1)" });
            BarChart chart = new BarChart();
            chart.ComplexData.Datasets = list;
            chart.ComplexData.Labels = labels;
            return chart;
        }

        [Authorize]
        public LineChart GraphicsForCompanyProfiencies(int id)
        {
            int companyId = GetActualUserId().CompanyId.Value;
            Proficiency profiency = ApplicationDbContext.Proficiencies.Find(id);
            List<Measure> measuresList = ApplicationDbContext.Measures.Where(m => m.CompanyId == companyId).OrderByDescending(m => m.MeasureId).Take(10).ToList();
            var listJsonTest = AverageOfScoreOfSpecificCompany(measuresList, profiency);
            List<ComplexDataset> list = new List<ComplexDataset>();
            List<String> labels = new List<string>();
            List<double> scoresValues = new List<double>();
            foreach (var Score in listJsonTest.ToList())
            {

                scoresValues.Add(Score.Value);
                labels.Add(Score.Results.ResultDate.ToShortDateString());
            }

            list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(255, 86, 3, 1)" });
            LineChart chart = new LineChart();
            chart.ComplexData.Datasets = list;
            chart.ComplexData.Labels = labels;
            return chart;
        }

        [Authorize]
        public ActionResult GraphicsForCompany1(int GraphicId)
        {
            var finishScores = GraphicsForCompanyProfiencies(GraphicId);
            AdminResultsViewModel model = new AdminResultsViewModel { ResultCharCompany = finishScores };
            return PartialView("_GraphicsForCompany", model);

        }
        [Authorize]
        public ActionResult GraphicsForCompanyGeneral()
        {
            var finishScores = JsonGraphics();
            AdminResultsViewModel model = new AdminResultsViewModel { ResultChart = finishScores };
            return PartialView("_GraphicsForCompany", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUserArea(AdminResultsViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchUserArea) || string.IsNullOrWhiteSpace(model.SearchUserArea))
            {
                var GetUserCompany = GetActualUserId().CompanyId;
                List<ApplicationUser> usuaurio = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && (x.Role == ROLES.UsuarioJefe || x.Role == ROLES.Usuario)).ToList();
                model = new AdminResultsViewModel { ActualRole = GetActualUserId().Role, Users = usuaurio, Sesion = GetActualUserId().SesionUser };
                return View("Results", model);
            }
            else
            {
                var GetUserCompany = GetActualUserId().CompanyId;
                var finishScores = AverageOfScoresOfCompany();
                var UserCompany = ApplicationDbContext.Users.Include(x => x.Area).Where(p => p.CompanyId == GetUserCompany && (p.Role == ROLES.UsuarioJefe || p.Role == ROLES.Usuario) && (p.FirstName.Contains(model.SearchUserArea) || p.LastName.Contains(model.SearchUserArea))).ToList();
                model = new AdminResultsViewModel { ActualRole = GetActualUserId().Role, FirstScore = new Result { Scores = finishScores }, ResultChart = JsonGraphics(), Users = UserCompany, Sesion = GetActualUserId().SesionUser };
                return View("Results", model);
            }
        }
        /*     [Authorize]
           public List<Score> AverageOfScoreOfSpecificCompanyArea(List<Measure> measures, Proficiency proficiency)
           {
               var companyId = GetActualUserId().CompanyId;
               var AreaUser = GetActualUserId().AreaId;
               List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.CompanyId == companyId && r.ResultOwner.AreaId== AreaUser).ToList();
               List<Score> finalScores = new List<Score>();
               foreach (Measure measure in measures)
               {
                   List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency && s.Results.MeasureId == measure.MeasureId).ToList();
                   finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value), Results = proficiencyScores.Where(pr => pr.Results.MeasureId == measure.MeasureId).ToList().FirstOrDefault().Results });

               }
               return finalScores;
           }
          [Authorize]
            public LineChart GraphicsForCompanyAreas(int id, int AreadId)
            {
                int companyId = GetActualUserId().CompanyId.Value;
                Proficiency profiency = ApplicationDbContext.Proficiencies.Find(id);
                List<Measure> measuresList = ApplicationDbContext.Measures.Where(m => m.CompanyId == companyId).OrderByDescending(m => m.MeasureId).Take(10).ToList();
                var listJsonTest = AverageOfScoreOfSpecificCompanyArea(measuresList, profiency);
                List<ComplexDataset> list = new List<ComplexDataset>();
                List<String> labels = new List<string>();
                List<double> scoresValues = new List<double>();
                foreach (var Score in listJsonTest.ToList())
                {

                    scoresValues.Add(Score.Value);
                    labels.Add(Score.Results.ResultDate.ToShortDateString());
                }

                list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(50, 174, 156, 0.2)" });
                LineChart chart = new LineChart();
                chart.ComplexData.Datasets = list;
                chart.ComplexData.Labels = labels;
                return chart;
            }
            public ActionResult GraphicsForArea(int GraphicId, int AreaId)
            {
                var finishScores = GraphicsForCompanyAreas(GraphicId, AreaId);
                AdminResultsViewModel model = new AdminResultsViewModel { ResultCharcompany = finishScores };
                return PartialView("_GraphicsForCompany", model);

            }*/

        [Authorize]
        public ActionResult Plans()
        {
            var companyId = GetActualUserId().CompanyId;
            AdminPlansViewModel model = new AdminPlansViewModel { ActualRole = GetActualUserId().Role, userId= GetActualUserId().Id,Plans = GetLowerScores(), Users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUserPlan(AdminPlansViewModel model)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.SearchUserPlan) || string.IsNullOrWhiteSpace(model.SearchUserPlan))
            {
                model = new AdminPlansViewModel { ActualRole = GetActualUserId().Role, Plans = GetLowerScores(), Users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList(), Sesion = GetActualUserId().SesionUser };
                return View("Plans", model);
            }
            else
            {
                model = new AdminPlansViewModel { ActualRole = GetActualUserId().Role, Plans = GetLowerScores(), Users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario) && (u.FirstName.Contains(model.SearchUserPlan) || u.LastName.Contains(model.SearchUserPlan))).ToList(), Sesion = GetActualUserId().SesionUser };
                return View("Plans", model);
            }
        }

        public List<Plan> GetLowerScores()
        {
            List<Score> lowerScores = AverageScoresOfCompany().OrderBy(s => s.Value).Take(3).ToList();
            List<Plan> selectPlans = new List<Plan>();
            foreach (Score score in lowerScores)
            {
                foreach (Plan plan in ApplicationDbContext.Plans.Where(p => p.PlanTo == PLAN_TO.Empleados))
                {
                    if ((plan.PlanMinScore <= score.Value && plan.PlanMaxScore >= score.Value) && plan.Proficiency == score.Proficiency)
                    {
                        selectPlans.Add(plan);
                    }
                }
            }
            return selectPlans;
        }

        [Authorize]
        public ActionResult Areas()
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            List<Area> areas = ApplicationDbContext.Areas.Include(x => x.ApplicationUser).Where(x => x.CompanyId == GetUserCompany).ToList();

            AdminAreaViewModel model = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, Areas = areas, Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [Authorize]
        //Get AddCompanies PartialView
        public ActionResult AddArea()
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            var ListsUser = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && x.Role == ROLES.UsuarioJefe).ToList();
            AdminAreaViewModel models = new AdminAreaViewModel { Users = ListsUser };
            return PartialView("_AddArea", models);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddArea(AdminAreaViewModel model)
        {

            var GetUserCompany = GetActualUserId().CompanyId;
            if (SearchBossSelected(model) != "false")
            {
                Area AreasAdd = new Area { AreaId = model.AreaId, AreaName = model.AreaName, CompanyId = GetUserCompany.Value, UserId = model.Selected };
                var UpdateUser = ApplicationDbContext.Users.Find(model.Selected);
                UpdateUser.AreaId = model.AreaId;
                ApplicationDbContext.Areas.Add(AreasAdd);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Areas");
            }
            else
            {
                return RedirectToAction("Areas");
            }
        }

        public ActionResult UpdateArea(int IdArea)
        {
            Area UpdateArea = ApplicationDbContext.Areas.Find(IdArea);
            var GetUserCompany = GetActualUserId().CompanyId;
            var ListsUser = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && x.Role == ROLES.UsuarioJefe).ToList();
            AdminAreaViewModel models = new AdminAreaViewModel { Users = ListsUser, AreaName = UpdateArea.AreaName, AreaId = IdArea };
            return PartialView("_UpdateAreas", models);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateArea(AdminAreaViewModel model)
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                Area UpdateArea = ApplicationDbContext.Areas.Find(model.AreaId);
                UpdateArea.AreaName = model.AreaName;
                UpdateArea.UserId = model.Selected;
                UpdateArea.CompanyId = GetUserCompany.Value;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Areas");
            }
            else
            {

                return RedirectToAction("Areas");
            }
        }


        [Authorize]
        public ActionResult DeleteArea(int id)
        {
            Area deletedArea = ApplicationDbContext.Areas.Find(id);
            ApplicationUser areaBoss = ApplicationDbContext.Users.Find(deletedArea.UserId);
            areaBoss.Area = null;
            UserManager.Update(areaBoss);
            ApplicationDbContext.Areas.Remove(deletedArea);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("Areas");
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchArea(AdminAreaViewModel model)
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            List<Area> areas = ApplicationDbContext.Areas.Include(i => i.ApplicationUser).Where(x => x.CompanyId == GetUserCompany).ToList();
            if (string.IsNullOrEmpty(model.AreaSearch) || string.IsNullOrWhiteSpace(model.AreaSearch))
            {
                model = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, Areas = areas, Sesion = GetActualUserId().SesionUser };
                return View("Areas", model);
            }
            else
            {
                List<Area> SearchAreas = ApplicationDbContext.Areas.Where(x => x.AreaName.Contains(model.AreaSearch) && x.CompanyId == GetUserCompany).ToList();
                model = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, Areas = SearchAreas , Sesion = GetActualUserId().SesionUser };
                return View("Areas", model);
            }
        }

        [Authorize]
        public ActionResult ModifyAreaAdduser(int AreaId)
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            List<Area> areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetUserCompany).ToList();
            List<ApplicationUser> UsersOfArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && x.AreaId == AreaId).ToList();
            List<ApplicationUser> UsersOutArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany).ToList();
            List<ApplicationUser> UsersOutAreaFinish = UsersOutArea.Except(UsersOfArea).ToList();

            AdminAreaViewModel models = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, UsersOfArea = UsersOfArea, UsersOutArea = UsersOutAreaFinish, Areas = areas, AreaId = AreaId, Sesion = GetActualUserId().SesionUser };
            return PartialView("_ModifyArea", models);
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddUserArea(string UserId, int AreaId)
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            if (UserId != null && AreaId != 0)
            {
                ApplicationUser UpdateUserOfArea = UserManager.FindById(UserId);
                UpdateUserOfArea.AreaId = AreaId;
                UserManager.Update(UpdateUserOfArea);
                ApplicationDbContext.SaveChanges();
            }
            List<Area> areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetUserCompany).ToList();
            List<ApplicationUser> UsersOfArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && x.AreaId == AreaId).ToList();
            List<ApplicationUser> UsersOutArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany).ToList();
            AdminAreaViewModel models = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, UsersOfArea = UsersOfArea, UsersOutArea = UsersOutArea, Areas = areas, Sesion = GetActualUserId().SesionUser };
            return View("Areas", models);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteUserArea(string UsersOfAreaId, int AreaId)
        {
            var GetUserCompany = GetActualUserId().CompanyId;
            if (UsersOfAreaId != null && AreaId != 0)
            {
                ApplicationUser DeleteUserOfArea = UserManager.FindById(UsersOfAreaId);
                DeleteUserOfArea.AreaId = null;
                UserManager.Update(DeleteUserOfArea);
                ApplicationDbContext.SaveChanges();
            }
            List<Area> areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetUserCompany).ToList();
            List<ApplicationUser> UsersOfArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany && x.AreaId == AreaId).ToList();
            List<ApplicationUser> UsersOutArea = ApplicationDbContext.Users.Where(x => x.CompanyId == GetUserCompany).ToList();
            AdminAreaViewModel models = new AdminAreaViewModel { ActualRole = GetActualUserId().Role, UsersOfArea = UsersOfArea, UsersOutArea = UsersOutArea, Areas = areas , Sesion = GetActualUserId().SesionUser };
            return View("Areas", models);
        }


        [HttpPost]
        public String SearchBossSelected(AdminAreaViewModel model)
        {
            if (String.IsNullOrEmpty(model.Selected))
            {
                return "false";
            }
            else
            {
                return model.Selected;
            }
        }

        [Authorize]
        public ActionResult Relations(int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList().ToPagedList(page ?? 1, 6);
            AdminRelationsViewModel model = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, Users = users, Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUserRelations(AdminRelationsViewModel model, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.SearchUser) || string.IsNullOrWhiteSpace(model.SearchUser))
            {
                model = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList().ToPagedList(page ?? 1, 5), Sesion = GetActualUserId().SesionUser };
                return View("Relations", model);
            }
            else
            {
                model = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario) && (u.FirstName.Contains(model.SearchUser) || u.LastName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 5), Sesion = GetActualUserId().SesionUser };
                return View("Relations", model);
            }
        }

        //Get Measures
        [Authorize]
        public ActionResult Measures()
        {
            var companyId = GetActualUserId().CompanyId;
            AdminMeasuresViewModel model = new AdminMeasuresViewModel { ActualRole = GetActualUserId().Role, Measures = ApplicationDbContext.Measures.Where(m => m.CompanyId == companyId).ToList(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchMeasure(AdminMeasuresViewModel model)
        {
            int? company = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.SearchMeasures) || string.IsNullOrWhiteSpace(model.SearchMeasures))
            {
                model = new AdminMeasuresViewModel { ActualRole = GetActualUserId().Role, Measures = ApplicationDbContext.Measures.Where(m => m.CompanyId == company).ToList(), Sesion = GetActualUserId().SesionUser };
                return View("Measures", model);
            }
            else
            {
                List<Measure> searchedMeasures = ApplicationDbContext.Measures.Where(p => p.Test.TestDescription.Contains(model.SearchMeasures) && p.CompanyId == company).ToList();
                model = new AdminMeasuresViewModel { ActualRole = GetActualUserId().Role, Measures = searchedMeasures, Sesion = GetActualUserId().SesionUser };
                return View("Measures", model);
            }
        }

        [Authorize]
        //Get AddMeasures PartialView
        public ActionResult AddMeasure()
        {
            AdminMeasuresViewModel model = new AdminMeasuresViewModel { Tests = ApplicationDbContext.Tests.ToList(), MeasureInitDate = DateTime.Now, MeasureFinishDate = DateTime.Now };
            return PartialView("_AddMeasure", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeasure(AdminMeasuresViewModel model)
        {
            if (ModelState.IsValid)
            {
                Measure newMeasure = new Measure
                {
                    Company = GetActualUserId().Company,
                    MeasureInitDate = model.MeasureInitDate,
                    MeasureFinishDate = model.MeasureFinishDate,
                    Test = ApplicationDbContext.Tests.Find(model.Selected),
                };
                ApplicationDbContext.Measures.Add(newMeasure);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Measures");
            }
            else
            {
                model.Measures = ApplicationDbContext.Measures.ToList();
                return View("Measures", model);
            }
        }


        [Authorize]
        public ActionResult DeleteMeasure(int id)
        {
            Measure deletedMeasure = ApplicationDbContext.Measures.Find(id);
            ApplicationDbContext.Measures.Remove(deletedMeasure);
            ApplicationDbContext.SaveChanges();
            AdminMeasuresViewModel model = new AdminMeasuresViewModel { ActualRole = GetActualUserId().Role, Measures = ApplicationDbContext.Measures.ToList(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Measures");
        }

        [Authorize]
        public ActionResult UpdateMeasure(int id)
        {
            Measure updatedMeasure = ApplicationDbContext.Measures.Find(id);
            AdminMeasuresViewModel model = new AdminMeasuresViewModel { MeasureId = updatedMeasure.MeasureId, MeasureFinishDate = updatedMeasure.MeasureFinishDate, MeasureInitDate = updatedMeasure.MeasureInitDate, Selected = updatedMeasure.Test.TestId, Tests = ApplicationDbContext.Tests.ToList() };
            return PartialView("_UpdateMeasure", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateMeasure(AdminMeasuresViewModel model)
        {
            if (ModelState.IsValid)
            {
                Test newTest = ApplicationDbContext.Tests.Find(model.Selected);
                Measure updatedMeasure = ApplicationDbContext.Measures.Find(model.MeasureId);
                updatedMeasure.MeasureInitDate = model.MeasureInitDate;
                updatedMeasure.MeasureFinishDate = model.MeasureFinishDate;
                updatedMeasure.Test = newTest;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Measures");
            }
            else
            {
                model.Measures = ApplicationDbContext.Measures.ToList();
                return View("Measures", model);
            }
        }

        public ActionResult LoadRelation(string id, int? page)
        {
            ApplicationUser user = UserManager.FindById(id);
            var company = GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> userSuperiors = user.SuperiorUsers.ToList().ToPagedList(page ?? 1, 5);
            IPagedList<ApplicationUser> userClients = user.ClientsUsers.ToList().ToPagedList(page ?? 1, 5);
            IPagedList<ApplicationUser> userEquals = user.EqualUsers.ToList().ToPagedList(page ?? 1, 5);
            IPagedList<ApplicationUser> userMyOffice = user.MyOfficeUsers.ToList().ToPagedList(page ?? 1, 5);
            AdminRelationsViewModel model = new AdminRelationsViewModel { SuperiorsUsers = userSuperiors, ActualRole = GetActualUserId().Role, ClientsUsers = userClients, EqualUsers = userEquals, MyOffice = userMyOffice, SearchUser = id };
            return PartialView("_LoadRelation", model);
        }

        public ActionResult AddUserRelations(string id, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            var company = GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> users = ApplicationDbContext.Users.Where(m => m.CompanyId == company && (m.Role == ROLES.UsuarioJefe || m.Role == ROLES.Usuario) && !m.SuperiorUsers.Any(x => x.Id == id) && !m.EqualUsers.Any(x => x.Id == id) && !m.ClientsUsers.Any(x => x.Id == id) && !m.MyOfficeUsers.Any(x => x.Id == id)).ToList().ToPagedList(page ?? 1, 5);
            AdminRelationsViewModel model = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, UserAddRelations = users, SearchUser = id, Sesion = GetActualUserId().SesionUser };
            return View("AddUserRelations", model);
        }
        [HttpPost]
        public ActionResult addRelation(string id, AdminRelationsViewModel model, int? page)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(model.SearchUser);
                ApplicationUser user2 = UserManager.FindById(id);
                try
                {
                    switch (model.ASINGUSER)
                    {
                        case ASINGUSER.Superiores:
                            user.SuperiorUsers.Add(user2);
                            break;
                        case ASINGUSER.Clientes:
                            user.ClientsUsers.Add(user2);
                            break;
                        case ASINGUSER.iguales:
                            user.EqualUsers.Add(user2);
                            break;
                        case ASINGUSER.miCargo:
                            user.MyOfficeUsers.Add(user2);
                            break;
                    }
                    ApplicationDbContext.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
            var companyId = GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList().ToPagedList(page ?? 1, 6);
            AdminRelationsViewModel models = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, Users = users, Sesion = GetActualUserId().SesionUser };

            return View("Relations", models);
        }

        public ActionResult deleteRelations(string id, int relation, AdminRelationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(model.SearchUser);
                ApplicationUser user2 = UserManager.FindById(id);
                try
                {
                    switch (relation)
                    {
                        case 1:
                            user.SuperiorUsers.Remove(user2);
                            break;
                        case 2:
                            user.ClientsUsers.Remove(user2);
                            break;
                        case 3:
                            user.EqualUsers.Remove(user2);
                            break;
                        case 4:
                            user.MyOfficeUsers.Remove(user2);
                            break;
                    }

                    ApplicationDbContext.SaveChanges();

                }
                catch (Exception)
                {
                }
            }
            var companyId = GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> users = ApplicationDbContext.Users.Where(u => u.CompanyId == companyId && (u.Role == ROLES.UsuarioJefe || u.Role == ROLES.Usuario)).ToList().ToPagedList(1, 6);
            AdminRelationsViewModel models = new AdminRelationsViewModel { ActualRole = GetActualUserId().Role, Users = users, Sesion = GetActualUserId().SesionUser };

            return View("Relations", models);
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }


        public ActionResult ReportUserMeasuare(int id)
        {

            ApplicationUser User = GetActualUserId();
            List<Result> allresults = ApplicationDbContext.Results.Include(x => x.QualifiedUser).Where(r => r.MeasureId == id && r.QualifiedUser.CompanyId == User.CompanyId).ToList();
            List<ClassReportUserMeasuare> n = new List<ClassReportUserMeasuare>();
            foreach (var item in allresults)
            {
                n.Add(new ClassReportUserMeasuare { user = ApplicationDbContext.Users.Find(item.QualifiedUser.Id), Score = item.Scores.Where(x => x.ResultId == item.ResultId).ToList() });
            }
            ModelReportUserMeasuare m = new ModelReportUserMeasuare
            {
                mode = n,
                id = id,
                ActualRole = User.Role,
                Sesion= GetActualUserId().SesionUser
            };
            return View(m);

        }
        public ActionResult ReportUserMeasuares(int id)
        {
            ApplicationUser User = GetActualUserId();
            List<Result> allresults = ApplicationDbContext.Results.Include(x => x.QualifiedUser).Where(r => r.MeasureId == id && r.QualifiedUser.CompanyId == User.CompanyId).ToList();
            List<ClassReportUserMeasuares> n = new List<ClassReportUserMeasuares>();
            foreach (var item in allresults)
            {
                n.Add(new ClassReportUserMeasuares
                {
                    Nombre_Completo = item.QualifiedUser.FirstName + item.QualifiedUser.LastName,
                    SERVICIO_AL_CLIENTE = item.Scores.Where(x => x.ResultId == item.ResultId && x.Proficiency.ProficiencyId == 1).FirstOrDefault().Value,
                    TRABAJO_EN_EQUIPO = item.Scores.Where(x => x.ResultId == item.ResultId && x.Proficiency.ProficiencyId == 2).FirstOrDefault().Value,
                    TOMA_DE_DECISIONES = item.Scores.Where(x => x.ResultId == item.ResultId && x.Proficiency.ProficiencyId == 3).FirstOrDefault().Value,
                    TRABAJO_BAJO_PRESIÓN = item.Scores.Where(x => x.ResultId == item.ResultId && x.Proficiency.ProficiencyId == 4).FirstOrDefault().Value,
                    ATENCIÓN_AL_DETALLE = item.Scores.Where(x => x.ResultId == item.ResultId && x.Proficiency.ProficiencyId == 5).FirstOrDefault().Value,
                });
            }

            var gv = new GridView();
            gv.DataSource = n;
            gv.DataBind();
            gv.HeaderStyle.ForeColor = System.Drawing.Color.Black;
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Resultados.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.AllowPaging = false;
            gv.DataBind();
            gv.HeaderRow.Style.Add("background-color", "#04B486");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "17px");
            gv.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("ReportUserMeasuare", new { id = id });

        }
    }
}