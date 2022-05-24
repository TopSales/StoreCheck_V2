using ZPF;

namespace StoreCheck.Pages;

public partial class HeaderContent : ContentView
{
   public HeaderContent()
   {
      InitializeComponent();

      labelVersion.Text = VersionInfo.Current.sVersion + " " + VersionInfo.Current.BuildOn;
   }
}
