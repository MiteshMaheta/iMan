using System;
using iMan.Data;
using Prism;
using Prism.Unity;
using Prism.Navigation;
using Prism.Ioc;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using iMan.Helpers;
using iMan.Pages.Views;

namespace iMan
{
    public partial class App : PrismApplication
    {
        public static SQLiteAsyncConnection Connection;
        public static DatabaseHelper DbHelper;
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            try
            {
                InitializeComponent();
                DbHelper = new DatabaseHelper();
                GetConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected async override void OnStart()
        {
            base.OnStart();
            bool isFirst = true;
            List<string> sqlFiles = TableInfo.Tables;
            if (sqlFiles != null && sqlFiles.Count > 0)
            {
                List<Info<string>> sqlexecuted = new List<Info<string>>();
                if (await App.DbHelper.GetTableCount() > 1)
                {
                    sqlexecuted = await App.DbHelper.GetScriptsLoaded();
                    isFirst = false;
                }
                foreach (string item in sqlFiles)
                {
                    if (sqlexecuted != null && sqlexecuted.Count(e => e.key.Equals(item)) == 0)
                    {
                        string file = DependencyService.Get<IFileHelper>().GetFile(item);
                        if (!string.IsNullOrEmpty(file))
                        {
                            List<string> queries = new List<string>(file.Split(';'));
                            foreach (string query in queries)
                            {
                                if (!string.IsNullOrEmpty(query))
                                {
                                    await DbHelper.ExecuteQuery(query);
                                }
                            }
                            await DbHelper.SaveInfo(item, "script");
                        }
                    }
                    //await App.Connection.CloseAsync();
                }
            }
            if (isFirst)
            {
                await DbHelper.SaveInfo("imageCompress", bool.TrueString);
            }
            Authenticate();
            await NavigationService.NavigateAsync("LoginPage");
            Xamarin.Forms.MessagingCenter.Subscribe<ActivityResult>(this, "success", Navigate);
        }

        public async void Navigate(ActivityResult obj)
        {
            if (obj.ResultCode.ToString() == "Ok")
            {
                string res = await DbHelper.GetInfo<string>("imageCompress");
                if (res == bool.TrueString)
                    await NavigationService.NavigateAsync("//MasterPage/NavigationPage/MainPage");
                else
                {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("imageCompress", true);
                    await NavigationService.NavigateAsync("//NavigationPage/UpgradePage", parameters);
                }
            }
            else if (obj.ResultCode.ToString() == "Cancel")
                Xamarin.Forms.DependencyService.Get<ISystemHelper>().CloseApp();
        }

        public static void Authenticate()
        {
            Xamarin.Forms.DependencyService.Get<IAuthHelper>().Authenticate(1);
        }



        public static void GetConnection()
        {
            if (Connection == null)
                Connection = DependencyService.Get<ISqlite>().GetConnection();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<AppMasterPage>();

            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<ProductAddPage>();
            containerRegistry.RegisterForNavigation<ProductDetailPage>();
            containerRegistry.RegisterForNavigation<ProductEditPage>();

            containerRegistry.RegisterForNavigation<ItemPage>();
            containerRegistry.RegisterForNavigation<ItemAddPage>();
            containerRegistry.RegisterForNavigation<ItemEditPage>();

            containerRegistry.RegisterForNavigation<CategoryPage>();
            
            containerRegistry.RegisterForNavigation<PartyPage>();
            containerRegistry.RegisterForNavigation<PartyAddPage>();

            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<UpgradePage>();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
