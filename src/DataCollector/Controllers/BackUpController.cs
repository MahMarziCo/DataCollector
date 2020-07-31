using DataAccess.Logic;
using Mah.DataCollector.Web.Models.Backups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Mah.DataCollector.Web.Controllers
{
    public class BackUpController : Controller
    {
        private SettingBL _SettingBL;
        public BackUpController(SettingBL settingBL)
        {
            _SettingBL = settingBL;
        }
        // GET: CheckingBackUp
        [Route("backups")]
        public ActionResult Index()
        {
            object dirPath = _SettingBL.getSettingAsString(SettingBL.SettingParameters.BackUpPath, "SYSTEM");
            return View("index", dirPath);
        }

        public JsonResult GetDirectoryContent(string directoryPath)
        {
            string dirPath = _SettingBL.getSettingAsString(SettingBL.SettingParameters.BackUpPath, "SYSTEM");
            if (!directoryPath.ToUpper().StartsWith(dirPath.ToUpper()))
                throw new Exception("Path is invalid");

                var directories = Directory.EnumerateDirectories(directoryPath);
                var files = Directory.EnumerateFiles(directoryPath);
                List<DirModel> drList = new List<DirModel>();
                List<FileModel> fileList = new List<FileModel>();
                foreach (var direct in directories)
                {
                    DirModel dir = new DirModel();
                    DirectoryInfo d = new DirectoryInfo(direct);
                    dir.DirName = Path.GetFileName(direct);
                    dir.DirDirectory = Path.GetDirectoryName(direct).Replace("\\", "/");
                    dir.DirAccessed = d.LastAccessTime.ToString("MM/dd/yyyy H:mm");
                    drList.Add(dir);
                }
                foreach (var file in files)
                {
                    FileModel fileModel = new FileModel();
                    FileInfo f = new FileInfo(file);
                    fileModel.FileName = Path.GetFileName(file);
                    fileModel.FileSizeText = GetFileSizeString(f.Length, "B");
                    fileModel.FileAccessed = f.LastAccessTime.ToString("yyyy/MM/dd H:mm");
                    fileList.Add(fileModel);
                }
                FolderContentModel allContent = new FolderContentModel(drList, fileList);
                return Json(allContent, JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        public JsonResult GetFileContent(string Path)
        {
            string dirPath = _SettingBL.getSettingAsString(SettingBL.SettingParameters.BackUpPath, "SYSTEM");
            if (!Path.ToUpper().StartsWith(dirPath.ToUpper()))
                throw new Exception("Path is invalid");
            FileInfo fileInfo = new FileInfo(Path);
            if (fileInfo.Length>(1024*100))
                throw new Exception("The file is big");
            var text = System.IO.File.ReadAllLines(Path);
            return Json(text);

        }
        private string GetFileSizeString(long length, string sizeUnit)
        {
            var fileSize = $"{length}{sizeUnit}";
            if (length > 1024)
            {
                length = length / 1024;
                switch (sizeUnit)
                {
                    case "B":
                        fileSize = GetFileSizeString(length, "KB");
                        break;
                    case "KB":
                        fileSize = GetFileSizeString(length, "MB");
                        break;
                    case "MB":
                        fileSize = GetFileSizeString(length, "GB");
                        break;
                }
            }
            return fileSize;
        }
    }
}