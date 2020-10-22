using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.MeasuringSystem;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class ContentAdminController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ContentAdminController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        //Get Proficiencies
        [Authorize]
        public ActionResult Proficiencies()
        {
            ContentProficiencyViewModel model = new ContentProficiencyViewModel { ActualRole = GetActualUserId().Role, Proficiencies = ApplicationDbContext.Proficiencies.ToList(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchProficiency(ContentProficiencyViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchProficiency) || string.IsNullOrWhiteSpace(model.SearchProficiency))
            {
                model = new ContentProficiencyViewModel { ActualRole = GetActualUserId().Role, Proficiencies = ApplicationDbContext.Proficiencies.ToList(), Sesion= GetActualUserId().SesionUser };
                return View("Proficiencies", model);
            }
            else
            {
                List<Proficiency> searchedProficiencies = ApplicationDbContext.Proficiencies.Where(p => p.ProficiencyDescription.Contains(model.SearchProficiency)).ToList();
                model = new ContentProficiencyViewModel { ActualRole = GetActualUserId().Role, Proficiencies = searchedProficiencies, Sesion = GetActualUserId().SesionUser };
                return View("Proficiencies", model);
            }
        }

        [Authorize]
        //Get AddProficiencies PartialView
        public ActionResult AddProficiency()
        {
            return PartialView("_AddProficiency");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddProficiency(ContentProficiencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                Proficiency proficiency = new Proficiency { ProficiencyDescription = model.ProficiencyDescription };
                ApplicationDbContext.Proficiencies.Add(proficiency);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Proficiencies");
            }
            else
            {
                model.Proficiencies = ApplicationDbContext.Proficiencies.ToList();
                return View("Proficiencies", model);
            }
        }


        [Authorize]
        public ActionResult DeleteProficiency(int id)
        {
            Proficiency deletedProficiency = ApplicationDbContext.Proficiencies.Find(id);
            ApplicationDbContext.Proficiencies.Remove(deletedProficiency);
            ApplicationDbContext.SaveChanges();
            ContentProficiencyViewModel model = new ContentProficiencyViewModel { ActualRole = GetActualUserId().Role, Proficiencies = ApplicationDbContext.Proficiencies.ToList(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Proficiencies");
        }



        [Authorize]
        public ActionResult UpdateProficiency(int id)
        {
            Proficiency updatedProficiency = ApplicationDbContext.Proficiencies.Find(id);
            ContentProficiencyViewModel model = new ContentProficiencyViewModel { ProficiencyId = updatedProficiency.ProficiencyId, ProficiencyDescription = updatedProficiency.ProficiencyDescription };
            return PartialView("_UpdateProficiency", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateProficiency(ContentProficiencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                Proficiency updatedproficiency = ApplicationDbContext.Proficiencies.Find(model.ProficiencyId);
                updatedproficiency.ProficiencyDescription = model.ProficiencyDescription;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Proficiencies");
            }
            else
            {
                model.Proficiencies = ApplicationDbContext.Proficiencies.ToList();
                return View("Proficiencies", model);
            }
        }

        //Get Questions
        [Authorize]
        public ActionResult Questions()
        {
            ContentQuestionViewModels model = new ContentQuestionViewModels { ActualRole = GetActualUserId().Role, Questions = ApplicationDbContext.Questions.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchQuestion(ContentQuestionViewModels model)
        {
            if (string.IsNullOrEmpty(model.SearchQuestion) || string.IsNullOrWhiteSpace(model.SearchQuestion))
            {
                model = new ContentQuestionViewModels { ActualRole = GetActualUserId().Role, Questions = ApplicationDbContext.Questions.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Questions", model);
            }
            else
            {
                List<Question> searchedQuestions = ApplicationDbContext.Questions.Where(p => p.QuestionDescription.Contains(model.SearchQuestion)).ToList();
                model = new ContentQuestionViewModels { ActualRole = GetActualUserId().Role, Questions = searchedQuestions, Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Questions", model);
            }
        }

        [Authorize]
        //Get AddProficiencies PartialView
        public ActionResult AddQuestion()
        {
            ContentQuestionViewModels model = new ContentQuestionViewModels();
            model.Questions = ApplicationDbContext.Questions.ToList();
            model.Proficiencies = GetProficienciesIEnum();
            return PartialView("_AddQuestion", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(ContentQuestionViewModels model)
        {
            if (ModelState.IsValid)
            {
                Question question = new Question { QuestionDescription = model.QuestionDescription, QuestionType = model.QuestionType, Proficiency = ApplicationDbContext.Proficiencies.Find(model.QuestionProficiencyId) };
                ApplicationDbContext.Questions.Add(question);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Questions");
            }
            else
            {
                model.Questions = ApplicationDbContext.Questions.ToList();
                model.Proficiencies = GetProficienciesIEnum();
                return View("Questions", model);
            }
        }


        [Authorize]
        public ActionResult DeleteQuestion(int id)
        {
            Question deletedQuestion = ApplicationDbContext.Questions.Find(id);
            ApplicationDbContext.Questions.Remove(deletedQuestion);
            ApplicationDbContext.SaveChanges();
            ContentQuestionViewModels model = new ContentQuestionViewModels { ActualRole = GetActualUserId().Role, Questions = ApplicationDbContext.Questions.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Questions");
        }

        [Authorize]
        public ActionResult UpdateQuestion(int id)
        {
            Question updateQuestion = ApplicationDbContext.Questions.Find(id);
            ContentQuestionViewModels model = new ContentQuestionViewModels { QuestionId = updateQuestion.QuestionId, QuestionDescription = updateQuestion.QuestionDescription, QuestionProficiencyId = updateQuestion.ProficiencyId, Proficiencies = GetProficienciesIEnum() };
            return PartialView("_UpdateQuestion", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateQuestion(ContentQuestionViewModels model)
        {
            if (ModelState.IsValid)
            {
                Question updatedQuestion = ApplicationDbContext.Questions.Find(model.QuestionId);
                updatedQuestion.QuestionDescription = model.QuestionDescription;
                updatedQuestion.QuestionType = model.QuestionType;
                updatedQuestion.Proficiency = ApplicationDbContext.Proficiencies.Find(model.QuestionProficiencyId);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Questions");
            }
            else
            {
                model = new ContentQuestionViewModels { ActualRole = GetActualUserId().Role, Questions = ApplicationDbContext.Questions.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Questions", model);
            }
        }

        private IEnumerable<SelectListItem> GetProficienciesIEnum()
        {
            var dbProficiencies = ApplicationDbContext.Proficiencies;
            var proficiencies = dbProficiencies
                        .Select(proficiency =>
                                new SelectListItem
                                {
                                    Value = proficiency.ProficiencyId.ToString(),
                                    Text = proficiency.ProficiencyDescription
                                });

            return new SelectList(proficiencies, "Value", "Text");
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

        //Get Tests
        [Authorize]
        public ActionResult Tests()
        {
            ContentTestViewModel model = new ContentTestViewModel { ActualRole = GetActualUserId().Role, Tests = ApplicationDbContext.Tests.ToList(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        private void PopulateAssignedQuestionsData()
        {
            List<Question> allQuestions = ApplicationDbContext.Questions.ToList();
            List<AssignedQuestionData> assignedQuestions = new List<AssignedQuestionData>();
            foreach (Question item in allQuestions)
            {
                assignedQuestions.Add(new AssignedQuestionData
                {
                    QuestionId = item.QuestionId,
                    QuestionDescription = item.QuestionDescription,
                    Assigned = false
                });
            }
            ViewBag.AssignedQuestions = assignedQuestions;
        }

        private void PopulateAssignedQuestionsDataWithTest(Test test)
        {
            List<Question> allQuestions = ApplicationDbContext.Questions.ToList();
            HashSet<int> testQuestions = new HashSet<int>(test.Questions.Select(q => q.QuestionId));
            List<AssignedQuestionData> assignedQuestions = new List<AssignedQuestionData>();
            foreach (Question item in allQuestions)
            {
                assignedQuestions.Add(new AssignedQuestionData
                {
                    QuestionId = item.QuestionId,
                    QuestionDescription = item.QuestionDescription,
                    Assigned = testQuestions.Contains(item.QuestionId)
                });
            }
            ViewBag.AssignedQuestions = assignedQuestions;
        }

        private void UpdateTestQuestions(string[] selectedQuestions, Test testToUpdate)
        {
            if (selectedQuestions == null)
            {
                testToUpdate.Questions = new List<Question>();
                return;
            }

            var selectedQuestionsHS = new HashSet<string>(selectedQuestions);
            var testQuestions = new HashSet<int>
                (testToUpdate.Questions.Select(c => c.QuestionId));
            foreach (var question in ApplicationDbContext.Questions)
            {
                if (selectedQuestionsHS.Contains(question.QuestionId.ToString()))
                {
                    if (!testQuestions.Contains(question.QuestionId))
                    {
                        if (!testToUpdate.Questions.Contains(question))
                        {
                            testToUpdate.Questions.Add(question);
                        }
                    }
                }
                else
                {
                    if (testQuestions.Contains(question.QuestionId))
                    {
                        testToUpdate.Questions.Remove(question);
                    }
                }
            }
        }

        private void AddTestQuestions(string[] selectedQuestions, Test testToAdd)
        {
            if (selectedQuestions == null)
            {
                testToAdd.Questions = new List<Question>();
                return;
            }
            var selectedQuestionsHS = new HashSet<string>(selectedQuestions);
            var listOfQuestionsToAdd = new List<Question>();
            foreach (var question in ApplicationDbContext.Questions)
            {
                if (selectedQuestionsHS.Contains(question.QuestionId.ToString()))
                {
                    listOfQuestionsToAdd.Add(question);
                }
            }
            testToAdd.Questions = listOfQuestionsToAdd;
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchTest(ContentTestViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchTest) || string.IsNullOrWhiteSpace(model.SearchTest))
            {
                model = new ContentTestViewModel { ActualRole = GetActualUserId().Role, Tests = ApplicationDbContext.Tests.ToList(), Sesion = GetActualUserId().SesionUser };
                return View("Tests", model);
            }
            else
            {
                List<Test> searchedTests = ApplicationDbContext.Tests.Where(p => p.TestDescription.Contains(model.SearchTest)).ToList();
                model = new ContentTestViewModel { ActualRole = GetActualUserId().Role, Tests = searchedTests, Sesion = GetActualUserId().SesionUser };
                return View("Tests", model);
            }
        }

        [Authorize]
        //Get AddTests PartialView
        public ActionResult AddTest()
        {
            PopulateAssignedQuestionsData();
            return PartialView("_AddTest");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(ContentTestViewModel model, string[] selectedQuestions)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Test test = new Test { TestDescription = model.TestDescription, EvaluateTo = model.EvaluateTo };
                    AddTestQuestions(selectedQuestions, test);
                    ApplicationDbContext.Tests.Add(test);
                    ApplicationDbContext.SaveChanges();
                    return RedirectToAction("Tests");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
                model.Tests = ApplicationDbContext.Tests.ToList();
                return View("Tests", model);
            }
            else
            {
                model.Tests = ApplicationDbContext.Tests.ToList();
                return View("Tests", model);
            }
        }


        [Authorize]
        public ActionResult DeleteTest(int id)
        {
            Test deletedTest = ApplicationDbContext.Tests.Find(id);
            ApplicationDbContext.Tests.Remove(deletedTest);
            ApplicationDbContext.SaveChanges();
            ContentTestViewModel model = new ContentTestViewModel { ActualRole = GetActualUserId().Role, Tests = ApplicationDbContext.Tests.ToList(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Tests");
        }

        [Authorize]
        public ActionResult UpdateTest(int id)
        {
            Test updatedTest = ApplicationDbContext.Tests.Include(i => i.Questions).
                Where(i => i.TestId == id).
                Single();
            ContentTestViewModel model = new ContentTestViewModel { TestId = updatedTest.TestId, TestDescription = updatedTest.TestDescription };
            PopulateAssignedQuestionsDataWithTest(updatedTest);
            return PartialView("_UpdateTest", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateTest(ContentTestViewModel model, string[] selectedQuestions)
        {
            if (ModelState.IsValid)
            {
                Test testToUpdate = ApplicationDbContext.Tests.Find(model.TestId);
                if (TryUpdateModel(testToUpdate))
                {
                    try
                    {
                        testToUpdate.TestDescription = model.TestDescription;
                        testToUpdate.EvaluateTo = model.EvaluateTo;
                        UpdateTestQuestions(selectedQuestions, testToUpdate);
                        ApplicationDbContext.Tests.Include(i => i.Questions);
                        ApplicationDbContext.SaveChanges();
                        return RedirectToAction("Tests");
                    }
                    catch (RetryLimitExceededException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                    model.Tests = ApplicationDbContext.Tests.ToList();
                    return View("Tests", model);
                }
                model.Tests = ApplicationDbContext.Tests.ToList();
                return View("Tests", model);
            }
            else
            {
                model.Tests = ApplicationDbContext.Tests.ToList();
                return View("Tests", model);
            }
        }

        //Get Plans
        [Authorize]
        public ActionResult Plans()
        {
            ContentPlansViewModel model = new ContentPlansViewModel { ActualRole = GetActualUserId().Role, userId = GetActualUserId().Id, Plans = ApplicationDbContext.Plans.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchPlan(ContentPlansViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchPlan) || string.IsNullOrWhiteSpace(model.SearchPlan))
            {
                model = new ContentPlansViewModel { ActualRole = GetActualUserId().Role, Plans = ApplicationDbContext.Plans.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Plans", model);
            }
            else
            {
                List<Plan> searchedPlans = ApplicationDbContext.Plans.Where(p => p.PlanDescription.Contains(model.SearchPlan)).ToList();
                model = new ContentPlansViewModel { ActualRole = GetActualUserId().Role, Plans = searchedPlans, Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Plans", model);
            }
        }

        [Authorize]
        //Get AddPlans PartialView
        public ActionResult AddPlan()
        {
            ContentPlansViewModel model = new ContentPlansViewModel { ActualRole = GetActualUserId().Role, Plans = ApplicationDbContext.Plans.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
            return PartialView("_AddPlan", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlan(ContentPlansViewModel model, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Resource newResource = new Resource
                        {
                            ResourceName = System.IO.Path.GetFileName(upload.FileName),
                            ResourceType = upload.ContentType,
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            newResource.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        ApplicationDbContext.Resources.Add(newResource);
                        Plan Plan = new Plan
                        {
                            PlanDescription = model.PlanDescription,
                            PlanMaxScore = model.PlanMaxScore,
                            Proficiency = ApplicationDbContext.Proficiencies.Find(model.PlanProficiencyId),
                            PlanMinScore = model.PlanMinScore,
                            PlanTo = model.PlanTo,
                        };
                        Plan.Resource.Add(newResource);
                        ApplicationDbContext.Plans.Add(Plan);
                        ApplicationDbContext.SaveChanges();
                        model.Plans = ApplicationDbContext.Plans.ToList();
                        return RedirectToAction("Plans");
                    }
                    else
                    {
                        model.ActualRole = GetActualUserId().Role;
                        model.Plans = ApplicationDbContext.Plans.ToList();
                        model.Proficiencies = GetProficienciesIEnum();
                        model.Sesion = GetActualUserId().SesionUser;
                        return View("Plans", model);
                    }
                }
                else
                {
                    model.ActualRole = GetActualUserId().Role;
                    model.Plans = ApplicationDbContext.Plans.ToList();
                    model.Proficiencies = GetProficienciesIEnum();
                    model.Sesion = GetActualUserId().SesionUser;
                    return View("Plans", model);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            model.Plans = ApplicationDbContext.Plans.ToList();
            model.Proficiencies = GetProficienciesIEnum();
            model.Sesion = GetActualUserId().SesionUser;
            return View("Plans", model);
        }


        [Authorize]
        public ActionResult DeletePlan(int id)
        {
            Plan deletedPlan = ApplicationDbContext.Plans.Find(id);
            // ApplicationDbContext.Resources.Remove(d.);
            ApplicationDbContext.Plans.Remove(deletedPlan);
            ApplicationDbContext.SaveChanges();
            ContentPlansViewModel model = new ContentPlansViewModel { ActualRole = GetActualUserId().Role, Plans = ApplicationDbContext.Plans.ToList(), Proficiencies = GetProficienciesIEnum(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Plans");
        }



        [Authorize]
        public ActionResult UpdatePlan(int id)
        {
            Plan updatedPlan = ApplicationDbContext.Plans.Find(id);
            ContentPlansViewModel model = new ContentPlansViewModel
            {
                PlanId = updatedPlan.PlanId,
                PlanDescription = updatedPlan.PlanDescription,
                PlanMaxScore = updatedPlan.PlanMaxScore,
                PlanMinScore = updatedPlan.PlanMinScore,
                PlanResourceId = updatedPlan.ResourceId,
                PlanTo = updatedPlan.PlanTo,
                Proficiencies = GetProficienciesIEnum()
            };
            return PartialView("_UpdatePlan", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdatePlan(ContentPlansViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                Plan updatedPlan = ApplicationDbContext.Plans.Find(model.PlanId);
                if (upload != null && upload.ContentLength > 0)
                {
                    Resource newResource = new Resource
                    {
                        ResourceName = System.IO.Path.GetFileName(upload.FileName),
                        ResourceType = upload.ContentType,
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        newResource.Content = reader.ReadBytes(upload.ContentLength);
                    }
                    Resource deletedResource = ApplicationDbContext.Resources.Find(updatedPlan.ResourceId);
                    ApplicationDbContext.Resources.Add(newResource);
                    updatedPlan.PlanDescription = model.PlanDescription;
                    updatedPlan.PlanMaxScore = model.PlanMaxScore;
                    updatedPlan.PlanMinScore = model.PlanMinScore;
                    updatedPlan.PlanTo = model.PlanTo;
                    updatedPlan.Proficiency = ApplicationDbContext.Proficiencies.Find(model.PlanProficiencyId);
                    updatedPlan.Resource.Add(newResource);
                    ApplicationDbContext.Resources.Remove(deletedResource);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Plans");
            }
            else
            {
                model.Plans = ApplicationDbContext.Plans.ToList();
                model.Proficiencies = GetProficienciesIEnum();
                return View("Plans", model);
            }
        }

        //Get Users
        [Authorize]
        public ActionResult Users()
        {
            ContentUserViewModel model = new ContentUserViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Companies = GetCompaniesIEnum(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUser(ContentUserViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchUser) || string.IsNullOrWhiteSpace(model.SearchUser))
            {
                model = new ContentUserViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Companies = GetCompaniesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
            else
            {
                List<ApplicationUser> searchedUsers = ApplicationDbContext.Users.Where(p => p.UserName.Contains(model.SearchUser)).ToList();
                model = new ContentUserViewModel { ActualRole = GetActualUserId().Role, Users = searchedUsers, Companies = GetCompaniesIEnum(), Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
        }

        [Authorize]
        //Get AddProficiencies PartialView
        public ActionResult AddUser()
        {
            ContentUserViewModel model = new ContentUserViewModel();
            model.Users = ApplicationDbContext.Users.ToList();
            model.Companies = GetCompaniesIEnum();
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(ContentUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Company company = ApplicationDbContext.Companies.Find(model.CompanyId);
                ApplicationUser user = new ApplicationUser { UserName = model.Username, Role = model.Role, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Document = model.Document, Company = company };
                UserManager.Create(user, model.Password);
                return RedirectToAction("Users");
            }
            else
            {
                model.Users = ApplicationDbContext.Users.ToList();
                model.Companies = GetCompaniesIEnum();
                return View("Users", model);
            }
        }


        [Authorize]
        public ActionResult DeleteUser(string id)
        {
            ApplicationUser deletedUser = UserManager.FindById(id);
            UserManager.Delete(deletedUser);
            ApplicationDbContext.SaveChanges();
            ContentUserViewModel model = new ContentUserViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Companies = GetCompaniesIEnum(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Users");
        }

        [Authorize]
        public ActionResult UpdateUser(string id)
        {
            ApplicationUser updateUser = UserManager.FindById(id);
            ContentUserViewModel model = new ContentUserViewModel { UserId = id, Username = updateUser.UserName, Role = updateUser.Role, Email = updateUser.Email, FirstName = updateUser.FirstName, LastName = updateUser.LastName, Document = updateUser.Document, CompanyId = updateUser.CompanyId, Companies = GetCompaniesIEnum() };
            return PartialView("_UpdateUser", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateUser(ContentUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser updatedUser = UserManager.FindById(model.UserId);
                updatedUser.UserName = model.Username;
                UserManager.RemovePassword(updatedUser.Id);
                UserManager.AddPassword(updatedUser.Id, model.Password);
                updatedUser.Role = model.Role;
                updatedUser.Email = model.Email;
                updatedUser.FirstName = model.FirstName;
                updatedUser.LastName = model.LastName;
                updatedUser.Document = model.Document;
                Company newCompany = ApplicationDbContext.Companies.Find(model.CompanyId);
                updatedUser.Company = newCompany;
                UserManager.Update(updatedUser);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Users");
            }
            else
            {
                model = new ContentUserViewModel { ActualRole = GetActualUserId().Role, Users = ApplicationDbContext.Users.ToList(), Companies = GetCompaniesIEnum() , Sesion = GetActualUserId().SesionUser };
                return View("Users", model);
            }
        }

        //Get Companies
        [Authorize]
        public ActionResult Companies()
        {
            ContentCompaniesViewModel model = new ContentCompaniesViewModel { ActualRole = GetActualUserId().Role, Companies = ApplicationDbContext.Companies.ToList(), Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchCompany(ContentCompaniesViewModel model)
        {
            if (string.IsNullOrEmpty(model.SearchCompany) || string.IsNullOrWhiteSpace(model.SearchCompany))
            {
                model = new ContentCompaniesViewModel { ActualRole = GetActualUserId().Role, Companies = ApplicationDbContext.Companies.ToList(), Sesion = GetActualUserId().SesionUser };
                return View("Companies", model);
            }
            else
            {
                List<Company> searchedCompanies = ApplicationDbContext.Companies.Where(p => p.CompanyName.Contains(model.SearchCompany)).ToList();
                model = new ContentCompaniesViewModel { ActualRole = GetActualUserId().Role, Companies = searchedCompanies , Sesion = GetActualUserId().SesionUser };
                return View("Companies", model);
            }
        }

        [Authorize]
        //Get AddCompanies PartialView
        public ActionResult AddCompany()
        {
            return PartialView("_AddCompany");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompany(ContentCompaniesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Company Company = new Company { CompanyName = model.CompanyName, CompanyNit = model.CompanyNit, CompanySector = model.CompanySector, CompanyType = model.CompanyType };
                ApplicationDbContext.Companies.Add(Company);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Companies");
            }
            else
            {
                model.Companies = ApplicationDbContext.Companies.ToList();
                return View("Companies", model);
            }
        }


        [Authorize]
        public ActionResult DeleteCompany(int id)
        {
            Company deletedCompany = ApplicationDbContext.Companies.Find(id);
            ApplicationDbContext.Companies.Remove(deletedCompany);
            ApplicationDbContext.SaveChanges();
            ContentCompaniesViewModel model = new ContentCompaniesViewModel { ActualRole = GetActualUserId().Role, Companies = ApplicationDbContext.Companies.ToList(), Sesion = GetActualUserId().SesionUser };
            return RedirectToAction("Companies");
        }

        [Authorize]
        public ActionResult UpdateCompany(int id)
        {
            Company updatedCompany = ApplicationDbContext.Companies.Find(id);
            ContentCompaniesViewModel model = new ContentCompaniesViewModel { CompanyId = updatedCompany.CompanyId, CompanyName = updatedCompany.CompanyName, CompanyNit = updatedCompany.CompanyNit, CompanySector = updatedCompany.CompanySector, CompanyType = updatedCompany.CompanyType };
            return PartialView("_UpdateCompany", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCompany(ContentCompaniesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Company updatedCompany = ApplicationDbContext.Companies.Find(model.CompanyId);
                updatedCompany.CompanyName = model.CompanyName;
                updatedCompany.CompanyNit = model.CompanyNit;
                updatedCompany.CompanySector = model.CompanySector;
                updatedCompany.CompanyType = model.CompanyType;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Companies");
            }
            else
            {
                model.Companies = ApplicationDbContext.Companies.ToList();
                return View("Companies", model);
            }
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
    }
}