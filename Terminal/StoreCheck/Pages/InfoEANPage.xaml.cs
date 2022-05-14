using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF;
using ZPF.XF.Compos;
using static UnitechViewModel;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class InfoEANPage : Page_Base
   {
      public InfoEANPage()
      {
         BindingContext = UnitechViewModel.Current;
         Title = "info EAN";

         InitializeComponent();

         slArticleEAN.BindingContext = EANViewModel.Current.CurrentArticleEAN;



         //DependencyService.Get<IScanner>().OpenScanner();
         //DependencyService.Get<IScanner>().EnableAllSymbologies();

         DoIt.Delay(100, () =>
         {
            DoIt.OnMainThread(() =>
            {
               //#if SCAN_WEDGE
               entry.Unfocused += Entry_Unfocused;
               //#endif
               entry.Focus();
            });
         });
      }

      bool OnAppearing_sema = true;

      protected override void OnAppearing()
      {
         base.OnAppearing();

         if (OnAppearing_sema)
         {
            OnAppearing_sema = false;

            //if (MainViewModel.Current.ArticlesEAN.Count == 0)
            //{
            //   SyncEAN();
            //};

            if (EANViewModel.Current.ArticlesEAN == null || EANViewModel.Current.ArticlesEAN.Count == 0)
            {
               EANViewModel.Current.CurrentArticleEAN = null;

               EANViewModel.Current.SetArticlesEAN();
            };

            OnAppearing_sema = true;
         };

         //DependencyService.Get<IScanner>().OpenScanner();
         //DependencyService.Get<IScanner>().EnableAllSymbologies();
         UnitechViewModel.Current.OnScann += OnScann;


         DoIt.Delay(100, () =>
         {
            DoIt.OnMainThread(() =>
            {
               //#if SCAN_WEDGE
               entry.Unfocused += Entry_Unfocused;
               //#endif
               entry.Focus();
            });
         });
      }

      private async void SyncEAN()
      {
         var dt = DateTime.Now;

         return;

         //ToDo: BackboneViewModel.Current.BusySubTitle = "Sync EAN ...";
         BackboneViewModel.Current.IncBusy();

         var ci = await SyncViewModel.Current.InitSyncArticlesEAN();

         if (ci != null)
         {
            for (double i = 0; i < ci.TotalChunks; i++)
            {
               BackboneViewModel.Current.BusySubTitle = $"Sync EAN {(long)(i / ci.TotalChunks * 100.0)}% ...";

               if (!await SyncViewModel.Current.SyncArticlesEAN((int)i, (int)ci.RowsPPage))
               {
                  BackboneViewModel.Current.BusySubTitle = $"Sync EAN {(long)(i / ci.TotalChunks * 100.0)}% E ...";
                  await SyncViewModel.Current.SyncArticlesEAN((int)i, (int)ci.RowsPPage);

                  BackboneViewModel.Current.DecBusy();
                  return;
               };
            };

            if (EANViewModel.Current.ArticlesEAN.Count() >= ci.TotalRows)
            {
               BackboneViewModel.Current.BusySubTitle = $"Sync EAN - OK";

               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.all);
            };
         };

         BackboneViewModel.Current.DecBusy();

         // - - -  - - - 

         if (EANViewModel.Current.ArticlesEAN.Count == 0)
         {
            var LastError = T("InfoEANPage: Problem when communicating with the server:") + Environment.NewLine + wsHelper.LastError;
            BackboneViewModel.Current.MessageBox(LastError);
         }
         else
         {
            var ts = DateTime.Now - dt;
            var msg = $"Sync EAN: {ts.ToString()}";
            BackboneViewModel.Current.MessageBox(msg);
         };
      }

      protected override bool OnBackButtonPressed()
      {
         entry.Unfocused -= Entry_Unfocused;
         entry.Unfocus();

         //UnitechViewModel.Current.OnScann -= OnScann;
         //DependencyService.Get<IScanner>().CloseScanner();

         return base.OnBackButtonPressed();
      }

      protected override void OnDisappearing()
      {
         UnitechViewModel.Current.OnScann -= OnScann;
         //DependencyService.Get<IScanner>().CloseScanner();

         base.OnDisappearing();
      }

      public bool OnScann(string data, int length, UnitechViewModel.Symbologies symbology, byte[] rawData)
      {
#if !SCAN_WEDGE
         if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
         {
            Xamarin.Essentials.Vibration.Vibrate(100);
         };
#endif
         {
            slArticleEAN.BindingContext = null;

            EANViewModel.Current.CurrentArticleEAN = EANViewModel.Current.ArticlesEAN.Where(x => x.EAN == data).FirstOrDefault();

            if (EANViewModel.Current.CurrentArticleEAN == null)
            {
               EANViewModel.Current.CurrentArticleEAN = new EAN_Article
               {
                  Label_FR = "*** EAN not found ***",
                  Label_EN = "*** EAN not found ***",
               };
            };

            slArticleEAN.BindingContext = EANViewModel.Current.CurrentArticleEAN;
         };

         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Entry_Completed(object sender, EventArgs e)
      {
         var entry = sender as Entry;
         entry.Text = entry.Text.Trim();

         OnScann(entry.Text, entry.Text.Length, UnitechViewModel.Symbologies.Unknown, Encoding.ASCII.GetBytes(entry.Text));
      }

      private void Entry_Focused(object sender, FocusEventArgs e)
      {
         var entry = sender as Entry;

         entry.CursorPosition = 0;
         entry.SelectionLength = entry.Text.Length;
      }

      //private void entry_Completed(object sender, EventArgs e)
      //{
      //   var entry = sender as Entry;

      //   if (entry != null && entry.Text != null)
      //   {
      //      var st = entry.Text.Trim();

      //      if (!string.IsNullOrEmpty(st))
      //      {
      //         entry.Unfocused -= Entry_Unfocused;
      //         slBarCode.IsVisible = false;
      //         UnitechViewModel.Current.NewBarcode(st, st.Length, 0, null);
      //      };
      //   };
      //}

      private void entry_TextChanged(object sender, TextChangedEventArgs e)
      {
         var entry = sender as Entry;
         entry.Unfocused -= Entry_Unfocused;

         UnitechViewModel.Current.Length = entry.Text.Length;
      }

      private void Entry_Unfocused(object sender, FocusEventArgs e)
      {
         DoIt.Delay(200, () =>
         {
            DoIt.OnMainThread(() =>
            {
               if (!btnExit.IsFocused)
               {
                  entry.Focus();
               };
            });
         });
      }

      private async void btnExit_Clicked(object sender, EventArgs e)
      {
         BackboneViewModel.Current.IncBusy();

         OnBackButtonPressed();

         await Navigation.PopAsync();

         BackboneViewModel.Current.DecBusy();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
