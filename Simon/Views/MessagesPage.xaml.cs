using System;
using System.Collections.Generic;
using Simon.Controls;
using Simon.Helpers;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Xamarin.Forms;

namespace Simon.Views
{
    public partial class MessagesPage : GradientColorStack
    {
        //private MessageViewModel ViewModel = null;
        string userId;
        IEnumerable<DealMessageList> result;
        bool isLoading;

        public MessagesPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, DealMessageList>(this, "ApprovalReadFilterApplied", (sender, args) =>
            {
                OpenData.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealMessageList>(this, "ApprovalUnReadFilterApplied", (sender, args) =>
            {
                OpenData.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealMessageList>(this, "ApprovalAllFilterApplied", (sender, args) =>
            {
                OpenData.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

            MessagingCenter.Subscribe<object, DealMessageList>(this, "MessageSortApplied", (sender, args) =>
            {
                OpenData.FlowScrollTo(args, ScrollToPosition.Start, true);
            });

        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                Settings.TypedMessage = null;
                App.FileName = null;
                App.FrameImage = null;

                if (Application.Current.Properties.ContainsKey("USERID"))
                {
                    userId = Convert.ToString(Application.Current.Properties["USERID"]);
                }

                if (NetworkCheck.IsInternet())
                {
                    if (App.IsBackFromPagesDetailPage)
                    {
                        App.IsBackFromPagesDetailPage = false;
                    }
                    else
                    {
                        MessageViewModel ViewModel = new MessageViewModel();
                        this.BindingContext = ViewModel;

                        await ViewModel.FetchData();
                        //ViewModel._helper = this;
                    }
                }
                else
                {
                    await DisplayAlert("Simon", "No network is available.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
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

        void SwipeToLeft(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            MessageViewModel ViewModel = new MessageViewModel();
            this.BindingContext = ViewModel;

            ViewModel.FooterNavigation(SessionService.BaseFooterItems[3]);
        }

        void SwipeToRight(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            MessageViewModel ViewModel = new MessageViewModel();
            this.BindingContext = ViewModel;

            ViewModel.FooterNavigation(SessionService.BaseFooterItems[1]);
        }
    }
}