using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataCollector.Filters
{
    public class UserRoleFilterAttribute : ActionFilterAttribute
    {
        public string RoleIds { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool access = false;
            foreach (var role in RoleIds.Split(','))
            {
                if (role == "SYSADMIN")
                    access = access || filterContext.RequestContext.HttpContext.User.Identity.Name == "SYSADMIN" ? true : false;
                else
                    access = access || filterContext.RequestContext.HttpContext.User.IsInRole(role);
            }
            if (!access)
            {
                filterContext.Result = new RedirectResult("/Account/UserAccessDeny");
            }
        }
    }
}