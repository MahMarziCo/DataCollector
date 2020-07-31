using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mah.DataCollector.Web.Models.Backups
{
    public class FolderContentModel
    {
        public List<DirModel> DirModelList;
        public List<FileModel> FileModelList;
        public FolderContentModel(List<DirModel> dirModelList, List<FileModel> fileModelsList)
        {
            DirModelList = dirModelList;
            FileModelList = fileModelsList;
        }

    }
}