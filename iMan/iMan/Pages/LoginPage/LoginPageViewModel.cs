using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iMan.Data;
using iMan.Helpers;
using iMan.Pages.ViewModels;
using Prism.Navigation;
using Prism.Services;

namespace iMan.Pages.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {}

        
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {

            await NavigationService.NavigateAsync("//AppMasterPage/NavigationPage/MainPage");

            //bool result = await setOrUpgradeDatabaseAndKey();
            //await Task.Delay(3000);
            //if (result)
            //{
            //    Xamarin.Forms.DependencyService.Get<IAuthHelper>().Authenticate(1);
            //    Xamarin.Forms.MessagingCenter.Subscribe<ActivityResult>(this, "success", Navigate);
            //}
            //else
            //{
            //    await DialogService.DisplayAlertAsync("Error", "DB Setting failed. Try again.", "Ok");
            //}
        }

        async Task<bool> setOrUpgradeDatabaseAndKey()
        {
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
                    if (sqlexecuted != null && !sqlexecuted.Any(e => e.key.Equals(item)))
                    {
                        string file = Xamarin.Forms.DependencyService.Get<IFileHelper>().GetFile(item);
                        if (!string.IsNullOrEmpty(file))
                        {
                            List<string> queries = new List<string>(file.Split(';'));
                            foreach (string query in queries)
                            {
                                if (!string.IsNullOrEmpty(query)) await App.DbHelper.ExecuteQuery(query);
                            }
                            await App.DbHelper.SaveInfo(item, "script");
                        }
                    }
                }
            }
            if (isFirst)
            {
                await App.DbHelper.SaveInfo("imageCompress", bool.TrueString);
            }

            return true;
        }

        public async void Navigate(ActivityResult obj)
        {
            if (obj.ResultCode.ToString() == "Ok")
            {
                string res = await App.DbHelper.GetInfo<string>("imageCompress");
                if (res == bool.TrueString)
                    await NavigationService.NavigateAsync("//AppMasterPage/NavigationPage/MainPage");
                else
                {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("imageCompress", true);
                    await NavigationService.NavigateAsync("/NavigationPage/UpgradePage", parameters);
                }
            }
            else Xamarin.Forms.DependencyService.Get<ISystemHelper>().CloseApp();
        }

    }
}
