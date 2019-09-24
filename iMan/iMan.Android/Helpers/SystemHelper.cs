using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iMan.Droid.Helpers;
using iMan.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(SystemHelper))]
namespace iMan.Droid.Helpers
{
    class SystemHelper : ISystemHelper
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}