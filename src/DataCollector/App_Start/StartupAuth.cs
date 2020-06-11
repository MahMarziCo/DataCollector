using DataAccess.Logic;
using DataCollector.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Linq;
using Mah.Common.Encrypt;

namespace DataCollector
{
    public partial class Startup
    {
        public static Func<UserManager<ApplicationUser>> UserManagerFactory { get; private set; }
        public static Func<RoleManager<IdentityRole>> RoleManagerFactory { get; private set; }

        public void ConfigurationAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/login")
            });

           
            Cryptor cryptor = new Cryptor("M@H&M@RZ!K3Y");
            string dbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["LogicDb"].ConnectionString);

            UserManagerFactory = () =>
            {
                var usermanager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(new ApplicationDbContext(dbConnection)));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<ApplicationUser>(usermanager)
                {
                    AllowOnlyAlphanumericUserNames = true
                };

                usermanager.PasswordValidator = new PasswordValidator()
                {
                    RequiredLength = 2
                };

                return usermanager;
            };

            RoleManagerFactory = () =>
            {
                var rolemanager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext(dbConnection)));

                return rolemanager;
            };

            #region set Defualts

            var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext(dbConnection)));
            if (!roleManager.RoleExists("SYSADMIN"))
            {
                IdentityRole role = new IdentityRole("SYSADMIN");
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("ADMIN"))
            {
                IdentityRole role = new IdentityRole("ADMIN");
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("USER"))
            {
                IdentityRole role = new IdentityRole("USER");
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("SUPERVISOR"))
            {
                IdentityRole role = new IdentityRole("SUPERVISOR");
                roleManager.Create(role);
            }

            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(new ApplicationDbContext(dbConnection)));

            if (!userManager.Users.Any(u => u.UserName == "SYSADMIN"))
            {
                ApplicationUser user = new ApplicationUser() { UserName = "SYSADMIN" };

                IdentityResult result = userManager.Create(user, "24511367");
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "SYSADMIN");
                    userManager.AddToRole(user.Id, "ADMIN");
                }
            }

            #endregion
        }

        
    }
}