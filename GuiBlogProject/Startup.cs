using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GuiBlogProject.Startup))]
namespace GuiBlogProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
