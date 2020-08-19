using System;

using Xamarin.Forms;

namespace Simon.Interfaces
{
    public interface IImageCropperWrapper
    {
        void ShowFromFile(ImageCropper imageCropper, string imageFile);
    }

    public class ImageCropper : ContentView
    {
        public static ImageCropper Current { get; set; }

        public ImageCropper()
        {
            Current = this;
        }
        public enum CropShapeType
        {
            Rectangle,
            Oval
        };

        public CropShapeType CropShape { get; set; } = CropShapeType.Rectangle;

        public int AspectRatioX { get; set; } = 0;

        public int AspectRatioY { get; set; } = 0;

        public string PageTitle { get; set; } = null;

        public string SelectSourceTitle { get; set; } = "Select source";

        public string TakePhotoTitle { get; set; } = "Take Photo";

        public string PhotoLibraryTitle { get; set; } = "Photo Library";

        public Action<string> Success { get; set; }

        public Action Faiure { get; set; }

        public void Show(string imageFile)
        {
            if (string.IsNullOrEmpty(imageFile)) { return; }

            DependencyService.Get<IImageCropperWrapper>().ShowFromFile(this, imageFile);
        }
    }
}

