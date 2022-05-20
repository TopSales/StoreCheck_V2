using System.Text;
using ZPF;
using ZPF.SQL;

namespace StoreCheck.Pages;

public partial class EANPage : ContentPage
{
   public EANPage()
   {
      InitializeComponent();

      BindingContext = UnitechViewModel.Current;
      slArticleEAN.BindingContext = EANViewModel.Current.CurrentArticleEAN;
   }

   private async void btnBack_Clicked(object sender, EventArgs e)
   {
      entry.Unfocused -= Entry_Unfocused;
      entry.Unfocus();

      await Navigation.PopModalAsync();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   bool OnAppearing_sema = true;

   protected override void OnAppearing()
   {
      base.OnAppearing();

      if (OnAppearing_sema)
      {
         OnAppearing_sema = false;

         //   //if (MainViewModel.Current.ArticlesEAN.Count == 0)
         //   //{
         //   //   SyncEAN();
         //   //};

         if (EANViewModel.Current.ArticlesEAN == null || EANViewModel.Current.ArticlesEAN.Count == 0)
         {
            EANViewModel.Current.CurrentArticleEAN = null;

            EANViewModel.Current.SetArticlesEAN();
         };

         //   OnAppearing_sema = true;
      };

      ////DependencyService.Get<IScanner>().OpenScanner();
      ////DependencyService.Get<IScanner>().EnableAllSymbologies();

      UnitechViewModel.Current.OnScann += OnScann;

      //DoIt.Delay(100, () =>
      //{
      //   DoIt.OnMainThread(() =>
      //   {
      //#if SCAN_WEDGE
      entry.Unfocused -= Entry_Unfocused;
      entry.Unfocused += Entry_Unfocused;
      //#endif
      entry.Focus();
      //   });
      //});
   }

   protected override void OnDisappearing()
   {
      UnitechViewModel.Current.OnScann -= OnScann;

      base.OnDisappearing();
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
      entry.SelectionLength = (entry.Text == null ? 0 : entry.Text.Length);
   }

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
            if (!btnBack.IsFocused)
            {
               entry.Focus();
            };
         });
      });
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public bool OnScann(string data, int length, UnitechViewModel.Symbologies symbology, byte[] rawData)
   {
#if !SCAN_WEDGE
      if (DeviceInfo.Platform == DevicePlatform.Android)
      {
         Vibration.Vibrate(100);
      };
#endif
      {
         slArticleEAN.BindingContext = null;

         if (DeviceInfo.Platform == DevicePlatform.Android)
         {
            EANViewModel.Current.CurrentArticleEAN = DB_SQL.QueryFirst<EAN_Article>( EANViewModel.Current.DBSQLViewModel, $"select * from EAN_Article where EAN = '{data}'");
         }
         else
         {
            EANViewModel.Current.CurrentArticleEAN = EANViewModel.Current.ArticlesEAN.Where(x => x.EAN == data).FirstOrDefault();
         };

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
}
