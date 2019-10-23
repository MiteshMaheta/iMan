using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using iMan.Helpers;

namespace iMan.Pages.ViewModels
{
    public class ProductDetailPageViewModel : ViewModelBase
    {
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand ShareCommand { get; set; }
        public DelegateCommand OnImageTapped { get; set; }
        public DelegateCommand ShareImage { get; set; }
        public DelegateCommand EditProduct { get; set; }

        public ProductDetailPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            OnImageTapped = new DelegateCommand(OpenViewer);
            ShareImage = new DelegateCommand(ShowCaptionDialog);
            EditProduct = new DelegateCommand(Edit);
            DeleteCommand = new DelegateCommand(Delete);

            ShareCommand = new DelegateCommand(ShareProduct);
            CancelCommand = new DelegateCommand(CancelProductShare);

            IsFull = false;
        }

        #region Properties
        private bool TempIsFull;
        public bool IsFull
        {
            get { return TempIsFull; }
            set { SetProperty(ref TempIsFull, value); }
        }

        private Product TempProduct;
        public Product Product
        {
            get { return TempProduct; }
            set { SetProperty(ref TempProduct, value); }
        }
        private bool TempIsShare;
        public bool IsShare
        {
            get { return TempIsShare; }
            set { SetProperty(ref TempIsShare, value); }
        }
        private string TempShareCaptionText;
        public string ShareCaptionText
        {
            get { return TempShareCaptionText; }
            set { SetProperty(ref TempShareCaptionText, value); }
        }
        #endregion

        public void OpenViewer()
        {
            if (!string.IsNullOrEmpty(Product.ImgName))
            {
                if (IsFull)
                    IsFull = false;
                else
                    IsFull = true;
            }
        }

        public async void ShowCaptionDialog()
        {
            if (Product != null && !string.IsNullOrEmpty(Product.ImgName))
            {
                IsShare = true;
            }
            else
            {
                await DialogService.DisplayAlertAsync("Alert", "Product/Image not proper.", "Ok");
            }
        }
        void ShareProduct()
        {
            IsShare = false;
            Xamarin.Forms.DependencyService.Get<IShareHelper>().SharePicture(Product.OriginalImgSource, ShareCaptionText);
        }
        void CancelProductShare()
        {
            Xamarin.Forms.DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            IsShare = false;
            ShareCaptionText= null;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Product"))
            {
                Product = parameters["Product"] as Product;
            }
        }


        public void Edit()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("piece", Product);
            NavigationService.NavigateAsync("ProductEditPage", parameters);
        }

        public async void Delete()
        {
            bool res = await DialogService.DisplayAlertAsync("Confirm", "Do you want to Delete?", "Delete", "Cancel");
            if (res)
            {
                int deleted = await App.DbHelper.DeleteProduct(int.Parse(Product.Id));
                if (deleted > 0)
                {
                    Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(Product.OriginalImgSource);
                    Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(Product.CompressImgSource);
                }
                Xamarin.Forms.MessagingCenter.Send<Product>(Product, "added");
                await NavigationService.GoBackAsync();
            }
        }
    }
}
