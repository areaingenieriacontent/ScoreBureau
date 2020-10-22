using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using SCORM1.Models.ViewModel;
using System.Web.Http.Cors;

namespace SCORM1.Controllers
{
    [EnableCors(origins: "*", headers: "Content-Type, authorization, accept, Origin,", methods: "GET,POST")]

    public class WebServiceController : ApiController
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


        public WebServiceController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        [BasicAuthentication]
        public HttpResponseMessage Get(int idGame)
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            if (username.ToLower() != null)
            {
                ApplicationUser UserToVerify = UserManager.FindByName(username);
                List<PointsObtainedForUser> pointsToSend = ApplicationDbContext.PointsObtainedForUser.Where(w => w.IdUser == UserToVerify.Id && w.IdGame == idGame).Distinct().ToList();
                List<Listlevel> x = new List<Listlevel>();

                foreach (var item in pointsToSend)
                {
                    List<AttemptsOfLevel> y = new List<AttemptsOfLevel>();
                    List<PointsObtainedForUser> pointsForLevel = pointsToSend.Where(z => z.IdLevelCode == item.IdLevelCode).ToList();
                    foreach (var point in pointsForLevel)
                    {
                        y.Add(new AttemptsOfLevel { date=(DateTime)point.Date, PointsAssigned= point.PointsAssigned});
                    }
                    if (x.Exists(t => t.Level == item.IdLevelCode))
                    {

                    }
                    else
                    {
                        x.Add(new Listlevel { Level = item.IdLevelCode, AttemptsOfLevel = y });
                    }

                }
                return Request.CreateResponse(HttpStatusCode.OK, x);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]PointsObtainedForUser points)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser UserToVerify = UserManager.FindByName(points.IdUser);

                if (UserToVerify!=null)
                {
                    PointsObtainedForUser PointsAssignedToTheUser = new PointsObtainedForUser
                    {
                        Date = DateTime.Now,
                        IdGame = points.IdGame,
                        IdLevelCode = points.IdLevelCode,
                        IdUser = UserToVerify.Id
                        
                    };
                    int VerifyIfhavePoint = ApplicationDbContext.PointsObtainedForUser.Where(x => x.IdUser == UserToVerify.Id && x.IdGame == PointsAssignedToTheUser.IdGame).Count(x => x.IdLevelCode == PointsAssignedToTheUser.IdLevelCode);

                    if (VerifyIfhavePoint <= 0)
                    {
                        PointsAssignedToTheUser.PointsAssigned = points.PointsAssigned;
                    }
                    else
                    {
                        PointsAssignedToTheUser.PointsAssigned = 0;
                    }
                    ApplicationDbContext.PointsObtainedForUser.Add(PointsAssignedToTheUser);
                    ApplicationDbContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
       
    }
}
