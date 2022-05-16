using System;
using Xamarin.Forms;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   class TopSalesPage : Page_Base
   {
      public TopSalesPage()
      {
         Title = "TopSales";

         // - - -  - - - 

         {
            var layout = new StackLayout
            {
               Margin = new Thickness(16, 0, 16, 16),
               Padding = new Thickness(8, 0, 8, 0),
            };

            var i = new Image()
            {
               Margin = new Thickness(0, 120, 10, 10),
               Source = ImageSource.FromResource("StoreCheck.Images.Logo.png", typeof(_HomePage)),
            };

            layout.Children.Add(i);

            {
               var l = new Label
               {
                  Text = "Avenue Marcel Thiry 11/2",
                  FontSize = 18,
                  HorizontalTextAlignment = TextAlignment.Center,
               };
               layout.Children.Add(l);
            };

            {
               var l = new Label
               {
                  Text = "1200 Woluwe-Saint -Lambert",
                  FontSize = 18,
                  HorizontalTextAlignment = TextAlignment.Center,
               };
               layout.Children.Add(l);
            };

            XFHelper.AddLF(layout);
            XFHelper.AddLF(layout);

            {
               var l = new Label
               {
                  Text = "Tel.: +32 2 247 29 30",
                  FontSize = 18,
                  HorizontalTextAlignment = TextAlignment.Center,
               };
               layout.Children.Add(l);
            };

            {
               var l = new Label
               {
                  Text = "E-mail : Contact@TopSales.be",
                  FontSize = 18,
                  HorizontalTextAlignment = TextAlignment.Center,
               };
               layout.Children.Add(l);
            };

            // - - -  - - - 

            SetMainContent(layout);

            // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

            #region AppBar
            {
               var tb = new Grid();
               tb.Margin = new Thickness(15, 8, 15, 16);
               tb.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
               //tb.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

               //{
               //   var b = new Tile() // ImageButton()
               //   {
               //      Text = "appeler",
               //      IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Contact),
               //      //Source = ImageSource.FromResource("AudEx.Images.AppBar.Telephone -02.png", typeof(_HomePage)),
               //      BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
               //      TextColor = ColorViewModel.Current.TextColor
               //   };
               //   b.Clicked += (sender, e) =>
               //   {
               //      (sender as Tile).IsEnabled = false;

               //      Device.OpenUri(new Uri("tel:+32 2 247 29 30"));

               //      (sender as Tile).IsEnabled = true;
               //   };
               //   tb.Children.Add(b, 0, 0);
               //}

               {
                  var b = new Tile() // ImageButton()
                  {
                     Text = "mail",
                     IconChar= ZPF.Fonts.IF.GetContent( ZPF.Fonts.IF.Mail_03),
                     BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                     TextColor = ColorViewModel.Current.TextColor
                  };
                  b.Clicked += (sender, e) =>
                  {
                     (sender as Tile).IsEnabled = false;

                     Device.OpenUri(new Uri("mailto:Contact@TopSales.be?subject=StoreCheck"));

                     (sender as Tile).IsEnabled = true;
                  };

                  tb.Children.Add(b, 0, 0);
               }

               SetAppBarContent(tb);
            };
            #endregion

            // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
         };
      }
   }
}
