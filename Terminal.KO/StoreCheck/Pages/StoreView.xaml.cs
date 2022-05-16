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
   public partial class StoreView : ContentView
   {
      public StoreView()
      {
         this.BindingContext = MainViewModel.Current;

         InitializeComponent();

         lTypeStore.Text = (MainViewModel.Current.SelectedIntervention != null ? MainViewModel.Current.SelectedIntervention.StoreType : "");
      }
   }
}
