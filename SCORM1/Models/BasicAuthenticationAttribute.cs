using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SCORM1.Models
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


        public BasicAuthenticationAttribute()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            }
            else
            {
                string authenticationtoken = actionContext.Request.Headers.Authorization.Parameter;
                string decodeAuthenticationtoken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationtoken));
                string[] usernamePassordArray = decodeAuthenticationtoken.Split(':');
                string username = usernamePassordArray[0];
                string password = usernamePassordArray[1];
                ApplicationUser User = new ApplicationUser();
                try
                {
                     User = UserManager.FindByName(username);

                }
                catch (Exception)
                {

                    throw;
                }
                if (User!=null && ValidationUser.CodeHashOfPassword(password, User.PasswordHash) == "Success")
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}