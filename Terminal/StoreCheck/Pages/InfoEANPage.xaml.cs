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
         slHeadder.BindingContext = MainViewModel.Current;

         // - - -  - - - 

         var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
         {
            //new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Clean_Brush), "clean"),
            //new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Sync), "sync"),
         }));

         //tiles[0].Clicked += async (object sender, System.EventArgs e) =>
         //{
         //   (sender as Tile).IsEnabled = false;

         //   if (await DisplayAlert("Confirmation", "Delete the EAN database?", "OK", "annuler") == true)
         //   {
         //      EANViewModel.Current.ArticlesEAN.Clear();
         //      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.all);
         //   };

         //   (sender as Tile).IsEnabled = true;
         //};

         //tiles[1].Clicked += async (object sender, System.EventArgs e) =>
         //{
         //   (sender as Tile).IsEnabled = false;

         //   if (await DisplayAlert("Confirmation", "Sync the EAN database?", "OK", "annuler") == true)
         //   {
         //      SyncEAN();
         //   };

         //   (sender as Tile).IsEnabled = true;
         //};

         // - - -  - - - 

         //DependencyService.Get<IScanner>().OpenScanner();
         //DependencyService.Get<IScanner>().EnableAllSymbologies();
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
            };

            EANViewModel.Current.SetArticlesEAN();

            OnAppearing_sema = true;
         };

         //DependencyService.Get<IScanner>().OpenScanner();
         //DependencyService.Get<IScanner>().EnableAllSymbologies();
         UnitechViewModel.Current.OnScann += OnScann;

         eData.Focus();
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
         UnitechViewModel.Current.OnScann -= OnScann;
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

            if(EANViewModel.Current.CurrentArticleEAN == null)
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

      private void Entry_Completed(object sender, EventArgs e)
      {
         var entry = sender as Entry;

         OnScann(entry.Text, entry.Text.Length, UnitechViewModel.Symbologies.Unknown, Encoding.ASCII.GetBytes(entry.Text));
      }

      private void Entry_Focused(object sender, FocusEventArgs e)
      {
         var entry = sender as Entry;

         entry.CursorPosition = 0;
         entry.SelectionLength = entry.Text.Length;
      }
   }
}
