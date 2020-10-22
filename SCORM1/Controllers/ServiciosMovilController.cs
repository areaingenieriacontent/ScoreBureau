using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Newspaper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SCORM1.Controllers
{
    [EnableCors(origins: "*", headers: "Content-Type, authorization, accept, Origin,", methods: "GET,POST")]

    public class ServiciosMovilController : ApiController
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ServiciosMovilController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public class UserModel
        {
            [Required]
            [StringLength(26, ErrorMessage = "El usuario no puede tener mas de 26 caracteres", MinimumLength = 2)]
            [DataType(DataType.Text)]
            public string User { get; set; }

            [Required]
            [StringLength(26, ErrorMessage = "la contraseña no puede tener mas de 26 caracteres", MinimumLength = 2)]
            [DataType(DataType.Password)]
            public string PassWord { get; set; }
        }
        public class UserResponse
        {
            public string IdUser { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public int Points { get; set; }
        }
        [HttpPost]
        public HttpResponseMessage ValidateUser([FromBody]UserModel models)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser User = ApplicationDbContext.Users.Where(x => x.UserName == models.User).FirstOrDefault();
                if (User != null && ValidationUser.CodeHashOfPassword(models.PassWord, User.PasswordHash) == "Success")
                {
                    UserResponse Res = new UserResponse
                    {
                        IdUser = User.Id,
                        Name = User.FirstName,
                        LastName = User.LastName,
                        Points = User.Point.Sum(x => x.Quantity_Points)
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, Res, "application/json");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "incorrecta");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Modelo no es valido");

        }

        public class ArticleResponse
        {
            public int IdArticle { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public string DescriptionLarge { get; set; }
        }
        [HttpGet]
        public HttpResponseMessage Searchedition(string IdUser)
        {
            try
            {
                if (string.IsNullOrEmpty(IdUser) == false)
                {
                    ApplicationUser UserActual = ApplicationDbContext.Users.Find(IdUser);
                    List<Article> Articles = ApplicationDbContext.Articles.Where(z => z.Section.Edition.CompanyId == UserActual.CompanyId &&
                    z.Section.Edition.Edit_StateEdition == Enum.EDITIONSTATE.Activo).ToList();
                    List<ArticleResponse> ListArticles = new List<ArticleResponse>();
                    foreach (Article item in Articles)
                    {
                        ListArticles.Add(new ArticleResponse
                        {
                            Title = item.Arti_Name,
                            Description = item.Arti_Description,
                            IdArticle = item.Arti_Id,
                            Url = "https://www.aprendeyavanza2.com.co/Resources/SourceSection/" + item.Arti_imagen
                        });
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, ListArticles, "application/json");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Id Vacio");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error en la consulta" + e.Message);
            }

        }


        public class PrizeReques
        {
            public int prizeId { get; set; }
            public string Title { get; set; }
            public int Quantity { get; set; }
            public int PointRequired { get; set; }
            public string Url { get; set; }
        }
        public class LsPrize
        {
            public int prizeId { get; set; }
            public List<PrizeReques> LsPrizes { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage RequesD(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id) == false)
                {
                    ApplicationUser UserActual = ApplicationDbContext.Users.Find(Id);
                    List<Prize> PrizeOfTheCompany = ApplicationDbContext.Prizes.Where(x => x.CompanyId == UserActual.CompanyId).ToList();
                    LsPrize Lsprize = new LsPrize();
                    Lsprize.prizeId = UserActual.Point.Sum(x => x.Quantity_Points);
                    List<PrizeReques> Lis = new List<PrizeReques>();
                    foreach (Prize item in PrizeOfTheCompany)
                    {
                        Lis.Add(new PrizeReques
                        {
                            prizeId = item.Priz_Id,
                            Quantity = item.Priz_Quantity,
                            Title = item.Priz_Name,
                            PointRequired = item.Priz_RequiredPoints,
                            Url = "https://www.aprendeyavanza2.com.co/Content/Images/" + item.Prize_Icon + ".png"
                        });
                    }
                    Lsprize.LsPrizes = Lis;

                    return Request.CreateResponse(HttpStatusCode.OK, Lsprize, "application/json");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Id Vacio");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error en la consulta" + e.Message);
            }
        }


        public class CourseRequest
        {
            public int CourseID { get; set; }
            public string Title { get; set; }
            public int ProgressBar { get; set; }
            public string Url { get; set; }
            public string DateOfClose { get; set; }
        }
        [HttpGet]
        public HttpResponseMessage RequestCourse(string IdC)
        {
            try
            {
                if (string.IsNullOrEmpty(IdC) == false)
                {
                    ApplicationUser UserActual = ApplicationDbContext.Users.Find(IdC);
                    List<Enrollment> n = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActual.Id).ToList();
                    List<CourseRequest> Course = new List<CourseRequest>();
                    foreach (var item in n)
                    {
                        var Progres = ApplicationDbContext.AdvanceCourses.Where(z => z.Enro_Id == item.Enro_Id).FirstOrDefault();
                        if (Progres == null)
                        {
                            Course.Add(new CourseRequest
                            {
                                Title = item.Module.Modu_Name,
                                Url = "http://aprendeyavanza2.com.co/Resources/" + item.Module.Modu_ImageName,
                                ProgressBar = 0,
                                DateOfClose = item.Enro_FinishDateModule.ToString(),
                                CourseID = item.Module.Modu_Id
                            });
                        }
                        else
                        {
                            Course.Add(new CourseRequest
                            {
                                Title = item.Module.Modu_Name,
                                Url = "http://aprendeyavanza2.com.co/Resources/" + item.Module.Modu_ImageName,
                                ProgressBar = (int)Progres.AdCo_ScoreObtanied,
                                DateOfClose = item.Enro_FinishDateModule.ToString(),
                                CourseID = item.Module.Modu_Id
                            });
                        }

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, Course, "application/json");

                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Id Vacio");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error en la consulta" + e.Message);
            }
        }
        public class themeOfCourse
        {
            public string Title { get; set; }
            public int Intents { get; set; }
            public string Date { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage RequestTheme(int Idthe, string UserId)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId) == false && Idthe != 0)
                {
                    var user = ApplicationDbContext.Users.Find(UserId);
                    List<TopicsCourse> T = ApplicationDbContext.TopicsCourses.Where(x => x.Modu_Id == Idthe).ToList();
                    List<themeOfCourse> n = new List<themeOfCourse>();
                    foreach (var item in T)
                    {
                        n.Add(new themeOfCourse { Title = item.ToCo_Name, Intents = item.ToCo_Attempt - ApplicationDbContext.Attempts.Where(z => z.BankQuestion.ToCo_Id == item.ToCo_Id && z.UserId == UserId).Count() });
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, n, "application/json");

                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Id Vacio");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Error en la consulta" + e.Message);
            }
        }

    }
}