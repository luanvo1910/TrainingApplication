using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrainingApplication.Startup))]
namespace TrainingApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
