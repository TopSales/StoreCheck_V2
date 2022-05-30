namespace ZPF.XF.Compos
{
    public class CheckBoxZPF : Tile
   {
      internal string ImageSourceChecked { get; set; }
      internal Microsoft.Maui.Graphics.Color ColorChecked { get; set; }

      internal string ImageSourceUnchecked { get; set; }
      internal Microsoft.Maui.Graphics.Color ColorUnchecked { get; set; }


      public CheckBoxZPF()
      {
         //string GetFontFamily(string FontName = "IconFont")
         //{
         //   switch (Device.RuntimePlatform)
         //   {
         //      case "macOS":
         //      case "iOS":
         //         // Add the font file with Build Action: BundleResource, and
         //         // Update the Info.plist file(Fonts provided by application, or UIAppFonts, key)
         //         return $"{FontName}";

         //      case "Android":
         //         // add the font file to the Assets folder in the application project and set Build Action: AndroidAsset. 
         //         return $"{FontName}.ttf#{FontName}";

         //      case "WPF":
         //         // https://github.com/xamarin/Xamarin.Forms/pull/3225
         //         // add the font file to the /Fonts/ folder in the application project and set the Build Action:Resource - Do not copy.
         //         return $"/Syscall.WPF;component/Fonts/#{FontName}";

         //      default:
         //      case "UWP":
         //         // add the font file to the /Assets/Fonts/ folder in the application project and set the Build Action:Content.
         //         return $"Assets/Fonts/{FontName}.ttf#{FontName}";
         //   };
         //}

         base.Clicked += new EventHandler(OnClicked);
         base.SizeChanged += new EventHandler(OnSizeChanged);

         //base.ContentLayout = new ButtonContentLayout(ButtonContentLayout.ImagePosition.Left, 10);

         Padding = 2;
         BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;

         BorderColor = Microsoft.Maui.Graphics.Colors.Transparent;
         BorderWidth = 1;

         ColorChecked = Microsoft.Maui.Graphics.Colors.Black;
         ColorUnchecked = Microsoft.Maui.Graphics.Colors.Black;

         RaiseCheckedChanged();
      }

      private void OnSizeChanged(object sender, EventArgs e)
      {
         //if (base.Height > 0)
         //{
         //    base.WidthRequest = base.Height;
         //}

         int fontSize = (int)FontSize;

         //ImageSourceChecked = ZPF.XF.Compos.SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Check_Mark_01, fontSize, TextColor);
         //ImageSourceUnchecked = ZPF.XF.Compos.SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Check_Mark_01, fontSize, Microsoft.Maui.Graphics.Color.Transparent);

         ImageSourceChecked = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01);
         ImageSourceUnchecked = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Boxed_Checkbox_01);

         RaiseCheckedChanged();
      }

      public static BindableProperty CheckedProperty = BindableProperty.Create(
          propertyName: "Checked",
          returnType: typeof(bool),
          declaringType: typeof(CheckBoxZPF),
          defaultValue: false,
          defaultBindingMode: BindingMode.TwoWay,
          propertyChanged: CheckedValueChanged);

      public bool Checked
      {
         get => _Checked;
         set
         {
            if (_Checked != value)
            {
               _Checked = value;
               OnPropertyChanged();
               RaiseCheckedChanged();

               SetValue(CheckedProperty, value);
            };
         }
      }
      bool _Checked = false;

      private static void CheckedValueChanged(BindableObject bindable, object oldValue, object newValue)
      {
         (bindable as CheckBoxZPF).Checked = (bool)newValue;
      }

      public event EventHandler CheckedChanged;
      internal void RaiseCheckedChanged()
      {
         Padding = BorderWidth;

         //if (Checked)
         //{
         //   ImageSource = ImageSourceChecked;
         //}
         //else
         //{
         //   ImageSource = ImageSourceUnchecked;
         //};

         if (Checked)
         {
            base.IconColor = ColorChecked;
            IconChar = ImageSourceChecked;
         }
         else
         {
            base.IconColor = ColorUnchecked;
            IconChar = ImageSourceUnchecked;
         };

         if (CheckedChanged != null)
            CheckedChanged(this, EventArgs.Empty);
      }

      private Boolean _IsEnabled = true;

      public new Boolean IsEnabled
      {
         get
         {
            return _IsEnabled;
         }
         set
         {
            _IsEnabled = value;
            OnPropertyChanged();
            if (value == true)
            {
               this.Opacity = 1;
            }
            else
            {
               this.Opacity = .5;
            }
            base.IsEnabled = value;
         }
      }

      public void OnEnabled_Changed()
      {

      }

      public void OnClicked(object sender, EventArgs e)
      {
         Checked = !Checked;

         // Call the base class event invocation method.
         //base.Clicked(sender, e);
      }
   }
}
