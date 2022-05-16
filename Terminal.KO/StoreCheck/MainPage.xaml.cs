using System;
using System.ComponentModel;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace StoreCheck
{
   // Learn more about making custom code visible in the Xamarin.Forms previewer
   // by visiting https://aka.ms/xamarinforms-previewer
   [DesignTimeVisible(false)]
   public partial class MainPage : ContentPage
   {
      public MainPage()
      {
         InitializeComponent();
      }

      private async void TakePhoto_Clicked(object sender, EventArgs e)
      {
         MediaFile _file;
         var source = await Application.Current.MainPage.DisplayActionSheet(
             "Where do you want to get the picture?",
             "Cancel",
             null,
             "From Gallery",
             "From Camera");

         if (source == "Cancel")
         {
            _file = null;
            return;
         }

         if (source == "From Camera")
         {
            _file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                   Directory = "Sample",
                   Name = "test.jpg",
                       //PhotoSize = PhotoSize.Small,
                    }
            );
         }
         else
         {
            _file = await CrossMedia.Current.PickPhotoAsync();
         }

         if (_file != null)
         {
            image.Source = ImageSource.FromStream(() =>
            {
               var stream = _file.GetStream();
               _file.Dispose();
               return stream;
            });
         }
      }
   }
}
