using System;
using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using iMan.Droid.Helpers;
using iMan.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(DroidKeyboardHelper))]
namespace iMan.Droid.Helpers
{
    public class DroidKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var context = MainApplication.CurrentContext;

            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}
