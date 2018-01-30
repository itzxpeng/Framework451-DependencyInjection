using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Internal
{
    internal delegate object ObjectFactory(IServiceProvider serviceProvider, object[] arguments);
}
