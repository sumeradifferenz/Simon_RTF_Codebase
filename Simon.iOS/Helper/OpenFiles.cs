using System;
using Foundation;
using Simon.Interfaces;
using Simon.iOS.Helper;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenFiles))]
namespace Simon.iOS.Helper
{
    public class OpenFiles : IOpenFiles
    {
        public OpenFiles()
        {
        }

        public void OpenAppFiles(string fileName, string filePath)
        {
            var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(filePath));
            PreviewController.Delegate = new UIDocumentInteractionControllerDelegateClass(UIApplication.SharedApplication.KeyWindow.RootViewController);
            Device.BeginInvokeOnMainThread(() =>
            {
                PreviewController.PresentPreview(true);
            });
        }

        public class UIDocumentInteractionControllerDelegateClass : UIDocumentInteractionControllerDelegate
        {
            UIViewController ownerVC;

            public UIDocumentInteractionControllerDelegateClass(UIViewController vc)
            {
                ownerVC = vc;
            }

            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
            {
                return ownerVC;
            }

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
            {
                return ownerVC.View;
            }
        }
    }
}
