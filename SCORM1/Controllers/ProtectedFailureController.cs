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
using System.Security.Cryptography.X509Certificates;
using SCORM1.Models.RigidCourse;
using iTextSharp.text.pdf.events;

namespace SCORM1.Controllers
{
    public class ProtectedFailureController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected AdminTrainingController adminTrainingController { get; set; }
        protected UserController userController { get; set; }

        public ProtectedFailureController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            userController = new UserController();
        }
        // GET: ProtectedFailure
        public ActionResult Index()
        {
            return View();
        }

        #region Protected failure admin functions

        //Add the created protected failure and its categories question bank
        [HttpPost]
        [Authorize]
        public ActionResult CreateProtectedFailureTest(AdminProtectedFailure model)
        {
            AdminProtectedFailure pft = null;
            int bankQuestionsAdded = 0;
            //if the model passed throug parameter is valid
            if (ModelState.IsValid)
            {
                pft = model;
                ProtectedFailureTest testToAdd = pft.protectedFailureTest;
                try
                {
                    //add the protected failure received to the database
                    ApplicationDbContext.ProtectedFailureTests.Add(testToAdd);
                    ApplicationDbContext.SaveChanges();
                }
                catch
                {
                    //if it can't be added, return to the view
                    TempData["Info"] = "No se pudo crear la evaluación de falla protegida";
                    return (adminTrainingController.Grades(model.protectedFailureTest.Modu_Id));
                }

                //cicles throug the categories to add their respective question banks
                for (int cont = 0; cont < pft.categoryList.Count; cont++)
                {
                    if (pft.bankToCreateList[cont])
                    {
                        CategoryQuestionBank cqb = new CategoryQuestionBank
                        {
                            Cate_Id = pft.categoryList[cont].Cate_Id,
                            Modu_Id = pft.protectedFailureTest.Modu_Id,
                            EvaluatedQuestionQuantity = pft.questionQuantityList[cont],
                            AprovedCategoryPercentage = pft.approvedPercentageList[cont]
                        };
                        ApplicationDbContext.CategoryQuestionsBanks.Add(cqb);
                    }
                }
                ApplicationDbContext.SaveChanges();
            }
            if (pft == null)
            {
                TempData["Info"] = "No se pudo crear la evaluación de falla protegida";
            }
            else
            {
                TempData["Info"] = "Evaluación de falla protegida creada correctamente, se agregaron " + bankQuestionsAdded + " categorias";
            }
            return (adminTrainingController.Grades(model.protectedFailureTest.Modu_Id));
        }

        [Authorize]
        public ActionResult CreateProtectedFailureTest(int id)
        {
            AdminProtectedFailure apf = new AdminProtectedFailure
            {
                modu_id = id
            };
            return View();
        }

        //Creates the Protected failure test with the view given attributes
        [Authorize]
        public ActionResult AddQuestionsToProtectedFailureTest(int modu_Id)
        {
            AdminProtectedFailure apf = null;
            if (ApplicationDbContext.ProtectedFailureTests.Find(modu_Id) != null)
            {
                apf = new AdminProtectedFailure
                {
                    protectedFailureTest = ApplicationDbContext.ProtectedFailureTests.Find(modu_Id),
                    categoryList = ApplicationDbContext.Categories.Where(x => x.ToCo_Id == x.TopicCourse.ToCo_Id && x.TopicCourse.Modu_Id == modu_Id).ToList()
                };
                apf.bankToCreateList = new List<bool>(apf.categoryList.Count);
                apf.questionQuantityList = new List<int>(apf.categoryList.Count);
                apf.approvedPercentageList = new List<float>(apf.categoryList.Count);
            }
            //To-Do customized view
            return View(apf);
        }
        #endregion

        #region ProtectedFailure user functions

        [Authorize]
        public ActionResult TestUser(int id)
        {
            //To-Do evaluar si el usuario tiene intentos disponibles

            Module mod = ApplicationDbContext.Modules.Find(id);
            string actualUserId = GetActualUserId().Id;
            bool hasSession = false;
            Enrollment matricula = ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == mod.Modu_Id && x.User_Id == actualUserId).FirstOrDefault();
            List<ProtectedFailureResults> userResults = ApplicationDbContext.ProtectedFailureResults.Where(x => x.Enro_id == matricula.Enro_Id).ToList();
            ProtectedFailureTest pft = ApplicationDbContext.ProtectedFailureTests.Where(x => x.Modu_Id == id).FirstOrDefault();
            List<TopicsCourse> topicsList = ApplicationDbContext.TopicsCourses.Where(x => x.Modu_Id == id).ToList();
            List<Category> listOfCategories = new List<Category>();
            List<CategoryQuestionBank> bankList = ApplicationDbContext.CategoryQuestionsBanks.Where(x => x.Modu_Id == id).ToList();
            List<ProtectedFailureMultiChoice> multipleChoiceQuestions = ApplicationDbContext.ProtectedFailureMultiChoices.Where(x => x.Modu_Id == id).ToList();
            multipleChoiceQuestions = RamdomizeData.Shuffle(multipleChoiceQuestions);//Randomiza las preguntas del banco de preguntas
            List<ProtectedFailureMultiChoice> multipleChoiceQuestionsForTest = new List<ProtectedFailureMultiChoice>();
            List<ProtectedFailureAnswer> multiChoiceQuestionsAnswers = new List<ProtectedFailureAnswer>();
            List<ProtectedFailureMultiChoiceAnswer> userAnswers = new List<ProtectedFailureMultiChoiceAnswer>();
            if (userResults == null || userResults.Count == 0)
            {
                foreach (TopicsCourse topic in topicsList)
                {
                    List<Category> cate = ApplicationDbContext.Categories.Where(x => x.ToCo_Id == topic.ToCo_Id).ToList();
                    for (int cont = 0; cont < cate.Count; cont++)
                    {
                        listOfCategories.Add(cate[cont]);
                    }
                }
                //Se seleccionan las preguntas que se le van a presentar al usuario
                foreach (CategoryQuestionBank bank in bankList)
                {
                    int questionsAdded = 0;
                    for (int k = 0; k < multipleChoiceQuestions.Count; k++)
                    {
                        //si la pregunta a evaluar concide con el banco de categorias actual, pues se agrega a las preguntas del usuario
                        //se repite el procedimiento hasta completar las preguntas que requiere el banco
                        if (multipleChoiceQuestions[k].Category_Id == bank.Cate_Id&&questionsAdded<bank.EvaluatedQuestionQuantity)
                        {
                            multipleChoiceQuestionsForTest.Add(multipleChoiceQuestions[k]);
                            questionsAdded++;
                            if (questionsAdded >= bank.EvaluatedQuestionQuantity)
                            {
                                k = multipleChoiceQuestions.Count;
                            }
                        }
                    }
                }
            }
            else
            {
                //To-Do load user data
                hasSession = true;
                userAnswers = ApplicationDbContext.ProtectedFailureMultiChoiceAnswers.Where(x => x.UserId == actualUserId).ToList();
                //Se seleccionan las preguntas que se le van a presentar al usuario
                foreach (CategoryQuestionBank bank in bankList)
                {
                    
                    int questionsAdded = 0;
                    //se recuperan la cantidad de preguntas que el usuario ya contesto
                    for(int a = 0; a < userResults.Count; a++)
                    {
                        if (userResults[a].Cate_Id == bank.Cate_Id)
                        {
                            questionsAdded = userResults[a].correctAnswersQuantity;
                        }
                    }
                    for (int k = 0; k < multipleChoiceQuestions.Count; k++)
                    {
                        //si la pregunta a evaluar concide con el banco de categorias actual, pues se agrega a las preguntas del usuario
                        //se repite el procedimiento hasta completar las preguntas que requiere el banco
                        if (multipleChoiceQuestions[k].Category_Id == bank.Cate_Id && questionsAdded < bank.EvaluatedQuestionQuantity)
                        {
                            for(int j = 0; j < userAnswers.Count; j++)
                            {
                                if (multipleChoiceQuestions[k].QuestionId == userAnswers[j].ProtectedFailureAnswer.QuestionId)
                                {
                                    userAnswers.RemoveAt(j);
                                    j = userAnswers.Count;
                                }else if(multipleChoiceQuestions[k].QuestionId != userAnswers[j].ProtectedFailureAnswer.QuestionId
                                    && j == userAnswers.Count - 1)
                                {
                                    multipleChoiceQuestionsForTest.Add(multipleChoiceQuestions[k]);
                                    questionsAdded++;
                                    if (questionsAdded >= bank.EvaluatedQuestionQuantity)
                                    {
                                        k = multipleChoiceQuestions.Count;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Se cargan las respuestas para las preguntas
            foreach (ProtectedFailureMultiChoice question in multipleChoiceQuestionsForTest)
            {
                List<ProtectedFailureAnswer> answers = ApplicationDbContext.ProtectedFailureAnswer.Where(x => x.QuestionId == question.QuestionId).ToList();
                for (int cont = 0; cont < answers.Count; cont++)
                {
                    multiChoiceQuestionsAnswers.Add(answers[cont]);
                }
            }
            ProtectedFailureTestViewModel protectedFailureTestVM = new ProtectedFailureTestViewModel {
                actualModule = mod,
                protectedFailureTest = pft,
                listOfCategories = listOfCategories,
                questionBanks = bankList,
                questionsList = multipleChoiceQuestionsForTest,
                answersList = multiChoiceQuestionsAnswers,
                resultList = userResults,
                isRecoverySession=hasSession,
                enrollment = matricula
            };
            protectedFailureTestVM.Sesion = GetActualUserId().SesionUser;
            return View(protectedFailureTestVM);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EvaluateProtectedFailureTest(ProtectedFailureTestViewModel pftvm)
        {
            ProtectedFailureTestViewModel model = pftvm;
            model.Sesion = GetActualUserId().SesionUser;
            string actualUserId = GetActualUserId().Id;
            float cateResult = 0;
            float userResult = 0;
            //Si es una nueva sesión se ejecuta esta condición
            if (!model.isRecoverySession)
            {
                //Se recorre el arreglo de los bancos de pregunta del test de falla protegida
                foreach (CategoryQuestionBank bank in model.questionBanks)
                {
                    userResult = 0;//se mprepara la variable para contar las respuestas del usuario
                    cateResult = bank.EvaluatedQuestionQuantity;//se prepara la variable del total de preguntas para el banco actual
                    //Se recorre la lista de respuestas
                    for (int cont = 0; cont < model.answersList.Count; cont++)
                    {
                        //se recorre la lista de respuestas del usuario
                        for (int cont2 = 0; cont2 < model.selectedAnswers.Count; cont2++)
                        {
                            //Si la respuesta del usuario coincide con una respuesta de la lista de respuestas y ademas esta es correcta
                            //se suma a la variable de respuestas correctas del usuario
                            if (model.selectedAnswers[cont2] == model.answersList[cont].answerId && model.answersList[cont].isCorrectQuestion
                                && model.answersList[cont].ProtectedFailureMultiChoice.Category_Id==bank.Cate_Id)
                            {
                                userResult += 1;
                                int actualAnswerId = model.selectedAnswers[cont2];
                                ProtectedFailureMultiChoiceAnswer answerToSave = new ProtectedFailureMultiChoiceAnswer
                                {
                                    UserId = actualUserId,
                                    AnswerId = actualAnswerId
                                };
                                model.selectedAnswers.RemoveAt(cont2);
                                //To-Do sorround in try-catch
                                ApplicationDbContext.ProtectedFailureMultiChoiceAnswers.Add(answerToSave);
                                ApplicationDbContext.SaveChanges();
                                cont2 = model.selectedAnswers.Count;
                                //To-Do remove answer element evaluated
                            }
                        }
                    }
                    //se preparan los datos que se van a guardar en la base de datos
                    ProtectedFailureResults result = new ProtectedFailureResults
                    {
                        Enro_id = model.enrollment.Enro_Id,
                        Cate_Id = bank.Cate_Id,
                        correctAnswersQuantity = (int)userResult,
                        Score = (userResult / cateResult) * 100
                    };
                    ApplicationDbContext.ProtectedFailureResults.Add(result);
                    ApplicationDbContext.SaveChanges();
                }
            }
            //si es una sesión recuperada, se evalua de la siguiente manera
            else
            {
                List<ProtectedFailureResults> oldResults = ApplicationDbContext.ProtectedFailureResults.Where
                    (x => x.Enro_id == model.enrollment.Enro_Id).ToList();
                foreach(CategoryQuestionBank bank in model.questionBanks)
                {
                    userResult = 0;
                    cateResult = bank.EvaluatedQuestionQuantity;
                    ProtectedFailureResults resultToModify = new ProtectedFailureResults();
                    for(int cont = 0; cont < oldResults.Count; cont++)
                    {
                        if (oldResults[cont].Cate_Id == bank.Cate_Id)
                        {
                            userResult = oldResults[cont].correctAnswersQuantity;
                            cont = oldResults.Count;
                        }
                    }
                    if (cateResult > userResult)
                    {
                        for(int cont = 0; cont < model.answersList.Count; cont++)
                        {
                            for (int cont1 = 0; cont1 < model.selectedAnswers.Count; cont1++)
                            {
                                if (model.selectedAnswers[cont1] == model.answersList[cont].answerId&&model.answersList[cont].isCorrectQuestion
                                && model.answersList[cont].ProtectedFailureMultiChoice.Category_Id == bank.Cate_Id)
                                {
                                    userResult += 1;
                                    int actualAnswerId = model.selectedAnswers[cont1];
                                    ProtectedFailureMultiChoiceAnswer answerToSave = new ProtectedFailureMultiChoiceAnswer
                                    {
                                        UserId = actualUserId,
                                        AnswerId = actualAnswerId
                                    };
                                    model.selectedAnswers.RemoveAt(cont1);
                                    //To-Do sorround in try-catch
                                    ApplicationDbContext.ProtectedFailureMultiChoiceAnswers.Add(answerToSave);
                                    ApplicationDbContext.SaveChanges();
                                    cont1 = model.selectedAnswers.Count;
                                    //To-Do remove answer element evaluated
                                }
                            }
                        }
                        resultToModify = ApplicationDbContext.ProtectedFailureResults.Where
                            (x => x.Enro_id == model.enrollment.Enro_Id && x.Cate_Id == bank.Cate_Id).FirstOrDefault();
                        if (resultToModify == null || resultToModify.Enro_id == 0)
                        {
                            //se preparan los datos que se van a guardar en la base de datos
                            resultToModify = new ProtectedFailureResults
                            {
                                Enro_id = model.enrollment.Enro_Id,
                                Cate_Id = bank.Cate_Id,
                                correctAnswersQuantity = (int)userResult,
                                Score = (userResult / cateResult) * 100
                            };
                            ApplicationDbContext.ProtectedFailureResults.Add(resultToModify);
                        }
                        else
                        {
                            resultToModify.correctAnswersQuantity = (int)userResult;
                            resultToModify.Score = (userResult / cateResult) * 100;
                        }
                        ApplicationDbContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Grades", "User", new { id = model.protectedFailureTest.Modu_Id });
        }

        //Muestra el resultado de la evaluación de falla protegida del usuario
        [Authorize]
        public ActionResult UserResults(int id)
        {
            //To-Do
            Module mod = ApplicationDbContext.Modules.Find(id);
            string actualUserId = GetActualUserId().Id;
            ProtectedFailureTest pft = ApplicationDbContext.ProtectedFailureTests.Where(x => x.Modu_Id == id).FirstOrDefault();
            Enrollment matricula = ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == mod.Modu_Id && x.User_Id == actualUserId).FirstOrDefault();
            List<ProtectedFailureResults> userResults = ApplicationDbContext.ProtectedFailureResults.Where(x => x.Enro_id == matricula.Enro_Id).ToList();

            ProtectedFailureTestViewModel protectedFailureTestVM = new ProtectedFailureTestViewModel
            {
                actualModule = mod,
                resultList = userResults,
                enrollment = matricula,
                protectedFailureTest = pft
            };
            protectedFailureTestVM.Sesion = GetActualUserId().SesionUser;

            return View(protectedFailureTestVM);
        }
        #endregion

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

    }

    public static class RamdomizeData
    {
        //-------------------------------------------------------------------------------
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            List<T> listToReturn = list;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = listToReturn[k];
                listToReturn[k] = listToReturn[n];
                listToReturn[n] = value;
            }
            return listToReturn;
        }

        private static Random rng = new Random();
        //--------------------------------------------------------------------------------
    }
}