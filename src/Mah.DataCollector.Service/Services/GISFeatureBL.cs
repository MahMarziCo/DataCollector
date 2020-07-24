using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mah.DataCollector.Interface.Dto.Feature;

namespace DataAccess.Logic
{
    public class GISFeatureBL
    {
        private DataCollectorContext _DbContext;
        private string _GdbConnection;
        private ClassesBL _ClassesBL;
        private FieldsBL _FieldsBl;
        private DomainBL _DomainBL;
        public GISFeatureBL(DataCollectorContext context,
            ClassesBL classesBL,
            FieldsBL fieldsBl,
            DomainBL domainBl,
            string gdbConnection)
        {
            _DbContext = context;
            _GdbConnection = gdbConnection;
            _ClassesBL = classesBL;
            _FieldsBl = fieldsBl;
            _DomainBL = domainBl;
        }
        private SqlConnection GetDBConnection()
        {
            return new SqlConnection(_GdbConnection);
        }
        public FeatureEditModel GetFeatureDetail(string ClassName, int ObjectId, bool forSupervisor)
        {
            FeatureEditModel editModel = null;
            try
            {
                Classes objClass = _ClassesBL.getClassByName(ClassName);
                List<Fields> fields = _FieldsBl.getClassFields(objClass);

                editModel = new FeatureEditModel()
                {
                    Class = objClass,
                    ObjectId = ObjectId,
                    FieldsValue = new List<FieldsValueModel>()
                };
                using (SqlConnection cnn = GetDBConnection())
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        string joinFields = "OBJECTID";
                        if (fields.Count > 0)
                        {
                            joinFields += "," + string.Join(",", fields.Select(a => a.FIELD_Name).ToArray());
                        }
                        if (!string.IsNullOrEmpty(objClass.AdressField))
                        {
                            joinFields += "," + objClass.AdressField;
                        }
                        if (!string.IsNullOrEmpty(objClass.SupervisorField))
                        {
                            joinFields += "," + objClass.SupervisorField;
                        }
                        if (!string.IsNullOrEmpty(objClass.SupervisorDateOfField))
                        {
                            joinFields += "," + objClass.SupervisorDateOfField;
                        }
                        if (!string.IsNullOrEmpty(objClass.UserId))
                        {
                            joinFields += "," + objClass.UserId;
                        }
                        if (!string.IsNullOrEmpty(objClass.DateOf))
                        {
                            joinFields += "," + objClass.DateOf;
                        }
                        if (!string.IsNullOrEmpty(objClass.TimeOf))
                        {
                            joinFields += "," + objClass.TimeOf;
                        }
                        cmd.CommandText = string.Format("SELECT {0} FROM [{1}] WHERE OBJECTID ={2}", joinFields,
                            ClassName, ObjectId);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            if (!string.IsNullOrEmpty(objClass.UserId))
                            {
                                Fields userField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "USER_ID_FIELD",
                                    FIELD_Caption = "نام کاربر",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(userField, dr[objClass.UserId], null,
                                    false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            if (!string.IsNullOrEmpty(objClass.DateOf))
                            {
                                Fields dateOfField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "DATEOF_FIELD",
                                    FIELD_Caption = "تاریخ ویرایش",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(dateOfField, dr[objClass.DateOf],
                                    null, false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            if (!string.IsNullOrEmpty(objClass.TimeOf))
                            {
                                Fields timeOfField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "TIMEOF_FIELD",
                                    FIELD_Caption = "ساعت ویرایش",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(timeOfField, dr[objClass.TimeOf],
                                    null, false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            foreach (Fields field in fields.ToList())
                            {
                                List<string> domains = _DomainBL.GetFieldsDomainValue(field);
                                bool isMultiSelect = _DomainBL.IsDomainMultiSelect(field);
                                object value = null;
                                bool isDefault = false;
                                if (forSupervisor || string.IsNullOrEmpty(field.DEF_VAL))
                                {
                                    value = dr[field.FIELD_Name];
                                    isDefault = false;
                                }
                                else if (dr[field.FIELD_Name] != null)
                                {
                                    if (string.IsNullOrEmpty(dr[field.FIELD_Name].ToString()))
                                    {
                                        value = field.DEF_VAL;
                                        isDefault = true;
                                    }
                                    else
                                    {
                                        value = dr[field.FIELD_Name];
                                        isDefault = false;
                                    }
                                }
                                else
                                {
                                    value = field.DEF_VAL;
                                    isDefault = false;
                                }
                                FieldsValueModel fieldValue = new FieldsValueModel(field, value, domains, isMultiSelect,
                                    isDefault);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            if (!string.IsNullOrEmpty(objClass.AdressField))
                            {
                                Fields addressField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "ADDRESS_FIELD",
                                    FIELD_Caption = "آدرس",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(addressField,
                                    dr[objClass.AdressField], null, false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            if (!string.IsNullOrEmpty(objClass.SupervisorField))
                            {
                                Fields supervisorField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "SUPERVISOR_FIELD",
                                    FIELD_Caption = "رای ناظر",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(supervisorField,
                                    dr[objClass.SupervisorField], null, false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                            if (!string.IsNullOrEmpty(objClass.SupervisorDateOfField))
                            {
                                Fields supervisorField = new Fields()
                                {
                                    FIELD_Type = "TEXT",
                                    FIELD_Name = "SUPERVISOR_DATOF_FIELD",
                                    FIELD_Caption = "تاریخ نظارت",
                                    REQUIERD = false
                                };
                                FieldsValueModel fieldValue = new FieldsValueModel(supervisorField,
                                    dr[objClass.SupervisorDateOfField], null, false, false);
                                editModel.FieldsValue.Add(fieldValue);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return editModel;
        }
    }
}
