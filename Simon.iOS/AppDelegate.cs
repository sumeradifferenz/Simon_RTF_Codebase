using System;
using System.Runtime.InteropServices;
using DLToolkit.Forms.Controls;
using Foundation;
using LabelHtml.Forms.Plugin.iOS;
using Plugin.FirebasePushNotification;
using UIKit;
using UserNotifications;
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
            FirebasePushNotificationManager.CurrentNotificationPresentationOption = UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge | UNNotificationPresentationOptions.Sound;

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
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                byte[] result = new byte[deviceToken.Length];
                Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
                deviceToken = BitConverter.ToString(result).Replace("-", "");
            }
            
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            Console.WriteLine(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return false;
        }
    }
}