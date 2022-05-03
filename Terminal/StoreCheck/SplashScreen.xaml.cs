using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCheck;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF.XF.Compos;

namespace ZPF.XF
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashScreen : ContentPage
   {
      public SplashScreen()
      {
         InitializeComponent();
         imageLogo.Source = ImageSource.FromResource("StoreCheck.Images.Icon.png");
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();

         DoIt.OnBackground(() => 
         {
            SetStyles();

            // - - -  - - - 

            // GetDeviceInfo();

            MainViewModel.Current.LoadLocalConfig();

            // - - -  - - - 

            DoIt.OnMainThread(() =>
            {
               //App.Current.MainPage = new NavigationPage(new MainPage());
               App.Current.MainPage = new NavigationPage(new _HomePage());
            });
         });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void GetDeviceInfo()
      {
         // - - -  - - - 
         // Device Model (SMG-950U, iPhone10,6)
         var device = Xamarin.Essentials.DeviceInfo.Model;

         // Manufacturer (Samsung)
         var manufacturer = DeviceInfo.Manufacturer;

         // Device Name (Motz's iPhone)
         var deviceName = DeviceInfo.Name;

         // Operating System Version Number (7.0)
         var version = DeviceInfo.VersionString;

         // Platform (Android)
         var platform = DeviceInfo.Platform;

         // Idiom (Phone)
         var idiom = DeviceInfo.Idiom;

         // Device Type (Physical)
         var deviceType = DeviceInfo.DeviceType;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SetStyles()
      {
         GridDlgOnTop.OuterBackgroundColor = "60FFFFFF"; // * 
         GridDlgOnTop.LightInnerBackgroundColor = "D0FFFFFF";
         GridDlgOnTop.DarkInnerBackgroundColor = "E0FFFFFF"; // *

         Tile.DisabledTextAlpha = 0x33;
         Tile.DisabledIconAlpha = 0x33;
         Tile.DisabledBackgroundAlpha = 0x88;

         ColorViewModel.Current.SetLight();
         ColorViewModel.Current.HighLightColor = Xamarin.Forms.Color.FromHex("#FF8eb4e3");
         ColorViewModel.Current.ActionBackgroundColor = ColorViewModel.Current.HighLightColor;

         App.Current.Resources = ColorViewModel.Current.SetStyles();
         App.Current.Resources.Add("kbdButton", new Style(typeof(KbdButtonSkin))
         {
            Setters = {
                       new Setter {
                           Property = KbdButtonSkin.FontFamilyProperty,
                           Value ="Montserrat",
                       },
                       new Setter {
                           Property = KbdButtonSkin.CornerRadiusProperty,
                           Value = 4,
                       },
                       new Setter {
                           Property = KbdButtonSkin.FontAttributesProperty,
                           Value =  FontAttributes.Bold,
                       },
                       new Setter {
                           Property = KbdButtonSkin.FontSizeProperty,
                           Value = 26,
                       },
                       new Setter {
                           Property = KbdButtonSkin.BackgroundColorProperty,
                           Value = Xamarin.Forms.Color.FromHex( "FFDADADA" ),
                       },
                   },
         });


         // Type = 0
         App.Current.Resources.Add("kbdButtonStd", new Style(typeof(KbdButtonSkin))
         {
            BaseResourceKey = "kbdButton",
            Setters = {
                       new Setter {
                           Property = KbdButtonSkin.BackgroundColorProperty,
                           //Value = Xamarin.Forms.Color.FromHex( "FFDADADA" ),
                           Value = Xamarin.Forms.Color.Black,
                       },
                   },
         });

         // Type = 1
         App.Current.Resources.Add("kbdButtonCmd", new Style(typeof(KbdButtonSkin))
         {
            BaseResourceKey = "kbdButton",
            Setters = {
                       new Setter {
                           Property = KbdButtonSkin.BackgroundColorProperty,
                           Value =  Xamarin.Forms.Color.FromHex( "FFA09F9F" ),
                       },
               },
         });

         // Type = 2
         App.Current.Resources.Add("kbdButtonResult", new Style(typeof(KbdButtonSkin))
         {
            BaseResourceKey = "kbdButton",
            Setters = {
                       new Setter {
                           Property = KbdButtonSkin.BackgroundColorProperty,
                           Value = Xamarin.Forms.Color.Orange.MultiplyAlpha(.7),
                       },
               },
         });

         // Type = 3
         App.Current.Resources.Add("kbdButtonMEM", new Style(typeof(KbdButtonSkin))
         {
            BaseResourceKey = "kbdButton",
            Setters = {
                       new Setter {
                           Property = KbdButtonSkin.BackgroundColorProperty,
                           Value = Xamarin.Forms.Color.Green.MultiplyAlpha(.7),
                       },
               },
         });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
