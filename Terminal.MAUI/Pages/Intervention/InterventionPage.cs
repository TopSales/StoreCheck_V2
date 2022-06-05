using StoreCheck.Views;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   public partial class InterventionPage : PageEx
   {
      Tile t1, t2, t21, t22, t3, tPhotos, t4, t5, t6 = null;

      public InterventionPage()
      {
         Title = "intervention";

         BindingContext = MainViewModel.Current.SelectedIntervention;

         HeaderContent = new StoreCheck.Views.HeaderContent();

         // - - -  - - - 

         {
            var layout = new StackLayout
            {
            };

            // - - -  - - - 

            layout.Children.Add(new StoreView()
            {
               Margin = new Thickness(0, 0, 0, 10),
            });

            layout.Children.Add(new InterventionView()
            {
               Margin = new Thickness(0, 0, 0, 10),
            });

            //XFHelper.AddLF(layout);

            var tm = new TileMenu(this);
            tm.TopMargin = 72 + 76 + 60;
            tm.TileBackgroundColor = ColorViewModel.Current.ActionBackgroundColor;
            tm.FontSize = 16;

            // - - -  - - - 

            switch (MainViewModel.Current.SelectedInterventionParams.FKActionType)
            {
               //   case (long)FKActionTypes.BeforeAfter:
               //      GetMenu_MShopBeforeAfter(tm);
               //      tm.OnClicked += x_Clicked_MShopBeforeAfter;
               //      UpdateTiles_MShopBeforeAfter();
               //      break;

               //   case (long)FKActionTypes.PhotoAuKM:
               //      GetMenu_MShopPhotoAuKM(tm);
               //      tm.OnClicked += x_Clicked_MShopPhotoAuKM;
               //      UpdateTiles_MShopPhotoAuKM();
               //      break;

               //   case (long)FKActionTypes.Durex:
               //      GetMenu_MShopDurex(tm);
               //      tm.OnClicked += x_Clicked_MShopDurex;
               //      UpdateTiles_Durex();
               //      break;

               //   case (long)FKActionTypes.ReleveLinairaire:
               //      GetMenuReleveLinairaire(tm);
               //      UpdateTiles();
               //      break;

               //   case (long)FKActionTypes.QCM_woMenu:
               //      GetMenu_QCM_woMenu(tm);
               //      tm.OnClicked += x_Clicked_QCM_woMenu;
               //      UpdateTiles_QCM_woMenu();
               //      break;

               default:
                  GetMenuDefault(tm);
                  tm.OnClicked += x_Clicked;
                  UpdateTiles();
                  break;
            };

            // - - -  - - -

            layout.Children.Add(tm);

            // - - -  - - - 

            SetMainContent(layout);
            SetAppBarContent();
         };
      }

      //      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //      protected override void OnAppearing()
      //      {
      //         base.OnAppearing();

      //         DoIt.OnMainThread(() =>
      //         {
      //            switch (MainViewModel.Current.SelectedInterventionParams.FKActionType)
      //            {
      //               case (long)FKActionTypes.BeforeAfter:
      //                  UpdateTiles_MShopBeforeAfter();
      //                  break;

      //               case (long)FKActionTypes.PhotoAuKM:
      //                  UpdateTiles_MShopPhotoAuKM();
      //                  break;

      //               case (long)FKActionTypes.Durex:
      //                  UpdateTiles_Durex();
      //                  break;

      //               case (long)FKActionTypes.ReleveLinairaire:
      //                  UpdateTiles();
      //                  break;

      //               case (long)FKActionTypes.QCM_woMenu:
      //                  UpdateTiles_QCM_woMenu();
      //                  break;

      //               default:
      //                  UpdateTiles();
      //                  break;
      //            };
      //         });
      //      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //      PhotosPage photosPage = new PhotosPage("", "");

      public void UpdateTiles()
      {
         //         switch (MainViewModel.Current.SelectedInterventionParams.FKActionType)
         //         {
         //            case 1008:
         //               UpdateTiles_Durex();
         //               break;

         //            case (long)FKActionTypes.ReleveLinairaire:
         //               UpdateTilesReleveLinairaire();
         //               break;

         //            default:
         //               UpdateTilesDefault();
         //               break;
         //         };

         //         // - - - memory  - - - 

         //         // Collect all generations of memory.
         //         GC.Collect();
         //         var mem = GC.GetTotalMemory(true);

         //         Log.Write(new AuditTrail
         //         {
         //            Level = ErrorLevel.Log,
         //            Tag = "MEM",
         //            Message = $"{mem:N0} bytes currently allocated in managed memory",
         //         });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //      void Report_OnUpdateOutput(object sender, MCEViewModel mce)
      //      {
      //         InterventionsViewModel.Current.SaveMCE(mce.ControlValues.Text);
      //      }

      bool Sema_x_Clicked = false;
      async void x_Clicked(object sender, EventArgs e)
      {
         if (Sema_x_Clicked)
         {
            return;
         };
         Sema_x_Clicked = true;


         //         string Tag = ((sender as Tile).CommandParameter as string);

         //         Log.Write("MENU Intervention", Tag);

         //         switch (Tag)
         //         {
         //            case "arrived":
         //               InterventionStartView.Display(this, mainGrid);
         //               break;

         //            case "report":
         //               {
         //                  var mce = new MCEPage();
         //                  mce.Input = MainViewModel.Current.SelectedIntervention.Input;
         //                  mce.Output = MainViewModel.Current.SelectedIntervention.Output;
         //                  mce.OnUpdateOutput += Report_OnUpdateOutput;

         //                  await Navigation.PushAsync(mce);
         //               };
         //               break;

         //            case "shelf":
         //               await Navigation.PushAsync(new ShelfInventoryPage());
         //               break;

         //            case "SLIM":
         //               await Navigation.PushAsync(new SLIMPage());
         //               break;

         //            case "Missing":
         //               await Navigation.PushAsync(new MissingPage());
         //               break;

         //            case "photos":
         //               photosPage.Title = "";
         //               photosPage.Comment = "";
         //               await Navigation.PushAsync(photosPage);
         //               break;

         //            case "scan":
         //               var p = new ShelfInventoryPage();
         //               p.Title = "scan";
         //               await Navigation.PushAsync(p);
         //               break;

         //            case "photo concurence":
         //               await InterventionsViewModel.Current.Start(null);

         //               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoConcurence);
         //               photosPage.Reset();
         //               photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoConcurence;
         //               photosPage.Comment = MainViewModel.Current.durexParams.PhotoConcurence_DocComment;
         //               photosPage.LoadData();
         //               await Navigation.PushAsync(photosPage);
         //               break;

         //            case "photo before":
         //               await InterventionsViewModel.Current.Start(null);

         //               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoBefore);
         //               photosPage.Reset();
         //               //ToDo: photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoBefore;
         //               //ToDo: photosPage.Comment = MainViewModel.Current.durexParams.PhotoBefore_DocComment;
         //               photosPage.LoadData();
         //               await Navigation.PushAsync(photosPage);
         //               break;

         //            case "photo after":
         //               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoAfter);
         //               photosPage.Reset();
         //               //ToDo: photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoAfter;
         //               //ToDo: photosPage.Comment = MainViewModel.Current.durexParams.PhotoAfter_DocComment;
         //               photosPage.LoadData();
         //               await Navigation.PushAsync(photosPage);
         //               break;

         //            case "validate":
         //               InterventionStopView.Display(this, mainGrid);
         //               break;
         //         };

         //         if (MainViewModel.Current.SelectedIntervention != null)
         //         {
         //            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;
         //         };

         Sema_x_Clicked = false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -    
   }
}
