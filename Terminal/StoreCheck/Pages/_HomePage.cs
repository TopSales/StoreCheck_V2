using System;
using System.Linq;
using StoreCheck.Pages;
using Xamarin.Forms;
using ZPF;
using ZPF.AT;
using ZPF.XF.Compos;

class _HomePage : Page_Base
{
   Tile tileLogin = null;
   Tile tileSites = null;

   public _HomePage()
   {
      Title = "StoreCheck";

      BackboneViewModel.Current.InitMsgCallBack(MsgCallBack);

      // - - -  - - - 

      var tm = new TileMenu(this);
      tm.Margin = new Thickness(0, 50, 0, 0);
      tm.TopMargin = 72;
      tm.TileBackgroundColor = ColorViewModel.Current.ActionBackgroundColor;
      tm.OnClicked += x_Clicked;
      tm.FontSize = 24;

      // - - -  - - - 

      {
         var l = tm.NewLine();
         l.Height = 120;
         tileLogin = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Entry_01_WF), T("entry"));
      };

      // - - -  - - - 

      {
         var l = tm.NewLine();
         l.Height = 120;
         tileSites = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Exit_WF), T("exit"));
      };

      // - - -  - - - 

      {
         var l = tm.NewLine();
         l.Height = 120;
         l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Messages_Information_01), T("contact"));
         l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Barcode_01), T("info EAN"));
      };

      // - - -  - - - 

      //{
      //   var l = tm.NewLine();
      //   l.Height = 120;
      //   l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Barcode_01), T("info EAN"));
      //};

      // - - -  - - - 

#if DEBUG
      {
         var l = tm.NewLine();
         l.Height = 30;
         l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Beaker), "test");
      };
#endif

      // - - -  - - - 

      SetMainContent(tm);
      SetAppBarContent();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   bool MsgCallBack(BackboneViewModel.MessageBoxType type, string Text, string Caption = "")
   {
      bool Result = true;

      DoIt.OnMainThread(() =>
      {
         switch (type)
         {
            case BackboneViewModel.MessageBoxType.Info:
               // MessageBox.Show(this, Text, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
               DisplayAlert(Caption, Text, "ok");
               break;

            case BackboneViewModel.MessageBoxType.Warning:
               // MessageBox.Show(this, Text, Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
               DisplayAlert((string.IsNullOrEmpty(Caption) ? "Warning" : Caption), Text, "ok");
               break;

            case BackboneViewModel.MessageBoxType.Error:
               // MessageBox.Show(this, Text, Caption, MessageBoxButton.OK, MessageBoxImage.Error);
               DisplayAlert((string.IsNullOrEmpty(Caption) ? "Error" : Caption), Text, "ok");
               break;

            case BackboneViewModel.MessageBoxType.Confirmation:
               // Result = (MessageBox.Show(this, Text, Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK);
               var t = DisplayAlert(Caption, Text, "ok", "cancel");
               t.Wait();
               Result = t.Result;
               break;
         };
      });

      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   static bool IsFirstInit = true;

   protected override void OnAppearing()
   {
      base.OnAppearing();

      if (IsFirstInit)
      {
         IsFirstInit = false;

#pragma warning disable 4014
         System.Threading.Tasks.Task.Run(() =>
         {
            AuditTrailViewModel.Current.Clean();
         });

         // - - -  - - -

         BackboneViewModel.Current.IncBusy();
         BackboneViewModel.Current.BusySubTitle = $"Load local data ...";

         DoIt.OnBackground(() =>
        {
           DoIt.Delay(1000, async () =>
           {
              try
              {
                 MainViewModel.Current.LoadLocalDB();
              }
              catch { };

              if (MainViewModel.Current.Stores.Count == 0)
              {
                 MainViewModel.Current.Config.LastSynchro = DateTime.MinValue;
              };

               //ToDo: SyncData
               //if (!string.IsNullOrEmpty(MainViewModel.Current.Config.Login) && (DateTime.Now - MainViewModel.Current.Config.LastSynchro).TotalHours > 12)
               //{
               //   DoIt.OnMainThread(() =>
               //   {
               //      BackboneViewModel.Current.BusySubTitle = $"Sync data ...";
               //   });
               //   await SyncViewModel.Current.SyncDataWithWeb(MainViewModel.Current.Config.Login);
               //};

               DoIt.OnMainThread(() =>
              {
                 BackboneViewModel.Current.DecBusy();
              });
           });
        });
      }
      else
      {
         MainViewModel.Current.Config.LastPage = this.Title;
         MainViewModel.Current.SaveLocalConfig();
      };

      RefreshTiles();
   }


   private void RefreshTiles()
   {
      //if (!MainViewModel.Current.Config.IsLogged)
      //{
      //   if (tileSites != null)
      //   {
      //      tileLogin.Text = T("login");
      //      tileLogin.IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Login_01);

      //      tileSites.IsEnabled = false;
      //   };
      //}
      //else
      //{
      //   if (tileSites != null)
      //   {
      //      tileLogin.Text = T("logout");
      //      tileLogin.IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Login2_WF);

      //      tileSites.IsEnabled = true;
      //   };
      //};
   }

   async void x_Clicked(object sender, EventArgs e)
   {
      var t = (sender as Tile);
      t.IsEnabled = false;

      string Tag = (t.CommandParameter as string);

      Log.Write("MENU", Tag);

      switch (Tag)
      {
         case "entry":
            {
               await Navigation.PushAsync(new EntryPage());
            };
            break;

         case "exit":
            //if (!MainViewModel.Current.Config.IsLogged)
            //{
            //   Navigation.PushModalAsync(new LoginPage());
            //}
            //else
            //{
            //   Navigation.PushAsync(new StoreListPage());
            //};
            break;

         case "scanner":
            Navigation.PushAsync(new ScanPage());
            break;

         case "info EAN":
            Navigation.PushAsync(new InfoEANPage());
            break;

         case "contact":
            Navigation.PushAsync(new TopSalesPage());
            break;

         case "test":
            {
               MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();

               //MainViewModel.Current.SelectedInterventionParams.Data.Scanns.LastOrDefault();

               await Navigation.PushAsync(new ShelfInventoryPage());

               //var list = await wsHelper.wGet<Intervention_CE[]>($@"/Interventions/ListInt/01.01.0001 00:00/114");


               //MediaFile _file;
               //var source = await Application.Current.MainPage.DisplayActionSheet(
               //    "Where do you want to get the picture?",
               //    "Cancel",
               //    null,
               //    "From Gallery",
               //    "From Camera");

               //if (source == "Cancel")
               //{
               //   _file = null;
               //   return;
               //}

               //if (source == "From Camera")
               //{
               //   _file = await CrossMedia.Current.TakePhotoAsync(
               //       new StoreCameraMediaOptions
               //       {
               //          Directory = "Sample",
               //          Name = "test.jpg",
               //          //PhotoSize = PhotoSize.Small,
               //       }
               //   );
               //}
               //else
               //{
               //   _file = await CrossMedia.Current.PickPhotoAsync();
               //}

               //if (_file != null)
               //{
               //   image.Source = ImageSource.FromStream(() =>
               //   {
               //      var stream = _file.GetStream();
               //      _file.Dispose();
               //      return stream;
               //   });
               //}

            };
            break;
      };

      t.IsEnabled = true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}
