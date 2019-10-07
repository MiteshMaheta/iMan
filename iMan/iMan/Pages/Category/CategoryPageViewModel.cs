using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using iMan.Helpers;

namespace iMan.Pages.ViewModels
{
    public class CategoryPageViewModel : ViewModelBase
    {
        public DelegateCommand<object> AddCategoryCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public CategoryPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Xamarin.Forms.MessagingCenter.Subscribe<Category>(this, "added", OnCategoryAdded);
            AddCategoryCommand = new DelegateCommand<object>(AddCategory);
            SaveCommand = new DelegateCommand(SaveCategory);
            CancelCommand = new DelegateCommand(CancelCategorySave);
            DeleteCommandWithObject = new DelegateCommand<object>(deleteCategory);
        }

        #region
        private ObservableCollection<Category> TempCategoryList;
        public ObservableCollection<Category> CategoryList
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
                if (TempSelectedCategory != null)
                    GoToItem(TempSelectedCategory);
            }
        }
        private bool TempIsAddCategory;
        public bool IsAddCategory
        {
            get { return TempIsAddCategory; }
            set{ SetProperty(ref TempIsAddCategory, value); }
        }

        private string TempCategoryName;
        public string CategoryName
        {
            get { return TempCategoryName; }
            set { SetProperty(ref TempCategoryName, value); }
        }

        #endregion

        public void OnCategoryAdded(Category obj)
        {
            if (obj != null)
            {
                GetAllCategory();
            }
        }

        public void GoToItem(Category obj)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("category", obj as Category);
            NavigationService.NavigateAsync("ItemPage", parameters);
        }

        public void AddCategory(object obj)
        {
            var entryObj = (Entry)obj;
            entryObj.Focus();
            IsAddCategory = true;
        }
        void CancelCategorySave()
        {
            Xamarin.Forms.DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            IsAddCategory = false;
            CategoryName = null;
        }

        async void deleteCategory(object obj)
        {
            var CategoryObject = (Category)obj;
            bool confirm = await DialogService.DisplayAlertAsync("Category Delete", "Do you want to Delete?", "Delete", "Cancel");
            if (confirm)
            {
                int deleted = await App.DbHelper.DeleteCategory(CategoryObject.Id.Value);
                Xamarin.Forms.MessagingCenter.Send<Category>(CategoryObject, "added");
            }
        }

        public async void SaveCategory()
        {
            string messages = "";
            if (string.IsNullOrEmpty(CategoryName))
            {
                messages += "Name is a required field";
            }
            if (!string.IsNullOrEmpty(messages))
            {
                await DialogService.DisplayAlertAsync("Alert", messages, "Ok");
                return;
            }
            Category NewCategory = new Category();
            NewCategory.Name = CategoryName;
            int add = await App.DbHelper.SaveCategory(NewCategory);
            Xamarin.Forms.MessagingCenter.Send<Category>(NewCategory, "added");
            CategoryName = null;
            IsAddCategory = false;
        }

        public async void GetAllCategory()
        {
            CategoryList = new ObservableCollection<Category>(await App.DbHelper.GetAllCategory("",false));
            if (CategoryList.Count > 0) noData = false; else noData = true;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            GetAllCategory();
        }
    }
}
