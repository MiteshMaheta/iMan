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
    public class ItemEditPageViewModel : ViewModelBase
    {
        public DelegateCommand ToggleRecentItemCommand { get; set; }
        public ItemEditPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            MessagingCenter.Subscribe<Item>(this, "added", OnItemAdded);
            UnitList = ConstantData.UnitList;
            SaveCommand = new DelegateCommand(SaveItemAsync);
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

        private Item TempItemObject;
        public Item ItemObject
        {
            get { return TempItemObject; }
            set {
                SetProperty(ref TempItemObject, value);
            }
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

        async void GetAllItems()
        {
            ItemsList = new ObservableCollection<Item>(await App.DbHelper.GetAllItems(ParentCategory.Id));
        }

        void toggleItemList()
        {
            IsVisibleItemList = !IsVisibleItemList;
        }

        public async void SaveItemAsync()
        {
            string messages = "";
            if (string.IsNullOrEmpty(ItemObject.Name))
            {
                messages += "Enter name of the Item.\n";
            }
            if (string.IsNullOrEmpty(ItemObject.Unit))
            {
                messages += "Choose a Unit.";
            }
            if (!string.IsNullOrEmpty(messages))
            {
                await DialogService.DisplayAlertAsync("Alert", messages, "Ok");
                return;
            }
            await App.DbHelper.SaveItem(ItemObject);
            Xamarin.Forms.MessagingCenter.Send<Item>(ItemObject, "added");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("item"))
            {
                ItemObject = parameters["item"] as Item;
                ParentCategory = parameters["Category"] as Category;
                if (ParentCategory != null && !string.IsNullOrEmpty(ParentCategory.Id))
                {
                    GetAllItems();
                    IsVisibleItemList = true;
                }
            }
        }
    }
}
