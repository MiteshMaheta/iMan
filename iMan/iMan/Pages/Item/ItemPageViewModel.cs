using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace iMan.Pages.ViewModels
{
    public class ItemPageViewModel : ViewModelBase
    {
        public ItemPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            MessagingCenter.Subscribe<Item>(this, "added", OnItemAdded);
            AddCommand = new DelegateCommand(AddItem);
            DeleteCommandWithObject = new DelegateCommand<object>(deleteItem);
        }

        #region Properties
        private ObservableCollection<Item> TempItemsList;
        public ObservableCollection<Item> ItemsList
        {
            get { return TempItemsList; }
            set { SetProperty(ref TempItemsList, value); }
        }

        private Item TempItem;
        public Item Item
        {
            get { return TempItem; }
            set
            {
                SetProperty(ref TempItem, value);
                if (TempItem != null)
                {
                    GoToEditPage(TempItem);
                }
            }
        }

        private String TempTitle;
        public String Title
        {
            get { return TempTitle; }
            set {SetProperty(ref TempTitle, value); }
        }

        Category category;
        #endregion

        public void OnItemAdded(Item obj)
        {
            if (obj != null && category != null)
            {
                GetAllItems(category.Id.ToString());
            }
        }

        public void AddItem()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Category", category);
            NavigationService.NavigateAsync("ItemAddPage", parameters);
        }

        public async void GoToEditPage(Item itemObject)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("item", itemObject);
            parameters.Add("Category", category);
            await NavigationService.NavigateAsync("ItemEditPage", parameters);
        }

        async void deleteItem(object obj)
        {
            var ItemObject = (Item)obj;
            bool confirm = await DialogService.DisplayAlertAsync("Item Delete", "Do you want to Delete?", "Delete", "Cancel");
            if (confirm)
            {
                int deleted = await App.DbHelper.DeleteItem(int.Parse(ItemObject.Id));
                Xamarin.Forms.MessagingCenter.Send<Item>(ItemObject, "added");
            }
        }

        public async void GetAllItems(string category)
        {
            ItemsList = new ObservableCollection<Item>(await App.DbHelper.GetAllItems(category));
            if (ItemsList.Count > 0) noData = false; else noData = true;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("category"))
            {
                category = parameters["category"] as Category;
            }
            if (category != null && !string.IsNullOrEmpty(category.Id))
            {
                Title = category.Name;
                GetAllItems(category.Id);
            }
        }
    }
}
