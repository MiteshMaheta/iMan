using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Plugin.CurrentActivity;
using Plugin.Permissions.Abstractions;
using iMan.Droid.Helpers;
using iMan.Helpers;
using Xamarin.Forms;
using Plugin.FilePicker.Abstractions;

[assembly: Dependency(typeof(FileHelper))]
namespace iMan.Droid.Helpers
{
    public class FileHelper : IFileHelper
    {
        public string GetFile(string fileName)
        {
            string fileContent = null;
            using (StreamReader sr = new StreamReader(Android.App.Application.Context.Assets.Open(fileName)))
            {
                fileContent = sr.ReadToEnd();
            }
            return fileContent;
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RenameFile(string oldFilename, string newFileName)
        {
            try
            {
                File.Move(newFileName, oldFilename);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveZipToFolder()
        {
            try
            {
                var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "priceCalculator.sqlite");
                if (File.Exists(path))
                {
                    PermissionStatus status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    if (!status.Equals(PermissionStatus.Granted))
                        await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    if (App.Connection != null)
                        await App.Connection.CloseAsync();
                    FileStream fsOut = File.Create(Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "price_backup.zip"));
                    ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                    //FastZip zip = new FastZip();
                    //zip.CreateZip( "price_backup.zip",Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path,true,null);
                    //zip.CreateEmptyDirectories = true;
                    zipStream.SetLevel(6);
                    zipStream.Password = "backup";
                    //FileInfo db = new FileInfo(path);
                    ZipEntry dbEntry = new ZipEntry(ZipEntry.CleanName(path.Split("/").LastOrDefault()));
                    //zip.
                    zipStream.PutNextEntry(dbEntry);
                    byte[] dbBuffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(path))
                    {
                        StreamUtils.Copy(streamReader, zipStream, dbBuffer);
                    }
                    zipStream.CloseEntry();
                    string imgDir = CrossCurrentActivity.Current.AppContext.GetExternalFilesDir(null).Path;
                    if (Directory.Exists(imgDir))
                    {
                        string[] files = Directory.GetDirectories(imgDir);
                        foreach (var item in files)
                        {
                            if (!item.Contains("override"))
                            {
                                string folderName = ZipEntry.CleanName(item.Split("/").LastOrDefault());
                                string[] file = Directory.GetFiles(item);
                                foreach (var fileName in file)
                                {
                                    ZipEntry zipEntry = new ZipEntry(ZipEntry.CleanName(folderName + "/" + fileName.Split("/").LastOrDefault()));
                                    byte[] buffer = new byte[4096];
                                    zipStream.PutNextEntry(zipEntry);
                                    using (FileStream fileReader = File.OpenRead(fileName))
                                    {
                                        StreamUtils.Copy(fileReader, zipStream, buffer);
                                    }
                                    zipStream.CloseEntry();
                                }
                            }
                        }
                    }
                    zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                    zipStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UnzipDb(byte[] zipFile, string fileName)
        {

            byte[] data = zipFile;

            Stream fileStream = new MemoryStream(data);
            //ZipFile zf = new ZipFile(zipFile);
            //zf.Password = "backup";
            //if (!zf.GetEntry("priceCalculator.sqlite").Name.Equals("priceCalculator.sqlite"))
            //    return false;

            if (App.Connection != null)
                await App.Connection.CloseAsync();
            List<string> folders = Directory.GetDirectories(CrossCurrentActivity.Current.AppContext.GetExternalFilesDir(null).Path).ToList();
            //files.AddRange(pics);
            //files.AddRange(compressedPics);
            foreach (var item in folders)
            {
                if (!item.Contains("override"))
                    Directory.Delete(item, true);
            }
            //foreach (ZipEntry item in zf)
            //{
            //    string fileName = item.Name;
            byte[] buffer = new byte[4096];
            //    Stream stream = zf.GetInputStream(item);
            string fullZipToPath = null;
                if (fileName.Contains(".sqlite"))
                {
                    fullZipToPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
                }
                //else
                //{
                //    if(fileName.Contains('/'))
                //        fullZipToPath = CrossCurrentActivity.Current.AppContext.GetExternalFilesDir(null).Path + "/" + fileName;
                //    else
                //        fullZipToPath = CrossCurrentActivity.Current.AppContext.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).Path + "/" + fileName;
                //}
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0 && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(fileStream, streamWriter, buffer);
                }
            //}
            return true;
            //}
            //else
            // {
            //}
            //await Task.Delay(5000);
            //return false;
        }
    }
}