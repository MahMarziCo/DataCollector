using Mah.DataCollector.Entity.Entities;
using DataAccess.Logic;
using DataCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mah.DataCollector.Web.Controllers
{
    public class ClassesController : Controller
    {
        private ClassesBL _ClassesBL;
        private UniqueStyleBL _UniqueStyleBL;
        public ClassesController(ClassesBL classesBl, UniqueStyleBL uniqueStyleBL)
        {
            _ClassesBL = classesBl;
            _UniqueStyleBL = uniqueStyleBL;
        }
        // GET: Classes
        public JsonResult GetClasses()
        {
            List<Classes> classes = _ClassesBL.GetAllClass();

           /* List<Classes> oList = new List<Classes>();
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
                    SupervisorField = classItem.SupervisorField,
                    SupervisorDateOfField = classItem.SupervisorDateOfField,
                    DateOf = classItem.DateOf,
                    UserId = classItem.UserId
                });
            }*/
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassUniqueStyle(string ClassName)
        {
            List<UniqueStyle> uniqueStyles = _UniqueStyleBL.GetClassUniqueStyle(ClassName);
            List<UniqueStyle> oList = new List<UniqueStyle>();
            foreach (UniqueStyle item in uniqueStyles)
            {
                oList.Add(new UniqueStyle()
                {
                    ID = item.ID,
                    CLASS_ID = item.CLASS_ID,
                    TEXT = item.TEXT,
                    FillColor = item.FillColor,
                    StrokColor = item.StrokColor,
                    Width = item.Width,
                    StrokWidth = item.StrokWidth
                });
            }
            return Json(oList, JsonRequestBehavior.AllowGet);
        }
    }
}