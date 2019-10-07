using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace iMan.Pages.Views
{
    public partial class ProductAddPage : ContentPage
    {
        public ProductAddPage()
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
