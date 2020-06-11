using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataAccess.Logic
{
    public class FieldsBL
    {
        private DataCollectorContext _DbContext;
        private ClassesBL _ClassesBl;
        public FieldsBL(DataCollectorContext context, ClassesBL classesBl)
        {
            _DbContext = context;
            _ClassesBl = classesBl;
        }
        public Fields GetFieldsByID(int FieldId)
        {

            Fields field = null;
            try
            {

                List<Fields> oList = _DbContext.Fields.Where(c => c.ID == FieldId).ToList();

                if (oList.Count > 0)
                {
                    field = oList[0];
                }

            }
            catch
            {
            }
            return field;
        }
        public List<Fields> getClassFields(string pClassName)
        {
            try
            {
                Classes oClass = _ClassesBl.getClassByName(pClassName);
                return getClassFields(oClass);
            }
            catch
            {
                return new List<Fields>();
            }
        }
        public List<Fields> getClassFields(Classes pClass)
        {
            try
            {
                return _DbContext.Fields.Where(c => c.Class_ID == pClass.ID).OrderBy(a => a.ORDER).ToList();
            }
            catch
            {
                return new List<Fields>();
            }
        }

        public bool ValidateFieldValue(Fields field, object value, out string error)
        {
            if (field.REQUIERD == true && string.IsNullOrEmpty(value.ToString().Trim()))
            {
                error = "مقدار این فیلد لازم است";
                return false;
            }

            if (field.FIELD_Type == "DOUBLE" && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                double number;
                if (!double.TryParse(value.ToString().Trim(), out number))
                {
                    error = "مقدار عددی باید وارد شود";
                    return false;
                }
            }

            if (field.FIELD_Type == "INT" && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                int number;
                if (!int.TryParse(value.ToString().Trim(), out number))
                {
                    error = "عدد صحیح باید وارد شود";
                    return false;
                }
            }

            if (field.MAX_VALUE != null && value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                if (field.FIELD_Type == "TEXT" && !string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    if (field.MAX_VALUE != null && value.ToString().Length > field.MAX_VALUE)
                    {
                        error = "حد اکثر طول این فیلد " + field.MAX_VALUE + " حرف است";
                        return false;
                    }
                }
                else if (double.Parse(value.ToString().Trim()) > field.MAX_VALUE)
                {
                    error = string.Format("مقدار عددی باید کوچکتر از {0}  باشد", field.MAX_VALUE);
                    return false;
                }
            }
            if (field.MIN_VALUE != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                if (double.Parse(value.ToString().Trim()) < field.MIN_VALUE)
                {
                    error = string.Format("مقدار عددی باید بزرگتر از {0} باشد", field.MIN_VALUE);
                    return false;
                }
            }
            error = "";
            return true;
        }

        public int createNewFields(int? Class_ID, string FIELD_Name,
            string FIELD_Type, string FIELD_Caption,
            int? Domain_ID, double? MAX_VALUE, double? MIN_VALUE, int? ORDER, bool? REQUIERD, string pDefVal)
        {

            Fields newField = _DbContext.Fields.Create();
            newField.Class_ID = Class_ID;
            newField.FIELD_Name = FIELD_Name;
            newField.FIELD_Type = FIELD_Type;
            newField.FIELD_Caption = FIELD_Caption;
            newField.Domain_ID = Domain_ID;
            newField.MAX_VALUE = MAX_VALUE;
            newField.MIN_VALUE = MIN_VALUE;
            newField.ORDER = ORDER;
            newField.REQUIERD = REQUIERD;
            newField.DEF_VAL = pDefVal;
            _DbContext.Fields.Add(newField);
            _DbContext.SaveChanges();
            return newField.ID;

        }

        public bool DeleteField(int pID)
        {
            try
            {

                Fields field = new Fields() { ID = pID };
                _DbContext.Fields.Attach(field);
                _DbContext.Fields.Remove(field);
                _DbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Fields pField)
        {
            try
            {
                _DbContext.Entry(pField).State = EntityState.Modified;
                _DbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
