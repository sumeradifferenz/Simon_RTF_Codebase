using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DLToolkit.Forms.Controls;
using Plugin.FirebasePushNotification;
using Plugin.PushNotification;
using Simon.Helpers;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.ViewModel;
using Simon.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        public static string ReadUnread { get; set; } = string.Empty;
        public static string OrderByText { get; set; } = string.Empty;
        public static string SelectedTitle { get; set; } = string.Empty;
        public static string SelectedName { get; set; } = string.Empty;
        public static string ApprovalSelectedTitle { get; set; } = string.Empty;
        public static string tempFile;
        public static string FileName;
        public static string base64String;

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
                BarTextColor = Color.Black
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
            //Debug.WriteLine($"Device Token: {CrossFirebasePushNotification.Current.Token}");
            CrossFirebasePushNotification.Current.Subscribe("general");

            if (string.IsNullOrEmpty(Settings.DeviceToken))
            {
                tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
                File.AppendAllText(tempFile, "\n\nDevice token is null....");
                Debug.WriteLine("File Name====" + tempFile);
            }
            else
            {
                tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
                File.AppendAllText(tempFile,"\n\n" + Settings.DeviceToken);
                Debug.WriteLine("File Name====" + tempFile);
            }

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Debug.WriteLine($"TOKEN REC: {Settings.DeviceToken}");
                    Console.WriteLine("Token ref : " + Settings.DeviceToken);
                }
                else
                {
                    Settings.DeviceToken = p.Token;
                    Debug.WriteLine($"TOKEN REC: {Settings.DeviceToken}");
                    Console.WriteLine("Token ref : " + Settings.DeviceToken);
                }

                Debug.WriteLine($"Token generated in plugin: {p.Token}");
            };

            Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");
            Console.WriteLine("Token " + CrossFirebasePushNotification.Current.Token);

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    Debug.WriteLine("Received");
                    tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
                    File.AppendAllText(tempFile, "\n\nNotification Received....");
                    Debug.WriteLine("File Name====" + tempFile);

                    foreach (var data in p.Data)
                    {
                        Debug.WriteLine($"{data.Key} : {data.Value}");

                        if (data.Key.Equals("MsgCount"))
                        {
                            MessagingCenter.Send(new MessageViewModel(), "OnNotificationReceived", Convert.ToInt32(data.Value));
                        }

                        if (selectedPageId == 4)
                        {
                            return;
                        }

                        SessionService.BaseFooterItems.All((arg) =>
                        {
                            if (arg.Id == selectedPageId)
                            {
                                arg.IsSelected = true;
                            }
                            else
                            {
                                arg.IsSelected = false;
                            }
                            return true;
                        });
                    }
                    LayoutService.Init();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                Debug.WriteLine("Opened");
                tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
                File.AppendAllText(tempFile, "\n\nNotification Opened....");
                Debug.WriteLine("File Name====" + tempFile);

                foreach (var data in p.Data)
                {
                    Debug.WriteLine($"{data.Key} : {data.Value}");
                    if (data.Key.Equals("MsgCount"))
                    {
                        Settings.MessageCount = Convert.ToInt32(data.Value);
                    }

                    if (selectedPageId == 4)
                    {
                        return;
                    }

                    SessionService.BaseFooterItems.All((arg) =>
                    {
                        if (arg.Id == selectedPageId)
                        {
                            arg.IsSelected = true;
                        }
                        else
                        {
                            arg.IsSelected = false;
                        }
                        return true;
                    });

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsFirstTime = true;
                        switch (selectedPageId)
                        {
                            case 0:
                                Current.MainPage = new NavigationPage(new LandingPage()) { BarTextColor = Color.Black };
                                break;
                            case 1:
                                Current.MainPage = new NavigationPage(new DealsPage()) { BarTextColor = Color.Black };
                                break;
                            case 2:
                                Current.MainPage = new NavigationPage(new MessagesPage()) { BarTextColor = Color.Black };
                                break;
                            case 3:
                                Current.MainPage = new NavigationPage(new AssentMainPage()) { BarTextColor = Color.Black };
                                break;
                        }
                    });
                }
                LayoutService.Init();
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        Debug.WriteLine($"{data.Key} : {data.Value}");
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
