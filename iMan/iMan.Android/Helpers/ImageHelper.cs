using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iMan.Helpers;
using Java.IO;
using Plugin.CurrentActivity;
using iMan.Droid.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageHelper))]
namespace iMan.Droid.Helpers
{
    class ImageHelper : IImageHelper
    {
        public async Task<string> ResizeImage(string imageArray)
        {
            try
            {
                string environmentPath = CrossCurrentActivity.Current.AppContext.ApplicationContext.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                if (Directory.Exists(environmentPath))
                {

                    Bitmap originalImage = await BitmapFactory.DecodeFileAsync(environmentPath + "/" + imageArray);
                    if (originalImage != null)
                    {
                        using (System.IO.MemoryStream stream = new MemoryStream())
                        {
                            stream.Position = 0;
                            originalImage.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                            byte[] byteArray = stream.GetBuffer();
                            string path = environmentPath + "/../Compress/";
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                                Java.IO.File file = new Java.IO.File(path + ".nomedia");
                                file.CreateNewFile();
                            }
                            using (var newFile = new Java.IO.File(path + imageArray))
                            {
                                newFile.CreateNewFile();
                                // TODO : Update visual studio and enable async method to write new file.
                                //await System.IO.File.WriteAllBytesAsync(newFile.Path,byteArray);
                                System.IO.File.WriteAllBytes(newFile.Path, byteArray);
                            }
                        }
                        return imageArray;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }

        public string GetCompressImagePath()
        {
            string environmentPath = CrossCurrentActivity.Current.AppContext.ApplicationContext.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            if (Directory.Exists(environmentPath))
            {
                string path = environmentPath + "/../Compress/";
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        public string GetOriginalImagePath()
        {
            string environmentPath = CrossCurrentActivity.Current.AppContext.ApplicationContext.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            if (Directory.Exists(environmentPath))
            {
                string path = environmentPath + "/";
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }
    }
}