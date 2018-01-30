using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.ConsoleApp.Example
{
    public class Dao : IDao
    {
        private const string Content = "This is DI sample";
        public string GetWriter()
        {
            return Content;
        }
    }
}
