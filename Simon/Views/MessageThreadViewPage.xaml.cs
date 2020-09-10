using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Simon.Helpers;
using Simon.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using ListView = Xamarin.Forms.ListView;

namespace Simon.Views
{
    public partial class MessageThreadViewPage : ContentPage
    {
        private MessageThreadViewModel vm = null;
        string URL, htmlMessage;

        public MessageThreadViewPage()
        {
            InitializeComponent();

            txtMessage.TextChanged += (sender, e) =>
            {
                this.InvalidateMeasure();

                if (App.IsFromAddParticipantPage == true)
                {
                    txtMessage.Text = Settings.TypedMessage;
                    App.IsFromAddParticipantPage = false;
                    return;
                }
                if (!string.IsNullOrEmpty(txtMessage.Text) && !string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    var Keyword = txtMessage.Text.Trim();
                    if (Keyword.Length >= 1)
                    {
                        Settings.TypedMessage = txtMessage.Text;
                    }
                }
            };

            if (Device.RuntimePlatform == Device.iOS)
            {
                //Divyesh - Date: 28 July, 2020
                //Embeded all controls inside scrollview to avoid header shifting up when keyboard is open.
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                if (mainDisplayInfo.Height <= 1334)
                {
                    // iPhone 6, 7, 8
                    MessagesThreadStackLayout.HeightRequest = 465;
                }
                else
                {
                    //iPhone X, XS, XR, 11
                    MessagesThreadStackLayout.HeightRequest = 565;
                }

                txtMessage.Focused += (sender, e) => PlaceHolder.HeightRequest = 20;
                txtMessage.Unfocused += (sender, e) => PlaceHolder.HeightRequest = 0;

                ScrollView scroll = new ScrollView
                {
                    Content = WrapperStackLayout
                };
                MainStackLayout.Children.Add(scroll);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            this.BindingContext = new MessageThreadViewModel();

            vm = this.BindingContext as MessageThreadViewModel;

            if (App.IsFromAddParticipantPage == true)
            {
                if (App.FrameImage != null)
                {
                    vm.ImageUrl = App.FrameImage;
                    vm.isImageVisible = true;
                }
                if (App.FileName != null)
                {
                    vm.FileName = App.FileName;
                    vm.isDocsVisible = true;
                }
                txtMessage.Text = Settings.TypedMessage;
            }
            else
            {
                txtMessage.Text = string.Empty;
            }

            if (vm != null)
            {
                if (NetworkCheck.IsInternet())
                {
                    await vm.PostReadThreadData();
                    await vm.FetchThreadUserData();
                }
                else
                {
                    await DisplayAlert("Simon", "No network is available.", "OK");
                }
            }
        }

        protected override void OnDisappearing()
        {
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        }

        public void onMessageStackClick(object Sender, EventArgs e)
        {
            txtMessage.Unfocus();
        }

        //more and less button Expand
        //public void onExpandBtnClicked(object Sender, EventArgs e)
        //{
        //    var buttonClickHandler = (Xamarin.Forms.Button)Sender;
        //    // access Parent Layout for Button  
        //    StackLayout ParentStackLayout = (StackLayout)buttonClickHandler.Parent;
        //    // access first Label "name"  
        //    Label nameLabel = (Label)ParentStackLayout.Children[0];
        //    if (buttonClickHandler.Text == "more")
        //    {
        //        nameLabel.MaxLines = 20;
        //        buttonClickHandler.Text = "less";
        //    }
        //    else
        //    {
        //        buttonClickHandler.Text = "more";
        //        nameLabel.MaxLines = 3;
        //    }
        //}

        protected override bool OnBackButtonPressed()
        {
            vm.Back();
            return true;
        }

        void MessageList_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            MessageList.SeparatorColor = Color.Transparent;
            MessageList.SeparatorVisibility = SeparatorVisibility.None;

            if (e.Item == null) return;

            // Optionally pause a bit to allow the preselect hint.
            Task.Delay(500);

            // Deselect the item.
            if (sender is ListView lv) lv.SelectedItem = null;
            txtMessage.Unfocus();
        }

        private string IgnoreVoidElementsInHTML(string inputString)
        {
            inputString = inputString.Replace("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\">", "");
            inputString = inputString.Replace("<br>", "<br/>");
            inputString = inputString.Replace("\n", "");
            inputString = inputString.Replace("\r", "");
            inputString = inputString.Replace("<title></title>", "");
            inputString = inputString.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE html PUBLIC" +
                " \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">", "");
            return inputString;
        }

        private string ValidationMessage(string Message)
        {
            if (Message.Contains("<strong>​</strong>"))
            {
                string text = Message.Replace("<strong>​</strong>", "");
                Message = text;
            }
            if (Message.Contains("\n"))
            {
                string text = Message.Replace("\n", "");
                Message = text;
            }
            if (Message.Contains("\n \n"))
            {
                string text = Message.Replace("\n \n", "");
                Message = text;
            }
            if (Message.Contains("> <"))
            {
                string text = Message.Replace("> <", "><");
                Message = text;
            }
            if (Message.Contains("https://") || Message.Contains("http://"))
            {
                string message = Regex.Replace(Message, "<.*?>", string.Empty);
                Message = "<p><a class=\"e-rte-anchor\" href=\"" + message + "\" title=\"" + message + "\">" + message + "</a></p>";
            }
            return Message;
        }

        async void SendMessageTappedCommand(object sender, EventArgs e)
        {
            this.BindingContext = new MessageThreadViewModel();
            vm = this.BindingContext as MessageThreadViewModel;
            
            if (!string.IsNullOrEmpty(App.base64String))
            {
                if (!string.IsNullOrEmpty(txtMessage.HtmlText))
                {
                    string htmlstring = IgnoreVoidElementsInHTML(txtMessage.HtmlText);
                    htmlMessage = ValidationMessage(htmlstring);
                    htmlMessage = App.base64String + htmlMessage;
                }
                else
                {
                    htmlMessage = App.base64String;
                }

                //var image = htmlMessage.Split(',')[1];
                //var image1 = image.Split('"')[0];

                //string[] delim = { "alt=\"\">" };
                //var stringMsg = image.Split(delim, StringSplitOptions.None);

                //var Base64Stream = Convert.FromBase64String(image1);
                //ImageMsg.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                //sample.Text = stringMsg[1];
            }
            else
            {
                string htmlstring = IgnoreVoidElementsInHTML(txtMessage.HtmlText);
                htmlMessage = ValidationMessage(htmlstring);
                //sample.Text = htmlMessage;
                Settings.TypedMessage = htmlMessage;
            }
            
            if (vm.sendEnable)
                return;

            vm.sendEnable = true;

            await vm.SendMessage(htmlMessage);

            txtMessage.Text = string.Empty;
            vm.sendEnable = false;
        }
    }
}


//<p><a class="e-rte-anchor" href="https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/populate-data" title="https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/populate-data">https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/populate-data</a></p>