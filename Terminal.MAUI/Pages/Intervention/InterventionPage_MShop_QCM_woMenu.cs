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

//      private void GetMenu_QCM_woMenu(TileMenu tm)
//      {
//         // QCM
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t1 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Inventory2), "QCM", CommandParameter: "QCM");
//         };

//         // Validate
//         {
//            var l = tm.NewLine();
//            l.Height = 40;

//            t2 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "validate", CommandParameter: "validate");
//            t2.BackgroundColor = Xamarin.Forms.Color.Green.MultiplyAlpha(.7);
//         };
//      }

//      private void UpdateTiles_QCM_woMenu()
//      {
//         var PK = MainViewModel.Current.SelectedIntervention.PK.ToString();

//         // QCM
//         t1.IsEnabled = true;

//         // Validate
//         t2.IsEnabled = ! string.IsNullOrEmpty( MainViewModel.Current.SelectedIntervention.Output );
//      }

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
//      async void x_Clicked_QCM_woMenu(object sender, EventArgs e)
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

//      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
//   }
//}
