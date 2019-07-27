using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TodoApplication.Startup))]
namespace TodoApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
