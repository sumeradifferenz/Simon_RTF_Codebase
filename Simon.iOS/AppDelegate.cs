﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using DLToolkit.Forms.Controls;
using Foundation;
using LabelHtml.Forms.Plugin.iOS;
using Plugin.FirebasePushNotification;
using Plugin.PushNotification;
using Simon.Helpers;
using UIKit;
using Xamarin;
using Xamarin.Forms;

namespace Simon.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        //public IAuthorizationFlowSession CurrentAuthorizationFlow { get; set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            Forms.SetFlags(new[]
            {
                "Expander_Experimental"
            });

            HtmlLabelRenderer.Initialize();
            global::Xamarin.Forms.Forms.Init();
           
            Rg.Plugins.Popup.Popup.Init();
            XF.Material.iOS.Material.Init();
 
            IQKeyboardManager.SharedManager.Enable = true;
            IQKeyboardManager.SharedManager.EnableAutoToolbar = true;
            IQKeyboardManager.SharedManager.ShouldResignOnTouchOutside = true;
            IQKeyboardManager.SharedManager.PreviousNextDisplayMode = IQPreviousNextDisplayMode.AlwaysHide;

            FlowListView.Init();

            LoadApplication(new App());

            FirebasePushNotificationManager.Initialize(options, true);
            FirebasePushNotificationManager.CurrentNotificationPresentationOption = UserNotifications.UNNotificationPresentationOptions.Alert;

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            Syncfusion.XForms.iOS.RichTextEditor.SfRichTextEditorRenderer.Init();

            //UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBarWindow")).ValueForKey(new NSString("statusBar")) as UIView;
            //statusBar.BackgroundColor = UIColor.White;

            var result = base.FinishedLaunching(app, options);
            app.KeyWindow.TintColor = UIColor.Gray;

            // Color of the tabbar background:
            UITabBar.Appearance.BarTintColor = UIColor.FromRGB(247, 247, 247);

            // Color of the selected tab text color:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    TextColor = UIColor.FromRGB(0, 122, 255)
                },
                UIControlState.Selected);

            // Color of the unselected tab icon & text:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    TextColor = UIColor.FromRGB(146, 146, 146)
                },
                UIControlState.Normal);
            //UITabBar.Appearance.TintColor = UIColor.Blue;


            return result;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            byte[] result = new byte[deviceToken.Length];
            Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
            var token = BitConverter.ToString(result).Replace("-", "");

            Debug.WriteLine($"TOKEN : {Settings.DeviceToken}");
            Settings.DeviceToken = token;

            App.tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
            File.AppendAllText(App.tempFile, "\n\nRegisteredForRemoteNotifications call \n\niOS Token : " + token);
            Debug.WriteLine("File Name====" + App.tempFile);

            FirebasePushNotificationManager.DidRegisterRemoteNotifications(token);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url,string sourceApplication, NSObject annotation)
        {
            // Sends the URL to the current authorization flow (if any) which will process it if it relates to
            // an authorization response.
            //if (CurrentAuthorizationFlow?.ResumeAuthorizationFlow(url) == true)
            //{
            //    return true;
            //}

            // Your additional URL handling (if any) goes here.

            return false;
        }
    }
}