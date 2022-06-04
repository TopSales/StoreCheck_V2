//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ZPF;
//using ZPF.AT;
//using ZPF.XF.Compos;

//namespace StoreCheck.Pages
//{
//   public partial class InterventionPage : Page_Base
//   {

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

//      private void GetMenu_MShopPhotoAuKM(TileMenu tm)
//      {
//         // Photo Concurence 
//         if (!string.IsNullOrEmpty(MainViewModel.Current.PhotoAuKM.PhotoConcurence))
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t1 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), $"Photo '{MainViewModel.Current.PhotoAuKM.PhotoConcurence}'", CommandParameter: "photo concurence");
//         };

//         // Photos 
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t2 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), $"Photo '{MainViewModel.Current.PhotoAuKM.Photos}'", CommandParameter: "photos");
//         };

//         // QCM
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t4 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Inventory2), "QCM", CommandParameter: "QCM");
//         };

//         // Validate
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t5 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "validate", CommandParameter: "validate");
//            t5.BackgroundColor = Xamarin.Forms.Color.Green.MultiplyAlpha(.7);
//         };
//      }

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 


//      private void UpdateTiles_MShopPhotoAuKM()
//      {
//         var PK = MainViewModel.Current.SelectedIntervention.PK.ToString();

//         // Photo Concurence
//         if (!string.IsNullOrEmpty(MainViewModel.Current.PhotoAuKM.PhotoConcurence))
//         {
//            t1.IsEnabled = string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Output);
//         };

//         // Photos  
//         t2.IsEnabled = string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Output);

//         // QCM
//         // t4.IsEnabled = MainViewModel.Current.Documents.Where(x => x.Comment == MainViewModel.Current.PhotoAuKM.Photos_DocComment && x.ExtRef == PK).Count() > 0;
//         t4.IsEnabled = MainViewModel.Current.Documents.Where(x => x.ExtRef == PK).Count() > 0;

//         // Validate
//         t5.IsEnabled = MainViewModel.Current.Documents.Where(x => x.ExtRef == PK).Count() > 0;
//      }

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

//      async void x_Clicked_MShopPhotoAuKM(object sender, EventArgs e)
//      {
//         if (Sema_x_Clicked)
//         {
//            return;
//         };
//         Sema_x_Clicked = true;

//         string Tag = ((sender as Tile).CommandParameter as string);

//         Log.Write("MENU Intervention", Tag);

//         switch (Tag)
//         {
//            case "photo concurence":
//               await InterventionsViewModel.Current.Start(null);

//               photosPage.Reset();
//               photosPage.att.OnUpdateData -= OnUpdateData;
//               photosPage.att.OnUpdateData += OnUpdateData;
//               photosPage.SubTitle = MainViewModel.Current.PhotoAuKM.PhotoConcurence;
//               photosPage.Comment = MainViewModel.Current.PhotoAuKM.PhotoConcurence_DocComment;
//               photosPage.LoadData();
//               await Navigation.PushAsync(photosPage);
//               break;

//            case "photos":
//               await InterventionsViewModel.Current.Start(null);

//               photosPage.Reset();
//               photosPage.att.OnUpdateData -= OnUpdateData;
//               photosPage.att.OnUpdateData += OnUpdateData;
//               photosPage.SubTitle = MainViewModel.Current.PhotoAuKM.Photos;
//               photosPage.Comment = MainViewModel.Current.PhotoAuKM.Photos_DocComment;
//               photosPage.LoadData();
//               await Navigation.PushAsync(photosPage);
//               break;

//            case "QCM":
//               {
//                  var mce = new MCEPage();
//                  mce.Input = MainViewModel.Current.SelectedIntervention.Input;
//                  mce.Output = MainViewModel.Current.SelectedIntervention.Output;
//                  mce.OnUpdateOutput += Report_OnUpdateOutput;

//                  await Navigation.PushAsync(mce);
//               };
//               break;

//            case "validate":
//               InterventionStopView.Display(this, mainGrid);
//               break;
//         };

//         if (MainViewModel.Current.SelectedIntervention != null)
//         {
//            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;
//         };

//         Sema_x_Clicked = false;
//      }

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
//   }
//}
