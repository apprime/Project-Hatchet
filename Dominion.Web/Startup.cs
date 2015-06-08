using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dominion.Web.Startup))]
namespace Dominion.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
