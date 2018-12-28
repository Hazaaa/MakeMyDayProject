using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MakeMyDayProject.Startup))]
namespace MakeMyDayProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
