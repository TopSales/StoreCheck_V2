using SkiaSharp;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ZPF.XF.Compos
{
#if true
    public class Tile : Button
    {
        public string IconChar { get; internal set; }
        public Microsoft.Maui.Graphics.Color IconColor { get; internal set; }
    }
#else
    public class Tile : Grid
    {
        public static byte DisabledTextAlpha = 0x88;
        public static byte DisabledIconAlpha = 0x88;
        public static byte DisabledBackgroundAlpha = 0xFF;

        SKCanvasView _CanvasView = new SKCanvasView();
        private Button _Button = new Button();

        // - - -  - - - 

        double _Scale = 1;

        public Tile()
        {
            base.BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;

            _CanvasView.PaintSurface += OnCanvasViewPaintSurface;
            Children.Add(_CanvasView, 0, 0);

            _Scale = 1;

            // - - -  - - - 

            _Button = new Button()
            {
                Text = "",
                BorderColor = Microsoft.Maui.Graphics.Colors.Transparent,
                BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
            };

            if (DeviceInfo.Platform != DevicePlatform.iOS)
            {
                _Button.Opacity = 0;
            };

            _Button.Clicked += X_Clicked;
            _Button.Pressed += _Button_Pressed;
            _Button.Released += _Button_Released;
            _Button.Unfocused += _Button_Released;

            //ToDo: Mouse down out or finger slide out

            this.Add(_Button, 0, 0);

            // - - -  - - - 

            SizeChanged += Tile_SizeChanged;
            Focused += Tile_Focused;
        }

        private void Tile_Focused(object sender, FocusEventArgs e)
        {
            SetFocus();
        }

        public void SetFocus()
        {
            _Button.Focus();
        }

        private void _Button_MeasureInvalidated(object sender, EventArgs e)
        {
        }

        private void _Button_Pressed(object sender, EventArgs e)
        {
            IsPressed = true;
        }

        private void _Button_Released(object sender, EventArgs e)
        {
            IsPressed = false;
        }

        private void Tile_SizeChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Tile_SizeChanged");

            Redraw();
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

        public static readonly BindableProperty IconCharProperty = BindableProperty.Create(
          nameof(IconChar),
          typeof(string),
          typeof(Tile),
          null,
          propertyChanging: (bindable, oldValue, newValue) =>
          {
              var control = bindable as Tile;
              control.IconChar = (string)newValue;
          });

        public string IconChar
        {
            get => _IconChar;
            set { _IconChar = value; Redraw(); }
        }
        string _IconChar = "";

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

        public ImageSource Source { get => _Source; set { _Source = value; Redraw(); } }
        ImageSource _Source = null;

        // - - -  - - - 

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
          nameof(FontFamily),
          typeof(string),
          typeof(Tile),
          "",
          propertyChanging: (bindable, oldValue, newValue) =>
          {
              var control = bindable as Tile;
              control.FontFamily = (string)newValue;
          });

        public string FontFamily { get => _FontFamily; set { _FontFamily = value; Redraw(); } }
        string _FontFamily = "";

        // - - -  - - - 

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
          nameof(FontAttributes),
          typeof(FontAttributes),
          typeof(Tile),
          FontAttributes.None,
          propertyChanging: (bindable, oldValue, newValue) =>
          {
              var control = bindable as Tile;
              control.FontAttributes = (FontAttributes)newValue;
          });

        public FontAttributes FontAttributes { get => _FontAttributes; set { _FontAttributes = value; Redraw(); } }
        FontAttributes _FontAttributes = FontAttributes.None;

        // - - -  - - - 

        public bool BoldText { get => _BoldText; set { _BoldText = value; Redraw(); } }
        bool _BoldText = true;

        // - - -  - - - 

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
          nameof(TextColor),
          typeof(Microsoft.Maui.Graphics.Color),
          typeof(Tile),
          ColorViewModel.Current.ActionTextColor,
          propertyChanging: (bindable, oldValue, newValue) =>
          {
              var control = bindable as Tile;
              control.TextColor = (Microsoft.Maui.Graphics.Color)newValue;
          });

        /// <summary>
        /// Gets or sets the Microsoft.Maui.Graphics.Color for the text of the tile. 
        /// </summary>
        public Microsoft.Maui.Graphics.Color TextColor
        {
            get => _TextColor;
            set
            {
                _TextColor = (Microsoft.Maui.Graphics.Color)(value);
                _IconColor = (Microsoft.Maui.Graphics.Color)(value);
                Redraw();
            }
        }
        Microsoft.Maui.Graphics.Color _TextColor = ColorViewModel.Current.ActionTextColor;

        // - - -  - - - 

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
                nameof(FontSize),
                typeof(double),
                typeof(Tile),
                24.0,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    control.FontSize = (double)newValue;
                });

        /// <summary>
        /// Gets or sets the font size of the tile.
        /// </summary>
        public double FontSize { get => _FontSize; set { _FontSize = (double)(value); Redraw(); } }
        double _FontSize = 24;

        // - - -  - - - 

        public static readonly BindableProperty FontYProperty = BindableProperty.Create(
                nameof(FontY),
                typeof(double),
                typeof(Tile),
                1.0,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    control.FontY = (double)newValue;
                });

        /// <summary>
        /// Gets or sets the Y decalibration of the tile.
        /// </summary>
        public double FontY { get => _FontY; set { _FontY = (double)(value); Redraw(); } }
        double _FontY = 1.0;

        // - - -  - - - 
        public static readonly BindableProperty ImageScaleProperty = BindableProperty.Create(
                nameof(ImageScale),
                typeof(double),
                typeof(Tile),
                (double)1,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    control.ImageScale = (double)newValue;
                });

        /// <summary>
        /// Gets or sets the image scale of the tile.
        /// </summary>
        public double ImageScale { get => _ImageScale; set { _ImageScale = (double)(value); Redraw(); } }
        double _ImageScale = 1;

        // - - -  - - - 

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
                nameof(CornerRadius),
                typeof(double),
                typeof(Tile),
                (double)5,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    control.CornerRadius = (double)newValue;
                });

        /// <summary>
        /// Gets or sets the corner radius of the tile.
        /// </summary>
        public double CornerRadius { get => _CornerRadius; set { _CornerRadius = (double)(value); Redraw(); } }
        double _CornerRadius = 5;

        // - - -  - - - 

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
                nameof(BorderWidth),
                typeof(double),
                typeof(Tile),
                (double)0,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    control.BorderWidth = (double)newValue;
                });

        /// <summary>
        /// Gets or sets the border width of the tile.
        /// </summary>
        public double BorderWidth { get => _BorderWidth; set { _BorderWidth = (double)(value); Redraw(); } }
        double _BorderWidth = 0;

        // - - -  - - - 

        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
                 nameof(BackgroundColor),
                 typeof(Microsoft.Maui.Graphics.Color),
                 typeof(Tile),
                 ColorViewModel.Current.ActionBackgroundColor,
                 propertyChanging: (bindable, oldValue, newValue) =>
                 {
                     var control = bindable as Tile;
                     control.BackgroundColor = (Microsoft.Maui.Graphics.Color)newValue;
                 });

        /// <summary>
        /// Gets or sets the color which will fill the background of the tile.
        /// </summary>
        public new Microsoft.Maui.Graphics.Color BackgroundColor
        {
            get => _BackgroundColor;
            set
            {
                if (_BackgroundColor != value)
                {
                    _BackgroundColor = value;
                    Redraw();
                };
            }
        }
        Microsoft.Maui.Graphics.Color _BackgroundColor = ColorViewModel.Current.ActionBackgroundColor;

        // - - -  - - - 

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                 nameof(BorderColor),
                 typeof(Microsoft.Maui.Graphics.Color),
                 typeof(Tile),
                 ColorViewModel.Current.ActionTextColor,
                 propertyChanging: (bindable, oldValue, newValue) =>
                 {
                     var control = bindable as Tile;
                     control.BorderColor = (Microsoft.Maui.Graphics.Color)newValue;
                 });

        /// <summary>
        /// Gets or sets the color which will fill the Border of the tile.
        /// </summary>
        public Microsoft.Maui.Graphics.Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor != value)
                {
                    _BorderColor = value;
                    Redraw();
                };
            }
        }
        Microsoft.Maui.Graphics.Color _BorderColor = ColorViewModel.Current.ActionTextColor;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public static readonly new BindableProperty IsEnabledProperty = BindableProperty.Create(
                nameof(IsEnabled),
                typeof(bool),
                typeof(Tile),
                true,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    var changingFrom = oldValue as object;
                    var changingTo = newValue as object;

                    control.IsEnabled = (bool)newValue;
                });

        /// <summary>
        /// Gets or sets a value indicating whether the tile is enabled in the user interface. 
        /// </summary>
        public new bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                if (base.IsEnabled != value)
                {
                    base.IsEnabled = value;

                    // - - -  - - - 

                    // _Frame.Opacity = (IsEnabled ? 1 : 0.4);

                    Redraw();
                };
            }
        }

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public bool IsPressed
        {
            get { return _IsPressed; }
            set
            {
                if (_IsPressed != value)
                {
                    _IsPressed = value;

                    Redraw();
                };
            }
        }
        bool _IsPressed = false;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                  nameof(Text),
                  typeof(string),
                  typeof(Tile),
                  null,
                  propertyChanging: (bindable, oldValue, newValue) =>
                  {
                      var control = bindable as Tile;
                      var changingFrom = oldValue as object;
                      var changingTo = newValue as object;

                      control.Text = (string)newValue;
                  });

        public string Text { get => _Text; set { _Text = value; Redraw(); } }
        string _Text = "";

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public static readonly BindableProperty BadgeProperty = BindableProperty.Create(
                nameof(Badge),
                typeof(string),
                typeof(Tile),
                null,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    var control = bindable as Tile;
                    var changingFrom = oldValue as object;
                    var changingTo = newValue as object;

                    control.Badge = (string)newValue;
                });

        public string Badge { get => _Badge; set { _Badge = value; Redraw(); } }
        string _Badge = "";

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public static readonly BindableProperty BadgeColorProperty = BindableProperty.Create(
           nameof(BadgeColor),
           typeof(Microsoft.Maui.Graphics.Color),
           typeof(Tile),
           ColorViewModel.Current.ActionTextColor,
           propertyChanging: (bindable, oldValue, newValue) =>
           {
               var control = bindable as Tile;
               control.BadgeColor = (Microsoft.Maui.Graphics.Color)newValue;
           });

        /// <summary>
        /// Gets or sets the Microsoft.Maui.Graphics.Color for the Badge of the tile. 
        /// </summary>
        public Microsoft.Maui.Graphics.Color BadgeColor { get => _BadgeColor; set { _BadgeColor = value; Redraw(); } }
        Microsoft.Maui.Graphics.Color _BadgeColor = Microsoft.Maui.Graphics.Colors.DarkRed;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public int BadgeFontSize { get => _BadgeFontSize; set { _BadgeFontSize = value; Redraw(); } }
        int _BadgeFontSize = 10;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public Microsoft.Maui.Graphics.Color IconColor { get => _IconColor; set { _IconColor = value; Redraw(); } }
        Microsoft.Maui.Graphics.Color _IconColor = ColorViewModel.Current.ActionTextColor;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        public View Content
        {
            get => _Content;
            set
            {
                if (_Content != value)
                {
                    _Content = value;

                    Children.Clear();

                    if (_Content == null)
                    {
                        this.Add(_CanvasView, 0, 0);
                        this.Add(_Button, 0, 0);
                    }
                    else
                    {
                        this.Add(_CanvasView, 0, 0);
                        this.Add(_Content, 0, 0);
                        this.Add(_Button, 0, 0);
                    };
                };
            }
        }
        View _Content = null;

        // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

        /// <summary>
        /// Occurs when the tile is clicked.
        /// </summary>
        public event EventHandler Clicked;

        // - - -  - - - 

        public string Hint { get => _Hint; set { _Hint = value; Redraw(); } }
        string _Hint = "";

        // - - -  - - - 

        /// <summary>
        /// Gets or sets the parameter to pass to the Command property. 
        /// </summary>
        public object CommandParameter { get => _Button.CommandParameter; set => _Button.CommandParameter = value; }

        // - - -  - - - 

        public Tile PlaceTile(AbsoluteLayout absoluteLayout, double PosX, double PosY, ImageSource imageSource, string Tag, EventHandler OnClicked, double width, double height = -1)
        {
            return PlaceTile(absoluteLayout, PosX, PosY, imageSource, "", Tag, OnClicked, width, height);
        }

        public Tile PlaceTile(AbsoluteLayout absoluteLayout, double PosX, double PosY, string iconChar, string Tag, EventHandler OnClicked, double width, double height = -1)
        {
            return PlaceTile(absoluteLayout, PosX, PosY, null, iconChar, Tag, OnClicked, width, height);
        }

        public Tile PlaceTile(AbsoluteLayout absoluteLayout, double PosX, double PosY, ImageSource imageSource, string iconChar, string Tag, EventHandler OnClicked, double width, double height = -1)
        {
            Debug.WriteLine("PlaceTile");

            // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/images/
            try
            {
                Clicked -= OnClicked;
                Clicked += OnClicked;

                if (height == -1)
                {
                    height = width;
                };

                Text = Tag;

                try
                {
                    absoluteLayout.Children.Add(this);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                };

                AbsoluteLayout.SetLayoutBounds(this, new Rect(PosX, PosY, width, height));

                if (imageSource != null)
                {
                    try
                    {
                        //// make your image your button should be
                        //_Image.Source = imageSource;

                        //double ImgSize = width / 2;

                        //_Image.WidthRequest = ImgSize;
                        //_Image.HeightRequest = ImgSize;
                    }
                    catch (Exception ex)
                    {
                        Tag = ex.Message;
                        Debug.WriteLine("missing " + imageSource);
                    };
                };

                IconChar = iconChar;

                _Button.CommandParameter = CommandParameter;

                Redraw();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PlaceTile: {ex.Message}");
            };

            return this;
        }

        private bool Sema_X_Clicked = true;
        private void X_Clicked(object sender, EventArgs e)
        {
            if (Sema_X_Clicked)
            {
                Sema_X_Clicked = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    _CanvasView.Opacity = 0.5;
                });

                // - - -  - - - 

                Clicked?.Invoke(this, e);

                // - - -  - - - 

                Device.BeginInvokeOnMainThread(() =>
                {
                    _CanvasView.Opacity = 1;
                });

                Sema_X_Clicked = true;
            };
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

        private void Redraw()
        {
            Debug.WriteLine("Redraw");

            if (MainThread.IsMainThread)
            {
                // Code to run if this is the main thread
                // --> OK
            }
            else
            {
                // Code to run if this is a secondary thread
                AT.Log.Write(new AT.AuditTrail { Level = AT.ErrorLevel.Error, Message = "Tile.Redraw: Not running on main thread!!!", DataInType = "TXT", DataIn = Environment.StackTrace });
                return;
            };

            try
            {
                if (_CanvasView != null)
                {
                    _CanvasView.InvalidateSurface();
                };
            }
            catch (Exception ex)
            {
                //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Tile.Redraw", new Dictionary<string, string>
                //   {
                //      { "Source", ex.Source },
                //      { "Message", ex.Message },
                //      { "StackTrace", ( ex.StackTrace != null ? ex.StackTrace : "empty")},
                //      { "InnerException", ( ex.InnerException != null ? ex.InnerException.Message : "no" )},
                //   });
            };
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

        public object Tag { get; set; }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

        enum TOrientation { Horizontal, Vertical, Square }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            try
            {
                SKImageInfo info;
                SKSurface surface = null;
                SKCanvas canvas = null;

                try
                {
                    info = args.Info;
                    surface = args.Surface;
                    canvas = surface.Canvas;
                }
                catch (Exception ex)
                {
                    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Tile.OnCanvasViewPaintSurface1", new Dictionary<string, string>
                    //{
                    //   { "Source", ex.Source },
                    //   { "Message", ex.Message },
                    //   { "StackTrace", ( ex.StackTrace != null ? ex.StackTrace : "empty")},
                    //   { "InnerException", ( ex.InnerException != null ? ex.InnerException.Message : "no" )},
                    //});

                    return;
                };

                _Scale = 1;

                canvas.Clear(SKColor.Parse("0000"));

                float PosX = 0;
                float PosY = 0;

                float width = info.Width;
                float height = info.Height;

                if (IsPressed)
                {
                    float Margin = 5;
                    PosX = Margin;
                    PosY = Margin;

                    width = width - 2 * Margin;
                    height = height - 2 * Margin;
                };

                float size = Math.Min(width, height);

                float IconTextSize = (float)(size * 1.228 * ImageScale);

                // - - -  - - - 

                SKPaint paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = BackgroundColor.ToSKColor(),
                    //Color = SKColors.Yellow,
                    IsAntialias = true,
                };

                SKRect rect = new SKRect(PosX, PosY, PosX + width, PosY + height);

                if (BorderWidth > 0)
                {
                    paint.Color = (IsEnabled ? BorderColor.ToSKColor() : BorderColor.ToSKColor().WithAlpha(DisabledBackgroundAlpha));
                    canvas.DrawRoundRect(rect, (float)(CornerRadius * _Scale), (float)(CornerRadius * _Scale), paint);

                    paint.Color = (IsEnabled ? BackgroundColor.ToSKColor() : BackgroundColor.ToSKColor().WithAlpha(DisabledBackgroundAlpha));

                    rect.Left = (float)(rect.Left + BorderWidth * _Scale);
                    rect.Right = (float)(rect.Right - BorderWidth * _Scale);
                    rect.Top = (float)(rect.Top + BorderWidth * _Scale);
                    rect.Bottom = (float)(rect.Bottom - BorderWidth * _Scale);

                    canvas.DrawRoundRect(rect, (float)((CornerRadius - BorderWidth) * _Scale), (float)((CornerRadius - BorderWidth) * _Scale), paint);
                }
                else
                {
                    paint.Color = (IsEnabled ? BackgroundColor.ToSKColor() : BackgroundColor.ToSKColor().WithAlpha(DisabledBackgroundAlpha));
                    canvas.DrawRoundRect(rect, (float)(CornerRadius * _Scale), (float)(CornerRadius * _Scale), paint);
                };

                // - - -  - - - 

                TOrientation orientation = ((width > (height * 2)) ? TOrientation.Horizontal : TOrientation.Vertical);

                // - - -  - - - 

                var posIcon = new SKPoint(PosX, PosY);
                var posText = new SKPoint(PosX, PosY);
                var posHint = new SKPoint(PosX, PosY);

                SKPaint paintIcon = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = IconColor.ToSKColor(),
                    IsAntialias = true,
                    //TextAlign = SKTextAlign.Center,

                    StrokeWidth = 1
                };

                paintIcon.Color = (IsEnabled ? paintIcon.Color : paintIcon.Color.WithAlpha(DisabledIconAlpha));

                {
                    // var tf = SKTypeface.FromStream(XFHelper.GetStreamFromResources(typeof(PageEx), "ZPF_XF_Compos.Fonts.IconFont.ttf"));
                    //var tf = SKTypeface.FromFile(ZPF.Fonts.IF.GetFamilyName().Replace(@"\\", @"\").Replace(@"file:///", ""));

                    string fn = ZPF.Fonts.IF.GetFamilyName().Replace(@"\\", @"\").Replace(@"file:///", "").Replace(@"ZPF_Fonts", "Fonts");
                    Debug.WriteLine(fn);

                    //string path = Environment.GetFolderPath( Environment.SpecialFolder.Resources);
                    //Xamarin.Essentials.FileSystem.OpenAppPackageFileAsync
                    //var files = System.IO.Directory.GetFileSystemEntries(path);
                    //foreach (var file in files)
                    //{
                    //   Debug.WriteLine(file);
                    //};

                    SKTypeface tf = null;

                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            tf = SKTypeface.FromStream(GetStreamFromResources(typeof(ZPF.Fonts.IF), "ZPF_Fonts.fonts.IconFont.ttf"));
                            break;

                        default:
                            tf = SKTypeface.FromFile(fn);
                            break;
                    };

                    // Environment.GetFolderPath(  Environment.SpecialFolder.LocalApplicationData )

                    paintIcon.Typeface = tf;
                    paintIcon.TextSize = IconTextSize;
                };

                // - - -  - - - 

                switch (orientation)
                {
                    case TOrientation.Horizontal:
                        paintIcon.TextSize = (float)(IconTextSize * 0.7);

                        posIcon = new SKPoint(PosX + width / 2, (float)(PosY + height * 0.5) + GetMidY(paintIcon, IconChar));

                        if (string.IsNullOrEmpty(Text))
                        {
                            posIcon = CenterH(paintIcon, posIcon, IconChar);
                        }
                        else
                        {
                            posIcon.X = (float)(height * 0.1);
                        };
                        break;

                    // - - -  - - - 

                    case TOrientation.Vertical:
                    case TOrientation.Square:
                        paintIcon.TextSize = (float)(IconTextSize * 0.55);

                        if (string.IsNullOrEmpty(Text))
                        {
                            posIcon = new SKPoint(PosX + width / 2, (float)(PosY + height * 0.5) + GetMidY(paintIcon, IconChar));
                        }
                        else
                        {
                            posIcon = new SKPoint(PosX + width / 2, (float)(PosY + height * 0.35) + GetMidY(paintIcon, IconChar));
                        };

                        //posIcon = CenterH(paintIcon, posIcon, IconChar);
                        posIcon.X = width / 2;
                        paintIcon.TextAlign = SKTextAlign.Center;

                        break;
                };

                // - - -  - - - 

                SKPaint paintText = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = TextColor.ToSKColor(),
                    IsAntialias = true,
                    IsStroke = false,
                    TextSize = (float)(FontSize * _Scale),
                    TextAlign = SKTextAlign.Center,
                    FakeBoldText = BoldText,
                };

                if (!string.IsNullOrEmpty(FontFamily))
                {
                    SKTypeface tf = null;

                    tf = SKTypeface.FromFamilyName(FontFamily);

                    if (tf == null)
                    {
                        //var f = XFHelper.GetStreamFromResources(typeof(PageEx), $"ZPF_XF_Compos.Fonts.{FontFamily}.ttf");
                        var f = XFHelper.GetStreamFromResources(typeof(ZPF.Fonts.IF), $"ZPF_Fonts.fonts.{FontFamily}.ttf");

                        if (f != null)
                        {
                            try
                            {
                                tf = SKTypeface.FromStream(f);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            };
                        };
                    };

                    paintText.Typeface = tf;
                };

                paintText.Color = (IsEnabled ? paintText.Color : paintText.Color.WithAlpha(DisabledTextAlpha));

                SKPaint paintHint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = paintText.Color,
                    IsAntialias = true,
                    IsStroke = false,
                    TextSize = (float)(FontSize * _Scale * 0.70),
                    TextAlign = SKTextAlign.Center,
                };

                float textHeight = 0;
                float textHeightText = 0;
                float textHeightHint = 0;

                if (!string.IsNullOrEmpty(Text))
                {
                    textHeightText = GetHeight(paintText, Text);

                    if (!string.IsNullOrEmpty(Hint))
                    {
                        textHeightHint = GetHeight(paintHint, Hint);
                    };
                };

                textHeight = textHeightText + (float)(textHeightHint * 0.5) + textHeightHint;

                // - - -  - - - 

                posText = new SKPoint(PosX + width / 2, (float)(PosY + height * 0.5));
                posText = CenterV(paintText, posText, Text);

                posHint = new SKPoint(PosX + width / 2, (float)(PosY + height * 0.5));
                posHint = CenterV(paintHint, posHint, Hint);

                switch (orientation)
                {
                    case TOrientation.Horizontal:
                        if (string.IsNullOrEmpty(Hint))
                        {
                            posText.Y = (float)(height / 2) + textHeightText / 2;
                        }
                        else
                        {
                            posText.Y = (float)(height / 2);
                        };

                        if (!string.IsNullOrEmpty(IconChar))
                        {
                            paintText.TextAlign = SKTextAlign.Left;
                            paintHint.TextAlign = SKTextAlign.Left;

                            posText.X = (float)(height * 1.1 * ImageScale);
                            posHint.X = posText.X;
                        };
                        break;

                    // - - -  - - - 

                    case TOrientation.Vertical:
                    case TOrientation.Square:
                        if (string.IsNullOrEmpty(IconChar))
                        {
                            posText.Y = (float)(height / 2);

                            if (Text == ")")
                            {
                            };

                            posText = CenterV(paintText, posText, Text);
                        }
                        else
                        {
                            posText.Y = (float)(height * 0.8);
                        };

                        break;
                };

                // - - -  - - - 

                if (!string.IsNullOrEmpty(IconChar))
                {
                    // ((int)(IconChar[0])).ToString("X")

                    //posIcon.X = width / 2;
                    //paintIcon.TextAlign = SKTextAlign.Center;

                    canvas.DrawText(IconChar, posIcon, paintIcon);
                };

                if (!string.IsNullOrEmpty(Text))
                {
                    if (Text.Contains("\n") || Text.Contains("|"))
                    {
                        var t = Text.Split(new char[] { '\n', '\r', '|' });

                        posText.Y = posText.Y - (float)(textHeightText * 0.5);
                        canvas.DrawText(t[0], posText, paintText);
                        posText.Y = posText.Y + textHeightText + (float)(textHeightText * 0.5);
                        canvas.DrawText(t[1], posText, paintText);
                    }
                    else
                    {
                        canvas.DrawText(Text, posText, paintText);
                    };

                    if (!string.IsNullOrEmpty(Hint))
                    {
                        posHint.Y = posText.Y + textHeightText + (float)(textHeightHint * 0.5);
                        //canvas.DrawText(Hint, posHint, paintHint);

                        DrawTextArea(canvas, paintHint, posHint.X, posHint.Y, (float)(width - height * 1.2 * ImageScale), (float)(textHeightHint * 1.5), Hint);
                    };
                };

                // - - -  - - - 

                if (!string.IsNullOrEmpty(_Badge))
                {
                    paintText.TextAlign = SKTextAlign.Center;
                    paintText.TextSize = (float)(BadgeFontSize * _Scale);

                    var posBadge = new SKPoint((float)(width * 0.83), (float)(height * 0.17));

                    if (orientation == TOrientation.Horizontal)
                    {
                        posBadge.Y = (float)(height / 2);
                        posBadge.X = (float)(width * 0.87);
                    };

                    SKPaint paintBadge = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = BadgeColor.ToSKColor(),
                        IsAntialias = true,
                    };

                    var bounds = new SKRect();
                    paintText.MeasureText(_Badge, ref bounds);

                    canvas.DrawCircle(posBadge, (float)(Math.Max(bounds.Width, bounds.Height) * 0.9), paintBadge);

                    posBadge = CenterV(paintText, posBadge, _Badge);
                    canvas.DrawText(_Badge, posBadge, paintText);


                    paintText.TextSize = (float)(FontSize * _Scale);
                };
            }
            catch (Exception ex)
            {
                //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Tile.OnCanvasViewPaintSurface2", new Dictionary<string, string>
                //   {
                //      { "Source", ex.Source },
                //      { "Message", ex.Message },
                //      { "StackTrace", ( ex.StackTrace != null ? ex.StackTrace : "empty")},
                //      { "InnerException", ( ex.InnerException != null ? ex.InnerException.Message : "no" )},
                //   });

                return;
            };
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
        public static Stream GetStreamFromResources(Type type, string resourceName)
        {
            var assembly = type.GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(resourceName);
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

        private SKPoint CenterV(SKPaint paint, SKPoint point, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return point;
            };

            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);

            if (FontY != 1.0)
            {
                point.Y = (float)(point.Y * FontY);
            };

            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                return new SKPoint(point.X, (float)(point.Y + bounds.Height * 0.52));
            }
            else
            {
                return new SKPoint(point.X, (float)(point.Y + bounds.Height * 0.54));
            };
        }

        private SKPoint CenterH(SKPaint paint, SKPoint point, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return point;
            };

            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);

            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                return new SKPoint((float)(point.X - bounds.Width * 0.59), point.Y);
            }
            else
            {
                return new SKPoint((float)(point.X - bounds.Width * 0.615), point.Y);
            };
        }

        float GetHeight(SKPaint paint, string text)
        {
            //text = "Ïg" + text;

            //var bounds = new SKRect();
            //paint.MeasureText(text, ref bounds);

            //return bounds.Height;

            return GetMidY(paint, Text) * 2;
        }

        float GetMidY(SKPaint paint, string text)
        {
            text = "Ïg" + text;

            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);

            return System.Math.Abs(bounds.MidY);
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

        private void DrawTextArea(SKCanvas canvas, SKPaint paint, float x, float y, float maxWidth, float lineHeight, string text)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines = lines.SelectMany(l => SplitLine(paint, maxWidth, l, spaceWidth)).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                canvas.DrawText(line, x, y, paint);
                y += lineHeight;
            }
        }

        private string[] SplitLine(SKPaint paint, float maxWidth, string text, float spaceWidth)
        {
            var result = new List<string>();

            var words = text.Split(new[] { " " }, StringSplitOptions.None);

            var line = new StringBuilder();
            float width = 0;

            foreach (var word in words)
            {
                var wordWidth = paint.MeasureText(word);
                var wordWithSpaceWidth = wordWidth + spaceWidth;
                var wordWithSpace = word + " ";

                if (width + wordWidth > maxWidth)
                {
                    result.Add(line.ToString());
                    line = new StringBuilder(wordWithSpace);
                    width = wordWithSpaceWidth;
                }
                else
                {
                    line.Append(wordWithSpace);
                    width += wordWithSpaceWidth;
                }
            }

            result.Add(line.ToString());

            return result.ToArray();
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
    }

#endif
}
