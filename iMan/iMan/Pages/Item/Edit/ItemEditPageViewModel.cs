using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.Generic;

namespace iMan.Pages.ViewModels
{
    public class ItemEditPageViewModel : ViewModelBase
    {
        public ItemEditPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            UnitList = new List<string>() { "Kg", "Grams", "Gross", "Piece", "Dozen" };
            SaveCommand = new DelegateCommand(SaveItemAsync);
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
            set { SetProperty(ref TempItemObject, value); }
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
            await NavigationService.GoBackAsync();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("item"))
            {
                ItemObject = parameters["item"] as Item;
            }
        }
    }
}
