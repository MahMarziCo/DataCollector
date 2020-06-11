using DataAccess.Logic;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollector
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigurationAuth(app);
        }
    }
}