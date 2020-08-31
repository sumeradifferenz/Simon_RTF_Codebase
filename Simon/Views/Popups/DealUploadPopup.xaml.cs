using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Simon.ViewModel;
using Xamarin.Forms;

namespace Simon.Views.Popups
{
    public partial class DealUploadPopup : PopupPage
    {
        private DealViewModel vm = null;
        public DealUploadPopup()
        {
            InitializeComponent();
            vm = new DealViewModel();
            this.BindingContext = vm;
        }
    }
}
