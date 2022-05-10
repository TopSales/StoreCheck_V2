using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ScanPage : Page_Base
   {
      public ScanPage()
      {
         BindingContext = UnitechViewModel.Current;
         Title = "scanner";

         InitializeComponent();

         SetAppBarContent();

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

      private void itemAppearing(object sender, ItemVisibilityEventArgs e)
      {
         var listView = (sender as ListView);

         var lastItem = UnitechViewModel.Current.LastScans.LastOrDefault();
         listView.ScrollTo(lastItem, ScrollToPosition.MakeVisible, true);
      }

      protected override bool OnBackButtonPressed()
      {
         //DependencyService.Get<IScanner>().CloseScanner();

         return base.OnBackButtonPressed();
      }

      protected override void OnDisappearing()
      {
         //DependencyService.Get<IScanner>().CloseScanner();

         base.OnDisappearing();
      }

      private void entry_Completed(object sender, EventArgs e)
      {
         var entry = sender as Entry;

         if (entry != null && entry.Text != null)
         {
            var st = entry.Text.Trim();

            if (!string.IsNullOrEmpty(st))
            {
               entry.Unfocused -= Entry_Unfocused;
               //slBarCode.IsVisible = false;
               UnitechViewModel.Current.NewBarcode(st, st.Length, 0, null);
            };
         };
      }

      private void entry_TextChanged(object sender, TextChangedEventArgs e)
      {
         var entry = sender as Entry;
         entry.Unfocused -= Entry_Unfocused;

         UnitechViewModel.Current.Length = entry.Text.Length;
      }

      private void Entry_Unfocused(object sender, FocusEventArgs e)
      {
         entry.Focus();
      }
   }
}
