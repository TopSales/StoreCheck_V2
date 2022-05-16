using System;
using System.Net;
using System.Net.Http;
using Xamarin.Forms;
using ZPF.Compos.XF;
using ZPF.XF.Compos;

namespace ZPF
{
   class EntryPwd : Grid
   {
      EntryEx _Entry = new EntryEx();

      public EntryPwd()
      {
         Padding = 0;
         Margin = 0;

         this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
         this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

         this.Children.Add(_Entry, 0, 0);

         var i = new Image
         {
            Source = SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Show_01_WF, 55),
            HeightRequest = 28,
            WidthRequest = 28,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
         };

         void UpdateImage()
         {
            i.Source = (_Entry.IsPassword
               ? SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Show_01_WF, 55)
               : SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Hide1_WF, 55));
         };

         var l2Tap = new TapGestureRecognizer();
         l2Tap.Tapped += (object sender, EventArgs e) =>
         {
            _Entry.IsPassword = !_Entry.IsPassword;
            UpdateImage();

            if (!_Entry.IsPassword)
            {
               _Entry.Focus();
            };
         };

         i.GestureRecognizers.Add(l2Tap);

         this.Children.Add(i, 1, 0);

         _Entry.IsPassword = true;

         _Entry.Completed += (object sender, EventArgs e) =>
         {
            if (Completed != null)
            {
               Completed(sender, e);
            };
         };

         //_Entry.Unfocused += (object sender, FocusEventArgs e) =>
         //{
         //   DoIt.OnMainThread(() => 
         //   {
         //      _Entry.IsPassword = false;
         //      UpdateImage();
         //   });
         //};
      }

      public string Text
      {
         get => _Entry.Text;
         set => _Entry.Text = value;
      }

      //
      // Summary:
      //     Occurs when the user finalizes the text in an entry with the return key.
      //
      // Remarks:
      //     This finalization will usually but not always be accompanied by IsFocused being
      //     set to false.
      public event EventHandler Completed;

   }

   class LoginPage : Page_Base
   {
      EntryEx username;
      EntryPwd password;

      public LoginPage()
      {
         Title = "Login";

         // - - -  - - - 

         {
            var bBackground = new BoxView
            {
               CornerRadius = 20,
               BackgroundColor = ColorViewModel.Current.ActionBackgroundColor.MultiplyAlpha(0.2),
               Margin = new Thickness(15, 5, 15, 10),
            };

            var layout = new StackLayout
            {
               //BackgroundColor = ColorViewModel.Current.ActionBackgroundColor.MultiplyAlpha(0.2),
               Margin = new Thickness(15, 15, 15, 15),
               Padding = 10,
            };

            var g2 = new Grid
            {
               Margin = new Thickness(0, 20, 0, 20),
            };

            var bUser = new BoxView
            {
               WidthRequest = 100,
               HeightRequest = 100,
               CornerRadius = 50,
               BackgroundColor = Xamarin.Forms.Color.Navy,
               VerticalOptions = LayoutOptions.Center,
               HorizontalOptions = LayoutOptions.Center,
            };

            var i = new Image
            {
               HorizontalOptions = LayoutOptions.Center,
               Margin = new Thickness(5, 10, 5, 10),
               Source = SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.User_Profile_1_WF, 60, Xamarin.Forms.Color.White),
            };

            g2.Children.Add(bUser, 0, 0);
            g2.Children.Add(i, 0, 0);
            layout.Children.Add(g2);

            // Show-01-WF
            // Hide1-WF

            {
               layout.Children.Add(new Label { Text = "Username", Margin = new Thickness(5, 10, 5, 0) });
               {
                  username = new EntryEx { Text = "", Margin = new Thickness(5, 0, 5, 0) };
                  layout.Children.Add(username);
               };

               layout.Children.Add(new Label { Text = "Password", Margin = new Thickness(5, 10, 5, 0) });
               {
                  password = new EntryPwd { Text = "", Margin = new Thickness(5, 0, 5, 0) };
                  password.Completed += (object sender, EventArgs e) =>
                  {
                     LoginTile_Clicked(null, null);
                  };

                  layout.Children.Add(password);
               };
            };

            SetMainContent(bBackground);
            SetMainContent(layout);

            username.Focus();

            // - - -  - - - 

            {
               var g = new Grid();
               g.Margin = new Thickness(15, 8, 15, 15);
               g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
               g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

               {
                  Tile loginTile = new Tile();
                  loginTile.IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Login_01);
                  loginTile.Text = "login";
                  loginTile.Clicked += LoginTile_Clicked;

                  g.Children.Add(loginTile, 0, 0);
               };

               {
                  Tile cancelTile = new Tile();
                  cancelTile.IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Close);
                  cancelTile.Text = "cancel";
                  cancelTile.Clicked += (sender, e) =>
                  {
                     Navigation.PopModalAsync();
                  };

                  g.Children.Add(cancelTile, 1, 0);

                  SetAppBarContent(g);
               };
            };

            // - - -  - - - 
         };
      }

      private async void LoginTile_Clicked(object sender, EventArgs e)
      {
         if (String.IsNullOrEmpty(username.Text) || String.IsNullOrEmpty(password.Text))
         {
            await DisplayAlertT("Validation Error", "Username and Password are required", "Re-try");
         }
         else
         {
            if (ZPF.XF.Basics.Current.Network.IsInternetAccessAvailable())
            {
               //using (HttpClient httpClient = new HttpClient())
               {
                  BackboneViewModel.Current.IncBusy();

                  try
                  {
                     //var json = await wsHelper.wGet(string.Format("/User/Login/{0}/{1}", username.Text, UserViewModel.Current.Salt(username.Text, password.Text)));
                     var json = await wsHelper.wPost_String($@"/User/Login/{WebUtility.UrlEncode(username.Text)}/{WebUtility.UrlEncode(username.Text)}", UserViewModel.Current.Salt(username.Text, password.Text));

                     int PK = -1;

                     try
                     {
                        PK = int.Parse(json);
                     }
                     catch { };


                     if (PK > 0)
                     {
                        //DisplayAlert("PK", PK.ToString(), "ok");

                        // - - - ? erase old data - - - 

                        if (MainViewModel.Current.Config.Login != username.Text)
                        {
                           MainViewModel.Current.Config.LastSynchro = DateTime.MinValue;

                           MainViewModel.Current.Interventions.Clear();
                           MainViewModel.Current.Documents.Clear();

                           // - - - clean photo folder - - -

                           var folder = ZPF.XF.Basics.Current.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"/Photos/");

                           if (!System.IO.Directory.Exists(folder) )
                           {
                              System.IO.Directory.CreateDirectory(folder);
                           };

                           var files = System.IO.Directory.GetFiles(folder);

                           foreach (var file in files)
                           {
                              try
                              {
                                 System.IO.File.Delete(file);
                              }
                              catch { };
                           };
                        };

                        // - - - remember login status - - - 
                        MainViewModel.Current.Config.IsLogged = true;
                        MainViewModel.Current.Config.Login = username.Text;
                        MainViewModel.Current.Config.UserFK = PK;
                        MainViewModel.Current.SaveLocalConfig();

                        MainViewModel.Current.Download(username.Text);
                        BackboneViewModel.Current.DecBusy();

                        await Navigation.PopModalAsync();
                     }
                     else
                     {
                        DoIt.OnMainThread(() =>
                       {
                          BackboneViewModel.Current.DecBusy();

                          DisplayAlertT("Validation Error", "Wrong Username and/or Password", "Re-try");
                       });
                     };
                  }
                  catch (Exception ex)
                  {
                     BackboneViewModel.Current.DecBusy();
                     await DisplayAlertT("Oups ...", ex.Message, "ok");
                  };
               };
            }
            else
            {
               await DisplayAlertT("Oups ...", "You need access to the internet ...", "ok");
            };
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
