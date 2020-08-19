using System;
using System.ComponentModel;
using Foundation;
using Simon.Controls;
using Simon.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportFont("Lato_Regular.ttf", Alias = "Lato")]
[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Simon.iOS.Renderers
{
    public class HtmlLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null && !string.IsNullOrWhiteSpace(Element.Text))
            {
                var attr = new NSAttributedStringDocumentAttributes();
                var nsError = new NSError();
                attr.DocumentType = NSDocumentType.HTML;

                nfloat r, g, b, a;
                Control.TextColor.GetRGBA(out r, out g, out b, out a);
                var textColor = string.Format("#{0:X2}{1:X2}{2:X2}", (int)(r * 255.0), (int)(g * 255.0), (int)(b * 255.0));

                UIKit.UIFont font = Control.Font;
                string fontName = font.Name;
                nfloat fontSize = font.PointSize;
                var htmlContents = "<span style=\"font-family: '" + fontName + "'; color: " + textColor + "; font-size: " + fontSize + "\">" + Element.Text + "</span>";
                var myHtmlData = NSData.FromString(htmlContents, NSStringEncoding.Unicode);
                Control.Lines = 0;
                Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                if (Control != null && Element != null && !string.IsNullOrWhiteSpace(Element.Text))
                {
                    var attr = new NSAttributedStringDocumentAttributes();
                    var nsError = new NSError();
                    attr.DocumentType = NSDocumentType.HTML;

                    Control.TextColor.GetRGBA(out nfloat r, out nfloat g, out nfloat b, out nfloat a);
                    var textColor = string.Format("#{0:X2}{1:X2}{2:X2}", (int)(r * 255.0), (int)(g * 255.0), (int)(b * 255.0));

                    UIKit.UIFont font = Control.Font;
                    string fontName = font.Name;
                    nfloat fontSize = font.PointSize;
                    var htmlContents = "<span style=\"font-family: '" + fontName + "'; color: " + textColor + "; font-size: " + fontSize + "\">" + Element.Text + "</span>";
                    var myHtmlData = NSData.FromString(htmlContents, NSStringEncoding.Unicode);
                    Control.Lines = 0;
                    Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
                }
            }
        }
    }
}
