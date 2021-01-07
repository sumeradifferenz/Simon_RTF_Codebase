using System;
using System.Collections.Generic;
using System.Linq;
using Simon.Controls;
using Simon.Helpers;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Xamarin.Forms;

namespace Simon.Views
{
    public partial class AssentMainPage : GradientColorStack
    {
        IEnumerable<ApprovalMainModel> ObjAssignList;
        string userId;
        //private ApprovalViewModel ViewModel = null;

        public AssentMainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, ApprovalMainModel>(this, "ApprovalSortBorrowerUp", (sender, args) =>
            {
                assignListView.ScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, ApprovalMainModel>(this, "ApprovalSortBorrowerDown", (sender, args) =>
            {
                assignListView.ScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, ApprovalMainModel>(this, "ApprovalSortDateUp", (sender, args) =>
            {
                assignListView.ScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, ApprovalMainModel>(this, "ApprovalSortDateDown", (sender, args) =>
            {
                assignListView.ScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, ApprovalMainModel>(this, "ClearApprovalSort", (sender, args) =>
            {
                assignListView.ScrollTo(args, ScrollToPosition.Start, true);
            });
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            App.ReadUnread = "null";
            App.FollowUp = "null";
            App.OrderByText = Constants.LastPostDateText;
            App.SelectedTitle = string.Empty;
            App.SelectedName = string.Empty;
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("USERID"))
            {
                userId = Convert.ToString(Xamarin.Forms.Application.Current.Properties["USERID"]);
            }
            assignListView.RefreshCommand = new Command(() =>
            {
                assignListView.IsRefreshing = false;
            });

            if (NetworkCheck.IsInternet())
            {
                if (App.IsBackFromPagesDetailPage)
                {
                    App.IsBackFromPagesDetailPage = false;
                }
                else
                {
                    ApprovalViewModel ViewModel = new ApprovalViewModel();
                    this.BindingContext = ViewModel;

                    await ViewModel.GetApprovalData();
                }
            }
            else
            {
                await DisplayAlert("Simon", "No network is available.", "OK");
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

        void SwipeToRight(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            ApprovalViewModel ViewModel = new ApprovalViewModel();
            this.BindingContext = ViewModel;

            ViewModel.FooterNavigation(SessionService.BaseFooterItems[2]);
        }

        private async void btnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
