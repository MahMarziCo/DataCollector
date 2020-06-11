using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mah.Common.Logger
{
    public interface ILogger
    {
        void LogError(Exception ex, string message);
        void LogInfo( string message);
    }
}
