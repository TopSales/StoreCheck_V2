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
   }
}
