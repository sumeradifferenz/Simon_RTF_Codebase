using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Widget;
using Simon.Controls;
using Simon.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Simon.Droid.Renderers
{
    [Obsolete]
    public class HtmlLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            Control?.SetText(Html.FromHtml(Element.Text), TextView.BufferType.Spannable);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                Control?.SetText(Html.FromHtml(Element.Text), TextView.BufferType.Spannable);
            }
        }
    }
}
