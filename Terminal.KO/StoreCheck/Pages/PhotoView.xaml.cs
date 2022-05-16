using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PhotoView : ContentView
   {
      public PhotoView()
      {
         this.BindingContext = MainViewModel.Current.SelectedIntervention;

         InitializeComponent();
      }

      public string Text { get => lbText.Text; set => lbText.Text = value; }
   }
}
