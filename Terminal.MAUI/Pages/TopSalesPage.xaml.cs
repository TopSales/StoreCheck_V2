using ZPF.XF.Compos;

namespace StoreCheck.Pages;

public partial class TopSalesPage : PageEx
{
   public TopSalesPage()
   {
      Title = "";
      NavigationPage.SetHasNavigationBar(this, false);

      InitializeComponent();
   }

   private async void btnBack_Clicked(object sender, EventArgs e)
   {
      await Navigation.PopModalAsync();
   }
}
