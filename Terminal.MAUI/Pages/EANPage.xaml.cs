using System.Text;
using System.Xml.Linq;
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

         if (EANViewModel.Current.ArticlesEAN == null || EANViewModel.Current.ArticlesEAN.Count == 0)
         {
            EANViewModel.Current.CurrentArticleEAN = null;

            EANViewModel.Current.SetArticlesEAN();
         };
      };

      UnitechViewModel.Current.OnScann += OnScann;

      entry.Unfocused -= Entry_Unfocused;
      entry.Unfocused += Entry_Unfocused;

      ZPF.DoIt.Delay(1000, () =>
      {
         ZPF.DoIt.OnMainThread(() =>
         {
            entry.BackgroundColor = Microsoft.Maui.Graphics.Colors.White;
            entry.Focus();
         });
      });
   }

   protected override void OnDisappearing()
   {
      UnitechViewModel.Current.OnScann -= OnScann;

      base.OnDisappearing();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   private void Entry_Completed(object sender, EventArgs e)
   {
      entry.Text = entry.Text.Trim();

      OnScann(entry.Text, entry.Text.Length, UnitechViewModel.Symbologies.Unknown, Encoding.ASCII.GetBytes(entry.Text));
   }

   private void Entry_Focused(object sender, FocusEventArgs e)
   {
      entry.CursorPosition = 0;
      entry.SelectionLength = (entry.Text == null ? 0 : entry.Text.Length);
   }

   private void entry_TextChanged(object sender, TextChangedEventArgs e)
   {
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
               entry.CursorPosition = 0;
               entry.SelectionLength = (entry.Text == null ? 0 : entry.Text.Length);

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

         EANViewModel.Current.CurrentArticleEAN = EANViewModel.Current.GetRecord(EANViewModel.Current.TxtFileName, data);

         //if (DeviceInfo.Platform == DevicePlatform.Android)
         //{
         //   EANViewModel.Current.CurrentArticleEAN = DB_SQL.QueryFirst<EAN_Article>( EANViewModel.Current.DBSQLViewModel, $"select * from EAN_Article where EAN = '{data}'");
         //}
         //else
         //{
         //   EANViewModel.Current.CurrentArticleEAN = EANViewModel.Current.ArticlesEAN.Where(x => x.EAN == data).FirstOrDefault();
         //};

         if (EANViewModel.Current.CurrentArticleEAN == null || string.IsNullOrEmpty(EANViewModel.Current.CurrentArticleEAN.Label_FR))
         {
            EANViewModel.Current.CurrentArticleEAN = new EAN_Article
            {
               Label_FR = "*** EAN not found ***",
               Label_EN = "*** EAN not found ***",
            };
         };

         slArticleEAN.BindingContext = EANViewModel.Current.CurrentArticleEAN;
      };

      Entry_Focused(entry, null);

      return true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}
