using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net451.Microsoft.Extensions.DependencyInjection.SystemWeb
{
    public abstract class DependencyInjectionStartup
    {
        protected IServiceCollection services;

        protected void Initialize()
        {
            services = new ServiceCollection();
            ServiceConfiguration(services);

        }

        protected abstract void ServiceConfiguration(IServiceCollection services);
    }
}
