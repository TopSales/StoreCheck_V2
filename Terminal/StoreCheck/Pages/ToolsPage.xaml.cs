using Xamarin.Forms.Xaml;
using ZPF;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ToolsPage : Page_Base
   {
      public ToolsPage()
      {
         InitializeComponent();

         Title = "config";
      }
   }
}
