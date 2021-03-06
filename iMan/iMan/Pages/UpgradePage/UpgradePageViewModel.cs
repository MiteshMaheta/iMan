﻿using iMan.Data;
using iMan.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iMan.Pages.ViewModels
{
    public class UpgradePageViewModel : ViewModelBase
    {
        private bool isComplete;
        public bool IsComplete
        {
            get { return isComplete; }
            set { SetProperty(ref isComplete, value); }
        }

        private string updateText;
        public string UpdateText
        {
            get { return updateText; }
            set { SetProperty(ref updateText, value); }
        }

        public DelegateCommand OnNextTap { get; set; }
        public UpgradePageViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            IsComplete = false;
            OnNextTap = new DelegateCommand(GoToHome);
            UpdateText = "iMan is updating...";
        }

        public async void GoToHome()
        {
            await App.DbHelper.SaveInfo<string>("imageCompress", bool.TrueString);
            await NavigationService.NavigateAsync("//AppMasterPage/NavigationPage/MainPage");
        }

        public async void ImageCompress()
        {
            IsComplete = false;
            List<Product> products = await App.DbHelper.GetAllProducts();
            foreach (Product item in products)
            {
                if(item!=null && !string.IsNullOrEmpty(item.ImgName) && !File.Exists(item.CompressImgSource))
                {
                    string imgName = item.ImgName.Split('/')?.LastOrDefault();
                    await Xamarin.Forms.DependencyService.Get<IImageHelper>().ResizeImage(imgName);
                }
            }
            UpdateText = "Please press on 'Next' button to continue";
            IsComplete = true;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.ContainsKey("imageCompress"))
            {
                ImageCompress();
            }
        }
    }
}
