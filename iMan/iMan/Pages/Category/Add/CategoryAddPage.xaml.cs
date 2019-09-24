using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace iMan.Pages.Views
{
    public partial class CategoryAddPage : ContentPage
    {
        public CategoryAddPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            categoryName.Focus();
        }
    }
}
