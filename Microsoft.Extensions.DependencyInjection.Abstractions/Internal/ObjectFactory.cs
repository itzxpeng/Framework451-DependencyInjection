using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net451.Microsoft.Extensions.Internal
{
    internal delegate object ObjectFactory(IServiceProvider serviceProvider, object[] arguments);
}
