using System;
using Foundation;
using Simon.Interfaces;
using Simon.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenSetting))]
namespace Simon.iOS
{
    public class OpenSetting : IOpenSetting
    {
        public OpenSetting()
        {
            
        }

        public void OpenAppSetting()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }
    }
}
