using System;
using System.Diagnostics;
using Plugin.FirebasePushNotification;
using Simon.Controls;
using Simon.Helpers;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Xamarin.Forms;

namespace Simon.Views
{
    public partial class LandingPage : GradientColorStack
    {
        private LandingViewModel ViewModel = null;

        public LandingPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            App.ReadUnread = "null";
            App.FollowUp = "null";
            App.OrderByText = Constants.LastPostDateText;
            App.SelectedTitle = string.Empty;
            App.SelectedName = string.Empty;
            var leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };

            ViewModel = new LandingViewModel();
            this.BindingContext = ViewModel;

            if (App.IsFirstTime)
            {
                App.IsFirstTime = false;
                ViewModel.FooterNavigation(SessionService.BaseFooterItems[0]);
            }

            txtName.Text = Settings.LoggedInUser.name;
           
            if (NetworkCheck.IsInternet())
            {
                await ViewModel.FetchClosingData();
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

        private void onDealBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DealsPage());
        }
        private void onApproveBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssentMainPage());
        }
        private void onPortfolioBtnClicked(object sender, EventArgs e)
        {
            // Navigation.PushAsync(new MessageThreadPage());
        }
        private void onMessagesBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MessagesPage());
        }
        private void onAlertBtnClicked(object sender, EventArgs e)
        {
            // Navigation.PushAsync(new MessageMainPage());
        }

        void SwipeGestureRecognizer_Swiped(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
             ViewModel.FooterNavigation(SessionService.BaseFooterItems[1]);
        }
    }
}
