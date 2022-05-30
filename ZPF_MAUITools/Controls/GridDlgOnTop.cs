namespace ZPF.XF.Compos
{
    /// <summary>
    /// Displays a DialogBox floating on top of a grid ..
    /// GridDlgOnTop is integrated in PageEx ... 
    /// </summary>
    public class GridDlgOnTop
   {
      public static string OuterBackgroundColor = "60FFFFFF";
      public static string LightInnerBackgroundColor = "60FFFFFF";
      public static string DarkInnerBackgroundColor = "A0FFFFFF";

      public static Thickness CustomPadding = new Thickness(10);

      public enum MarginWidths { narrow, normal, wide, custom }

      // - - -  - - - 

      //View DisplayOnTop_View = null;
      static TStrings stackDisplayOnTop_View = new TStrings();
      public string LastMessage = "";

      public List<Tile> Tiles { get => _Tiles; }
      List<Tile> _Tiles = new List<Tile>();

      // - - -  - - - 

      public bool DisplayOnTop(Grid mainGrid, View view)
      {
         LastMessage = "";

         //if (DisplayOnTop_View != null)
         if (stackDisplayOnTop_View.Count > 0)
         {
            RemoveFromTop(mainGrid);
         };

         var colCount = (mainGrid.ColumnDefinitions.Count < 1 ? 1 : mainGrid.ColumnDefinitions.Count);
         var rowCount = (mainGrid.RowDefinitions.Count < 1 ? 1 : mainGrid.RowDefinitions.Count);

         mainGrid.Children.Add(view, 0, colCount, 0, rowCount);
         //DisplayOnTop_View = view;
         stackDisplayOnTop_View.PushObject("", view);

         return true;
      }

      public static void RemoveFromTop(Grid mainGrid)
      {
         //mainGrid.Children.Remove(DisplayOnTop_View);
         //DisplayOnTop_View = null;

         try
         {
            mainGrid.Children.Remove((View)stackDisplayOnTop_View.PopObject());
         }
         catch
         {
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static Task<string> MDDlgOnTop(Grid mainGrid, string Msg, Double widthOnDesktop = -1)
      {
         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "ok"),
         });

         return MDDlgOnTop(mainGrid, Msg, tiles, widthOnDesktop);
      }

      public static Task<string> MDDlgOnTop(Grid mainGrid, string Msg, List<AppBarItem> ActionTiles, Double widthOnDesktop = -1, MarginWidths marginWidth = MarginWidths.normal)
      {
         var dlg = new GridDlgOnTop();

         TaskCompletionSource<string> Is_MDDlgOnTop_Terminated = new TaskCompletionSource<string>();
         var gb = new Grid
         {
            BackgroundColor =
               (ColorViewModel.Current.ColorMode == ColorViewModel.ColorModes.Light ?
               Xamarin.Forms.Color.FromHex("8000") : Xamarin.Forms.Color.FromHex("A000")),
            Padding = 60,
         };

         mainGrid.SizeChanged += (object sender, EventArgs e) =>
         {
            dlg.AdaptiveLayout(gb, widthOnDesktop, marginWidth);
         };
         dlg.AdaptiveLayout(gb, widthOnDesktop, marginWidth);

         var g = new Grid
         {
            Padding = 5,
         };

         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

         var bv = new BoxView
         {
            BackgroundColor = Xamarin.Forms.Color.FromHex("40FFFFFF"),
            CornerRadius = 20,
         };

         g.Children.Add(bv, 0, 1, 0, 3);

         var bvC = new BoxView
         {
            BackgroundColor = (ColorViewModel.Current.ColorMode == ColorViewModel.ColorModes.Light ?
               Xamarin.Forms.Color.FromHex("A0FFFFFF") : Xamarin.Forms.Color.FromHex("60FFFFFF")),
            Margin = new Thickness(15, 15, 15, 0),
            CornerRadius = 5,
         };

         var sv = new ScrollView()
         {
            Margin = new Thickness(20, 20, 20, 0),
            BackgroundColor = Xamarin.Forms.Color.Transparent,
         };

         //var t = new DarkMarkdownTheme(); // Default is white, you also modify various values
         //t.BackgroundColor = Xamarin.Forms.Color.Transparent;
         //t.Paragraph.ForegroundColor = Xamarin.Forms.Color.Black;
         //t.Paragraph.AsBoxView = false;
         //t.Heading1.ForegroundColor = Xamarin.Forms.Color.Black;
         //t.Heading2.ForegroundColor = Xamarin.Forms.Color.Black;
         //t.Heading3.ForegroundColor = Xamarin.Forms.Color.Black;

         //ToDo: var view = new MarkdownView();
         //view.Markdown = Msg;
         //view.Theme = t; // Default is white, you also modify various values

         var view = new Label()
         {
            TextColor = Xamarin.Forms.Color.Black,
         };
         view.FormattedText = (FormattedString)new Conv.HTML2LabelConverter().Convert(Msg, null, null, null);
         sv.Content = view;

         g.Children.Add(bvC, 0, 1);
         g.Children.Add(sv, 0, 1);

         // - - -  - - - 

         var gTiles = dlg.GenerateTiles(mainGrid, ActionTiles, Is_MDDlgOnTop_Terminated);
         if (gTiles == null)
         {
            g.Children.Add(new Label { HeightRequest = 10 }, 0, 2);
         }
         else
         {
            g.Children.Add(gTiles, 0, 2);
         };

         // - - -  - - - 

         gb.Children.Add(g, 0, 0);

         Device.BeginInvokeOnMainThread(() =>
         {
            dlg.DisplayOnTop(mainGrid, gb);
         });

         // - - -  - - - 

         return Is_MDDlgOnTop_Terminated.Task;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static Task<string> DlgOnTop(Grid mainGrid, View view, Double widthOnDesktop = -1)
      {
         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "ok"),
         });

         return DlgOnTop(mainGrid, view, tiles, widthOnDesktop);
      }

      /// <summary>
      /// <code>
      /// if (await GridDlgOnTop.DlgOnTop( 
      ///         mainGrid, 
      ///         new Label { Text = "Holla ...", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }, 
      ///         GridDlgOnTop.OkCancelTiles()) == "OK")
      /// {
      /// };
      /// </code>
      /// </summary>
      /// <param name="mainGrid"></param>
      /// <param name="view"></param>
      /// <param name="ActionTiles"></param>
      /// <param name="widthOnDesktop"></param>
      /// <returns></returns>
      public static Task<string> DlgOnTop(Grid mainGrid, View view, List<AppBarItem> ActionTiles, Double widthOnDesktop = -1, MarginWidths marginWidth = MarginWidths.normal, Action PostInit = null)
      {
         var dlg = new GridDlgOnTop();

         TaskCompletionSource<string> Is_MDDlgOnTop_Terminated = new TaskCompletionSource<string>();
         _Is_MDDlgOnTop_Terminated = Is_MDDlgOnTop_Terminated;

         var gb = new Grid
         {
            BackgroundColor =
               (ColorViewModel.Current.ColorMode == ColorViewModel.ColorModes.Light ?
               Xamarin.Forms.Color.FromHex("8000") : Xamarin.Forms.Color.FromHex("A000")),
            Padding = 60,
         };

         mainGrid.SizeChanged += (object sender, EventArgs e) =>
         {
            dlg.AdaptiveLayout(gb, widthOnDesktop, marginWidth);
         };
         dlg.AdaptiveLayout(gb, widthOnDesktop, marginWidth);

         var g = new Grid
         {
            Padding = 5,
         };

         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

         var bv = new BoxView
         {
            BackgroundColor = Xamarin.Forms.Color.FromHex(OuterBackgroundColor),
            CornerRadius = 20,
         };

         g.Children.Add(bv, 0, 1, 0, 3);

         var bvC = new BoxView
         {
            BackgroundColor = (ColorViewModel.Current.ColorMode == ColorViewModel.ColorModes.Light ?
               Xamarin.Forms.Color.FromHex(DarkInnerBackgroundColor) : Xamarin.Forms.Color.FromHex(LightInnerBackgroundColor)),
            Margin = new Thickness(15, 15, 15, 0),
            CornerRadius = 5,
         };

         g.Children.Add(bvC, 0, 1);
         g.Children.Add(view, 0, 1);

         // - - -  - - - 

         var gTiles = dlg.GenerateTiles(mainGrid, ActionTiles, Is_MDDlgOnTop_Terminated);
         if (gTiles == null)
         {
            g.Children.Add(new Label { HeightRequest = 10 }, 0, 2);
         }
         else
         {
            g.Children.Add(gTiles, 0, 2);
         };

         // - - -  - - - 

         gb.Children.Add(g, 0, 0);

         Device.BeginInvokeOnMainThread(() =>
         {
            dlg.DisplayOnTop(mainGrid, gb);

            if (PostInit != null)
            {
               PostInit();
            };
         });

         // - - -  - - - 

         return Is_MDDlgOnTop_Terminated.Task;
      }

      public static TaskCompletionSource<string> _Is_MDDlgOnTop_Terminated = null;

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void AdaptiveLayout(Grid gb, double widthOnDesktop, MarginWidths marginWidth = MarginWidths.normal)
      {
         var scale = ZPF.XF.Basics.Current.Display.Scale;

         double hMargin = ZPF.XF.Basics.Current.Display.Width / 10;
         double vMargin = ZPF.XF.Basics.Current.Display.Height / 10;

         if (Device.Idiom == TargetIdiom.Desktop)
         {
            hMargin = ZPF.XF.Basics.Current.Display.Width / 5;
         };

         if (widthOnDesktop > 1)
         {
            if ((widthOnDesktop + 2 * hMargin) < ZPF.XF.Basics.Current.Display.Width)
            {
               hMargin = (ZPF.XF.Basics.Current.Display.Width - widthOnDesktop) / 2;
            }
            else
            {
               hMargin = ZPF.XF.Basics.Current.Display.Width / 10;
            };

            //if (Device.Idiom == TargetIdiom.Desktop)
            //{
            //   double m = (ZPF.XF.Basics.Current.Display.Width - widthOnDesktop) / 2;
            //   gb.Padding = new Thickness(m, 60, m, 60);
            //};
         };

         if (Device.Idiom == TargetIdiom.Phone)
         {
            switch (marginWidth)
            {
               case MarginWidths.narrow:
                  hMargin = ZPF.XF.Basics.Current.Display.Width / 25 / scale;
                  vMargin = ZPF.XF.Basics.Current.Display.Height / 25 / scale;
                  break;

               default:
               case MarginWidths.normal:
                  hMargin = ZPF.XF.Basics.Current.Display.Width / 15 / scale;
                  vMargin = ZPF.XF.Basics.Current.Display.Height / 15 / scale;
                  break;

               case MarginWidths.wide:
                  hMargin = ZPF.XF.Basics.Current.Display.Width / 9 / scale;
                  vMargin = ZPF.XF.Basics.Current.Display.Height / 9 / scale;
                  break;
            };
         };

         if (marginWidth == MarginWidths.custom && CustomPadding != null)
         {
            gb.Padding = CustomPadding;
         }
         else
         {
            gb.Padding = new Thickness(hMargin, vMargin);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      /// Takees the ActionTiles --> new Tile()
      /// </summary>
      /// <param name="mainGrid"></param>
      /// <param name="ActionTiles"></param>
      /// <param name="Is_MDDlgOnTop_Terminated"></param>
      /// <returns></returns>
      private Grid GenerateTiles(Grid mainGrid, List<AppBarItem> ActionTiles, TaskCompletionSource<string> Is_MDDlgOnTop_Terminated)
      {
         var gTiles = new Grid();
         gTiles.Margin = new Thickness(15, 8, 15, 15);

         var w = ZPF.XF.Basics.Current.Display.Width * 0.8;

         if (ActionTiles != null && ActionTiles.Count > 0)
         {
            foreach (var i in ActionTiles)
            {
               gTiles.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

               Tile tile = new Tile();
               tile.Style = ColorViewModel.Current.ActionTilesStyle;
               tile.IconChar = i.IconChar;
               tile.Text = i.Text;
               tile.FontSize = (Device.RuntimePlatform == Device.WPF ? 36 : 22);
               tile.BackgroundColor = ColorViewModel.Current.ActionBackgroundColor;
               tile.CornerRadius = 10;

               tile.Clicked += (object sender, EventArgs e) =>
               {
                  i.DoClick();

                  if (i.IsCancel)
                  {
                     RemoveFromTop(mainGrid);

                     // https://github.com/ZeProgFactory/StoreCheck/issues/107
                     try
                     {
                        Is_MDDlgOnTop_Terminated.SetResult(i.Text);
                     }
                     catch (Exception ex)
                     {
                        ZPF.AT.Log.Write(new AT.AuditTrail(ex) { Tag= "GridDlgOnTop01" });
                     };
                  };
               };

               tile.SizeChanged += (object sender, EventArgs e) =>
               {
                  tile.Text = (tile.Width < 150 ? "" : i.Text);
               };

               _Tiles.Add(tile);
               gTiles.Children.Add(tile, gTiles.ColumnDefinitions.Count - 1, 0);
            }

            return gTiles;
         }
         else
         {
            return null;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static List<AppBarItem> OkCancelTiles()
      {
         return new List<AppBarItem>(new AppBarItem[]
                  {
                     new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "OK"),
                     new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), "cancel"),
                  });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
