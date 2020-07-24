using Mah.DataCollector.Entity.Entities;
using DataAccess.Logic;
using Mah.DataCollector.Web.Filters;
using DataCollector.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using Mah.Common.Encrypt;
using Mah.DataCollector.Web.Models.Setting;
using Mah.Common.Logger;

namespace Mah.DataCollector.Web.Controllers
{
    [UserRoleFilter(RoleIds = "ADMIN")]
    public class SettingController : Controller
    {
        private SettingBL _SettingBL;
        private ClassesBL _ClassesBL;
        private FieldsBL _FieldsBL;
        private string _GdbConnection;
        private DomainBL _DomainBL;
        private UniqueStyleBL _UniqueStyleBL;
        private ILogger _Logger;
        public SettingController(SettingBL settingBL, ClassesBL classesBL, FieldsBL fieldsBL,
            DomainBL domainBL,
            UniqueStyleBL uniqueStyleBL, Cryptor cryptor,
             ILogger logger)
        {
            _UniqueStyleBL = uniqueStyleBL;
            _DomainBL = domainBL;
            _FieldsBL = fieldsBL;
            _ClassesBL = classesBL;
            _SettingBL = settingBL;
            _GdbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["gdbConn"].ConnectionString);
            _Logger = logger;
        }
        #region SysSetting
        [UserRoleFilterAttribute(RoleIds = "SYSADMIN")]
        public ActionResult SYSSetting()
        {
            DateTime? dateOf = _SettingBL.getSettingAsDate(SettingBL.SettingParameters.ExpireDateTime, "SYSTEM");
            var model = new SysSettingParam();
            model.MapDefCentroidX = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidX, User.Identity.Name);
            model.MapDefCentroidY = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidY, User.Identity.Name);
            model.MapDefultZoom = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefultZoom, User.Identity.Name);


            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = dateOf ?? DateTime.Now.AddDays(2);
            model.ExpireDate = string.Format("'{0}/{1}/{2}'", pc.GetYear(thisDate), pc.GetMonth(thisDate), pc.GetDayOfMonth(thisDate));

            return View(model);
        }
        [UserRoleFilterAttribute(RoleIds = "SYSADMIN")]
        public JsonResult SetExpireTime(string ExpireDate)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrEmpty(ExpireDate))
                {
                    DateTime expireDateTime = DateTime.ParseExact(ExpireDate, "yyyy/MM/dd", null);

                    _SettingBL.insertSetting(SettingBL.SettingParameters.ExpireDateTime, expireDateTime, "SYSTEM");
                }
                success = true;
            }
            catch { }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [UserRoleFilterAttribute(RoleIds = "SYSADMIN")]
        public JsonResult SetLicenseValidate()
        {
            bool success = false;
            try
            {
                string gdbConn = _GdbConnection;

                _SettingBL.insertSetting(SettingBL.SettingParameters.LicenseText, gdbConn, "SYSTEM");

                success = true;
            }
            catch { }
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        [UserRoleFilterAttribute(RoleIds = "SYSADMIN")]
        public JsonResult SetMapExtent(double CenterX, double CenterY, double Zoom)
        {
            bool success = false;
            try
            {
                _SettingBL.insertSetting(SettingBL.SettingParameters.MapDefCentroidX, CenterX, "SYSTEM");
                _SettingBL.insertSetting(SettingBL.SettingParameters.MapDefCentroidY, CenterY, "SYSTEM");
                _SettingBL.insertSetting(SettingBL.SettingParameters.MapDefultZoom, Zoom, "SYSTEM");

                success = true;
            }
            catch { }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        #endregion
        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        #region classes
        public ActionResult LayerList()
        {
            return PartialView();
        }
        public JsonResult GetClasses([DataSourceRequest]DataSourceRequest request)
        {
            List<Classes> classes = _ClassesBL.GetAllClass();

            List<Classes> oList = new List<Classes>();
            foreach (Classes classItem in classes)
            {
                oList.Add(new Classes()
                {
                    ID = classItem.ID,
                    Class_name = classItem.Class_name,
                    Caption = classItem.Caption,
                    Class_type = classItem.Class_type,
                    SpatialRefrence = classItem.SpatialRefrence,
                    Scale = classItem.Scale,
                    FillColor = classItem.FillColor,
                    StrokColor = classItem.StrokColor,
                    Width = classItem.Width,
                    StrokWidth = classItem.StrokWidth,
                    UniqueField = classItem.UniqueField,
                    HasFlow = classItem.HasFlow,
                    SupervisorDateOfField = classItem.SupervisorDateOfField,
                    SupervisorField = classItem.SupervisorField,
                    DateOf = classItem.DateOf,
                    TimeOf = classItem.TimeOf,
                    UserId = classItem.UserId,
                    RequieredPhoto = classItem.RequieredPhoto
                });
            }
            return Json(oList.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateClasses([DataSourceRequest] DataSourceRequest request, Classes oClass)
        {
            if (oClass != null && ModelState.IsValid)
            {
                if (IsClassNameValid(oClass.Class_name))
                {
                    if (_ClassesBL.getClassByName(oClass.Class_name) == null)
                    {
                        if (!_ClassesBL.createNewClass(oClass.Class_name,
                             oClass.Caption,
                             oClass.Class_type,
                             oClass.SpatialRefrence,
                             oClass.RequieredPhoto))
                        {
                            ModelState.AddModelError("Class_name", "خطایی در ذخیره جدول رخ داده است.مجددا تلاش کنید");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Class_name", "جدول وارد شده تکراری می باشد");
                    }

                }
                else
                {
                    ModelState.AddModelError("Class_name", "نام جدول وارد شده و یا فرمت آن اشتباه می باشد");
                }

            }

            return Json(new[] { oClass }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateClasses([DataSourceRequest] DataSourceRequest request, Classes pClass)
        {
            if (pClass != null && ModelState.IsValid)
            {
                if (IsClassNameValid(pClass.Class_name))
                {
                    Classes oClass = _ClassesBL.getClassByName(pClass.Class_name);
                    if (oClass.ID == pClass.ID)
                    {
                        oClass.Class_name = pClass.Class_name;
                        oClass.Caption = pClass.Caption;
                        oClass.Class_type = pClass.Class_type;
                        oClass.SpatialRefrence = pClass.SpatialRefrence;
                        oClass.Scale = pClass.Scale;
                        oClass.FillColor = pClass.FillColor;
                        oClass.StrokColor = pClass.StrokColor;
                        oClass.StrokWidth = pClass.StrokWidth;
                        oClass.Width = pClass.Width;
                        oClass.HasFlow = pClass.HasFlow;
                        oClass.SupervisorDateOfField = pClass.SupervisorDateOfField;
                        oClass.SupervisorField = pClass.SupervisorField;
                        oClass.DateOf = pClass.DateOf;
                        oClass.TimeOf = pClass.TimeOf;
                        oClass.UserId = pClass.UserId;
                        oClass.RequieredPhoto = pClass.RequieredPhoto;
                        if (!_ClassesBL.UpdateClass(oClass))
                        {
                            ModelState.AddModelError("Class_name", "خطایی در ذخیره جدول رخ داده است.مجددا تلاش کنید");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Class_name", "جدول وارد شده تکراری می باشد");
                    }
                }
                else
                {
                    ModelState.AddModelError("Class_name", "نام جدول وارد شده و یا فرمت آن اشتباه می باشد");
                }
            }

            return Json(new[] { pClass }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DestroyClasses([DataSourceRequest] DataSourceRequest request, Classes oClass)
        {
            if (oClass != null)
            {
                if (!_ClassesBL.DeleteClass(oClass.ID))
                {
                    ModelState.AddModelError("Class_name", "خطایی در حذف جدول رخ داده است.مجددا تلاش کنید");

                }
            }

            return Json(new[] { oClass }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SetClassUniqueField(int ClassId, string FieldName)
        {
            Classes oClass = _ClassesBL.getClass(ClassId);
            oClass.UniqueField = FieldName;
            bool res = _ClassesBL.UpdateClass(oClass);

            return Json(res);
        }
        private bool IsClassNameValid(string ClassName)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(_GdbConnection))
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cnn.Open();

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "SELECT objectid,shape FROM " + ClassName + " WHERE 1= 2 ";
                        cmd.ExecuteReader();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult ClassesSpecialFields(int ClassID)
        {
            Classes classes = new Classes() { ID = -1 };
            if (ClassID != -1)
            {

                classes = _ClassesBL.getClass(ClassID);

                List<KeyValuePair<string, string>> fields =
                    new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("", "") };
                fields.AddRange(_ClassesBL.GetClassFieldsFromDB(classes.ID).Where(a => a.Value == "TEXT"));
                ViewBag.Fields = fields;

            }
            return PartialView(classes);
        }

        [HttpPost]
        public ActionResult ClassesSpecialFields(Classes pClass)
        {
            if (pClass != null)
            {
                Classes classes = _ClassesBL.getClass(pClass.ID);
                List<Fields> exixtFields = _FieldsBL.getClassFields(classes);
                bool hasError = false;
                string message = "";
                classes.AdressField = pClass.AdressField ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.AdressField).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد آدرس تکراری وارد شده است\r\n";
                }
                classes.DateOf = pClass.DateOf ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.DateOf).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد تاریخ تغییرات تکراری وارد شده است\r\n";
                }
                classes.TimeOf = pClass.TimeOf ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.TimeOf).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد ساعت تغییرات تکراری وارد شده است\r\n";
                }
                classes.UserId = pClass.UserId ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.UserId).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد نام کاربر تکراری وارد شده است\r\n";
                }
                classes.SupervisorField = pClass.SupervisorField ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.SupervisorField).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد ثبت ناظر تکراری وارد شده است\r\n";
                }
                classes.SupervisorDateOfField = pClass.SupervisorDateOfField ?? "";
                if (exixtFields.Where(a => a.FIELD_Name == classes.SupervisorDateOfField).Count() > 0)
                {
                    hasError = true;
                    message += "فیلد تاریخ نظارت تکراری وارد شده است";
                }
                if (!hasError)
                {
                    _ClassesBL.UpdateClass(classes);
                }
                else
                {
                    ModelState.AddModelError("", message);
                }

                List<KeyValuePair<string, string>> fields =
                    new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("", "") };
                fields.AddRange(_ClassesBL.GetClassFieldsFromDB(classes.ID).Where(a => a.Value == "TEXT"));
                ViewBag.Fields = fields;
            }
            return PartialView(pClass);
        }
        #endregion

        #region fields
        public ActionResult FieldsList(int ClassID)
        {
            ViewBag.DomainList = _DomainBL.GetAllDomains();
            ViewBag.Classes = _ClassesBL.GetAllClass();
            return PartialView(ClassID);
        }

        public JsonResult GetFields([DataSourceRequest]DataSourceRequest request, int ClassID)
        {
            Classes classes = _ClassesBL.getClass(ClassID);

            List<Fields> fields = _FieldsBL.getClassFields(classes);
            List<FieldsModel> oList = new List<FieldsModel>();
            foreach (Fields fieldItem in fields)
            {
                oList.Add(new FieldsModel()
                {
                    ID = fieldItem.ID,
                    FIELD_Name = fieldItem.FIELD_Name,
                    FIELD_Caption = fieldItem.FIELD_Caption,
                    DEF_VAL = fieldItem.DEF_VAL,
                    FIELD_Type = fieldItem.FIELD_Type,
                    MAX_VALUE = fieldItem.MAX_VALUE,
                    MIN_VALUE = fieldItem.MIN_VALUE,
                    ORDER = fieldItem.ORDER,
                    REQUIERD = fieldItem.REQUIERD,
                    Class_ID = fieldItem.Class_ID,
                    Domain_ID = fieldItem.Domain_ID
                });
            }
            return Json(oList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetClassFields(int ClassID)
        {
            Classes classes = _ClassesBL.getClass(ClassID);

            List<Fields> fields = new List<Fields>();
            fields.AddRange(_FieldsBL.getClassFields(classes));

            return Json(fields, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassFieldsFromDB(int ClassID)
        {
            try
            {
                List<KeyValuePair<string, string>> fields = _ClassesBL.GetClassFieldsFromDB(ClassID);
                return Json(fields, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "GetClassFieldsFromDB");
                throw;
            }
        }

        public JsonResult GetFieldDistinctValues(int ClassID, string FieldName)
        {
            var fields = _ClassesBL.GetDistinctField(ClassID, FieldName);
            return Json(fields, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateField([DataSourceRequest]DataSourceRequest request, FieldsModel pField)
        {
            if (pField != null && ModelState.IsValid)
            {
                Classes oClass = _ClassesBL.getClass(pField.Class_ID ?? -1);
                List<string> specialFields = new List<string>()
                {
                    oClass.AdressField,oClass.DateOf,oClass.UserId,oClass.SupervisorDateOfField,oClass.SupervisorField
                };
                if (!specialFields.Contains(pField.FIELD_Name))
                {
                    List<Fields> oFields = _FieldsBL.getClassFields(oClass);
                    if (pField.FIELD_Type == "TEXT")
                    {
                        pField.MAX_VALUE = _ClassesBL.GetTextFieldLengthFromDB(pField.Class_ID ?? -1, pField.FIELD_Name);
                    }
                    string error = "";
                    if (CheckFieldDef(pField, out error))
                    {
                        if (oFields.Where(a => a.FIELD_Name.ToUpper() == pField.FIELD_Name.ToUpper()).Count() == 0)
                        {
                            int id = _FieldsBL.createNewFields(pField.Class_ID,
                                 pField.FIELD_Name,
                                 pField.FIELD_Type,
                                 pField.FIELD_Caption,
                                 pField.Domain_ID,
                                 pField.MAX_VALUE,
                                 pField.MIN_VALUE,
                                 pField.ORDER,
                                 pField.REQUIERD,
                                 pField.DEF_VAL);
                            if (id == -1)
                            {
                                ModelState.AddModelError("FIELD_Name", "خطایی در ذخیره جدول رخ داده است.مجددا تلاش کنید");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("FIELD_Name", "ستون وارد شده تکراری می باشد");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DEF_VAL", "خطا در مقدار پیش فرض. " + error);
                    }
                }
                else
                {
                    ModelState.AddModelError("FIELD_Name", "ستون وارد شده در لیست ستون های خاص است");
                }

            }

            return Json(new[] { pField }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult UpdateField([DataSourceRequest]DataSourceRequest request, FieldsModel pField)
        {
            if (pField != null && ModelState.IsValid)
            {
                Classes oClass = _ClassesBL.getClass(pField.Class_ID ?? -1);
                List<Fields> oFields = _FieldsBL.getClassFields(oClass);
                if (oFields.Where(a => a.FIELD_Name.ToUpper() != pField.FIELD_Name.ToUpper() && a.ID == pField.ID).Count() == 0)
                {
                    string error = "";
                    if (CheckFieldDef(pField, out error))
                    {
                        Fields field = oFields.Where(a => a.ID == pField.ID).Select(a => a).First();

                        field.FIELD_Name = pField.FIELD_Name;
                        field.FIELD_Type = pField.FIELD_Type;
                        field.FIELD_Caption = pField.FIELD_Caption;
                        field.DEF_VAL = pField.DEF_VAL;
                        field.Domain_ID = pField.Domain_ID;
                        field.MAX_VALUE = pField.MAX_VALUE;
                        field.MIN_VALUE = pField.MIN_VALUE;
                        field.ORDER = pField.ORDER;
                        field.REQUIERD = pField.REQUIERD;

                        if (!_FieldsBL.Update(field))
                        {
                            ModelState.AddModelError("FIELD_Name", "خطایی در ذخیره جدول رخ داده است.مجددا تلاش کنید");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DEF_VAL", "خطا در مقدار پیش فرض. " + error);
                    }
                }
                else
                {
                    ModelState.AddModelError("FIELD_Name", "در هنگام ویرایش، تغییر نام ستون غیر مجاز است");
                }

            }

            return Json(new[] { pField }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult DeleteField([DataSourceRequest]DataSourceRequest request, FieldsModel pField)
        {
            if (pField != null)
            {
                if (!_FieldsBL.DeleteField(pField.ID))
                {
                    ModelState.AddModelError("FIELD_Name", "خطایی در حذف ستون رخ داده است.مجددا تلاش کنید");

                }
            }
            return Json(new[] { pField }.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        public PartialViewResult AddDomainToField(int fieldId)
        {
            Fields field = _FieldsBL.GetFieldsByID(fieldId);

            if (field != null)
            {
                if (field.FIELD_Type.ToUpper() != "TEXT")
                {
                    ViewBag.FieldID = -2;
                    ViewBag.DomainID = -2;
                }
                else
                {
                    ViewBag.FieldID = field.ID;
                    ViewBag.DomainID = field.Domain_ID;
                }
            }
            else
            {
                ViewBag.FieldID = -1;
                ViewBag.DomainID = -1;
            }
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddDomainToField(int FieldID, int DomainID)
        {
            Fields field = _FieldsBL.GetFieldsByID(FieldID);
            if (field != null)
            {
                if (DomainID == -1)
                    field.Domain_ID = null;
                else
                    field.Domain_ID = DomainID;
                _FieldsBL.Update(field);
            }
            return Json("Success");
        }

        private bool CheckFieldDef(FieldsModel pField, out string error)
        {
            error = "";
            bool success = true;
            if (!string.IsNullOrEmpty(pField.DEF_VAL))
            {
                if (pField.FIELD_Type == "TEXT" && pField.DEF_VAL.Length > pField.MAX_VALUE)
                {
                    error = string.Format("طول مقدار پیش فرض نباید از {0} بزرگتر باشد", pField.MAX_VALUE);
                    success = false;
                }
                int i = 0;
                if (pField.FIELD_Type == "INT" && !int.TryParse(pField.DEF_VAL, out i))
                {
                    error = "مقدار پیش فرض باید عدد صحیح باشد";
                    success = false;
                }
                double j = 0;
                if (pField.FIELD_Type == "DOUBLE" && !double.TryParse(pField.DEF_VAL, out j))
                {
                    error = "مقدار پیش فرض باید عدد باشد";
                    success = false;
                }
            }

            return success;
        }
        #endregion

        #region Domanis
        public ActionResult DomainList(int DomainID)
        {
            return PartialView(DomainID);
        }

        public JsonResult GetAllDomains()
        {
            List<Domain> oList = _DomainBL.GetAllDomains();
            return Json(oList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDomainValues([DataSourceRequest]DataSourceRequest request, int DomainID)
        {
            List<DomainValue> oList = _DomainBL.GetDomainValues(DomainID);
            return Json(oList.ToDataSourceResult(request));
        }

        public JsonResult GetDomainValueText(int DomainID)
        {
            List<DomainValue> oList = _DomainBL.GetDomainValues(DomainID);
            var values = oList.Select(a => a.Value).ToList();
            return Json(values, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult EditDomain(int DomainId)
        {
            Domain model = null;
            if (DomainId != -1)
            {
                model = _DomainBL.GetAllDomains().Where(a => a.ID == DomainId).FirstOrDefault();
            }
            else
            {
                model = new Domain() { ID = -1 };
            }
            return PartialView(model);
        }
        [HttpPost]
        public PartialViewResult EditDomain(Domain pDomainValue)
        {
            if (pDomainValue.ID == -1)
            {
                int id = -1;
                List<Domain> oList = _DomainBL.GetAllDomains().Where(a => a.Caption == pDomainValue.Caption).ToList();
                if (oList.Count == 0)
                {

                    id = _DomainBL.AddDomain(pDomainValue);
                    if (id == -1)
                    {
                        ModelState.AddModelError("", "ایجاد دامنه جدید با خطا مواجه شد.مجددا تلاش کنید");
                    }
                }
                else
                    ModelState.AddModelError("", "مقدار وارد شده تکراری می باشد");
            }
            else
            {
                List<Domain> oList = _DomainBL.GetAllDomains().Where(a => a.Caption == pDomainValue.Caption && a.ID != pDomainValue.ID).ToList();
                if (oList.Count == 0)
                {

                    bool resualt = _DomainBL.UpdateDomain(pDomainValue);
                    if (!resualt)
                    {
                        ModelState.AddModelError("", "ویرایش دامنه جدید با خطا مواجه شد.مجددا تلاش کنید");
                    }
                }
                else
                    ModelState.AddModelError("", "مقدار وارد شده تکراری می باشد");
            }
            return PartialView(pDomainValue);
        }
        public JsonResult DeleteDomain(int pID)
        {
            bool result = _DomainBL.DeleteDomain(pID);
            return Json(result);
        }
        public JsonResult CreateDomainValue([DataSourceRequest]DataSourceRequest request, DomainValue pDomainValue)
        {
            if (pDomainValue != null && ModelState.IsValid)
            {
                List<DomainValue> oList = _DomainBL.GetDomainValues(pDomainValue.Domain_ID ?? -1).Where(a => a.Value == pDomainValue.Value).ToList();
                if (oList.Count == 0)
                {
                    int id = _DomainBL.AddValueToDomain(pDomainValue.Value, pDomainValue.Domain_ID);
                    if (id == -1)
                    {
                        ModelState.AddModelError("FIELD_Name", "خطایی در ذخیره مقدار دامنه رخ داده است.مجددا تلاش کنید");
                    }
                }
                else
                {
                    ModelState.AddModelError("FIELD_Name", "مقدار وارد شده تکراری می باشد");
                }

            }

            return Json(new[] { pDomainValue }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult UpdateDomainValue([DataSourceRequest]DataSourceRequest request, DomainValue pDomainValue)
        {
            if (pDomainValue != null && ModelState.IsValid)
            {
                var domains = _DomainBL.GetDomainValues(pDomainValue.Domain_ID ?? -1);
                if (!domains.Any(a => a.Value == pDomainValue.Value && a.ID != pDomainValue.ID))
                {
                    var domain = domains.First(a => a.ID == pDomainValue.ID);
                    domain.Value = pDomainValue.Value;
                    if (!_DomainBL.UpdateDomainValue(domain))
                    {
                        ModelState.AddModelError("Value", "خطایی در ذخیره مقدار دامنه رخ داده است.مجددا تلاش کنید");
                    }
                }
                else
                {
                    ModelState.AddModelError("Value", "مقدار دامنه تکراری وارد شده است");
                }

            }

            return Json(new[] { pDomainValue }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult DeleteDomainValue([DataSourceRequest]DataSourceRequest request, DomainValue pDomainValue)
        {
            if (pDomainValue != null)
            {
                if (!_DomainBL.DeleteDomainValue(pDomainValue.ID))
                {
                    ModelState.AddModelError("Value", "خطایی در حذف مقدار دامنه رخ داده است.مجددا تلاش کنید");

                }
            }
            return Json(new[] { pDomainValue }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Class Symbol
        public PartialViewResult GetClassSymbol(int classId)
        {
            var classSymbol = new ClassSymbolViewModel();
            var layer = _ClassesBL.getClass(classId);
            classSymbol.ClassId = layer.ID;
            classSymbol.MaxScale = layer.Scale;
            classSymbol.ClassType = layer.Class_type;
            classSymbol.HasFlow = layer.HasFlow ?? false;
            classSymbol.Symbol = new SymbolViewModel()
            {
                FillColor = layer.FillColor,
                StrokeColor = layer.StrokColor,
                StrokeWidth = layer.StrokWidth ?? 0,
                Width = layer.Width ?? 0
            };

            classSymbol.UniqueField = layer.UniqueField;
            foreach (var style in layer.UniqueStyles)
            {
                classSymbol.UniqueItems.Add(new UniqueSymbolItem()
                {
                    Symbol = new SymbolViewModel()
                    {
                        FillColor = style.FillColor,
                        StrokeColor = style.StrokColor,
                        StrokeWidth = style.StrokWidth ?? 0,
                        Width = style.Width ?? 0
                    },
                    Value = style.TEXT
                });
            }

            return PartialView("ClassSymbol", classSymbol);
        }

        public JsonResult SetClassSymbol(ClassSymbolViewModel model)
        {
            try
            {
                var layer = _ClassesBL.getClass(model.ClassId);
                layer.Scale = model.MaxScale;
                layer.HasFlow = model.HasFlow;
                layer.FillColor = model.Symbol.FillColor;
                layer.StrokColor = model.Symbol.StrokeColor;
                layer.Width = model.Symbol.Width;
                layer.StrokWidth = model.Symbol.StrokeWidth;

                layer.UniqueField = model.UniqueField;
                if (layer.UniqueStyles != null)
                {
                    var styles = layer.UniqueStyles.Count();
                    for (int i = 0; i < styles; i++)
                    {
                        _UniqueStyleBL.DeleteUniqueStyles(layer.UniqueStyles.ElementAt(0));
                    }
                }
                layer.UniqueStyles = new List<UniqueStyle>();
                if (model.UniqueItems != null)
                {
                    foreach (var item in model.UniqueItems)
                    {
                        layer.UniqueStyles.Add(new UniqueStyle()
                        {
                            CLASS_ID = model.ClassId,
                            FillColor = item.Symbol.FillColor,
                            StrokColor = item.Symbol.StrokeColor,
                            Width = item.Symbol.Width,
                            StrokWidth = item.Symbol.StrokeWidth,
                            TEXT = item.Value
                        });
                    }
                }

                _ClassesBL.UpdateClassSymbol(layer);
                return Json("0");
            }
            catch (Exception ex)
            {
                return Json("1");
            }
        }
        #endregion

        #region General Setting
        public PartialViewResult GeneratSetting()
        {
            var model = new GeneralSettingViewModel();
            model.SnapTolerance = _SettingBL.getSettingAsInt(DataAccess.Logic.SettingBL.SettingParameters.SnapTolorance, "SYSTEM");

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SetSnapTolorance(SnapToleranceViewModel model)
        {
            try
            {
                _SettingBL.insertSetting(SettingBL.SettingParameters.SnapTolorance, model.SnapValue, "SYSTEM");
                return Json(true);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "SetSnapTolorance");
                ///todo set loger 
                throw;
            }

        }
        #endregion
    }
}