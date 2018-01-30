using Microsoft.Extensions.DependencyInjection.SystemWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DependencyInjection.WebForm.Example
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var dao = ServiceResolve.ResolveHttpFactory(HttpContext.Current).GetRequiredService<IDao>();
            Response.Write(dao.Println());
        }
    }
}