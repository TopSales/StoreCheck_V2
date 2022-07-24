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
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using ZPF;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class XamarinDlg : FormsApplicationPage
   {
      public XamarinDlg()
      {
         InitializeComponent();

         // - - -  - - -  - - -  - - -  - - -  - - -  - - -  - - -  - - -  - - - 

         Forms.Init();
         LoadApplication(new DynamicNavigation.App());
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         Basics.ReadFormPos(MainViewModel.IniFileName, this, false, false);
      }

      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         Basics.WriteFormPos(MainViewModel.IniFileName, this, false);
      }
   }
}
