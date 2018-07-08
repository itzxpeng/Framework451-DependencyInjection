using System;
using Net451.Microsoft.Extensions.DependencyInjection;
using Net451.Microsoft.Extensions.DependencyInjection.ConsoleApp;

namespace DependencyInjection.ConsoleApp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Init the DI container
            DependencyInjectionStartup.Initialize(services =>
            {
                services.AddSingleton<IDao, Dao>();
            });

            //Get instnace by service type from DI container
            var dao = DIProviderInstance.ProviderInstance.GetRequiredService<IDao>();

            Console.WriteLine(dao.GetWriter());
            Console.ReadKey();
        }
    }
}
