using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZPF;
using ZPF.AT;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   public partial class InterventionPage : Page_Base
   {

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void GetMenu_MShopDurex(TileMenu tm)
      {
         // Photo Veet
         {
            var l = tm.NewLine();
            l.Height = 40;

            t1 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), $"Photo '{MainViewModel.Current.durexParams.PhotoConcurence}'", CommandParameter: "photo concurence");
         };

         // Photo Durex on arrival
         {
            var l = tm.NewLine();
            l.Height = 40;

            t2 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), $"Photo '{MainViewModel.Current.durexParams.PhotoBefore}'", CommandParameter: "photo before");
         };

         // Scann & QCM
         {
            var l = tm.NewLine();
            l.Height = 40;

            t3 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Inventory2), "Scann & QCM", CommandParameter: "scan");
         };

         // Missing products
         {
            var l = tm.NewLine();
            l.Height = 40;

            t4 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.To_Do_List_WF), "Missing products", CommandParameter: "Missing");
         };

         // Durex photo on departure
         {
            var l = tm.NewLine();
            l.Height = 40;

            t5 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), $"Photo '{MainViewModel.Current.durexParams.PhotoAfter}'", CommandParameter: "photo after");
         };

         // Validate
         {
            var l = tm.NewLine();
            l.Height = 40;

            t6 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "validate", CommandParameter: "validate");
            t6.BackgroundColor = Xamarin.Forms.Color.Green.MultiplyAlpha(.7);
         };
      }

      private void UpdateTiles_Durex()
      {
         var PK = MainViewModel.Current.SelectedIntervention.PK.ToString();

         // Photo Veet
         t1.IsEnabled = MainViewModel.Current.Documents.Where(x => x.Comment == MainViewModel.Current.durexParams.PhotoConcurence_DocComment && x.ExtRef == PK).Count() == 0;

         // Photo Durex on arrival
         t2.IsEnabled = MainViewModel.Current.Documents.Where(x => x.ExtRef == PK).Count() > 0;
         t2.IsEnabled = t2.IsEnabled && MainViewModel.Current.Documents.Where(x => x.Comment == MainViewModel.Current.durexParams.PhotoBefore_DocComment && x.ExtRef == PK).Count() == 0;

         // Scann & QCM
         t3.IsEnabled = MainViewModel.Current.Documents.Where(x => x.ExtRef == PK).Count() > 0;

         // Missing products
         t4.IsEnabled = InterventionsViewModel.Current.ScannCount() > 0;

         // Durex photo on departure
         t5.IsEnabled = InterventionsViewModel.Current.ScannCount() > 0;
         t5.IsEnabled = t5.IsEnabled && MainViewModel.Current.Documents.Where(x => x.Comment == MainViewModel.Current.durexParams.PhotoAfter_DocComment && x.ExtRef == PK).Count() == 0;

         // Validate
         t6.IsEnabled = MainViewModel.Current.Documents.Where(x => x.ExtRef == PK).Count() > 0;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
      async void x_Clicked_MShopDurex(object sender, EventArgs e)
      {
         if (Sema_x_Clicked)
         {
            return;
         };
         Sema_x_Clicked = true;

         string Tag = ((sender as Tile).CommandParameter as string);

         Log.Write("MENU Intervention", Tag);

         switch (Tag)
         {
            case "photo concurence":
               await InterventionsViewModel.Current.Start(null);

               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoConcurence);
               photosPage.Reset();
               photosPage.att.OnUpdateData -= OnUpdateData;
               photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoConcurence;
               photosPage.Comment = MainViewModel.Current.durexParams.PhotoConcurence_DocComment;
               photosPage.LoadData();
               await Navigation.PushAsync(photosPage);
               break;

            case "photo before":
               await InterventionsViewModel.Current.Start(null);

               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoBefore);
               photosPage.Reset();
               photosPage.att.OnUpdateData -= OnUpdateData;
               photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoBefore;
               photosPage.Comment = MainViewModel.Current.durexParams.PhotoBefore_DocComment;
               photosPage.LoadData();
               await Navigation.PushAsync(photosPage);
               break;

            case "scan":
               var p = new ShelfInventoryPage();
               p.Title = "scan";
               await Navigation.PushAsync(p);
               break;

            case "Missing":
               await Navigation.PushAsync(new MissingPage());
               break;

            case "photo after":
               //photosPage = new PhotosPage("Intervention", MainViewModel.Current.durexParams.PhotoAfter);
               photosPage.Reset();
               photosPage.att.OnUpdateData -= OnUpdateData;
               photosPage.SubTitle = MainViewModel.Current.durexParams.PhotoAfter;
               photosPage.Comment = MainViewModel.Current.durexParams.PhotoAfter_DocComment;
               photosPage.LoadData();
               await Navigation.PushAsync(photosPage);
               break;

            case "validate":
               InterventionStopView.Display(this, mainGrid);
               break;
         };

         if (MainViewModel.Current.SelectedIntervention != null)
         {
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;
         };

         Sema_x_Clicked = false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   }
}
