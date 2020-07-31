using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mah.DataCollector.Web.Models.Backups
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string FileSizeText { get; set; }
        public string FileAccessed { get; set; }
    }
}