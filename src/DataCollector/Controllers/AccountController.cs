using DataCollector.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Kendo.Mvc.Extensions;
using DataCollector.Filters;
using Mah.DataCollector.Entity.Entities;
using DataAccess.Logic;
using System.Globalization;
using Mah.Common.Encrypt;
using DataCollector.Models.Map;

namespace DataCollector.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private string _GdbConnection = "";
        private SettingBL _SettingBL;
        private UpdateLogBL _UpdateLogBL;
        private UserLocationBL _UserLocationBL;
        public AccountController(SettingBL settingBL, UpdateLogBL updateLogBL, UserLocationBL userLocationBL
            , Cryptor cryptor)
            : this(Startup.UserManagerFactory.Invoke())
        {
            _UserLocationBL = userLocationBL;
            _UpdateLogBL = updateLogBL;
            _SettingBL = settingBL;

            _GdbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["gdbConn"].ConnectionString);

        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _UserManager = userManager;
        }


        private readonly UserManager<ApplicationUser> _UserManager;


        protected override void Dispose(bool disposing)
        {
            if (disposing && _UserManager != null)
            {
                _UserManager.Dispose();
            }
            base.Dispose(disposing);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Account
        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new AccountModel
            {
                ReturnUrl = returnUrl
            };

            try
            {
                _SettingBL.ConfigSpatialLogic();
            }
            catch (Exception ex)
            {

            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(AccountModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }
            if (model.UserName.ToUpper() != "SYSADMIN")
            {
                DateTime? dateOf = _SettingBL.getSettingAsDate(SettingBL.SettingParameters.ExpireDateTime, "SYSTEM");
                if (dateOf != null)
                {
                    DateTime ExpireDate = dateOf ?? DateTime.Now.AddDays(2);
                    if (ExpireDate < DateTime.Now)
                    {
                        ModelState.AddModelError("", "خطای 001 اتفاق افتاده است.با مدیریت تماس بگیرید");
                        model.Password = "";
                        return View(model);
                    }
                }

                string conn = _SettingBL.getSettingAsString(SettingBL.SettingParameters.LicenseText, "SYSTEM");
                string gdbConn = _GdbConnection;// System.Configuration.ConfigurationManager.ConnectionStrings["gdbConn"].ConnectionString;
                if (conn != gdbConn)
                {
                    ModelState.AddModelError("", "خطای 002 اتفاق افتاده است.با مدیریت تماس بگیرید");
                    model.Password = "";
                    return View(model);
                }
            }
            var user = await _UserManager.FindAsync(model.UserName, model.Password);
            _UpdateLogBL.Log(model.UserName.ToUpper(), "", -1, "User Login", "");
            if (user != null)
            {
                var identity = await _UserManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = model.IsPersistent }, identity);


                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
            return View(model);
        }


        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword(string UserName)
        {
            ChangePasswordModel model = new ChangePasswordModel() { UserName = UserName };
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePasswordControl()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePasswordControl(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView();
            }

            var user = _UserManager.Find(model.UserName, model.OldPassword);

            if (user != null)
            {
                IdentityResult result = _UserManager.ChangePassword(user.Id, model.OldPassword, model.NewPassword);
                _UpdateLogBL.Log(User.Identity.Name, "", -1, "Change Password", model.UserName);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "رمز قدیم اشتباه است");
            }
            return PartialView();
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        public ActionResult Register()
        {
            return PartialView();
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName.ToUpper()
            };

            var result = await _UserManager.CreateAsync(user, model.Password);
            _UpdateLogBL.Log(User.Identity.Name, "", -1, "Create User", model.UserName.ToUpper() + "\t Role:" + model.UserRole);
            if (result.Succeeded)
            {
                _UserManager.AddToRole(user.Id, model.UserRole);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return PartialView(model);
            }

            return PartialView();
        }
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "map");
            }

            return returnUrl;
        }

        private ApplicationUser GetUserByName(string userName)
        {
            return _UserManager.Users.Where(a => a.UserName.ToUpper() == userName.ToUpper()).First();
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        [HttpGet]
        public ActionResult UnRegister(string UserName)
        {
            try
            {
                ApplicationUser user = GetUserByName(UserName);
                _UserManager.Delete(user);
                _UpdateLogBL.Log(User.Identity.Name, "", -1, "Delete User", UserName);
            }
            catch { }
            return Json(new { Status = "SUCCESS" }, JsonRequestBehavior.AllowGet);
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        public ActionResult ResetPassword(string UserName)
        {
            RegisterModel model = new RegisterModel() { UserName = UserName, UserRole = "ss" };
            return PartialView(model);
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                //List<ModelError> errors = new List<ModelError>();
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        errors.Add(error);
                //    }
                //}
                return PartialView(model);
            }
            try
            {
                ApplicationUser user = GetUserByName(model.UserName);
                _UserManager.RemovePassword(user.Id);
                _UserManager.AddPassword(user.Id, model.Password);
                _UpdateLogBL.Log(User.Identity.Name, "", -1, "Reset Password", model.UserName);
            }
            catch { ModelState.AddModelError("", "خطایی در انجام فرآیند رخ داده است"); }
            return PartialView();
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        public ActionResult EditUser(string UserName)
        {
            RegisterModel model = new RegisterModel() { UserName = UserName, Password = "123", ConfirmPassword = "123" };
            return PartialView(model);
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> EditUser(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }
            try
            {
                ApplicationUser user = GetUserByName(model.UserName);
                var roles = await _UserManager.GetRolesAsync(user.Id);
                await _UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());

                _UserManager.AddToRole(user.Id, model.UserRole);

                _UpdateLogBL.Log(User.Identity.Name, "", -1, "Edit User", model.UserName + "\t Role:" + model.UserRole);
            }
            catch { ModelState.AddModelError("", "خطایی در انجام فرآیند رخ داده است"); }
            return PartialView(model);
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN,SUPERVISOR")]
        public ActionResult UserView()
        {
            MapConfigViewModel mapConfigViewModel = new MapConfigViewModel();
            mapConfigViewModel.MapDefCentroidX = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidX, User.Identity.Name);
            mapConfigViewModel.MapDefCentroidY = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidY, User.Identity.Name);
            mapConfigViewModel.MapDefultZoom = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefultZoom, User.Identity.Name);

            return View(mapConfigViewModel);
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN,SUPERVISOR")]
        public JsonResult UsersLastLocation()
        {
            List<UserLocation> users = _UserLocationBL.UsersLastLocation();
            return Json(users.Select(a => new { a.User_name, a.Coordinate, DateTime = a.DateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss") }), JsonRequestBehavior.AllowGet);
        }

        [UserRoleFilterAttribute(RoleIds = "ADMIN,SUPERVISOR")]
        public JsonResult UsersLocationTrack(string UserName, string DateOf)
        {
            DateTime date;
            if (!DateTime.TryParseExact(DateOf, "yyyy/MM/dd", new CultureInfo("en-US"), DateTimeStyles.None, out date))
                return Json("");
            List<UserLocation> users = _UserLocationBL.UsersLocationTrack(UserName.ToUpper(), date);
            return Json(users.Select(a => new { a.User_name, a.Coordinate, DateTime = a.DateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss") }), JsonRequestBehavior.AllowGet);
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        public ActionResult Manage()
        {
            return View();
        }


        [UserRoleFilterAttribute(RoleIds = "ADMIN")]
        public JsonResult UserList_Manege([DataSourceRequest]DataSourceRequest request)
        {
            List<UserModel> userList = UserList();
            foreach (UserModel user in userList)
            {
                user.Role = _UserManager.IsInRole(user.UserId, "ADMIN") ? "ADMIN" : _UserManager.IsInRole(user.UserId, "SUPERVISOR") ? "SUPERVISOR" : "USER";
            }
            return Json(userList.ToDataSourceResult(request));
        }

        public JsonResult UserList_Read()
        {
            List<UserModel> userList = UserList();

            return Json(userList, JsonRequestBehavior.AllowGet);
        }
        private List<UserModel> UserList()
        {
            List<UserModel> userList = new List<UserModel>();
            foreach (var item in _UserManager.Users.Where(a => a.UserName != "SYSADMIN").OrderBy(a => a.UserName))
            {
                userList.Add(new UserModel() { UserName = item.UserName, UserId = item.Id });
            }
            return userList;
        }

        public ActionResult UserAccessDeny()
        {
            return View();
        }
    }
}