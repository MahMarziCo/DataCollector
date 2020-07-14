using DataAccess.Logic;
using DataCollector.Models;
using Mah.Common.Encrypt;
using Mah.Common.Logger;
using Mah.DataCollector.Interface.Interfaces.Features;
using Mah.DataCollector.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mah.DataCollector.Web.Controllers
{
    public class UserOperationController : Controller
    {
        private readonly ClassesBL _ClassesBL;
        private readonly FieldsBL _FieldsBL;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IFeatureService _FeatureService;
        private readonly ILogger _Logger;

        public UserOperationController(ClassesBL classesBL, FieldsBL fieldsBL
            , IFeatureService featureService,
            ILogger logger)
        {
            _FeatureService = featureService;
            _UserManager = Startup.UserManagerFactory.Invoke();
            _ClassesBL = classesBL;
            _FieldsBL = fieldsBL;
            _Logger = logger;
        }

        // GET: UserOperation
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUser()
        {
            try
            {
                var users = new List<string>();
                if (User.IsInRole("ADMIN"))
                {
                    users = _UserManager.Users.Where(a => a.UserName != "SYSADMIN").OrderBy(a => a.UserName).Select(a => a.UserName).ToList();
                }
                else
                {
                    users = new List<string>() { User.Identity.Name };
                }
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetUser");
                throw;
            }
        }


        public JsonResult GetLayer()
        {
            try
            {
                var classes = _ClassesBL.GetClassesWithUserField().Select(a => new { a.ID, a.Class_name, a.Caption, a.UserId });
                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetLayer");
                throw;
            }
        }

        public JsonResult GetFields(int classID)
        {
            try
            {
                var fileds = _FieldsBL.getClassFields(classID).Select(a => new { a.FIELD_Caption, a.FIELD_Name }).ToList();
                return Json(fileds, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetFields");
                throw;
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetReportModel(UserOperationParameter useroperation)
        {
            try
            {
                if (!User.IsInRole("ADMIN") && User.Identity.Name.ToUpper() != useroperation.UserName.ToUpper())
                    throw new Exception($"{User.Identity.Name.ToUpper()} user cant query {useroperation.UserName.ToUpper()} operation report");
                var userName = useroperation.UserName;
                var layerName = useroperation.LayerName;
                var fieldName = useroperation.FieldName;
                var userField = useroperation.UserField;

                return Json(await _FeatureService.GetUserReportOnLayer(layerName, userName, userField, fieldName));
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetReportModel");
                throw;
            }
        }
    }
}