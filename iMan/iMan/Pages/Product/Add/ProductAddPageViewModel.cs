using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iMan.Data;
using Xamarin.Forms;
using Plugin.Media;
using iMan.Helpers;
using Plugin.Media.Abstractions;

namespace iMan.Pages.ViewModels
{
    public class ProductAddPageViewModel : ViewModelBase
    {
        public DelegateCommand OnImageTapped { get; set; }
        public DelegateCommand AddNewItem { get; set; }
        public DelegateCommand<object> AddTotalCommand { get; set; }
        public DelegateCommand<ItemUsed> RemoveItemCommand { get; set; }
        public DelegateCommand ShareImage { get; set; }
        public DelegateCommand ToggleRecentProductCommand { get; set; }

        public ProductAddPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Xamarin.Forms.MessagingCenter.Subscribe<Product>(this, "added", OnProductAdded);

            OnImageTapped = new DelegateCommand(OpenViewer);
            AddNewItem = new DelegateCommand(AddItem);
            AddTotalCommand = new DelegateCommand<object>(AddTotal);
            RemoveItemCommand = new DelegateCommand<ItemUsed>(RemoveItem);

            ToggleRecentProductCommand = new DelegateCommand(toggleItemList);

            setNewProduct();
            UnitList = ConstantData.UnitList;
            SaveCommand = new DelegateCommand(SaveProduct);
            IsFull = false;
        }

        #region Poperties

        private ImageSource TempImage;
        public ImageSource Image
        {
            get { return TempImage; }
            set { SetProperty(ref TempImage, value); }
        }

        private Product TempProduct;
        public Product Product
        {
            get { return TempProduct; }
            set { SetProperty(ref TempProduct, value); }
        }

        private List<string> TempUnitList;
        public List<string> UnitList
        {
            get { return TempUnitList; }
            set { SetProperty(ref TempUnitList, value); }
        }

        private List<Item> TempItemList;
        public static Action<string, string> Success { get; set; }
        public List<Item> ItemList
        {
            get { return TempItemList; }
            set { SetProperty(ref TempItemList, value); }
        }

        private List<Category> TempCategoryList;
        public List<Category> CategoryList
        {
            get { return TempCategoryList; }
            set { SetProperty(ref TempCategoryList, value); }
        }

        private Category TempSelectedCategory;
        public Category SelectedCategory
        {
            get { return TempSelectedCategory; }
            set
            {
                SetProperty(ref TempSelectedCategory, value);
                OnCategoryChanged(TempSelectedCategory);
                getAllProducts();
            }
        }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            set { SetProperty(ref isFull, value); }
        }

        private ObservableCollection<Product> TempProductsList;
        public ObservableCollection<Product> ProductsList
        {
            get { return TempProductsList; }
            set { SetProperty(ref TempProductsList, value); }
        }

        private bool TempIsVisibleItemList;
        public bool IsVisibleItemList
        {
            get { return TempIsVisibleItemList; }
            set { SetProperty(ref TempIsVisibleItemList, value); }
        }

        #endregion

        public async void OpenViewer()
        {
            if (string.IsNullOrEmpty(Product.ImgName))
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DialogService.DisplayAlertAsync("No Camera", " No camera available.", "OK");
                    return;
                }
                var imgFile = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                    DefaultCamera = CameraDevice.Rear,
                });
                if (imgFile == null)
                    return;
                // create a file, overwriting any existing file
                Product.ImgName = imgFile.Path.Split('/')?.LastOrDefault();
                Image = $"{imgFile.Path}";
                Xamarin.Forms.DependencyService.Get<IImageCropHelper>().ShowFromFile(imgFile.Path);
            }
            else if (IsFull)
                IsFull = false;
            else
                IsFull = true;
            Success = (newFile, oldFile) =>
            {
                Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(oldFile);
                Xamarin.Forms.DependencyService.Get<IFileHelper>().RenameFile(oldFile, newFile);
                Image = oldFile;
            };

            await Xamarin.Forms.DependencyService.Get<IImageHelper>().ResizeImage(Product.ImgName);
        }

        public async void SaveProduct()
        {
            string messages = "";
            if (Product.ItemsUsed == null || (Product.ItemsUsed != null && Product.ItemsUsed.Count == 0))
            {
                messages += "Add an Item\n";
            }
            if (Product.ProfitPercent == 0)
            {
                messages += "Enter the Profit %\n";
            }
            if (string.IsNullOrEmpty(Product.Name))
            {
                messages += "Enter the Name of the Product";
            }
            if (!string.IsNullOrEmpty(messages))
            {
                await DialogService.DisplayAlertAsync("Alert", messages, "Ok");
                return;
            }

            int add = await App.DbHelper.SaveProduct(Product);
            foreach (var item in Product.ItemsUsed)
            {
                if (item.Quantity > 0)
                {
                    item.ProductId = Product.Id;
                    await App.DbHelper.SaveItemUsed(item);
                }
            }
            Xamarin.Forms.MessagingCenter.Send<Product>(Product, "added");
            setNewProduct();

        }

        public async void AddItem()
        {
            if (!string.IsNullOrEmpty(Product.Category))
            {
                if (Product.ItemsUsed == null)
                    Product.ItemsUsed = new ObservableCollection<ItemUsed>();
                Product.ItemsUsed.Add(new ItemUsed());
            }
            else
            {
                await DialogService.DisplayAlertAsync("Alert", "Select Category before adding an item", "Ok");
            }
        }

        public void AddTotal(object obj)
        {
            Product.CostPrice = Product.ItemsUsed.Sum(e => e.Total);
        }

        public void RemoveItem(ItemUsed obj)
        {
            Product.ItemsUsed.Remove(obj);
            if (Product.ItemsUsed != null && Product.ItemsUsed.Count > 0)
            {
                AddTotal(obj);
            }
            else
                Product.CostPrice = 0;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CategoryList = await App.DbHelper.GetAllCategory();
            string categoryId = parameters["categoryId"] as String;
            SelectedCategory = CategoryList.Find(e => e.Id.Equals(categoryId));
            IsVisibleItemList = true;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            //Useful when add cancels. Need to add condition to check whether product saved or not.
            base.OnNavigatedFrom(parameters);
        }

        public async void OnCategoryChanged(Category category)
        {
            if (category != null)
            {
                Product.Category = category.Id;
                ItemList = new List<Item>(await App.DbHelper.GetAllItems(category.Id));
                if (ItemList != null && ItemList.Count > 0)
                {
                    removeOldItems();

                    foreach (Item item in ItemList)
                    {
                        Product.ItemsUsed.Add(new ItemUsed() { ItemSelected = item });
                    }
                }
                else removeOldItems();
            }
        }

        void setNewProduct()
        {
            Product = new Product();
            //Image = "addimage.png";
            IsFull = false;
            OnCategoryChanged(SelectedCategory);
        }

        void removeOldItems()
        {
            for (int i = Product.ItemsUsed.Count - 1; i >= 0; i--)
            {
                Product.ItemsUsed.RemoveAt(i);
            }
        }

        void toggleItemList()
        {
            IsVisibleItemList = !IsVisibleItemList;
        }

        public void OnProductAdded(Product obj)
        {
            if (obj != null && SelectedCategory != null)
            {
                getAllProducts();
            }
        }

        async void getAllProducts()
        {
            ProductsList = new ObservableCollection<Product>(await App.DbHelper.GetAllProducts(SelectedCategory.Id, 0));

        }
    }
}
