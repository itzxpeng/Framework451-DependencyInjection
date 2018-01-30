using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection.SystemWeb;
using Microsoft.Extensions.DependencyInjection;

[assembly: OwinStartup(typeof(DependencyInjection.Mvc.Example.Startup))]

namespace DependencyInjection.Mvc.Example
{
    public class Startup : DependencyInjectionStartup
    {
        public Startup()
        {
            base.Initialize();
        }

        public void Configuration(IAppBuilder app)
        {
            app.Use<DependencyInjectionMiddleware>(base.services);
        }

        protected override void ServiceConfiguration(IServiceCollection services)
        {
            services.AddSingleton<IDao, Dao>();
        }
    }
}
