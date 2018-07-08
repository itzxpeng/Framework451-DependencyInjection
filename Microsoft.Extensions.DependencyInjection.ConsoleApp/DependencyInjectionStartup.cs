using System;

namespace Net451.Microsoft.Extensions.DependencyInjection.ConsoleApp
{
    public class DependencyInjectionStartup
    {
        public static void Initialize(System.Action<IServiceCollection> action)
        {
            IServiceCollection services = new ServiceCollection();
            action(services);
            DIProviderInstance.SetProviderInstance(services);
        }
    }
}
