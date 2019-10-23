using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace iMan.Pages.ViewModels
{
    public class ProductEditPageViewModel : ViewModelBase
    {
        public ProductEditPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            OnImageTapped = new DelegateCommand(OpenViewer);
            AddNewItem = new DelegateCommand(AddItem);
            AddTotalCommand = new DelegateCommand<object>(AddTotal);
            RemoveItemCommand = new DelegateCommand<ItemUsed>(RemoveItem);
            UnitList = ConstantData.UnitList;
            SaveCommand = new DelegateCommand(SaveProduct);
            IsFull = false;
        }

        #region Poperties
        public DelegateCommand OnImageTapped { get; set; }
        public DelegateCommand AddNewItem { get; set; }
        public DelegateCommand<object> AddTotalCommand { get; set; }
        public DelegateCommand<ItemUsed> RemoveItemCommand { get; set; }

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
            }
        }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            set { SetProperty(ref isFull, value); }
        }

        public DelegateCommand CancelCommand { get; set; }

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
            bool confirm = await DialogService.DisplayAlertAsync("Confirm", "Do you want to save?", "Yes", "No");
            if (confirm)
            {
                int add = await App.DbHelper.SaveProduct(Product);
                foreach (var item in Product.ItemsUsed)
                {
                    if (item.Quantity > 0)
                    {
                        item.ProductId = Product.Id;
                        await App.DbHelper.SaveItemUsed(item);
                    }
                }
                await DialogService.DisplayAlertAsync("Success", "Product edited Successfully", "Ok");
                Xamarin.Forms.MessagingCenter.Send<Product>(Product, "added");
                await NavigationService.GoBackAsync();
            }
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
            if (parameters.ContainsKey("piece"))
            {
                Product = parameters["piece"] as Product;
                SelectedCategory = CategoryList.FirstOrDefault(e => e.Id.Equals(Product.Category));
                ItemList = new List<Item>(await App.DbHelper.GetAllItems(SelectedCategory.Id));
                Product.ItemsUsed = new ObservableCollection<ItemUsed>(await App.DbHelper.GetAllItemUsed(Product.Id));
                foreach (ItemUsed item in Product.ItemsUsed)
                {
                    Item temp = ItemList.FirstOrDefault(e => e.Name.Equals(item.Type));
                    item.ItemSelected = temp;
                }
            }
        }

        public async void OnCategoryChanged(Category category)
        {
            if (category != null && Product.Category != category.Id)
            {
                ItemList = new List<Item>(await App.DbHelper.GetAllItems(category.Id));
                Product.Category = category.Id;
                if (ItemList != null && ItemList.Count > 0)
                {
                    removeOldItems();

                    foreach (Item item in ItemList)
                    {
                        if (Product.ItemsUsed.Count(e => e.Type.Equals(item.Name)) == 0)
                            Product.ItemsUsed.Add(new ItemUsed() { ItemSelected = item });
                    }
                }
                else removeOldItems();
            }
        }

        void removeOldItems()
        {
            for (int i = Product.ItemsUsed.Count - 1; i >= 0; i--)
            {
                Product.ItemsUsed.RemoveAt(i);
            }
        }
    }
}
