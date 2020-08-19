using System;
using System.Diagnostics;
using Com.Theartofdev.Edmodo.Cropper;
using Plugin.CurrentActivity;
using Simon.Droid.Helper;
using Simon.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageCropperImplementation))]
namespace Simon.Droid.Helper
{
    public class ImageCropperImplementation : IImageCropperWrapper
    {
        public void ShowFromFile(ImageCropper imageCropper, string imageFile)
        {
            try
            {
                CropImage.ActivityBuilder activityBuilder = CropImage.Activity(Android.Net.Uri.FromFile(new Java.IO.File(imageFile)));

                if (imageCropper.CropShape == ImageCropper.CropShapeType.Oval)
                {
                    activityBuilder.SetCropShape(CropImageView.CropShape.Oval);
                }
                else
                {
                    activityBuilder.SetCropShape(CropImageView.CropShape.Rectangle);
                }

                if (imageCropper.AspectRatioX > 0 && imageCropper.AspectRatioY > 0)
                {
                    activityBuilder.SetFixAspectRatio(true);
                    activityBuilder.SetAspectRatio(imageCropper.AspectRatioX, imageCropper.AspectRatioY);
                }
                else
                {
                    activityBuilder.SetFixAspectRatio(false);
                }

                if (!string.IsNullOrWhiteSpace(imageCropper.PageTitle))
                {
                    activityBuilder.SetActivityTitle(imageCropper.PageTitle);
                }

                activityBuilder.Start(CrossCurrentActivity.Current.Activity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
