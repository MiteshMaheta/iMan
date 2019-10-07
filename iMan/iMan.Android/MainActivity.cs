using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Crashlytics;
using Android.Content;
using iMan.Data;
using Com.Theartofdev.Edmodo.Cropper;
using Plugin.Permissions;
using iMan.Pages.ViewModels;

namespace iMan.Droid
{
    [Activity(Label = "iMan", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            Stormlion.ImageCropper.Droid.Platform.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            LoadApplication(new App());

            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
                 Crashlytics.Crashlytics.LogException(MonoExceptionHelper.Create(e.ExceptionObject as Exception));

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Canceled)
                {
                    Xamarin.Forms.MessagingCenter.Send(new ActivityResult { RequestCode = requestCode, ResultCode = resultCode, Data = data }, ActivityResult.key);
                }
            }
            else if (requestCode == 1)
            {
                Xamarin.Forms.MessagingCenter.Send(new ActivityResult { RequestCode = requestCode, ResultCode = resultCode, Data = data }, "success");
            }
            else
            {
                if (resultCode != Result.Canceled)
                {
                    CropImage.ActivityResult result = CropImage.GetActivityResult(data);
                    ProductAddPageViewModel.Success?.Invoke(result.Uri.Path, result.OriginalUri.Path);
                }
            }
        }

    }
}