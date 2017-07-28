using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(POTracker.Startup))]
namespace POTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
