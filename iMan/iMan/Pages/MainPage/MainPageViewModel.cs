using System;
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
            else
                SearchProduct(false);
        }

        #region Property
        public DelegateCommand<object> GetAllProducts { get; set; }

        private ObservableCollection<Product> productsList;
        public ObservableCollection<Product> ProductsList
        {
            get { return productsList; }
            set { SetProperty(ref productsList, value); }
        }
        bool hasMore;
        private Product TempSelectedProduct;
        public Product SelectedProduct
        {
            get { return TempSelectedProduct; }
            set
            {
                SetProperty(ref TempSelectedProduct, value);
                if (TempSelectedProduct != null)
                {
                    NavigationParameters parameter = new NavigationParameters();
                    parameter.Add("Product", TempSelectedProduct);
                    NavigationService.NavigateAsync("ProductDetailPage", parameter);
                }
            }
        }

        private Category TempselectedCategory;
        public Category selectedCategory
        {
            get { return TempselectedCategory; }
            set { SetProperty(ref TempselectedCategory, value); }
        }

        private List<string> categories;
        public List<string> Categories
        {
            get { return categories; }
            set { SetProperty(ref categories, value); }
        }

        private List<string> TempCategoryIds;
        public List<string> CategoryIds
        {
            get { return TempCategoryIds; }
            set { SetProperty(ref TempCategoryIds, value); }
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
                if (!string.IsNullOrEmpty(searchText) && tempList != null && tempList.Count > 0)
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
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("categoryId", CategoryIds?[Position.Value]);
            await NavigationService.NavigateAsync("ProductAddPage", parameter);
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
                        List<Product> list = await App.DbHelper.GetAllProducts(CategoryIds?[Position.Value], ProductsList.Count);
                        if (list.Count == 0)
                            hasMore = false;
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
                if (parameters.GetNavigationMode().Equals(NavigationMode.Back))
                {
                    SelectedProduct = null;
                }
                else
                {
                    List<Category> category = await GetAllCategories();
                    if (category != null && category.Count > 0)
                    {
                        Categories = category.Select(e => e.Name).ToList();
                        CategoryIds = category.Select(e => e.Id).ToList();
                    }
                    else
                    {
                        Categories = new List<string>();
                        CategoryIds = new List<string>();
                    }

                    Position = 0;
                }
                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
