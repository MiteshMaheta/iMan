using System;
using System.IO;
using SQLite;
using iMan.Helpers;
using Xamarin.Forms;

namespace iMan.iOS.Helpers
{
    public class SqliteHelper : ISqlite
    {
        public SqliteHelper()
        {
        }

        static string DBName = "priceCalculator.sqlite";

        public SQLiteAsyncConnection GetConnection()
        {
            try
            {
                //var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal, System.Environment.SpecialFolderOption.Create), DBName);
                //Console.WriteLine("Database Path:" + Path.GetFullPath(path));
                //if (!File.Exists(path))
                //{
                //    using (var asset = Android.App.Application.Context.Assets.Open(DBName))
                //    using (var dest = File.Create(path))
                //        asset.CopyTo(dest);
                //    // return new SQLiteAsyncConnection(path);
                //}
                //return new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
