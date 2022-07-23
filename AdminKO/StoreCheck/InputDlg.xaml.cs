using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ZPF;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for InputDlg.xaml
   /// </summary>
   public partial class InputDlg : Window, INotifyPropertyChanged
   {
      decimal _Value = 0;

      public decimal Value { get => _Value; set => _Value = value; }

      public InputDlg(String Title, String TextEx, String Label)
      {
         InitializeComponent();
         // I18nViewModel.Current.T(this);

         this.Title = Title;
         tbInfo.TextEx = TextEx;
         tbInput.Label = Label;

         tbInput.Focus();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged(string propertyName)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {
         //DialogResult = SelectedItem != null;
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
            //if (SelectedItem != null)
            {
               btnOK_Click(null, null);
            }
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   
   }
}
