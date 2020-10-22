using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SCORM11.Startup))]
namespace SCORM11
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
