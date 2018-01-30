using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependencyInjection.Mvc.Example
{
    public class Dao : IDao
    {
        private const string Content = "This is a DI example";

        public string Println()
        {
            return Content;
        }
    }
}