using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace iMan.Pages.Views
{
    public partial class ItemEditPage : ContentPage
    {
        public ItemEditPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            itemName.Focus();
        }
    }
}
