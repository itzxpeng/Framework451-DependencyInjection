using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection.ConsoleApp
{
    public class DIProviderInstance
    {
        private static IServiceCollection services;

        private static Lazy<IServiceProvider> lazyInstance = new Lazy<IServiceProvider>(() => services.BuildServiceProvider());

        internal static void SetProviderInstance(IServiceCollection serviceCollection)
        {
            services = serviceCollection;
        }

        public static IServiceProvider ProviderInstance => lazyInstance.Value;
    }
}
