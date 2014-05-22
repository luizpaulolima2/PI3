using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PI3.Startup))]
namespace PI3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
