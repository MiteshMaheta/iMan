﻿using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using iMan.Helpers;

namespace iMan.Pages.ViewModels
{
    public class ProductDetailPageViewModel : ViewModelBase
    {
        public ProductDetailPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            OnImageTapped = new DelegateCommand(OpenViewer);
            ShareImage = new DelegateCommand(Share);
            EditProduct = new DelegateCommand(Edit);
            DeleteCommand = new DelegateCommand(Delete);
            IsFull = false;
        }

        public DelegateCommand OnImageTapped { get; set; }
        public DelegateCommand ShareImage { get; set; }
        public DelegateCommand EditProduct { get; set; }
        //public DelegateCommand DeleteCommand { get; set; }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            set { SetProperty(ref isFull, value); }
        }

        private Product product;
        public Product Product
        {
            get { return product; }
            set { SetProperty(ref product, value); }
        }

        public void OpenViewer()
        {
            if (IsFull)
                IsFull = false;
            else
                IsFull = true;
        }

        public async void Share()
        {
            if (Product != null && !string.IsNullOrEmpty(Product.ImgName))
            {
                await NavigationService.NavigateAsync("PopupPage");
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Product"))
            {
                Product = parameters["Product"] as Product;
            }
            if (parameters.ContainsKey("Text"))
            {
                string text = parameters["Text"] as string;
                Xamarin.Forms.DependencyService.Get<IShareHelper>().SharePicture(Product.ImgName, text);
            }
        }


        public void Edit()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("piece", Product);
            NavigationService.NavigateAsync("ProductEditPage", parameters, true, false);
        }

        public async void Delete()
        {
            bool res = await DialogService.DisplayAlertAsync("Confirm", "Do you want to Delete?", "Delete", "Cancel");
            if (res)
            {
                int deleted = await App.DbHelper.DeleteProduct(int.Parse(Product.Id));
                if (deleted > 0)
                {
                    Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(Product.ImgName);
                }
                await NavigationService.GoBackAsync();
            }
        }
    }
}
