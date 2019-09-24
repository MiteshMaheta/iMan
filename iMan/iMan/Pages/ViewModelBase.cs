using System;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace iMan.Pages.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        public INavigationService NavigationService { get; set; }
        public IPageDialogService DialogService { get; set; }

        public ViewModelBase(INavigationService navigationService, IPageDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }
        public virtual void Destroy()
        {

        }
    }
}
