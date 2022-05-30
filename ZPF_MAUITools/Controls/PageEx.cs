using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System.Reflection;
using ZPF.AT;

namespace ZPF.XF.Compos
{
    public class PageEx : ContentPage
    {
        public Grid mainGrid = new Grid();
        public Label labelTitle = new Label();

        Image _BackgroundImage = new Image();
        public new Image BackgroundImage { get => _BackgroundImage; /*set => _BackgroundImage = value;*/ }

        public Action OnPrevious { get; set; }

        public PageEx()
        {
            if (DeviceInfo.Platform != DevicePlatform.iOS && DeviceInfo.Platform != DevicePlatform.macOS)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            };

            // - - -  - - - 

            mainGrid.Padding = 0;
            mainGrid.Margin = 0;
            mainGrid.RowSpacing = 0;
            mainGrid.ColumnSpacing = 0;

            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Absolute) });   // iPhone
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(72, GridUnitType.Absolute) });  // Header
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });       // Body
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(72, GridUnitType.Absolute) });  // Footer

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(20, GridUnitType.Absolute);
            };

            //mainGrid.Add(_BackgroundImage, 0, 1, 0, 4);
            mainGrid.Add(_BackgroundImage, 0, 0);
            Grid.SetColumnSpan(_BackgroundImage, 2);
            Grid.SetRowSpan(_BackgroundImage, 3);

            // - - -  - - - 

            AddHeader();
            Content = mainGrid;

            // - - -  - - - 

            this.Appearing += PageEx_Appearing;
            this.Disappearing += PageEx_Disappearing;
            this.PropertyChanged += Page_Base_PropertyChanged;
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        private void PageEx_Appearing(object sender, EventArgs e)
        {
            this.SetBinding(Page.IsBusyProperty, new Binding("IsBusy", BindingMode.OneWay, source: BackboneViewModel.Current));
        }

        private void PageEx_Disappearing(object sender, EventArgs e)
        {
            this.RemoveBinding(Page.IsBusyProperty);
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        void AddHeader()
        {
            labelTitle = new Label
            {
                //FontSize = Device.OnPlatform(48, 36, 44),
                FontSize = 48,
                Margin = new Thickness(10, 5, 0, 0),
            };

            SetHeaderContent(labelTitle);
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        private void Page_Base_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
            {
                labelTitle.Text = this.Title;

                // - - -  - - - 

                Double sWidth = -1;

                //sWidth = DeviceDisplay.MainDisplayInfo.Width;
                //Double sHeight = DeviceDisplay.MainDisplayInfo.Height;
                //Double scale = DeviceDisplay.MainDisplayInfo.Scale;

                sWidth = DeviceDisplay.MainDisplayInfo.Width;
                Double sHeight = DeviceDisplay.Current.MainDisplayInfo.Height;
                Double scale = 1;

                if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    sWidth = sWidth / scale;
                    sHeight = sHeight / scale;
                };

                // - - -  - - - 

                //var size = Basics.Current.TextHelper.MeasureTextSize(labelTitle.Text, sWidth - labelTitle.Margin.Left - labelTitle.Margin.Right - 4, labelTitle.FontSize);

                //while (size.Height > 72)
                //{
                //    labelTitle.FontSize = labelTitle.FontSize * 0.9;
                //    size = ZPF.XF.Basics.Current.TextHelper.MeasureTextSize(labelTitle.Text, sWidth - labelTitle.Margin.Left - labelTitle.Margin.Right - 4, labelTitle.FontSize);
                //};
            };

            if (e.PropertyName == "IsBusy")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (_Interlude == null)
                    {
                        _Interlude = Interlude();
                    };

                    if (IsBusy)
                    {
                        //mainGrid.Add(_Interlude, 0, 1, 0, mainGrid.RowDefinitions.Count);
                        mainGrid.Add(_Interlude, 0, 0);
                        Grid.SetColumnSpan(_BackgroundImage, 1);
                        Grid.SetRowSpan(_BackgroundImage, mainGrid.RowDefinitions.Count);

                        _Interlude.IsVisible = true;
                    }
                    else
                    {
                        _Interlude.IsVisible = false;

                        if (mainGrid != null && mainGrid.Children != null)
                        {
                            if (_Interlude != null)
                            {
                                try
                                {
                                    if (mainGrid.Children.IndexOf(_Interlude) != -1)
                                    {
                                        mainGrid.Children.Remove(_Interlude);
                                    };
                                }
                                catch (Exception ex)
                                {
                                    var a = new AuditTrail(ex, AuditTrail.TextFormat.Txt);

                                    //ToDo: AnalyticsHelper.URL = "http://WinRT.ZPF.fr/Appli.aspx";
                                    //ToDo: AnalyticsHelper.Send("Exception", "PageEx.Page_Base_PropertyChanged" + Environment.NewLine + a.DataOut);
                                };
                            };
                        };
                    };
                });
            };
        }

        AbsoluteLayout _Interlude = null;

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public void SetTitleMargin(Thickness Margin)
        {
            if (Margin == null) return;

            labelTitle.Margin = Margin;
        }

        public void SetTitleColor(Microsoft.Maui.Graphics.Color TextColor)
        {
            if (TextColor == null) return;

            labelTitle.TextColor = TextColor;
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public void SetHeaderContent(View view, double Height = -1)
        {
            mainGrid.Add(view, 0, 1);

            if (Height != -1)
            {
                mainGrid.RowDefinitions[1].Height = new GridLength(Height, GridUnitType.Absolute);
            };
        }

        public View HeaderContent
        {
            set
            {
                mainGrid.Add(value, 0, 1);
            }
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public void SetMainContent(View view)
        {
            mainGrid.Add(view, 0, 2);
        }

        public View MainContent
        {
            set
            {
                mainGrid.Add(value, 0, 2);
            }
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public void SetAppBarContent()
        {
            SetAppBarContent(Microsoft.Maui.Graphics.Colors.Transparent, null);
        }

        public void SetAppBarContent(View view)
        {
            SetAppBarContent(Microsoft.Maui.Graphics.Colors.Transparent, view);
        }

        public void SetAppBarContent(Microsoft.Maui.Graphics.Color backgroundColor, View view, GridLength? Height = null)
        {
            if (view == null)
            {
                mainGrid.RowDefinitions[mainGrid.RowDefinitions.Count - 1].Height = new GridLength(0, GridUnitType.Absolute);
            }
            else
            {
                if (backgroundColor != Microsoft.Maui.Graphics.Colors.Transparent)
                {
                    var b = new BoxView()
                    {
                        BackgroundColor = backgroundColor,
                    };

                    mainGrid.Add(b, 0, mainGrid.RowDefinitions.Count - 1);
                };

                mainGrid.Add(view, 0, mainGrid.RowDefinitions.Count - 1);
            };

            if (Height != null)
            {
                mainGrid.RowDefinitions[mainGrid.RowDefinitions.Count - 1].Height = (GridLength)Height;
            };
        }

        public View AppBarContent
        {
            set
            {
                mainGrid.Add(value, 0, mainGrid.RowDefinitions.Count - 1);
            }
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public Tile[] SetAppBarContent(List<AppBarItem> items)
        {
            return SetAppBarContent(items, null);
        }

        public Tile[] SetAppBarContent(List<AppBarItem> items, GridLength? Height = null)
        {
            if (Height != null)
            {
                mainGrid.RowDefinitions[mainGrid.RowDefinitions.Count - 1].Height = (GridLength)Height;
            };

            List<Tile> tiles = new List<Tile>();

            var g = new Grid();
            g.Margin = new Thickness(10, 8, 15, 10);

            foreach (var i in items)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Tile tile = new Tile();
                tile.IconChar = i.IconChar;
                tile.Text = i.Text;
                tile.FontSize = 22;

                g.Add(tile, g.Children.Count, 0);

                tiles.Add(tile);
            }

            SetAppBarContent(g);

            return tiles.ToArray();
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public static ImageSource SpinnerImageSource = null;

        private AbsoluteLayout Interlude()
        {
            var labelHistory = new Label()
            {
                Text = "BusyHistory",
                FontSize = 12,
                TextColor = Microsoft.Maui.Graphics.Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            labelHistory.BindingContext = BackboneViewModel.Current;
            labelHistory.SetBinding(Label.TextProperty, "BusyHistory");

            var labelTitle = new Label()
            {
                Text = "working ...",
                FontSize = 40,
                TextColor = Microsoft.Maui.Graphics.Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            labelTitle.BindingContext = BackboneViewModel.Current;
            labelTitle.SetBinding(Label.TextProperty, "BusyTitle");

            var labelSubTitle = new Label()
            {
                Text = "BusySubTitle",
                FontSize = 24,
                TextColor = Microsoft.Maui.Graphics.Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            labelSubTitle.BindingContext = BackboneViewModel.Current;
            labelSubTitle.SetBinding(Label.TextProperty, "BusySubTitle");


            AbsoluteLayout layout = new AbsoluteLayout();
            layout.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#A000");
            layout.Padding = 0;

            try
            {
                var image = new Image();
                //image.BackgroundColor = Microsoft.Maui.Graphics.Color.AliceBlue;

                if (SpinnerImageSource == null)
                {
                    var assembly = this.GetType().GetTypeInfo().Assembly;

                    //SpinnerImageSource = ImageSource.FromResource( "ZPF_XF_Compos.Images.Loading - 01.Black.png", typeof(PageEx));
                    SpinnerImageSource = ImageSource.FromResource("ZPF_XF_Compos.Images.Loading - 01.png", typeof(PageEx));

                    image.Source = SpinnerImageSource;
                };

                if (PageEx.SpinnerImageSource != null)
                {
                    //???
                    //ToDo: Could crash on UWP on XF 2.3.4.231 (Expertises.SitePage: second display)
                    //ToDo: Could crash on Android on XF 2.3.5.233-pre1 (Expertises.SitePage: second display)

                    image.WidthRequest = 128;
                    image.HeightRequest = 128;

                    image.Source = PageEx.SpinnerImageSource;

                    if (DeviceInfo.Platform == DevicePlatform.macOS)
                    {
                        image.AnchorY = 1;
                        image.AnchorX = 1;
                    };
                };

                AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(image, new Rect(0.5, 0.45, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                layout.Children.Add(image);

                var _animation = new Animation(val => { image.Rotation = val; }, 0, 360, Easing.Linear);
                _animation.Commit(this, "Rotation", length: 2000, repeat: () => true);
            }
            catch
            {
            };

            AbsoluteLayout.SetLayoutFlags(labelHistory, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(labelHistory, new Rect(0.5, 0.1, AbsoluteLayout.AutoSize, 0.3));
            layout.Children.Add(labelHistory);

            AbsoluteLayout.SetLayoutFlags(labelTitle, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(labelTitle, new Rect(0.5, 0.7, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            layout.Children.Add(labelTitle);

            AbsoluteLayout.SetLayoutFlags(labelSubTitle, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(labelSubTitle, new Rect(0.5, 0.85, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            layout.Children.Add(labelSubTitle);

            return layout;
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
    }
}

