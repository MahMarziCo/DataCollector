using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mah.DataCollector.Entity.Entities;
using System.IO;

namespace DataAccess.Logic
{
    public class FeaturePicBL
    {
        private DataCollectorContext _DbContext;
        private UpdateLogBL _UpdateLogBL;
        public FeaturePicBL(DataCollectorContext context, UpdateLogBL updateLogBL)
        {
            _DbContext = context;
            _UpdateLogBL =updateLogBL;
        }

        public void InsertFeaturePic(string fileName,
            string userName, 
            string caption,
            string className,
            int objectid
            )
        {
            
            Feature_Pic pic = _DbContext.Feature_Pic.Create();
            pic.Save_ID = fileName;
            pic.USER_NAME = userName;
            pic.Name_Of =caption ;
            pic.DateOf = DateTime.Now;
            pic.Class_name =className ;
            pic.Feature_ID = objectid;
            _DbContext.Feature_Pic.Add(pic);
            _DbContext.SaveChanges();
        }
        public  List<Feature_Pic> GetFeaturePic(string ClassName, int Objectid)
        {
            List<Feature_Pic> list = new List<Feature_Pic>();
            try
            {

                list = _DbContext.Feature_Pic.Where(a => a.Class_name == ClassName && a.Feature_ID == Objectid).Select(a => a).ToList();
                

            }
            catch { }
            return list;
        }

        public  void DeleteImages(int OID, string serverPath, string pUserName)
        {

            var picture = _DbContext.Feature_Pic.Where(a => a.ID == OID).First();
                try
                {
                    string filePath = Path.Combine(serverPath, picture.Save_ID);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    _UpdateLogBL.Log(pUserName, picture.Class_name, picture.Feature_ID ?? -1, "DELETE_PIC", picture.Save_ID);
                }
                catch { }
                _DbContext.Feature_Pic.Remove(picture);
                _DbContext.SaveChanges();
            

        }
    }
}
