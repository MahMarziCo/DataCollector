using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Logg
{
    public class ErrorLogg
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public DateTime DateOf { get; set; }
        public string LogLevel { get; set; }
    }
}
