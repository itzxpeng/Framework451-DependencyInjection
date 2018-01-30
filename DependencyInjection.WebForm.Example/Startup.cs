using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection.SystemWeb;
using Microsoft.Extensions.DependencyInjection;

[assembly: OwinStartup(typeof(DependencyInjection.WebForm.Example.Startup))]

namespace DependencyInjection.WebForm.Example
{
    public class Startup : DependencyInjectionStartup
    {
        public Startup()
        {
            base.Initialize();
        }

        protected override void ServiceConfiguration(IServiceCollection services)
        {
            services.AddSingleton<IDao, Dao>();
        }

        public void Configuration(IAppBuilder app)
        {
            app.Use<DependencyInjectionMiddleware>(base.services);
        }
    }
}
