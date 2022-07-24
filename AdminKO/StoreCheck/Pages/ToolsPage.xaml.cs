using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZPF;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for ToolsPage.xaml
   /// </summary>
   public partial class ToolsPage : Page
   {
      public ToolsPage()
      {
         InitializeComponent();

         // - - -  - - - 

#if DEBUG
         btnScript.Visibility = Visibility.Visible;
         //btnXamarin.Visibility = Visibility.Visible;
#else
         btnScript.Visibility = Visibility.Hidden;
         //btnXamarin.Visibility = Visibility.Hidden;
#endif

         // - - -  - - - 

         // I18nViewModel.Current.T(this);
      }

      private void Tile_Click(object sender, RoutedEventArgs e)
      {
         string cmd = (sender as MahApps.Metro.Controls.Tile).CommandParameter.ToString();
         MenuViewModel.Instance.NavigateMenu(cmd);
      }

      //private void BtnXamarin_Click(object sender, RoutedEventArgs e)
      //{
      //   var dlg = new XamarinDlg();

      //   if (dlg.ShowDialog() == true)
      //   {
      //      MainViewModel.Current.RefreshAll();

      //      BackboneViewModel.Current.IncBusy();
      //      DoIt.Delay(2500, () =>
      //      {
      //         DoIt.OnMainThread(() => MenuViewModel.Instance.NavigateMenu("ART"));
      //         BackboneViewModel.Current.DecBusy();
      //      });
      //   }
      //}
   }
}
