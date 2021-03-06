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
using System.Windows.Shapes;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for InputMonth.xaml
   /// </summary>
   public partial class InputComboBox : Window
   {
      public InputComboBox()
      {
         InitializeComponent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   

      private void Window_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            DialogResult = false;
         }

         if (e.Key == Key.Enter)
         {
            btnOK_Click(null, null);
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         comboBox.Focus();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   
   }
}
