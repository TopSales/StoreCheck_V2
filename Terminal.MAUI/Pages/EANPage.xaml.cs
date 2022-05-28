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
      entryInput.Unfocused -= Entry_Unfocused;
      entryInput.Unfocus();

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

         EANViewModel.Current.CurrentArticleEAN = null;
         EANViewModel.Current.SetArticlesEAN();
      };

      UnitechViewModel.Current.OnScann += OnScann;

      entryInput.Unfocused -= Entry_Unfocused;
      entryInput.Unfocused += Entry_Unfocused;

      ZPF.DoIt.Delay(1000, () =>
      {
         ZPF.DoIt.OnMainThread(() =>
         {
            entryInput.Focus();
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
      entryInput.Text = entryInput.Text.Trim();
      entryOutPut.Text = entryInput.Text;

      OnScann(entryInput.Text, entryInput.Text.Length, UnitechViewModel.Symbologies.Unknown, Encoding.ASCII.GetBytes(entryInput.Text));

      entryInput.Text = "";
   }

   private void Entry_Focused(object sender, FocusEventArgs e)
   {
      entryInput.CursorPosition = 0;
      entryInput.SelectionLength = (entryInput.Text == null ? 0 : entryInput.Text.Length);
   }

   private void entry_TextChanged(object sender, TextChangedEventArgs e)
   {
      if (entryInput.Text != "")
      {
         entryOutPut.Text = "";
      };

      entryInput.Unfocused -= Entry_Unfocused;
      UnitechViewModel.Current.Length = entryInput.Text.Length;
   }

   private void Entry_Unfocused(object sender, FocusEventArgs e)
   {
      DoIt.Delay(200, () =>
      {
         DoIt.OnMainThread(() =>
         {
            //ToDo: if (!headerContent.btnBack.IsFocused)
            {
               entryInput.CursorPosition = 0;
               entryInput.SelectionLength = (entryInput.Text == null ? 0 : entryInput.Text.Length);

               entryInput.Focus();
            };
         });
      });
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public bool OnScann(string data, int length, UnitechViewModel.Symbologies symbology, byte[] rawData)
   {
      if (DeviceInfo.Platform == DevicePlatform.Android)
      {
         Vibration.Vibrate(100);
      };

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

      Entry_Focused(entryInput, null);

      return true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}
