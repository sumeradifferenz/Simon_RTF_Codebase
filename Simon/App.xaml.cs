using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using DLToolkit.Forms.Controls;
using Plugin.FirebasePushNotification;
using Simon.Helpers;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Simon.Views;
using Xamarin.Forms;

namespace Simon
{
    public partial class App : Application
    {
        public static string ScreenTitle { get; set; } = string.Empty;

        public static UserListModel SelectedUserData { get; set; }
        public static ObservableCollection<messages> MessageThreadData { get; set; }

        public static int buttonClick;
        public static int selectedPageId;

        public static bool IsFromKeyboardDoneButton { get; set; } = false;
        public static bool IsFromAddParticipantPage { get; set; } = false;
        public static bool IsSelectRead { get; set; } = false;
        public static bool IsSelectUnRead { get; set; } = false;
        public static bool AsceDsce { get; set; } = true;
        public static bool AsceDsceName { get; set; } = true;
        public static bool IsFirstTime { get; set; } = false;
        public static bool AsceDsceApproval { get; set; } = true;
        public static bool AsceDsceNameApproval { get; set; } = true;

        public static string FollowUp { get; set; } = string.Empty;
        public static string ReadUnread { get; set; } = string.Empty;
        public static string OrderByText { get; set; } = string.Empty;
        public static string SelectedTitle { get; set; } = string.Empty;
        public static string SelectedName { get; set; } = string.Empty;
        public static string ApprovalSelectedTitle { get; set; } = string.Empty;
        public static string tempFile { get; set; } = string.Empty;
        public static string FileName { get; set; } = string.Empty;
        public static string base64String { get; set; } = string.Empty;
        public static string Link { get; set; } = string.Empty;

        public static ImageSource FrameImage;

        public App()
        {
            InitializeComponent();

            //tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
            //File.AppendAllText(App.tempFile, "Add log to File");
            //Debug.WriteLine("File Name====" + App.tempFile);

            FlowListView.Init();

            LayoutService.Init();

            XF.Material.Forms.Material.Init(this);

            NavigationPage navPage = new NavigationPage
            {
                BarBackgroundColor = Color.White,
                BarTextColor = Color.Black,
            };
            
            if (Settings.LoggedInUser != null)
            {
                IsFirstTime = true;
                MainPage = new NavigationPage(new LandingPage())
                {
                    BarTextColor = Color.Black,
                    BarBackgroundColor = Color.White
                };
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage())
                {
                    BarTextColor = Color.Black,
                    BarBackgroundColor = Color.White
                };
            }
        }

        protected override void OnStart()
        {
            Settings.DeviceToken = CrossFirebasePushNotification.Current.Token;
            Debug.WriteLine($"Device Token: " + Settings.DeviceToken);
            //App.Current.MainPage.DisplayAlert("Simon", Settings.DeviceToken, "OK");

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                Settings.DeviceToken = p.Token;
                Debug.WriteLine($"TOKEN REC: {Settings.DeviceToken}");
                Console.WriteLine("Token ref : " + Settings.DeviceToken);
            };

            Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");
            Console.WriteLine("Token " + CrossFirebasePushNotification.Current.Token);

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    Debug.WriteLine("Received");
                    foreach (var data in p.Data)
                    {
                        Debug.WriteLine($"{data.Key} : {data.Value}");

                        if (data.Key.Equals("MsgCount"))
                        {
                            MessagingCenter.Send(new MessageViewModel(), "OnNotificationReceived", Convert.ToInt32(data.Value));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    Debug.WriteLine($"{data.Key} : {data.Value}");
                    if (data.Key.Equals("MsgCount"))
                    {
                        MessagingCenter.Send(new MessageViewModel(), "OnNotificationOpen", Convert.ToInt32(data.Value));
                    }
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                Debug.WriteLine("Dismissed");
            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}