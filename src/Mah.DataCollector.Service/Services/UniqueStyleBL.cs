using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Logic
{
    public class UniqueStyleBL
    {
        private DataCollectorContext _DbContext;
        private ClassesBL _ClassesBl;
        public UniqueStyleBL(DataCollectorContext context, ClassesBL classesBl)
        {
            _DbContext = context;
            _ClassesBl = classesBl;
        }
        public List<UniqueStyle> GetClassUniqueStyle(string pClassName)
        {
            Classes oClass = _ClassesBl.getClassByName(pClassName);
            return GetClassUniqueStyle(oClass);
        }
        public List<UniqueStyle> GetClassUniqueStyle(int pClassId)
        {
            Classes oClass = _ClassesBl.getClass(pClassId);
            return GetClassUniqueStyle(oClass);
        }

        public List<UniqueStyle> GetClassUniqueStyle(Classes pClass)
        {
            List<UniqueStyle> oList = new List<UniqueStyle>();

            foreach (var item in _DbContext.UniqueStyles.Where(a => a.CLASS_ID == pClass.ID).ToList())
            {

                oList.Add(new UniqueStyle()
                {
                    ID = item.ID,
                    CLASS_ID = item.CLASS_ID,
                    FillColor = item.FillColor,
                    StrokColor = item.StrokColor,
                    StrokWidth = item.StrokWidth,
                    TEXT = item.TEXT,
                    Width = item.Width
                });
            }

            return oList;
        }

        public bool DeleteUniqueStyles(int pID)
        {
            try
            {
                UniqueStyle delStyle = new UniqueStyle() { ID = pID };
                _DbContext.UniqueStyles.Attach(delStyle);
                _DbContext.UniqueStyles.Remove(delStyle);
                _DbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool DeleteUniqueStyles(UniqueStyle delStyle)
        {
            try
            {
                _DbContext.UniqueStyles.Attach(delStyle);
                _DbContext.UniqueStyles.Remove(delStyle);
                _DbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            { return false; }
        }

        public int AddNewUniqueStyle(int? pClassId, string pText, string pFillColor, string pStrokColor, double? pStrokWidth, double? pWidth)
        {
            try
            {
                UniqueStyle newStyle = new UniqueStyle();
                int id = -1;


                newStyle = _DbContext.UniqueStyles.Create();
                newStyle.CLASS_ID = pClassId;
                newStyle.TEXT = pText;
                newStyle.FillColor = pFillColor;
                newStyle.StrokColor = pStrokColor;
                newStyle.StrokWidth = pStrokWidth;
                newStyle.Width = pWidth;
                _DbContext.UniqueStyles.Add(newStyle);
                _DbContext.SaveChanges();
                id = newStyle.ID;

                return id;
            }
            catch { return -1; }
        }

        public bool UpdateUniqueStyle(UniqueStyle pUniqueStyles)
        {
            try
            {
                UniqueStyle updateStyle = new UniqueStyle() { ID = pUniqueStyles.ID };


                updateStyle = _DbContext.UniqueStyles.Attach(updateStyle);
                updateStyle.CLASS_ID = pUniqueStyles.CLASS_ID;
                updateStyle.TEXT = pUniqueStyles.TEXT;
                updateStyle.FillColor = pUniqueStyles.FillColor;
                updateStyle.StrokColor = pUniqueStyles.StrokColor;
                updateStyle.StrokWidth = pUniqueStyles.StrokWidth;
                updateStyle.Width = pUniqueStyles.Width;
                _DbContext.SaveChanges();

                return true;
            }
            catch { return false; }
        }
    }
}
