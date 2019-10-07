﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using iMan.Data;
using iMan.Helpers;
using iMan.Pages.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
namespace iMan.Pages.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            AddCommand = new DelegateCommand(Add);
            Xamarin.Forms.MessagingCenter.Subscribe<Product>(this, "added", OnProductAdded);
            //Position = 0;
            SearchProductCommand = new DelegateCommand(SearchProduct);
            GetAllProducts = new DelegateCommand<object>(GetAllProduct);
            ProductsList = new ObservableCollection<Product>();
            IsFetching = false;
            hasMore = true;

        }

        private void SearchProduct()
        {
            if (string.IsNullOrEmpty(SearchText))
                SearchProduct(false);
        }

        #region Property
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand<object> GetAllProducts { get; set; }

        private ObservableCollection<Product> productsList;
        public ObservableCollection<Product> ProductsList
        {
            get { return productsList; }
            set { SetProperty(ref productsList, value); }
        }
        bool hasMore;
        private Product selectedProduct;
        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                SetProperty(ref selectedProduct, value);
                if (selectedProduct != null)
                {
                    NavigationParameters parameter = new NavigationParameters();
                    parameter.Add("Product", selectedProduct);
                    NavigationService.NavigateAsync("ProductDetailPage", parameter);
                }
            }
        }

        private List<string> categories;
        public List<string> Categories
        {
            get { return categories; }
            set { SetProperty(ref categories, value); }
        }

        private int? position;
        public int? Position
        {
            get { return position; }
            set
            {
                SetProperty(ref position, value);
                if (Categories != null && Categories.Count > 0)
                {
                    hasMore = true;
                    ProductsList = new ObservableCollection<Product>();
                    GetAllProduct(0);
                }
            }
        }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                if (string.IsNullOrEmpty(searchText) && tempList != null && tempList.Count > 0)
                {
                    SearchProduct();
                }
            }
        }

        private bool isFetching;
        public bool IsFetching
        {
            get { return isFetching; }
            set { SetProperty(ref isFetching, value); }
        }

        private List<Product> tempList;
        public DelegateCommand SearchProductCommand { get; set; }

        #endregion

        public async void Add()
        {
            await NavigationService.NavigateAsync("ProductAddPage", null, true, false);
        }

        public async void GetAllProduct(object start)
        {
            if (hasMore)
            {
                IsFetching = true;
                if (Position.HasValue)
                {
                    List<Product> temp = ProductsList.ToList();
                    if (Categories != null && Categories.Count > 0)
                    {
                        string imgPath = Xamarin.Forms.DependencyService.Get<IImageHelper>().GetCompressImagePath();
                        List<Product> list = await App.DbHelper.GetAllProducts(Categories?[Position.Value], ProductsList.Count);
                        if (list.Count == 0)
                            hasMore = false;
                        list.ForEach((Product p) =>
                        {
                            if (p.ImgName.Split('/').Count() == 1)
                                p.ImgName = imgPath + "/" + p.ImgName;
                        });
                        temp.AddRange(list);
                        temp = temp.Distinct().ToList();
                        tempList = temp;
                        SearchProduct(false);
                    }
                }
                IsFetching = false;
            }
        }

        public void OnProductAdded(Product obj)
        {
            if (obj != null)
            {
                if (Categories != null && Categories.Count > 0)
                {
                    ProductsList = new ObservableCollection<Product>();
                    hasMore = true;
                    GetAllProduct(0);
                }
            }
        }

        public void SearchProduct(bool reset)
        {
            int prevCount = ProductsList.Count;
            if (!string.IsNullOrEmpty(SearchText) && tempList != null && tempList.Count > 0)
                ProductsList = new ObservableCollection<Product>(tempList?.Where(e => e.Name.ToLower().Contains(SearchText.ToLower())));
            else
                ProductsList = new ObservableCollection<Product>(tempList);
            if (!string.IsNullOrEmpty(SearchText) && (ProductsList.Count == 0 || prevCount == ProductsList.Count))
                hasMore = false;
            if (reset)
                hasMore = true;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await App.DbHelper.GetAllCategory();
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                base.OnNavigatedTo(parameters);
                List<Category> category = await GetAllCategories();
                if (category != null && category.Count > 0)
                    Categories = category.Select(e => e.Name).ToList();
                else
                    Categories = new List<string>();
                Position = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
