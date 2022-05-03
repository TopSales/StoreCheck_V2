using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class InterventionStartView : ContentView
   {
      public InterventionStartView()
      {
         InitializeComponent();

         NbKM = MainViewModel.Current.SelectedIntervention.NbKM.ToString();

         slKM.IsVisible = MainViewModel.Current.SelectedInterventionParams.AskForKM;
      }

      public string NbKM { get => kmEntry.Text; set { kmEntry.Text = value; } }

      internal static void Display(ContentPage parent, Grid mainGrid)
      {
         var view = new InterventionStartView();

         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "ok"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), "cancel"),
         });

         tiles[0].Clicked += async (object s, EventArgs ea) =>
         {
            if ( await InterventionsViewModel.Current.Start(decimal.Parse(view.NbKM)))
            {
               (parent as InterventionPage).UpdateTiles();
            }
            else
            {
               await parent.DisplayAlert("Error", "The beginning cannot be changed after the end ...", "ok");
            };
         };
         tiles[1].IsCancel = true;

         GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
      {
         if (string.IsNullOrWhiteSpace(e.NewTextValue))
         {
            ((Entry)sender).Text = "0";
            return;
         }

         var isValid = e.NewTextValue.ToCharArray()
                           .All(char.IsDigit) || (e.NewTextValue.Length > 1 && e.NewTextValue.StartsWith("-")); //Make sure all characters are numbers

         var current = e.NewTextValue;
         current = current.TrimStart('0');

         if (current.Length == 0)
         {
            current = "0";
         }

         ((Entry)sender).Text = isValid ? current : current.Remove(current.Length - 1);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
