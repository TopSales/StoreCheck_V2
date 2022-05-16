using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;

namespace ZPF
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class AboutView : ContentView
   {
      public AboutView()
      {
         BindingContext = VersionInfo.Current;

         InitializeComponent();

         imgIconZPF.Source = ImageSource.FromResource("StoreCheck.Images.ZeProgramFactory.350.png", typeof(Page_Base).GetTypeInfo().Assembly);
         imgIcon.Source = ImageSource.FromResource("StoreCheck.Images.Icon.png", typeof(Page_Base).GetTypeInfo().Assembly);
         //imgLogoCB.Source = ImageSource.FromResource("StoreCheck.Images.LogoCB.png", typeof(Page_Base).GetTypeInfo().Assembly);
      }
   }
}
