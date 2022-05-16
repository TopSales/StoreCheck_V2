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
   public partial class InterventionView : ContentView
   {
      public InterventionView()
      {
         this.BindingContext = MainViewModel.Current.SelectedIntervention;

         InitializeComponent();
      }
   }
}
