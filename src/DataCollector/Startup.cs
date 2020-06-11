using Owin;

namespace Mah.DataCollector.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigurationAuth(app);
        }
    }
}