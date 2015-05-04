using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkingWithImages.Startup))]
namespace WorkingWithImages
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
