using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace DataAccess.Logic
{
    public class ClassesBL
    {
        private DataCollectorContext _DbContext;
        private string _GdbConnection;
        public ClassesBL(DataCollectorContext context, string gdbConnection)
        {
            _DbContext = context;
            _GdbConnection = gdbConnection;
        }
        public List<Classes> GetAllClass()
        {
            var oList = _DbContext.Classes.ToList();

            return oList;
        }
        public List<Classes> GetClassesWithUserField()
        {
            var oList = _DbContext.Classes.Where(a => !string.IsNullOrEmpty(a.UserId)).ToList();

            return oList;
        }
        public Classes getClass(int pClassID)
        {
            try
            {
                return _DbContext.Classes.Where(c => c.ID == pClassID).First();

            }
            catch
            {
                return null;
            }
        }

        public Classes getClassByName(string pClassName)
        {
            try
            {
                pClassName = pClassName.ToUpper();

                return _DbContext.Classes.Where(c => c.Class_name.ToUpper() == pClassName.ToUpper()).First();

            }
            catch
            {
                return null;
            }
        }

        public bool createNewClass(string pClassName, string pCaption, string pClass_type
            , string pSpatialRefrence, bool? RequieredPhoto)
        {
            try
            {
                Classes oClass = _DbContext.Classes.Create();
                oClass.Caption = pCaption;
                oClass.Class_name = pClassName;
                oClass.Class_type = pClass_type;
                oClass.SpatialRefrence = pSpatialRefrence;
                oClass.Scale = 5;
                oClass.FillColor = "#4287f5";
                oClass.StrokColor = "#2858a6";
                oClass.StrokWidth = 2;
                oClass.Width = 5;
                oClass.HasFlow = false;
                oClass.RequieredPhoto = RequieredPhoto;
                _DbContext.Classes.Add(oClass);
                _DbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateClass(Classes pClass)
        {
            try
            {
                var oClass = getClass(pClass.ID);
                oClass.Caption = pClass.Caption;
                oClass.Class_name = pClass.Class_name;
                oClass.Class_type = pClass.Class_type;
                oClass.SpatialRefrence = pClass.SpatialRefrence;
                oClass.RequieredPhoto = pClass.RequieredPhoto;

                _DbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateClassSymbol(Classes pClass)
        {
            try
            {
                _DbContext.Entry(pClass).State = System.Data.Entity.EntityState.Modified;

                _DbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteClass(int pId)
        {
            try
            {
                Classes oClass = new Classes() { ID = pId };
                _DbContext.Classes.Attach(oClass);
                _DbContext.Classes.Remove(oClass);
                _DbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<string> GetDistinctField(int classId, string FieldName)
        {
            List<string> values = new List<string>();
            try
            {
                Classes classes = this.getClass(classId);
                using (SqlConnection cnn = new SqlConnection(_GdbConnection))
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "select DISTINCT " + FieldName + " AS FIELD_VALUE from "
                            + classes.Class_name + " where " + FieldName + " is not null";

                        DataTable table = new DataTable();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            values.Add(reader["FIELD_VALUE"].ToString());
                        }
                    }
                }
            }
            catch
            {
            }
            return values;
        }

        public List<KeyValuePair<string, string>> GetClassFieldsFromDB(int classId)
        {
            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>();

            Classes classes = this.getClass(classId);
            using (SqlConnection cnn = new SqlConnection(_GdbConnection))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = $"SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{classes.Class_name}' ";

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        var columnName = reader.GetString(0).ToUpper();
                        var columnType = reader.GetString(1).ToUpper();
                        if (columnName  != "OBJECTID" && columnName != "SHAPE")
                        {
                            string fieldType = "";
                            switch (columnType)
                            {
                                case "INT":
                                case "SMALLINT":
                                    fieldType = "INT";
                                    break;
                                case "NVARCHAR":
                                case "VARCHAR":
                                    fieldType = "TEXT";
                                    break;
                                case "NUMERIC":
                                    fieldType = "DOUBLE";
                                    break;
                                case "DATETIME2":
                                case "DATETIME":
                                case "DATE":
                                    fieldType = "DATE";
                                    break;
                                default:
                                    fieldType = "TEXT";
                                    break;
                            }
                            fields.Add(new KeyValuePair<string, string>(columnName, fieldType));
                        }
                    }
                }
            }

            return fields;
        }

        public int GetTextFieldLengthFromDB(int classIs, string fieldName)
        {
            int length = 100000;
            if (classIs == -1) return length;
            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>();
            try
            {
                Classes classes = this.getClass(classIs);
                using (SqlConnection cnn = new SqlConnection(_GdbConnection))
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cnn.Open();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "select " + fieldName + " from " + classes.Class_name + " where 1=2";

                        DataTable table = new DataTable();
                        SqlDataReader reader = cmd.ExecuteReader();
                        table = reader.GetSchemaTable();
                        length = int.Parse(table.Rows[0]["ColumnSize"].ToString());
                    }
                }
            }
            catch { }
            return length;
        }
    }
}
