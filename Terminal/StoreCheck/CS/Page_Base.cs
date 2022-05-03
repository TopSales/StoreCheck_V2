using System;
using System.Diagnostics;
using Xamarin.Forms;
using ZPF.AT;
using ZPF.XF;
using ZPF.XF.Compos;

namespace ZPF
{
   public class Page_Base : PageEx
   {
      public Page_Base()
      {
         BackgroundColor = ColorViewModel.Current.BackgroundColor;

         BackgroundImage.Source = ImageSource.FromResource("StoreCheck.Images.Background_Main.png", typeof(_HomePage));
         BackgroundImage.Aspect = Aspect.Fill;

         SetTitleMargin(new Thickness(10, 14, 100, 0));

         AddHeader();

         Log.Write(ErrorLevel.Log, "ctor " + this.GetType().ToString());
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static bool IsBounceButtonAnimationInProgress;

      void AddHeader()
      {
         SetHeaderContent(GetHeader(this, mainGrid));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static View GetHeader(PageEx parent, Grid mainGrid)
      {
         var headerGrid = new Grid();
         headerGrid.BackgroundColor = Xamarin.Forms.Color.White;

         headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
         headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) });

         var labelVersion = new Label
         {
            FontSize = 10,
            Margin = new Thickness(3, 1, 0, 0),
            Text = VersionInfo.Current.sVersion + " " + VersionInfo.Current.BuildOn,
            HorizontalTextAlignment = TextAlignment.Start,
            HorizontalOptions = LayoutOptions.Start,
         };

         headerGrid.Children.Add(labelVersion, 0, 0);

         var s = new StackLayout
         {
            Orientation = StackOrientation.Horizontal,
            Padding = 0,
         };

         //double MarginX = 10;
         double MarginY = (Device.RuntimePlatform == Device.iOS ? 15 : 0);

         {
            //var b = new Tile()
            //{
            //   Opacity = 1,
            //   BackgroundColor = Xamarin.Forms.Color.Transparent,
            //   Text = "",
            //   Margin = new Thickness(-15, 0 + MarginY, -35, 0),
            //   IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Previous),
            //};

            //b.Clicked += async (object sender, EventArgs e) =>
            //{
            //   (sender as Tile).IsEnabled = false;

            //   if (parent.OnPrevious != null)
            //   {
            //      parent.OnPrevious();
            //   }
            //   else
            //   {
            //      // https://github.com/ZeProgFactory/StoreCheck/issues/91
            //      try
            //      {
            //         // Navigate away
            //         await parent.Navigation.PopAsync();
            //      }
            //      catch (Exception ex)
            //      {
            //         Log.Write(new AuditTrail(ex));
            //      };
            //   };

            //   (sender as Tile).IsEnabled = true;
            //};

            //if (!(parent is _HomePage))
            //{
            //   s.Children.Add(b);
            //};

            //MarginX = -5;
         };

         var labelTitle = new LabelEx
         {
            FontSize = Device.OnPlatform(48, 26, 47),
            Margin = new Thickness(10, 10, 0, 0),
            Text = parent.Title,
         };
         labelTitle.SetBinding(Label.TextProperty, new Binding("Title", BindingMode.OneWay, source: parent));
         s.Children.Add(labelTitle);

         headerGrid.Children.Add(s, 0, 0);

         {
            var i = new Image()
            {
               Margin = new Thickness(0, 5, 10, 0),
               Source = ImageSource.FromResource("StoreCheck.Images.Logo.png", typeof(_HomePage)),
            };

            var l2Tap = new TapGestureRecognizer();
            l2Tap.Tapped += (object sender, EventArgs e) =>
            {
               if (IsBounceButtonAnimationInProgress)
                  return;

               var bounceButton = (Image)sender;
               IsBounceButtonAnimationInProgress = true;

               Device.BeginInvokeOnMainThread(async () =>
               {
                  parent.Unfocus();
                  await AnimationHelper.Bounce(bounceButton);
                  IsBounceButtonAnimationInProgress = false;

                  //if (Debugger.IsAttached && DependencyService.Get<IScanner>().IsOpen())
                  //{
                  //   StoreCheck.Pages.ScanView.Display(parent, mainGrid);
                  //}
                  //else
                  {
                     await parent.Navigation.PushAsync(new AboutPage());
                  }
               });
            };

            i.GestureRecognizers.Add(l2Tap);

            headerGrid.Children.Add(i, 1, 0);
         }

         return headerGrid;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}

