using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Simon.Views.SubViews
{
    public partial class HeaderWithImageView : ContentView
    {
        public HeaderWithImageView()
        {
            InitializeComponent();

            TapGestureRecognizer RightClickTGR = new TapGestureRecognizer();
            RightClickTGR.Tapped += (object sender, EventArgs e) =>
            {
                if (RightCommand != null)
                {
                    RightCommand.Execute(this);
                }
            };
            RightImageView.GestureRecognizers.Add(RightClickTGR);

            TapGestureRecognizer ThirdRightClickTGR = new TapGestureRecognizer();
            ThirdRightClickTGR.Tapped += (object sender, EventArgs e) =>
            {
                if (RightThirdCommand != null)
                {
                    RightThirdCommand.Execute(this);
                }
            };
            RightThirdImageView.GestureRecognizers.Add(ThirdRightClickTGR);

            TapGestureRecognizer SecondRightClickTGR = new TapGestureRecognizer();
            SecondRightClickTGR.Tapped += (object sender, EventArgs e) =>
            {
                if (RightSecondCommand != null)
                {
                    RightSecondCommand.Execute(this);
                }
            };
            RightSecondImageView.GestureRecognizers.Add(SecondRightClickTGR);

            TapGestureRecognizer FirstRightClickTGR = new TapGestureRecognizer();
            FirstRightClickTGR.Tapped += (object sender, EventArgs e) =>
            {
                if (RightFirstCommand != null)
                {
                    RightFirstCommand.Execute(this);
                }
            };
            RightFirstImageView.GestureRecognizers.Add(FirstRightClickTGR);
        }

        public static BindableProperty LeftCommandProperty =
                 BindableProperty.Create(nameof(LeftCommand), typeof(Command), typeof(HeaderWithImageView), default(Command), BindingMode.TwoWay, propertyChanged: OnLeftCommandChanged);
        private static void OnLeftCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.LeftCommand = (Xamarin.Forms.Command)newvalue;
            //picker.headertext.Text = newvalue;
        }

        public Command LeftCommand
        {
            get
            {
                return (Command)this.GetValue(LeftCommandProperty);
            }
            set
            {
                this.SetValue(LeftCommandProperty, value);
            }
        }


        public static BindableProperty RightCommandProperty =
                BindableProperty.Create(nameof(RightCommand), typeof(Command), typeof(HeaderWithImageView), default(Command), BindingMode.TwoWay, propertyChanged: OnRightCommandChanged);
        private static void OnRightCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightCommand = (Xamarin.Forms.Command)newvalue;
        }

        public Command RightCommand
        {
            get
            {
                return (Command)this.GetValue(RightCommandProperty);
            }
            set
            {
                this.SetValue(RightCommandProperty, value);
            }
        }

        public static BindableProperty RightThirdCommandProperty =
                BindableProperty.Create(nameof(RightThirdCommand), typeof(Command), typeof(HeaderWithImageView), default(Command), BindingMode.TwoWay, propertyChanged: OnRightThirdCommandChanged);
        private static void OnRightThirdCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightThirdCommand = (Xamarin.Forms.Command)newvalue;
        }

        public Command RightThirdCommand
        {
            get
            {
                return (Command)this.GetValue(RightThirdCommandProperty);
            }
            set
            {
                this.SetValue(RightThirdCommandProperty, value);
            }
        }

        public static BindableProperty RightSecondCommandProperty =
                BindableProperty.Create(nameof(RightSecondCommand), typeof(Command), typeof(HeaderWithImageView), default(Command), BindingMode.TwoWay, propertyChanged: OnRightSecondCommandChanged);
        private static void OnRightSecondCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightSecondCommand = (Xamarin.Forms.Command)newvalue;
        }

        public Command RightSecondCommand
        {
            get
            {
                return (Command)this.GetValue(RightSecondCommandProperty);
            }
            set
            {
                this.SetValue(RightSecondCommandProperty, value);
            }
        }

        public static BindableProperty RightFirstCommandProperty =
            BindableProperty.Create(nameof(RightFirstCommand), typeof(Command), typeof(HeaderWithImageView), default(Command), BindingMode.TwoWay, propertyChanged: OnRightFirstCommandChanged);
        private static void OnRightFirstCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightFirstCommand = (Xamarin.Forms.Command)newvalue;
        }

        public Command RightFirstCommand
        {
            get
            {
                return (Command)this.GetValue(RightFirstCommandProperty);
            }
            set
            {
                this.SetValue(RightFirstCommandProperty, value);
            }
        }

        public static BindableProperty HeaderTextProperty = BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(HeaderWithImageView), default(string), BindingMode.TwoWay, propertyChanged: OnHeaderTextChanged);
        private static void OnHeaderTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            string value = (string)newvalue;
            picker.HeaderText = value;
            picker.headertext.Text = value;
        }

        public string HeaderText
        {
            get
            {
                return (string)this.GetValue(HeaderTextProperty);
            }
            set
            {
                this.SetValue(HeaderTextProperty, value);
            }
        }

        public static BindableProperty RightImageSourceProperty = BindableProperty.Create(nameof(RightImageSource), typeof(ImageSource), typeof(HeaderWithImageView), default(ImageSource), BindingMode.TwoWay, propertyChanged: OnRightImageSourceChanged);
        private static void OnRightImageSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightImageSource = (Xamarin.Forms.ImageSource)newvalue;
            picker.RightImage.Source = (Xamarin.Forms.ImageSource)newvalue;
        }

        public ImageSource RightImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(RightImageSourceProperty);
            }
            set
            {
                this.SetValue(RightImageSourceProperty, value);
            }
        }

        public static BindableProperty RightThirdImageSourceProperty = BindableProperty.Create(nameof(RightThirdImageSource), typeof(ImageSource), typeof(HeaderWithImageView), default(ImageSource), BindingMode.TwoWay, propertyChanged: OnRightThirdImageSourceChanged);
        private static void OnRightThirdImageSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightThirdImageSource = (Xamarin.Forms.ImageSource)newvalue;
            picker.RightThirdImage.Source = (Xamarin.Forms.ImageSource)newvalue;
        }

        public ImageSource RightThirdImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(RightThirdImageSourceProperty);
            }
            set
            {
                this.SetValue(RightThirdImageSourceProperty, value);
            }
        }

        public static BindableProperty RightSecondImageSourceProperty = BindableProperty.Create(nameof(RightSecondImageSource), typeof(ImageSource), typeof(HeaderWithImageView), default(ImageSource), BindingMode.TwoWay, propertyChanged: OnRightSecondImageSourceChanged);
        private static void OnRightSecondImageSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightSecondImageSource = (Xamarin.Forms.ImageSource)newvalue;
            picker.RightSecondImage.Source = (Xamarin.Forms.ImageSource)newvalue;
        }

        public ImageSource RightSecondImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(RightSecondImageSourceProperty);
            }
            set
            {
                this.SetValue(RightSecondImageSourceProperty, value);
            }
        }

        public static BindableProperty RightFirstImageSourceProperty = BindableProperty.Create(nameof(RightFirstImageSource), typeof(ImageSource), typeof(HeaderWithImageView), default(ImageSource), BindingMode.TwoWay, propertyChanged: OnRightFirstImageSourceChanged);
        private static void OnRightFirstImageSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as HeaderWithImageView;
            picker.RightFirstImageSource = (Xamarin.Forms.ImageSource)newvalue;
            picker.RightFirstImage.Source = (Xamarin.Forms.ImageSource)newvalue;
        }

        public ImageSource RightFirstImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(RightFirstImageSourceProperty);
            }
            set
            {
                this.SetValue(RightFirstImageSourceProperty, value);
            }
        }
    }
}
