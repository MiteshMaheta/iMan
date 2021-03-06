﻿using System;
using Prism.Navigation;
using Prism.Services;
using Prism.Commands;
using iMan.Data;
using iMan.Helpers;
using Plugin.FilePicker.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace iMan.Pages.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            ViewItem = new DelegateCommand(ViewItems);
            ViewParty = new DelegateCommand(ViewParties);
            Xamarin.Forms.MessagingCenter.Subscribe<ActivityResult>(this, ActivityResult.key, OnAuthFailed);
            BackupDbCommand = new DelegateCommand(BackupDbAsync);
            RestoreDbCommand = new DelegateCommand(RestoreDb);
            IsBusy = false;
        }

        public void OnAuthFailed(ActivityResult obj)
        {
            NavigationService.NavigateAsync("//AppMasterPage/NavigationPage/MainPage");
        }

        public DelegateCommand ViewItem { get; set; }
        public DelegateCommand ViewParty { get; set; }
        public DelegateCommand BackupDbCommand { get; set; }
        public DelegateCommand RestoreDbCommand { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public void ViewItems()
        {
            NavigationService.NavigateAsync("CategoryPage");
        }

        public void ViewParties()
        {
            NavigationService.NavigateAsync("PartyPage");
        }

        public async void BackupDbAsync()
        {
            IsBusy = true;
            await Task.Delay(500);
            bool res = await Xamarin.Forms.DependencyService.Get<IFileHelper>().SaveZipToFolder();
            if (res)
            {
                await DialogService.DisplayAlertAsync("Success", "Data backup sucessfull", "Ok");
            }
            else

                await DialogService.DisplayAlertAsync("Alert", "Data backup not sucessfull", "Ok");
            IsBusy = false;
        }

        public async void RestoreDb()
        {
            bool res = await DialogService.DisplayAlertAsync("Confirm", "Restoring the previous data will replace all your current data.\n Do you wish to continue?", "Yes", "No");
            if (res)
            {
                FileData file = null;
                try
                {
                    file = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (file == null)
                    return;

                IsBusy = true;
                List<byte> dataArray = new List<byte>(file.DataArray);
                await UnZipDb(dataArray,file.FileName);

                //Stream s = file.GetStream();
                //UnZipDb(s);
            }
        }

        public async Task UnZipDb(List<byte> dataArray,string fileName)
        {
            bool restore = await Xamarin.Forms.DependencyService.Get<IFileHelper>().UnzipDb(dataArray.ToArray(), fileName);
            IsBusy = false;
            if (restore)
            {
                await DialogService.DisplayAlertAsync("Success", "Data restored successfully", "Ok");
            }
            else
            {
                await DialogService.DisplayAlertAsync("Alert", "There is some issue in restoring", "Ok");
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.GetNavigationMode().Equals(NavigationMode.New))
                Xamarin.Forms.DependencyService.Get<IAuthHelper>().Authenticate(0);
        }
    }
}
