using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class InventoryPage : PageEx
   {
      public InventoryPage()
      {
         BindingContext = UnitechViewModel.Current;
         Title = "inventory";

         InitializeComponent();

         SetAppBarContent();

         //DependencyService.Get<IScanner>().OpenScanner();
         //DependencyService.Get<IScanner>().EnableEAN13Only();
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
