using System;
using System.IO;
using Xamarin.Forms;
using System.Diagnostics;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Android.Services;

[assembly: Dependency(typeof(DeviceStorageService_Droid))]
namespace ASWorkoutTracker.Android.Services
{
    public class DeviceStorageService_Droid : IDeviceStorageService
    {
        private string RootPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public string GetAbsoluteFilePath(string filePath)
        {
            return Path.Combine(RootPath, filePath);
        }

        public bool FileExists(string absolutePath)
        {
            return File.Exists(absolutePath);
        }

        public Stream ReadFile(string absolutePath)
        {
            return File.OpenRead(GetAbsoluteFilePath(absolutePath));
        }

        public Stream CreateOrWriteFile(string absolutePath)
        {
            return File.OpenWrite(GetAbsoluteFilePath(absolutePath));
        }

        public void DeleteFile(string absoluteFilePath)
        {
            if (FileExists(absoluteFilePath)) File.Delete(absoluteFilePath);
        }

        public string GetDatabaseFileLocation(string fileName)
        {
            string folderPath = GetDatabaseFolderLocation();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.WriteLine($"***** Android DeviceStorageService created folder path");
            }

            string fullPath = Path.Combine(folderPath, fileName);

            Debug.WriteLine($"***** Android DeviceStorageService folderPath: { fullPath }");
            return fullPath;
        }

        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        public string GetDatabaseFolderLocation()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}
