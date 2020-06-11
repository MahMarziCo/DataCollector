using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Logic
{
    public class UpdateLogBL
    {
        private DataCollectorContext _DbContext;
        public UpdateLogBL(DataCollectorContext context)
        {
            _DbContext = context;
        }
        public  bool Log(string userName, string className, int objectid,string pActionOf,string pDataOf)
        {
            try
            {
                Update_Log log = _DbContext.Update_Log.Create();
                    log.DateTime = DateTime.Now;
                    log.User_NAME = userName;
                    log.Class_name = className;
                    log.Feature_ID = objectid;
                    log.ActionOf = pActionOf;
                    log.DataOf = pDataOf;
                    _DbContext.Update_Log.Add(log);
                    _DbContext.SaveChanges();
                
                return true;
            }
            catch { return false; }
        }
    }
}
