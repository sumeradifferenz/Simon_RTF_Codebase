using System;
using Simon.Droid.Helper;
using Simon.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenSetting))]
namespace Simon.Droid.Helper
{
    public class OpenSetting : IOpenSetting
    {
        public OpenSetting()
        {
        }

        public void OpenAppSetting()
        {
            Forms.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionSettings));
        }
    }
}
