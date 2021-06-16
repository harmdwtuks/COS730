using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InteractionLayer.Startup))]
namespace InteractionLayer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
