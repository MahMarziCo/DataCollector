using Mah.DataCollector.Entity.Entities;
using DataAccess.Logic;
using DataCollector.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.IO;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Globalization;
using Mah.Common.Encrypt;
using Mah.DataCollector.Interface.Dto.Feature;
using DataCollector.Models.Map;
using Mah.Common.Logger;
using Mah.Common.Extentions;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Mah.DataCollector.Web.Models.Map;

namespace Mah.DataCollector.Web.Controllers
{
    public class MapController : Controller
    {
        private string _GdbConnection = "";
        private ClassesBL _ClassesBL;
        private UpdateLogBL _UpdateLogBL;
        private FeaturePicBL _FeaturePicBL;
        private FieldsBL _FieldsBL;
        private UserLocationBL _UserLocationBL;
        private GISFeatureBL _GISFeatureBL;
        private SettingBL _SettingBL;
        private ILogger _LoggerService;
        public MapController(ClassesBL classesBL, UpdateLogBL updateLogBL, FeaturePicBL featurePicBL, FieldsBL fieldsBL
            , UserLocationBL userLocationBL,
            GISFeatureBL gISFeatureBL, Cryptor cryptor
            , SettingBL settingBL,
            ILogger loggerService)
        {
            _LoggerService = loggerService;
            _SettingBL = settingBL;
            _GISFeatureBL = gISFeatureBL;
            _UserLocationBL = userLocationBL;
            _FieldsBL = fieldsBL;
            _FeaturePicBL = featurePicBL;
            _UpdateLogBL = updateLogBL;
            _ClassesBL = classesBL;
            _GdbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["gdbConn"].ConnectionString);

        }
        // GET: Map
        public ActionResult Index()
        {
            MapConfigViewModel mapConfigViewModel = new MapConfigViewModel();
            mapConfigViewModel.MapDefCentroidX = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidX, User.Identity.Name);
            mapConfigViewModel.MapDefCentroidY = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefCentroidY, User.Identity.Name);
            mapConfigViewModel.MapDefultZoom = _SettingBL.getSettingAsDouble(DataAccess.Logic.SettingBL.SettingParameters.MapDefultZoom, User.Identity.Name);
            mapConfigViewModel.MapSnapTolerance = _SettingBL.getSettingAsInt(DataAccess.Logic.SettingBL.SettingParameters.SnapTolorance, User.Identity.Name);
            ViewBag.NeedUserPositionLog = _SettingBL.getSettingAsBool(DataAccess.Logic.SettingBL.SettingParameters.NeedUserPositionLog, "SYSTEM");
            return View(mapConfigViewModel);
        }

        [HttpPost]
        public async Task<string> GetFeatures(GetFeaturesViewModel model)
        {
            Dictionary<string, List<Dictionary<string, object>>> classObjects = new Dictionary<string, List<Dictionary<string, object>>>();
            List<string> errorLayer = new List<string>();
            try
            {
                if (model.LayerNames.Count() > 0)
                {
                    await model.LayerNames.ForEachAsync(async (layerItem, i) =>
                    {
                        try
                        {
                            using (SqlConnection cnn = new SqlConnection(_GdbConnection))
                            {
                                cnn.Open();
                                double[] MapExtent = layerItem.MapExtent;
                                string polytext = string.Format("POLYGON(({0} {2},{0} {3},{1} {3},{1} {2},{0} {2}))", MapExtent[0], MapExtent[2], MapExtent[1], MapExtent[3]);


                                string ClassName = layerItem.LayerName;
                                string uniqueField = string.IsNullOrEmpty(layerItem.UniqueField) ? "" : "," + layerItem.UniqueField;
                                using (SqlCommand cmd = cnn.CreateCommand())
                                {
                                    var reduce = (MapExtent[2] - MapExtent[0]) / (model.MapWidth);

                                    DataTable dataTable = new DataTable();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = "DECLARE @g geometry; SET @g = geometry::STGeomFromText('" + polytext + "', " + layerItem.SRID + ");  "
                                    + "SELECT [objectid],[Shape].Reduce(" + reduce + ").STAsText() geom " + uniqueField + " FROM [" + ClassName + "] WHERE shape.STIntersects(@g)=1";

                                    dataTable.Load(await cmd.ExecuteReaderAsync());

                                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        Dictionary<string, object> rowDic = new Dictionary<string, object>();
                                        foreach (DataColumn col in dataTable.Columns)
                                        {
                                            rowDic.Add(col.ColumnName, row[col]);
                                        }
                                        list.Add(rowDic);
                                    }

                                    lock (classObjects)
                                    {
                                        classObjects.Add(layerItem.LayerName, list);
                                    }
                                }
                                cnn.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (errorLayer)
                            {
                                errorLayer.Add(layerItem.LayerName);
                            }
                        }
                    });


                }

                return JsonConvert.SerializeObject(new { LayerData = classObjects, ErrorLayer = errorLayer });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Error = ex.Message + "\r\n" + ex.StackTrace });
            }
        }

        [HttpPost]
        public JsonResult InsertFeature(string ClassName, string Shape, string SRID)
        {
            int oid = -1;
            try
            {
                Classes oClass = _ClassesBL.getClassByName(ClassName);
                using (SqlConnection con = new SqlConnection(_GdbConnection))
                {
                    con.Open();
                    bool validGeometry = true;
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "select geometry::STGeomFromText('" + Shape + "'," + SRID + ").STIsValid()";
                            string ss = cmd.ExecuteScalar().ToString();
                            if (ss == "True")
                                validGeometry = true;
                            else
                                validGeometry = false;
                        }
                        catch { validGeometry = false; }
                    }
                    if (validGeometry)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            string insertText = "declare @p1 table (oid int);INSERT INTO [" + ClassName + "] (shape{0}) output INSERTED.OBJECTID into @p1 VALUES(geometry::STGeomFromText('" + Shape + "'," + SRID + "){1});select oid from @p1";
                            string fields = "";
                            string values = "";
                            if (!string.IsNullOrEmpty(oClass.UserId))
                            {
                                fields += "," + oClass.UserId;
                                values += ",'" + User.Identity.Name + "'";
                            }
                            if (!string.IsNullOrEmpty(oClass.DateOf))
                            {
                                fields += "," + oClass.DateOf;
                                DateTime thisDate = DateTime.Now;
                                PersianCalendar pc = new PersianCalendar();
                                values += ",'" + string.Format("{0}/{1}/{2} {3}:{4}",
                                        pc.GetYear(thisDate),
                                        pc.GetMonth(thisDate),
                                        pc.GetDayOfMonth(thisDate),
                                        pc.GetHour(thisDate),
                                        pc.GetMinute(thisDate)) + "'";
                            }
                            cmd.CommandText = string.Format(insertText, fields, values);
                            int modified = (int)cmd.ExecuteScalar();

                            if (con.State == System.Data.ConnectionState.Open) con.Close();
                            oid = modified;
                            _UpdateLogBL.Log(User.Identity.Name, ClassName, oid, "INSERT", Shape + "," + SRID);
                        }
                    }
                }
            }
            catch
            {
            }
            return Json(oid);
        }

        [HttpPost]
        public JsonResult DeleteFeature(string ClassName, int ObjectID)
        {
            bool success = false;
            if (User.IsInRole("SUPERVISOR")) return Json(success);
            try
            {

                using (SqlConnection con = new SqlConnection(_GdbConnection))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE [" + ClassName + "] WHERE OBJECTID =" + ObjectID, con))
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                        success = true;
                    }
                }
                _UpdateLogBL.Log(User.Identity.Name, ClassName, ObjectID, "DELETE", "");

                List<Feature_Pic> picList = _FeaturePicBL.GetFeaturePic(ClassName, ObjectID);
                foreach (var item in picList)
                {
                    _FeaturePicBL.DeleteImages(item.ID, Server.MapPath("~/Uploads/"), User.Identity.Name);
                }
            }
            catch { }
            return Json(success);
        }

        [HttpGet]
        public ActionResult FeatureEdit(string ClassName, int ObjectId)
        {
            ViewBag.NeedUserPositionLog = _SettingBL.getSettingAsBool(DataAccess.Logic.SettingBL.SettingParameters.NeedUserPositionLog, "SYSTEM");
            ViewBag.IsSupervisor = User.IsInRole("SUPERVISOR");
            ViewBag.RequeiredPhoto = _ClassesBL.getClassByName(ClassName).RequieredPhoto??false;
            if (string.IsNullOrEmpty(ClassName))
            {
                return PartialView();
            }
            try
            {
                FeatureEditModel model = _GISFeatureBL.GetFeatureDetail(ClassName, ObjectId, ViewBag.IsSupervisor);
                return PartialView(model);
            }
            catch
            {
                return PartialView();
            }
        }
        [HttpPost]
        public JsonResult FeatureEdit()
        {
            try
            {
                string className = Request.Form["CLASS_NAME"];
                
                int objectId = int.Parse(Request.Form["OBJECTID"]);
                bool requierdPhotho = _ClassesBL.getClassByName(className).RequieredPhoto??false;
                int countPicture = _FeaturePicBL.GetFeaturePic(className, objectId).Count;
                if (requierdPhotho && (countPicture == 0))
                {
                    return (Json(new { Status = "ERRORPHOTO" }));
                }
                double[] userPosition = null;
                if (Request.Form["CURRENT_USER_COORDINATE"] != null && Request.Form["CURRENT_USER_COORDINATE"] != "0,0")
                    userPosition = Request.Form["CURRENT_USER_COORDINATE"].Split(',').Select(a => double.Parse(a)).ToArray();

                string commandText = "UPDATE [{0}] SET {1} WHERE OBJECTID = {2};";
                string updateText = "";
                Classes oClass = _ClassesBL.getClassByName(className);
                List<Fields> Fields = _FieldsBL.getClassFields(className);
                List<SqlParameter> parameters = new List<SqlParameter>();
                List<string> errors = new List<string>();
                string error;

                string oDataOf = "";
                foreach (Fields field in Fields)
                {
                    oDataOf += string.Format("{0} : {1}; ", field.FIELD_Name, Request.Form[field.FIELD_Name]);
                    if (!_FieldsBL.ValidateFieldValue(field, Request.Form[field.FIELD_Name], out error))
                    {
                        errors.Add(field.FIELD_Name + ":" + error);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", field.FIELD_Name);
                    if (field.FIELD_Type == "TEXT")
                    {
                        string value = Request.Form[field.FIELD_Name];
                        SqlParameter parameter = new SqlParameter("@" + field.FIELD_Name, value ?? "");
                        parameters.Add(parameter);
                    }
                    else if (field.FIELD_Type == "DOUBLE")
                    {
                        SqlParameter parameter = new SqlParameter("@" + field.FIELD_Name, SqlDbType.Decimal);
                        parameter.Value = string.IsNullOrEmpty(Request.Form[field.FIELD_Name]) ? (object)DBNull.Value : (object)double.Parse(Request.Form[field.FIELD_Name]);
                        parameters.Add(parameter);
                    }
                    else if (field.FIELD_Type == "INT")
                    {
                        SqlParameter parameter = new SqlParameter("@" + field.FIELD_Name, SqlDbType.Int);
                        parameter.Value = string.IsNullOrEmpty(Request.Form[field.FIELD_Name]) ? (object)DBNull.Value : (object)int.Parse(Request.Form[field.FIELD_Name]);
                        parameters.Add(parameter);
                    }
                    else if (field.FIELD_Type == "DATE")
                    {
                        SqlParameter parameter = new SqlParameter("@" + field.FIELD_Name, Request.Form[field.FIELD_Name]);
                        parameters.Add(parameter);
                    }
                    else if (field.FIELD_Type == "BOOL")
                    {
                        SqlParameter parameter = new SqlParameter("@" + field.FIELD_Name, Request.Form[field.FIELD_Name]);
                        parameters.Add(parameter);
                    }
                }
                if (errors.Count > 0)
                {
                    return Json(new { Status = "ERROR", errors = errors });
                }
                if (!string.IsNullOrEmpty(oClass.UserId))
                {
                    SqlParameter parameter = new SqlParameter("@" + oClass.UserId, User.Identity.Name);
                    parameters.Add(parameter);
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", oClass.UserId);
                }
                if (!string.IsNullOrEmpty(oClass.DateOf))
                {
                    PersianCalendar pc = new PersianCalendar();
                    DateTime thisDate = DateTime.Now;

                    SqlParameter parameter = new SqlParameter("@" + oClass.DateOf, string.Format("{0}/{1}/{2} {3}:{4}",
                      pc.GetYear(thisDate),
                      pc.GetMonth(thisDate),
                      pc.GetDayOfMonth(thisDate),
                      pc.GetHour(thisDate),
                      pc.GetMinute(thisDate)));
                    parameters.Add(parameter);
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", oClass.DateOf);
                }
                if (!string.IsNullOrEmpty(oClass.AdressField))
                {
                    oDataOf += string.Format("{0} : {1}; ", oClass.AdressField, Request.Form["ADDRESS_FIELD"]);
                    SqlParameter parameter = new SqlParameter("@" + oClass.AdressField, Request.Form["ADDRESS_FIELD"]);
                    parameters.Add(parameter);
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", oClass.AdressField);
                }
                using (SqlConnection connection = new SqlConnection(_GdbConnection))
                {
                    SqlCommand command = connection.CreateCommand();

                    command.CommandText = string.Format(commandText, className, updateText, objectId);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                }
                if (userPosition != null)
                    _UserLocationBL.LogUserLocation(User.Identity.Name, userPosition);

                _UpdateLogBL.Log(User.Identity.Name, className, objectId, "UPDATE", oDataOf);
            }
            catch (Exception ex)
            {
                _LoggerService.LogError(ex, "error on edit feature");
                return (Json(new { Status = "EXCEPTION", message = ex.Message, stackTrace = ex.StackTrace }));
            }
            return (Json(new { Status = "SUCCESS" }));
        }

        [HttpPost]
        public JsonResult FeatureSupervise()
        {
            try
            {

                string className = Request.Form["CLASS_NAME"];
                int objectId = int.Parse(Request.Form["OBJECTID"]);
                string commandText = "UPDATE [{0}] SET {1} WHERE OBJECTID = {2};";
                string updateText = "";
                Classes oClass = _ClassesBL.getClassByName(className);

                if (string.IsNullOrEmpty(oClass.SupervisorField) || !Request.Form.AllKeys.Contains("SUPERVISOR_FIELD"))
                {
                    return Json(new { Status = "CANT_SUPERVISED" });
                }

                List<SqlParameter> parameters = new List<SqlParameter>();
                FeatureEditModel model = _GISFeatureBL.GetFeatureDetail(className, objectId, true);

                string oDataOf = "";
                foreach (var item in model.FieldsValue)
                {
                    if (item.field.FIELD_Name == "SUPERVISOR_DATOF_FIELD" ||
                        item.field.FIELD_Name == "SUPERVISOR_FIELD")
                    {
                        continue;
                    }
                    oDataOf += string.Format("{0} : {1}; ", item.field.FIELD_Name, item.value);
                }

                if (!string.IsNullOrEmpty(oClass.SupervisorField))
                {
                    oDataOf += string.Format("{0} : {1}; ", oClass.SupervisorField, Request.Form["SUPERVISOR_FIELD"]);
                    SqlParameter parameter = new SqlParameter("@" + oClass.SupervisorField, Request.Form["SUPERVISOR_FIELD"]);
                    parameters.Add(parameter);
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", oClass.SupervisorField);
                }

                if (!string.IsNullOrEmpty(oClass.SupervisorDateOfField))
                {
                    PersianCalendar pc = new PersianCalendar();
                    DateTime thisDate = DateTime.Now;
                    string dateOf = string.Format("{0}/{1}/{2} {3}:{4}",
                      pc.GetYear(thisDate),
                      pc.GetMonth(thisDate),
                      pc.GetDayOfMonth(thisDate),
                      pc.GetHour(thisDate),
                      pc.GetMinute(thisDate));

                    oDataOf += string.Format("{0} : {1}; ", oClass.SupervisorDateOfField, dateOf);

                    SqlParameter parameter = new SqlParameter("@" + oClass.SupervisorDateOfField, dateOf);
                    parameters.Add(parameter);
                    if (!string.IsNullOrEmpty(updateText))
                        updateText += ",";
                    updateText += string.Format("{0} = @{0} ", oClass.SupervisorDateOfField);
                }

                using (SqlConnection connection = new SqlConnection(_GdbConnection))
                {
                    SqlCommand command = connection.CreateCommand();

                    command.CommandText = string.Format(commandText, className, updateText, objectId);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                }

                _UpdateLogBL.Log(User.Identity.Name, className, objectId, "SUPERVISE", oDataOf);
            }
            catch (Exception ex)
            {
                return (Json(new { Status = "EXCEPTION", message = ex.Message, stackTrace = ex.StackTrace }));
            }
            return (Json(new { Status = "SUCCESS" }));
        }

        public JsonResult GetObjectShape(string ClassName, int ObjectID)
        {
            string shape = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(_GdbConnection))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "SELECT [Shape].STAsText()  FROM [" + ClassName + "] WHERE OBJECTID = " + ObjectID;
                        connection.Open();
                        SqlDataReader redear = cmd.ExecuteReader();
                        if (redear.Read())
                        {
                            shape = redear[0].ToString();
                        }
                    }
                }
            }
            catch { }
            return Json(shape, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AttributeTable(string ClassName)
        {
            Classes oClass = _ClassesBL.getClassByName(ClassName);
            if (oClass != null)
            {
                List<Fields> fields = _FieldsBL.getClassFields(oClass);
                ViewBag.Fields = fields;
            }
            else
            {
                ViewBag.Fields = new List<Fields>();
            }
            return PartialView(oClass);
        }

        public ActionResult AttributeTable_Read([DataSourceRequest]DataSourceRequest request, string ClassName)
        {
            DataTable dataTable = null;
            if (!string.IsNullOrEmpty(ClassName))
            {
                dataTable = new DataTable();
                Classes oClass = _ClassesBL.getClassByName(ClassName);
                List<Fields> Fields = _FieldsBL.getClassFields(ClassName);
                string joinFields = string.Join(",", Fields.Select(a => a.FIELD_Name).ToArray());
                if (joinFields.Length > 0)
                    joinFields = "," + joinFields;
                joinFields = "OBJECTID" + joinFields;
                dataTable.Columns.Add(new DataColumn("OBJECTID") { Caption = "شناسه" });
                foreach (Fields item in Fields)
                {
                    dataTable.Columns.Add(new DataColumn(item.FIELD_Name) { Caption = item.FIELD_Caption });
                }

                if (!string.IsNullOrEmpty(oClass.UserId))
                {
                    joinFields += "," + oClass.UserId;
                    dataTable.Columns.Add(new DataColumn(oClass.UserId) { Caption = "نام کاربر" });
                }
                if (!string.IsNullOrEmpty(oClass.DateOf))
                {
                    joinFields += "," + oClass.DateOf;
                    dataTable.Columns.Add(new DataColumn(oClass.DateOf) { Caption = "تاریخ ویرایش" });
                }
                if (!string.IsNullOrEmpty(oClass.AdressField))
                {
                    joinFields += "," + oClass.AdressField;
                    dataTable.Columns.Add(new DataColumn(oClass.AdressField) { Caption = "آدرس" });
                }
                if (!string.IsNullOrEmpty(oClass.SupervisorField))
                {
                    joinFields += "," + oClass.SupervisorField;
                    dataTable.Columns.Add(new DataColumn(oClass.SupervisorField) { Caption = "وضعیت نظارت" });
                }
                if (!string.IsNullOrEmpty(oClass.SupervisorDateOfField))
                {
                    joinFields += "," + oClass.SupervisorDateOfField;
                    dataTable.Columns.Add(new DataColumn(oClass.SupervisorDateOfField) { Caption = "تاریخ نظارت" });
                }
                using (SqlConnection connection = new SqlConnection(_GdbConnection))
                {
                    SqlCommand cmd = connection.CreateCommand();

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT " + joinFields + "  FROM [" + ClassName + "]";
                    connection.Open();
                    dataTable.Load(cmd.ExecuteReader());
                }
            }
            return Json(dataTable.ToDataSourceResult(request));
        }

        public PartialViewResult SymbologyView(ClassSymbolViewModel classSymbol)
        {
            return PartialView("~/Views/Setting/ClassSymbol.cshtml", classSymbol);
        }

        public ActionResult FeaturePictures(string ClassName, int ObjectId)
        {
            
            List<Feature_Pic> list = _FeaturePicBL.GetFeaturePic(ClassName, ObjectId);
            ViewBag.ClassName = ClassName;
            ViewBag.ObjectId = ObjectId;
            return PartialView(list);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileUpload(int ObjectId, string ClassName)//HttpPostedFileBase uploadFile)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string extention = "";
                        if (fname.Split('.').Length > 1)
                        {
                            extention = fname.Split('.')[fname.Split('.').Length - 1];
                        }
                        string fileName = "";
                        int counter = 1;
                        do
                        {
                            fileName = string.Format("{0}_{1}_{2}.{3}", ClassName, ObjectId, counter++, extention);
                        } while (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads/"), fileName)));


                        string fullGuidName = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(fullGuidName);

                        _FeaturePicBL.InsertFeaturePic(fileName, User.Identity.Name, fname, ClassName, ObjectId);

                        _UpdateLogBL.Log(User.Identity.Name, ClassName, ObjectId, "INSERT_PIC", fileName);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public JsonResult DeleteFeaturePicture(int OID)
        {
            _FeaturePicBL.DeleteImages(OID, Server.MapPath("~/Uploads/"), User.Identity.Name);
            return Json("");
        }
    }
}