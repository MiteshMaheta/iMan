using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace iMan.Pages.ViewModels
{
    public class ItemAddPageViewModel : ViewModelBase
    {
        public DelegateCommand ToggleRecentItemCommand { get; set; }
        public DelegateCommand<object> SaveCommandWithObject { get; set; }

        public ItemAddPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            MessagingCenter.Subscribe<Item>(this, "added", OnItemAdded);
            UnitList = ConstantData.UnitList;
            SaveCommandWithObject = new DelegateCommand<object>(SaveItemAsync);
            ToggleRecentItemCommand = new DelegateCommand(toggleItemList);
        }

        #region Properties
        private ObservableCollection<Item> TempItemsList;
        public ObservableCollection<Item> ItemsList
        {
            get { return TempItemsList; }
            set { SetProperty(ref TempItemsList, value); }
        }

        private List<string> TempUnitList;
        public List<string> UnitList
        {
            get { return TempUnitList; }
            set { SetProperty(ref TempUnitList, value); }
        }

        private Item TempItem;
        public Item Item
        {
            get { return TempItem; }
            set { SetProperty(ref TempItem, value); }
        }

        private Category TempParentCategory;
        public Category ParentCategory
        {
            get { return TempParentCategory; }
            set
            {
                SetProperty(ref TempParentCategory, value);
            }
        }

        private bool TempIsVisibleItemList;
        public bool IsVisibleItemList
        {
            get { return TempIsVisibleItemList; }
            set { SetProperty(ref TempIsVisibleItemList, value); }
        }
        #endregion

        public void OnItemAdded(Item obj)
        {
            if (obj != null && ParentCategory != null)
            {
                GetAllItems();
            }
        }

        void setNewItem(object obj=null)
        {
            Item = new Item();
            Item.CategoryId = ParentCategory.Id;
            Item.Unit = ConstantData.GetEnumName(ConstantData.Units.Kg);
            if (obj != null)
            {
                var entryObj = (Entry)obj;
                entryObj.Focus();
            }
        }

        public async void SaveItemAsync(Object obj = null)
        {
            string messages = "";
            if (string.IsNullOrEmpty(Item.Name))
            {
                messages += "Enter name of the Item.";
            }
            if (string.IsNullOrEmpty(Item.Unit))
            {
                messages += "Choose a Unit.";
            }
            if (!string.IsNullOrEmpty(messages))
            {
                await DialogService.DisplayAlertAsync("Alert", messages, "Ok");
                return;
            }
            await App.DbHelper.SaveItem(Item);
            Xamarin.Forms.MessagingCenter.Send<Item>(Item, "added");
            setNewItem(obj);
        }

        async void GetAllItems()
        {
            ItemsList = new ObservableCollection<Item>(await App.DbHelper.GetAllItems(ParentCategory.Id, false));
        }

        void toggleItemList()
        {
            IsVisibleItemList = !IsVisibleItemList;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Category"))
            {
                ParentCategory = parameters["Category"] as Category;
                if (ParentCategory != null && !string.IsNullOrEmpty(ParentCategory.Id))
                {
                    setNewItem();
                    GetAllItems();
                    IsVisibleItemList = true;
                }
            }
        }
    }
}
