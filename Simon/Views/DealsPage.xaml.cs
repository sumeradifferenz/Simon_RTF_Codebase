using System;
using System.Collections.Generic;
using Simon.Controls;
using Simon.Helpers;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

namespace Simon.Views
{
    public partial class DealsPage : GradientColorStack
    {
        DealsMainModel ObjAssignList = new DealsMainModel();
        private DealViewModel vm = null;
        IEnumerable<DealsMainModel> _result;
        string userId;
        public DealsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsFilterApplied", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortAmountUp", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortAmountDown", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortBorrowerUp", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortBorrowerDown", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortDueUp", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortDueDown", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortClosingUp", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealsMainModel>(this, "DealsSortClosingDown", (sender, args) =>
            {
                DealList.FlowScrollTo(args, ScrollToPosition.Start, true);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.ReadUnread = "null";
            App.OrderByText = Constants.LastPostDateText;
            App.SelectedTitle = string.Empty;
            App.SelectedName = string.Empty;
            vm = new DealViewModel();
            this.BindingContext = vm;

            if (NetworkCheck.IsInternet())
            {
                _ = vm.FetchDealData();
            }
            else
            {
                DisplayAlert("Simon", "No network is available.", "OK");
            }

            if (Application.Current.Properties.ContainsKey("USERID"))
            {
                userId = Convert.ToString(Application.Current.Properties["USERID"]);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
         
        public void btnCancelClicked(object sender, EventArgs e)
        {
            //sortView.IsVisible = false;
        }

        private async void btnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        void SwipeToLeft(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            vm.FooterNavigation(SessionService.BaseFooterItems[2]);
        }

        void SwipeToRight(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            vm.FooterNavigation(SessionService.BaseFooterItems[0]);
        }
    }
}