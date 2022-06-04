//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ZPF;
//using ZPF.XF.Compos;

//namespace StoreCheck.Pages
//{
//   public partial class InterventionPage : Page_Base
//   {
//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

//      private void GetMenuReleveLinairaire(TileMenu tm)
//      {
//         {
//            var l = tm.NewLine();
//            l.Height = 50;

//            t1 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Data_Import), "arrived");
//         };

//         {
//            var l = tm.NewLine();
//            l.Height = 50;

//            t21 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Inventory2), "shelf");
//         };

//         {
//            var l = tm.NewLine();
//            l.Height = 50;

//            t22 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Barcode_01), "SLIM");
//         };

//         if (MainViewModel.Current.SelectedInterventionParams.GlobalPictures)
//         {
//            var l = tm.NewLine();
//            l.Height = 50;

//            tPhotos = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), "photos");
//         };

//         {
//            var l = tm.NewLine();
//            l.Height = 50;

//            t4 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "validate");
//            t4.BackgroundColor = Xamarin.Forms.Color.Green.MultiplyAlpha(.7);
//         };
//      }

//      private void UpdateTilesReleveLinairaire()
//      {
//         t1.IsEnabled = MainViewModel.Current.SelectedIntervention.DateBeginIntervention == DateTime.MinValue;
//         t21.IsEnabled = !t1.IsEnabled /*&& MainViewModel.Current.SelectedIntervention.DateEndIntervention == DateTime.MinValue;*/;
//         if (t22 != null)
//         {
//            t22.IsEnabled = !t1.IsEnabled
//               //&& MainViewModel.Current.SelectedIntervention.DateEndIntervention == DateTime.MinValue
//               && MainViewModel.Current.SelectedInterventionParams.Data.Scanns.Count() > 0;
//         };

//         if (tPhotos != null)
//         {
//            tPhotos.BadgeFontSize = 24;
//            tPhotos.Badge = photosPage.Count.ToString();

//            photosPage.LoadData();

//            if (photosPage.Count >= MainViewModel.Current.SelectedInterventionParams.GlobalPicturesMin && photosPage.Count <= MainViewModel.Current.SelectedInterventionParams.GlobalPicturesMax)
//            {
//               tPhotos.BadgeColor = Xamarin.Forms.Color.Green;
//            }
//            else
//            {
//               tPhotos.BadgeColor = Xamarin.Forms.Color.Red;
//            };
//         };

//         if (MainViewModel.Current.SelectedInterventionParams.GlobalPictures) tPhotos.IsEnabled = !t1.IsEnabled /*&& MainViewModel.Current.SelectedIntervention.DateEndIntervention == DateTime.MinValue;*/;

//         t4.IsEnabled = !t1.IsEnabled /*&& MainViewModel.Current.SelectedIntervention.DateEndIntervention == DateTime.MinValue;*/;
//         if (t22 != null)
//         {
//            t4.IsEnabled = t4.IsEnabled && t22.IsEnabled;
//         };
//      }

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
//   }
//}
