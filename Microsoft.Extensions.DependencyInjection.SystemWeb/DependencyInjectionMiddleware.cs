using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Web;

namespace Net451.Microsoft.Extensions.DependencyInjection.SystemWeb
{
    public class DependencyInjectionMiddleware : OwinMiddleware
    {
        private readonly IServiceCollection serviceCollection;

        public DependencyInjectionMiddleware(OwinMiddleware next, IServiceCollection services) : base(next)
        {
            serviceCollection = services;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var provider = serviceCollection.BuildServiceProvider();
            
            context.Request.Environment[RequestKey.IServiceProvider.ToString()] = provider;
            context.Request.Environment[RequestKey.IServiceCollection.ToString()] = serviceCollection;

            await this.Next.Invoke(context);
        }
    }
}
