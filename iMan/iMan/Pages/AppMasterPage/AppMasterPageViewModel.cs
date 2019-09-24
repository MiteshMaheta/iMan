using System;
using iMan.Pages.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
namespace iMan.Pages.ViewModels
{
    public class AppMasterPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> NavigateCommand { get; set; }
        public AppMasterPageViewModel(INavigationService navigationService, IPageDialogService dialogSerivce) : base(navigationService, dialogSerivce)
        {
            NavigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        public void Navigate(string obj)
        {
            NavigationService.NavigateAsync(obj);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
