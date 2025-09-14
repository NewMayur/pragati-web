using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pragati.Web.Startup))]
namespace Pragati.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
