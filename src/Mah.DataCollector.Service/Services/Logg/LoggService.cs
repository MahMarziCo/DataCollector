using DataAccess.Entities.Logg;
using Mah.Common.Logger;
using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mah.DataCollector.Service.Services.Log
{
    public class LoggService : ILogger
    {
        DataCollectorContext _Context;
        public LoggService(DataCollectorContext context)
        {
            _Context = context;
        }

        public void LogError(Exception ex, string message)
        {
           var entity = new ErrorLogg();
           entity.DateOf = DateTime.Now;
           entity.LogLevel = "Error";
           entity.Message = message;
           entity.Stack = ex.ToString();
           _Context.Set<ErrorLogg>().Add(entity);
           _Context.SaveChanges();
        }

        public void LogInfo(string message)
        {
            var entity = new ErrorLogg();
            entity.DateOf = DateTime.Now;
            entity.LogLevel = "Info";
            entity.Message = message;
            entity.Stack = "";
            _Context.Set<ErrorLogg>().Add(entity);
            _Context.SaveChanges();
        }
    }
}
