using System;
using System.Threading.Tasks;
using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(B2b.Web.v4.Startup))]

namespace B2b.Web.v4
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
