using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Winter2022_PassionProject_N01519420.Startup))]
namespace Winter2022_PassionProject_N01519420
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
