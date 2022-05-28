using ZPF;

namespace StoreCheck.Pages;

public partial class HeaderContent : ContentView
{
   public HeaderContent()
   {
      InitializeComponent();

      labelVersion.Text = VersionInfo.Current.sVersion + " " + VersionInfo.Current.BuildOn;
   }

   public bool NavBack 
   { 
      get => _NavBack;
      set
      {
         _NavBack = value;

         hslHome.IsVisible = !_NavBack;
         hslNav.IsVisible = _NavBack;
      }
   }
   bool _NavBack  = true;


   private async void btnBack_Clicked(object sender, EventArgs e)
   {
      await Navigation.PopModalAsync();
   }
}
