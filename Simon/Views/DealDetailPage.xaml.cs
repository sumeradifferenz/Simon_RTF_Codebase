﻿using System;
using System.Collections.Generic;
using Simon.Models;
using Xamarin.Forms;

namespace Simon.Views
{
    public partial class DealDetailPage : ContentPage
    {
        public DealDetailPage(DealsMainModel mainModel)
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.buttonClick = 0;
        }
    }
}
