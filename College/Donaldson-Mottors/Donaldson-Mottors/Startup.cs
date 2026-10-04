using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Donaldson_Mottors.Startup))]
namespace Donaldson_Mottors
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
