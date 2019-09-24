using System;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using iMan.Data;
using System.Collections.ObjectModel;

namespace iMan.Pages.ViewModels
{
    public class PartyPageViewModel : ViewModelBase
    {
        public DelegateCommand AddPartyCommand { get; set; }
        public PartyPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Xamarin.Forms.MessagingCenter.Subscribe<Party>(this, "added", OnPartyAdded);
            AddPartyCommand = new DelegateCommand(AddParty);
            //GetAllParty();
        }

        public void OnPartyAdded(Party obj)
        {
            if (obj != null)
            {
                GetAllParty();
            }
        }

        public void AddParty()
        {
            NavigationService.NavigateAsync("PartyAddPage", null, true, true);
        }

        private ObservableCollection<Party> partyList;
        public ObservableCollection<Party> PartyList
        {
            get { return partyList; }
            set { SetProperty(ref partyList, value); }
        }

        public async void GetAllParty()
        {
            PartyList = new ObservableCollection<Party>(await App.DbHelper.GetAllParty());
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            GetAllParty();
        }

    }
}
