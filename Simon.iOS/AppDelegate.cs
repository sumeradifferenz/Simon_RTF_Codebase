using System;
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
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
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
        
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            App.tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
            File.AppendAllText(App.tempFile, "\n\nDidReceiveRemoteNotification call");
            Debug.WriteLine("File Name====" + App.tempFile);

            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            Console.WriteLine(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url,string sourceApplication, NSObject annotation)
        {
            return false;
        }
    }
}