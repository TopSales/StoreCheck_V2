//using SignaturePad.Forms;
//using ZPF.SQL;
using ZPF.XF.Compos;

namespace ZPF
{
   public class SignaturePage : PageEx
   {
      //SignaturePad.Forms.SignaturePadView signaturePadView = null;

      public SignaturePage()
      {
         Title = "Signature";

         // - - -  - - -
         // https://github.com/xamarin/SignaturePad
         // - - -  - - -

         var g = new Grid
         {
         };

         //signaturePadView = new SignaturePad.Forms.SignaturePadView
         //{
         //   BackgroundColor = Xamarin.Forms.Color.FromHex("DFFF"),
         //   CaptionText = "sign here",
         //   ClearText = "clear signature",
         //   PromptText = "X",
         //   PromptFontSize = 24,
         //   PromptTextColor = Xamarin.Forms.Color.FromHex("F666"),
         //   StrokeWidth = 3,
         //};

         //signaturePadView.StrokeCompleted += S_StrokeCompleted;


         //if (Device.RuntimePlatform == Device.UWP)
         //{
         //   signaturePadView.Margin = 5;
         //};

         //if (Device.RuntimePlatform == Device.Android)
         //{
         //   g.Rotation = 90;
         //   signaturePadView.Margin = new Thickness(-50, 80, -50, 80);
         //};

         //g.Children.Add(signaturePadView, 0, 0);

         // - - -  - - - 

         SetMainContent(g);

         // - - -  - - -

         {
            Grid tb = new Grid();
            tb.Margin = new Thickness(10, 5, 10, 10);
            tb.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            tb.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            {
               Tile b = new Tile()
               {
                  Text = "OK",
                  IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01),
                  BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                  TextColor = ColorViewModel.Current.TextColor
               };

               b.Clicked += async (object sender, System.EventArgs e) =>
               {
                  (sender as Tile).IsEnabled = false;

                  //if (signaturePadView.IsBlank)
                  //{
                  //   await DisplayAlertT("Oups ...", "You should sign ...", "ok");
                  //}
                  //else
                  //{
                  //   string FileName = "Signature.png";
                  //   string RefType = "Intervention";
                  //   string ExtRef = MainViewModel.Current.SelectedIntervention.PK.ToString();

                  //   FileName = ZPF.XF.Basics.Current.GetDataDirectory() + @"/" + FileName;
                  //   FileName = ZPF.XF.Basics.Current.FileIO.CleanPath(FileName);

                  //   Stream bitmap = await signaturePadView.GetImageStreamAsync(SignatureImageFormat.Png);

                  //   ZPF.XF.Basics.Current.FileIO.WriteStream(bitmap, FileName);

                  //   if ( MainViewModel.Current.IsInternetAccessAvailable &&
                  //         await MainViewModel.Current.UploadDoc(FileName, Document.InternalDocumentTypes.signature, RefType, ExtRef, "Signature", "", Guid.NewGuid().ToString() ))
                  //   {
                  //      await Navigation.PopAsync();
                  //   }
                  //   else
                  //   {
                  //      await DisplayAlertT("Oups ...", "??? Signature.UploadDoc", "ok");
                  //   };
                  //};

                  (sender as Tile).IsEnabled = true;
               };

               // - - -  - - - 

               tb.Add(b, 0, 0);
            }

            {
               Tile b = new Tile()
               {
                  Text = "cancel",
                  IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Close),
                  BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                  TextColor = ColorViewModel.Current.TextColor
               };

               b.Clicked += async (object sender, System.EventArgs e) =>
               {
                  await Navigation.PopAsync();
               };

               // - - -  - - - 

               tb.Add(b, 1, 0);
            }

            SetAppBarContent(tb);
         }
      }

      private void S_StrokeCompleted(object sender, System.EventArgs e)
      {
         //var strokes = (sender as SignaturePadView).Strokes;
         //Debug.WriteLine(strokes.ToString());
      }
   }
}
