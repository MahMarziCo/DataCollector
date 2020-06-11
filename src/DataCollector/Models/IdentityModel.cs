using DataAccess.Logic;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class ApplicationUser : IdentityUser
    {
    }
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string dbConnection)
            : base(dbConnection)
        {
        }

    }
}