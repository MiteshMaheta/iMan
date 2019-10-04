using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.Generic;

namespace iMan.Pages.ViewModels
{
    public class ItemAddPageViewModel : ViewModelBase
    {
        public DelegateCommand SaveCommand { get; set; }

        public ItemAddPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            UnitList = new List<string>() { "Kg", "Grams", "Gross", "Piece", "Dozen" };
            Item = new Item();
            Item.Unit = "Kg";
            SaveCommand = new DelegateCommand(SaveItemAsync);
        }

        #region Properties
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
        #endregion

        public async void SaveItemAsync()
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
            await NavigationService.GoBackAsync();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Category"))
            {
                Category category = parameters["Category"] as Category;
                if (category != null && category.Id.HasValue)
                {
                    Item.CategoryId = category.Id.ToString();
                }
            }
        }
    }
}
