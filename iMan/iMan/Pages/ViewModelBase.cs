using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace iMan.Pages.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        public INavigationService NavigationService { get; set; }
        public IPageDialogService DialogService { get; set; }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand<object> DeleteCommandWithObject { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private bool TempnoData;
        public bool noData
        {
            get { return TempnoData; }
            set { SetProperty(ref TempnoData, value); }
        }

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
