using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Net451.Microsoft.Extensions.DependencyInjection.SystemWeb
{
    public class ServiceResolve
    {
        internal IServiceProvider provider;
        internal IServiceCollection services;

        public static ServiceResolve ResolveHttpFactory(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext), "Argument can not null");
            }

            if (!httpContext.Items.Contains("owin.Environment"))
            {
                throw new ArgumentException("Can not find Owin environment", "owin.Environment");
            }

            IDictionary<string, object> environment = httpContext.Items["owin.Environment"] as IDictionary<string, object>;

            return ResolveDictionaryFactory(environment);
        }

        public static ServiceResolve ResolveOwinFactory(IOwinContext owinContext)
        {
            if (owinContext == null)
            {
                throw new ArgumentNullException(nameof(owinContext), "Argument can not null");
            }

            IDictionary<string, object> environment = owinContext.Environment as IDictionary<string, object>;

            return ResolveDictionaryFactory(environment);
        }

        private static ServiceResolve ResolveDictionaryFactory(IDictionary<string, object> environment)
        {
            ServiceResolve result = new ServiceResolve();

            if (environment == null)
            {
                throw new ArgumentNullException("owin.Environment", "Owin evnironment can not null");
            }

            if (!environment.ContainsKey(RequestKey.IServiceProvider.ToString()) && !environment.ContainsKey(RequestKey.IServiceCollection.ToString()))
            {
                throw new InvalidOperationException("Can not get ServiceProvider or ServiceCollection");
            }

            object providerObj = null;
            object servicesObj = null;
            if (!environment.TryGetValue(RequestKey.IServiceProvider.ToString(), out providerObj))
            {
                if (environment.TryGetValue(RequestKey.IServiceCollection.ToString(), out servicesObj))
                {
                    result.services = servicesObj as IServiceCollection;
                }
            }
            else
            {
                result.provider = providerObj as IServiceProvider;
            }

            return result;
        }

        public object GetRequiredService(Type serviceType)
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if (serviceProvider != null)
            {
                return serviceProvider.GetRequiredService(serviceType);
            }
            else
            {
                return null;
            }
        }

        public T GetRequiredService<T>() where T: class
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if(serviceProvider != null)
            {
                return serviceProvider.GetRequiredService<T>();
            }else
            {
                return null;
            }
        }

        public T GetService<T>() where T : class
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if (serviceProvider != null)
            {
                return serviceProvider.GetService<T>();
            }
            else
            {
                return null;
            }
        }

        public object GetService(Type serviceType)
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if (serviceProvider != null)
            {
                return serviceProvider.GetService(serviceType);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if (serviceProvider != null)
            {
                return serviceProvider.GetServices(serviceType);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<T> GetServices<T>() where T : class
        {
            IServiceProvider serviceProvider = this.provider ?? this.services.BuildServiceProvider();
            if (serviceProvider != null)
            {
                return serviceProvider.GetServices<T>();
            }
            else
            {
                return null;
            }
        }
    }
}
