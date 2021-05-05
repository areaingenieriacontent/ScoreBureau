using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.RigidCourse;
using SCORM1.Models.ViewModel;
using SCORM1.Models.Lms;
using System.Security.Cryptography.X509Certificates;

namespace SCORM1.Controllers
{
    public class FlashTestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected UserController userController { get; set; }

        public FlashTestsController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            userController = new UserController();
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        // GET: FlashTests
        public ActionResult Index(int ToCo_Id)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            var FlashTest = ApplicationDbContext.FlashTest.Where(x => x.ToCo_Id == ToCo_Id).ToList();
            if (FlashTest.Count != 0)
            {
                var a = FlashTest.Single(x => x.ToCo_Id == ToCo_Id);
                cursoRigidoViewModel.FlashTestId = a.FlashTestId;
                cursoRigidoViewModel.ListFlashTest = FlashTest;
                ViewData["ToCo_Id"] = ToCo_Id;

            }
            else
            {
                return RedirectToAction("Create", new { ToCo_Id = ToCo_Id });
            }

            return View(cursoRigidoViewModel);


        }


        // GET: FlashTests/Create
        public ActionResult Create(int ToCo_Id)
        {
            ViewBag.ToCo_Id = new SelectList(db.TopicsCourses.Where(x => x.ToCo_Id == ToCo_Id), "ToCo_Id", "ToCo_Name");
            return View();
        }

        // POST: FlashTests/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FlashTestId,ToCo_Id,FlashTestName,AprovedPercentage")] FlashTest flashTest)
        {
            if (ModelState.IsValid)
            {
                db.FlashTest.Add(flashTest);
                db.SaveChanges();
                return RedirectToAction("Index", new { ToCo_Id = flashTest.ToCo_Id });
            }

            ViewBag.ToCo_Id = new SelectList(db.TopicsCourses, "ToCo_Id", "ToCo_Name", flashTest.ToCo_Id);
            return View(flashTest);
        }


        // GET: FlashTestsQuestion
        public ActionResult IndexQuestion(int FlashTestId, int Toco_id)
        {

            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            var flashQuestions = ApplicationDbContext.FlashQuestion.Where(x => x.FlashTestId == FlashTestId).ToList();
            if (flashQuestions.Count != 0)
            {
                cursoRigidoViewModel.flashQuestions = flashQuestions;
                ViewData["IdTest"] = FlashTestId;
                ViewData["TocoId"] = Toco_id;

                return View(cursoRigidoViewModel);

            }
            else
            {
                return RedirectToAction("CreateFlashQuestion", new { FlashTestId = FlashTestId });
            }

        }

        public ActionResult CreateFlashQuestion(int FlashTestId)
        {
            ViewBag.ToCo_Id = new SelectList(db.FlashTest.Where(x => x.FlashTestId == FlashTestId), "FlashTestId", "FlashTestName");
            return View();
        }

        // POST: FlashTests/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFlashQuestion([Bind(Include = "FlashTestId,Enunciado")] FlashQuestion flashQuestion)
        {
            if (ModelState.IsValid)
            {
                db.FlashQuestion.Add(flashQuestion);
                db.SaveChanges();
                return RedirectToAction("IndexQuestion", new { FlashTestId = flashQuestion.FlashTestId, Toco_Id = flashQuestion.FlashTestId });


            }

            ViewBag.ToCo_Id = new SelectList(db.FlashTest.Where(x => x.FlashTestId == flashQuestion.FlashTestId), "FlashTestId", "FlashTestName");
            return View(flashQuestion);
        }
        // GET: FlashQuestions/Edit/5
        public ActionResult EditFlashQuestion(int FlashQuestionId)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;
            if (FlashQuestionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashQuestion flashQuestion = db.FlashQuestion.Find(FlashQuestionId);
            if (flashQuestion == null)
            {
                return HttpNotFound();
            }
            ViewData["IdTest"] = flashQuestion.FlashTestId;
            cursoRigidoViewModel.FlashQuestionId = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.Enunciado = flashQuestion.Enunciado;
            ViewBag.FlashTestId = new SelectList(db.FlashTest, "FlashTestId", "FlashTestName", flashQuestion.FlashTestId);
            return View(cursoRigidoViewModel);
        }

        // POST: FlashQuestions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFlashQuestion([Bind(Include = "FlashQuestionId,FlashTestId,Enunciado")] FlashQuestion flashQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flashQuestion).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexQuestion", new { FlashTestId = flashQuestion.FlashTestId, Toco_Id = flashQuestion.FlashTestId });
            }
            ViewBag.FlashTestId = new SelectList(db.FlashTest, "FlashTestId", "FlashTestName", flashQuestion.FlashTestId);
            return View(flashQuestion);
        }

        // GET: FlashQuestions/Delete/5
        public ActionResult DeleteFlashQuestion(int? FlashQuestionId)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            if (FlashQuestionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashQuestion flashQuestion = db.FlashQuestion.Find(FlashQuestionId);
            cursoRigidoViewModel.FlashQuestionId = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.Enunciado = flashQuestion.Enunciado;
            if (flashQuestion == null)
            {
                return HttpNotFound();
            }
            return View(cursoRigidoViewModel);
        }

        // POST: FlashQuestions/Delete/5
        public ActionResult DeleteFlashQuestionConfirm(int FlashQuestionId)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            FlashQuestion flashQuestion = db.FlashQuestion.Find(FlashQuestionId);
            cursoRigidoViewModel.FlashQuestionId = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.Enunciado = flashQuestion.Enunciado;
            db.FlashQuestion.Remove(flashQuestion);
            db.SaveChanges();
            return RedirectToAction("IndexQuestion", new { FlashTestId = flashQuestion.FlashTestId, Toco_Id = flashQuestion.FlashTestId });
        }


        public ActionResult IndexFlashQuestionAnswer(int FlashTestId)
        {

            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            var flashQuestionsAnswer = ApplicationDbContext.FlashQuestionAnswer.Where(x => x.FlashQuestionId == FlashTestId).ToList();
            if (flashQuestionsAnswer.Count != 0)
            {
                cursoRigidoViewModel.ListFlashQuestionAnswer = flashQuestionsAnswer;
                cursoRigidoViewModel.FlashQuestionId = flashQuestionsAnswer.FirstOrDefault().FlashQuestionId;
                ViewData["IdTest"] = FlashTestId;
                FlashQuestion flashQuestion = db.FlashQuestion.Find(cursoRigidoViewModel.FlashQuestionId);
                ViewData["TocoId"] = db.FlashTest.Where(x => x.FlashTestId == flashQuestion.FlashTestId).FirstOrDefault().ToCo_Id;
                return View(cursoRigidoViewModel);

            }
            else
            {
                return RedirectToAction("CreateFlashQuestionAnswer", new { FlashTestId = FlashTestId });
            }

        }

        public ActionResult CreateFlashQuestionAnswer(int FlashTestId)
        {
            ViewBag.FlashQuestionId = new SelectList(db.FlashQuestion.Where(x => x.FlashQuestionId == FlashTestId), "FlashQuestionId", "Enunciado");
            return View();
        }

        // POST: FlashTests/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFlashQuestionAnswer([Bind(Include = "FlashQuestionId,Content,CorrectAnswer")] FlashQuestionAnswer flashQuestion)
        {
            if (ModelState.IsValid)
            {
                db.FlashQuestionAnswer.Add(flashQuestion);
                db.SaveChanges();
                return RedirectToAction("IndexFlashQuestionAnswer", new { FlashTestId = flashQuestion.FlashQuestionId });
            }

            ViewBag.ToCo_Id = new SelectList(db.FlashTest.Where(x => x.FlashTestId == flashQuestion.FlashQuestionId), "FlashTestId", "FlashTestName");
            return View(flashQuestion);
        }
        // GET: FlashQuestions/Edit/5
        public ActionResult EditFlashQuestionAnswer(int FlashQuestionAnswerId)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;
            if (FlashQuestionAnswerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashQuestionAnswer flashQuestion = db.FlashQuestionAnswer.Find(FlashQuestionAnswerId);
            if (flashQuestion == null)
            {
                return HttpNotFound();
            }
            ViewData["IdTest"] = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.FlashQuestionId = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.FlashQuestionAnswerId = FlashQuestionAnswerId;
            cursoRigidoViewModel.Content = flashQuestion.Content;
            return View(cursoRigidoViewModel);
        }

        // POST: FlashQuestions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFlashQuestionAnswer([Bind(Include = "FlashQuestionAnswerId,FlashQuestionId,Content,CorrectAnswer")] FlashQuestionAnswer flashQuestionAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flashQuestionAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexFlashQuestionAnswer", new { FlashTestId = flashQuestionAnswer.FlashQuestionId });
            }
            ViewBag.FlashQuestionId = new SelectList(db.FlashTest, "FlashTestId", "FlashTestName", flashQuestionAnswer.FlashQuestionId);
            return View(flashQuestionAnswer);
        }

        // GET: FlashQuestions/Delete/5
        public ActionResult DeleteFlashQuestionAnswer(int FlashQuestionAnswerId)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;

            if (FlashQuestionAnswerId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashQuestionAnswer flashQuestion = db.FlashQuestionAnswer.Find(FlashQuestionAnswerId);
            cursoRigidoViewModel.FlashQuestionId = flashQuestion.FlashQuestionId;
            cursoRigidoViewModel.FlashQuestionAnswerId = FlashQuestionAnswerId;
            cursoRigidoViewModel.Content = flashQuestion.Content;
            if (flashQuestion == null)
            {
                return HttpNotFound();
            }
            return View(cursoRigidoViewModel);
        }

        // POST: FlashQuestions/Delete/5
        public ActionResult DeleteConfirmedFlashQuestionAnswer(int FlashQuestionAnswerId)
        {
            FlashQuestionAnswer flashQuestion = db.FlashQuestionAnswer.Find(FlashQuestionAnswerId);
            db.FlashQuestionAnswer.Remove(flashQuestion);
            db.SaveChanges();
            return RedirectToAction("IndexFlashQuestionAnswer", new { FlashTestId = flashQuestion.FlashQuestionId });
        }

        [Authorize]
        public ActionResult TestUser(int Toco_Id)
        {
            CursoRigidoViewModel cursoRigidoViewModel = new CursoRigidoViewModel();
            cursoRigidoViewModel.Sesion = GetActualUserId().SesionUser;
            List<FlashQuestion> flashQuestion = new List<FlashQuestion>();
            List<FlashQuestionAnswer> flashQuestionAnswer = new List<FlashQuestionAnswer>();
            FlashTest flashTest1 = db.FlashTest.Where(x=>x.ToCo_Id==Toco_Id).FirstOrDefault();
            
            if (flashTest1 != null)
            {
                flashQuestion = db.FlashQuestion.Where(i => i.FlashTestId == flashTest1.FlashTestId).ToList();
                cursoRigidoViewModel.flashTest = flashTest1;
                cursoRigidoViewModel.FlashTestId = flashTest1.FlashTestId;
            }

            foreach(FlashQuestion question in flashQuestion)
            {
                List<FlashQuestionAnswer> fqa = ApplicationDbContext.FlashQuestionAnswer.Where(x => x.FlashQuestionId == question.FlashQuestionId).ToList();
                if (fqa.Count >= 1)
                {
                    foreach (FlashQuestionAnswer answerToAdd in fqa)
                    {
                        flashQuestionAnswer.Add(answerToAdd);
                    }
                }
            }
            List<int> userAnswers = new List<int>();
            for(int cont = 0; cont < flashQuestion.Count; cont++)
            {
                userAnswers.Add(0);
            }
            int module = db.TopicsCourses.Where(x => x.ToCo_Id == Toco_Id).FirstOrDefault().Modu_Id;
            Module mod = db.Modules.Find(module);
            string IdUser = GetActualUserId().Id;

            Enrollment enrollment = db.Enrollments.Where(x => x.Modu_Id== module && x.User_Id==IdUser).FirstOrDefault();
            int enrollmentId = enrollment.Enro_Id;

            List<UserModuleAdvance> userModuleAdvances = db.UserModuleAdvances.Where(x=>x.ToCo_id==Toco_Id && x.Enro_id== enrollmentId).OrderBy(z => z.ToCo_id).ToList();

            cursoRigidoViewModel.ToCo_Id = Toco_Id;
            cursoRigidoViewModel.module = mod;
            cursoRigidoViewModel.flashQuestions = flashQuestion;
            cursoRigidoViewModel.ListFlashQuestionAnswer = flashQuestionAnswer;
            cursoRigidoViewModel.userAnswers = userAnswers;

            if (/*userModuleAdvances.Last().Completed*/1 == 1)
            {
                return View(cursoRigidoViewModel);
            }
            else
            {
                return RedirectToAction("IndexFlashQuestionAnswer", "Devuelve");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult EvaluateFlashTest(CursoRigidoViewModel crvm1)
        {
            CursoRigidoViewModel crvm = crvm1;
            crvm.Sesion = GetActualUserId().SesionUser;
            List<int> answersSelected = crvm.userAnswers;
            List<FlashQuestionAnswer> answers = new List<FlashQuestionAnswer>();
            List<FlashQuestion> flashQuestion = new List<FlashQuestion>();
            FlashTest flashTest = crvm.flashTest;
            if (flashTest != null)
            {
                flashQuestion = db.FlashQuestion.Where(i => i.FlashTestId == flashTest.FlashTestId).ToList();
            }
            foreach (FlashQuestion question in flashQuestion)
            {
                List<FlashQuestionAnswer> fqa = ApplicationDbContext.FlashQuestionAnswer.Where(x => x.FlashQuestionId==question.FlashQuestionId).ToList();
                if (fqa.Count >= 1)
                {
                    foreach (FlashQuestionAnswer answerToAdd in fqa)
                    {
                        answers.Add(answerToAdd);
                    }
                }
            }
            string IdUser = GetActualUserId().Id;
            Enrollment enrollment = db.Enrollments.Where(x => x.Modu_Id == crvm.module.Modu_Id && x.User_Id == IdUser).FirstOrDefault();
            int enrollmentId = enrollment.Enro_Id;
            float percentage = CalculatePercentage(answersSelected, answers);
            if (percentage >= flashTest.AprovedPercentage)
            {
                int tocoId = crvm.flashTest.ToCo_Id;
                int enroId = enrollmentId;
                UserModuleAdvance um = ApplicationDbContext.UserModuleAdvances.Where(x => x.ToCo_id == tocoId && x.Enro_id == enrollmentId).FirstOrDefault();
                if (um != null)
                {
                    UserModuleAdvance uma = new UserModuleAdvance
                    {
                        Enro_id = enroId,
                        ToCo_id = tocoId,
                        Completed = 1
                    };
                    ApplicationDbContext.UserModuleAdvances.Add(uma);
                    ApplicationDbContext.SaveChanges();
                }
            }
            return RedirectToAction("Grades", "User", new { id = crvm.module.Modu_Id});
        }

        public float CalculatePercentage(List<int> userAnswers, List<FlashQuestionAnswer> answers)
        {
            float percentageToReturn = 0;
            float correctAnswers = 0;
            float totalQuestions = userAnswers.Count;
            for(int cont = 0; cont < userAnswers.Count; cont++)
            {
                foreach(FlashQuestionAnswer answer in answers)
                {
                    if (userAnswers[cont] == answer.FlashQuestionAnswerId&&answer.CorrectAnswer==1)
                    {
                        correctAnswers++;
                    }
                }
            }
            percentageToReturn = (correctAnswers / totalQuestions) * 100;
            return percentageToReturn;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
