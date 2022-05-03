using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ZPF
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class AboutWhatsNewView : ContentView
   {
      public AboutWhatsNewView()
      {
         InitializeComponent();

         // - - -  - - - 

         TStrings changelog = new TStrings();
         changelog.Text = XFHelper.GetFromResources(this, "StoreCheck.ChangeLog.XF.md");
         label.FormattedText = (FormattedString)new ZPF.Conv.HTML2LabelConverter().Convert(changelog.Text, null, null, null);
      }
   }
}
