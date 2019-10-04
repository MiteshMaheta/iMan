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
            await NavigationService.NavigateAsync("/LoginPage");
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
